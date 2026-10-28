using System.Buffers;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ZLinq.Internal;

// Accesses the elements of List<T> for the operators specialized for List<T>.
// .NET 8 or later uses System.Runtime.InteropServices.CollectionsMarshal.
// netstandard has no public API to access the backing array of List<T>, so it goes through the public List<T> API only.
// Do not rely on the private field layout of List<T> here; it differs between runtimes (e.g. Unity's Mono/IL2CPP).
internal static class ListMarshal
{
#if NET8_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsSpan<T>(List<T> list) => CollectionsMarshal.AsSpan(list);
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ListSpan<T> AsSpan<T>(List<T> list) => new(list);
#endif

#if !NET8_0_OR_GREATER
    // Up to this count, reading through the List<T> indexer is cheaper than renting a buffer for ListChunks<T>.
    // Measured on Unity 2022.3 and 6000.6 IL2CPP, where the break-even point is between 32 and 64 elements.
    private const int ChunkedReadThreshold = 48;
#endif

    // Whether the operators should read the list with ListChunks<T> rather than AsSpan.
    // Always true on .NET 8 or later, so the JIT removes the other branch.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool UseChunks<T>(List<T> list)
    {
#if NET8_0_OR_GREATER
        return true;
#else
        return list.Count > ChunkedReadThreshold;
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetSpan<T>(List<T> list, out ReadOnlySpan<T> span)
    {
#if NET8_0_OR_GREATER
        span = CollectionsMarshal.AsSpan(list);
        return true;
#else
        span = default;
        return false;
#endif
    }

    public static bool TryCopyTo<T>(List<T> list, Span<T> destination, Index offset)
    {
#if NET8_0_OR_GREATER
        if (EnumeratorHelper.TryGetSlice<T>(CollectionsMarshal.AsSpan(list), offset, destination.Length, out var slice))
        {
            slice.CopyTo(destination);
            return true;
        }
        return false;
#else
        if (EnumeratorHelper.TryGetSliceRange(list.Count, offset, destination.Length, out var start, out var count))
        {
            // Small ranges (including the single element copies of First/Last/ElementAt) are read through the indexer,
            // which is cheaper than renting a buffer (see ChunkedReadThreshold).
            if (count <= ChunkedReadThreshold)
            {
                for (var i = 0; i < count; i++)
                {
                    destination[i] = list[start + i];
                }
                return true;
            }

            var written = 0;
            foreach (var chunk in new ListChunks<T>(list, start, count))
            {
                chunk.CopyTo(destination.Slice(written));
                written += chunk.Length;
            }
            return true;
        }
        return false;
#endif
    }

#if !NET8_0_OR_GREATER

    // netstandard can neither read the elements of List<T> into Span<T> nor write Span<T> into List<T> in bulk,
    // so ToArray detects List<T> sources, and ToList/CopyTo detect List<T> and array sources,
    // and they use the bulk copies of the public API instead (List<T>.ToArray, List<T>(IEnumerable<T>), List<T>.AddRange).
    // The source kind is read from SourceKind<TEnumerator, T> rather than by comparing typeof directly:
    // the JIT and the IL2CPP of Unity 6 evaluate such a comparison as a constant, but the IL2CPP of Unity 2022.3
    // compares the Type objects at run time on every call, which costs about as much as copying dozens of elements.

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetListSource<TEnumerator, T>(in TEnumerator enumerator, [NotNullWhen(true)] out List<T>? list)
        where TEnumerator : struct, IValueEnumerator<T>
    {
        if (SourceKind<TEnumerator, T>.IsFromList)
        {
            list = Unsafe.As<TEnumerator, FromList<T>>(ref Unsafe.AsRef(in enumerator)).GetSource();
            return true;
        }

        list = null;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetArraySource<TEnumerator, T>(in TEnumerator enumerator, [NotNullWhen(true)] out T[]? array)
        where TEnumerator : struct, IValueEnumerator<T>
    {
        if (SourceKind<TEnumerator, T>.IsFromArray)
        {
            array = Unsafe.As<TEnumerator, FromArray<T>>(ref Unsafe.AsRef(in enumerator)).GetSource();
            return true;
        }

        array = null;
        return false;
    }

    static class SourceKind<TEnumerator, T>
    {
        public static readonly bool IsFromList = typeof(TEnumerator) == typeof(FromList<T>);
        public static readonly bool IsFromArray = typeof(TEnumerator) == typeof(FromArray<T>);
    }

#endif
}

#if !NET8_0_OR_GREATER

// Exposes the same members as ReadOnlySpan<T> that the List<T> specialized operators use (Length and indexer),
// so the operators can be written once for both ReadOnlySpan<T> (.NET 8 or later) and this type (netstandard).
// Like a span over the backing array, Length is fixed when this is created, so a loop bounded by Length terminates
// even if the loop body adds elements to the list. If the list shrinks, the indexer throws instead of reading stale elements.
internal readonly struct ListSpan<T>(List<T> list)
{
    readonly int length = list.Count;

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => length;
    }

    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => list[index];
    }
}

#endif

// Reads the elements of List<T> as a sequence of ReadOnlySpan<T> chunks.
// Usage: `foreach (var chunk in new ListChunks<T>(list)) { ... }`
// On netstandard, check ListMarshal.UseChunks first; small lists are cheaper to read through the indexer.
// Use it only with foreach, which disposes it. Do not call MoveNext directly: on a `using` local (which is read-only)
// it would advance a hidden copy, and on netstandard each copy would rent a buffer that is never returned.
// .NET 8 or later returns the whole list (or the requested range) as a single span over the backing array.
// netstandard copies the elements into a pooled buffer with List<T>.CopyTo chunk by chunk,
// which avoids calling the List<T> indexer for each element.
// A chunk is valid only until the next MoveNext or Dispose; on netstandard the buffer is returned to ArrayPool on Dispose,
// so never keep a chunk beyond the loop.
// The number of elements read never exceeds the count at creation; on netstandard it also stops early if the list shrinks.
internal ref struct ListChunks<T>
{
#if NET8_0_OR_GREATER

    readonly ReadOnlySpan<T> span;
    bool consumed;

    public ListChunks(List<T> list)
    {
        span = CollectionsMarshal.AsSpan(list);
    }

    public ListChunks(List<T> list, int start, int count)
    {
        span = CollectionsMarshal.AsSpan(list).Slice(start, count);
    }

    public readonly ReadOnlySpan<T> Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => span;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        if (consumed)
        {
            return false;
        }

        consumed = true;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Dispose()
    {
    }

#else

    const int ChunkSize = 512;

    readonly List<T> list;
    readonly int end;
    int index;
    T[]? buffer;
    int length;
    int usedLength;

    public ListChunks(List<T> list)
        : this(list, 0, list.Count)
    {
    }

    public ListChunks(List<T> list, int start, int count)
    {
        Debug.Assert(start >= 0 && count >= 0 && start + count <= list.Count);

        this.list = list;
        index = start;
        end = start + count;
    }

    public readonly ReadOnlySpan<T> Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(buffer, 0, length);
    }

    public bool MoveNext()
    {
        // The list may shrink while it is being read (e.g. by a predicate); never read beyond its current count.
        var remaining = Math.Min(end, list.Count) - index;
        if (remaining <= 0)
        {
            return false;
        }

        buffer ??= ArrayPool<T>.Shared.Rent(Math.Min(remaining, ChunkSize));

        length = Math.Min(remaining, buffer.Length);
        list.CopyTo(index, buffer, 0, length);
        index += length;
        usedLength = Math.Max(usedLength, length);
        return true;
    }

    public void Dispose()
    {
        if (buffer != null)
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                Array.Clear(buffer, 0, usedLength);
            }

            ArrayPool<T>.Shared.Return(buffer);
            buffer = null;
        }
    }

#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ListChunks<T> GetEnumerator()
    {
#if !NET8_0_OR_GREATER
        // foreach enumerates a copy. A copy made after MoveNext would share the rented buffer
        // and return it to ArrayPool twice.
        Debug.Assert(buffer == null);
#endif
        return this;
    }
}

// Fills a List<T> with `count` elements through Span<T>: either a new list, or an existing empty list.
// Usage: create, write the elements to Span, call Commit with the number of elements written, then Dispose (use `using`).
// Commit returns the filled list.
// Fewer than `count` elements may be written (e.g. when a selector shrinks the source list while it is read).
// On netstandard, Span is a buffer shared with earlier fills and other ArrayPool users, so Commit clears the unwritten rest
// to keep their data out of the list; the list then ends with default elements.
// On netstandard, a new list is created from the buffer with List<T>(IEnumerable<T>), which copies it directly into the new list.
// An existing list is filled with AddRange, which on .NET Framework (and likely Mono) copies it through a temporary array.
// If Commit is not called (e.g. an exception is thrown while filling), an existing list is left empty on netstandard,
// whereas on .NET 8 or later it already has `count` elements, of which the unwritten ones are default.
internal ref struct ListFiller<T>
{
#if NET8_0_OR_GREATER

    readonly List<T> list;
    readonly Span<T> span;

    public ListFiller(int count)
        : this(new List<T>(count), count)
    {
    }

    public ListFiller(List<T> list, int count)
    {
        this.list = list;
        CollectionsMarshal.SetCount(list, count);
        span = CollectionsMarshal.AsSpan(list);
    }

    public readonly Span<T> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => span;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly List<T> Commit(int written)
    {
        return list;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
    }

#else

    readonly List<T>? list;
    FillCollection<T>? buffer;

    public ListFiller(int count)
    {
        list = null;
        buffer = FillCollection<T>.Rent(count);
    }

    public ListFiller(List<T> list, int count)
    {
        this.list = list;
        buffer = FillCollection<T>.Rent(count);
    }

    public readonly Span<T> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => buffer!.Span;
    }

    public readonly List<T> Commit(int written)
    {
        buffer!.Span.Slice(written).Clear();

        if (list == null)
        {
            return new List<T>(buffer);
        }

        list.AddRange(buffer);
        return list;
    }

    public void Dispose()
    {
        buffer?.Return();
        buffer = null;
    }

#endif
}

#if !NET8_0_OR_GREATER

// Read-only ICollection<T> over a temporary buffer, filled through Span and then passed to List<T>(IEnumerable<T>) or List<T>.AddRange.
// The result is correct however they consume it (CopyTo or enumeration); only the performance differs.
internal sealed class FillCollection<T> : ICollection<T>
{
    // Buffers up to this length are kept by the cached instance, so that small fills need no ArrayPool round trip.
    // Larger buffers are rented from ArrayPool.
    const int MaxRetainedLength = 256;

    [ThreadStatic]
    static FillCollection<T>? cache;

    T[] retained = [];
    T[] array = [];
    int count;

    public static FillCollection<T> Rent(int count)
    {
        // Take the cached instance out while in use, so that a nested fill (e.g. ToList in a selector) never shares it.
        var collection = cache ?? new FillCollection<T>();
        cache = null;

        if (count <= MaxRetainedLength)
        {
            if (collection.retained.Length < count)
            {
                var length = Math.Max(16, collection.retained.Length * 2);
                while (length < count)
                {
                    length *= 2;
                }
                collection.retained = new T[length];
            }
            collection.array = collection.retained;
        }
        else
        {
            collection.array = ArrayPool<T>.Shared.Rent(count);
        }

        collection.count = count;
        return collection;
    }

    public Span<T> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => array.AsSpan(0, count);
    }

    public void Return()
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            Array.Clear(array, 0, count);
        }

        if (!ReferenceEquals(array, retained))
        {
            ArrayPool<T>.Shared.Return(array);
        }

        array = [];
        count = 0;
        cache = this;
    }

    public int Count => count;

    public bool IsReadOnly => true;

    public void CopyTo(T[] destination, int arrayIndex)
    {
        Array.Copy(array, 0, destination, arrayIndex, count);
    }

    public bool Contains(T item)
    {
        return Array.IndexOf(array, item, 0, count) >= 0;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < count; i++)
        {
            yield return array[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(T item) => throw new NotSupportedException();

    public void Clear() => throw new NotSupportedException();

    public bool Remove(T item) => throw new NotSupportedException();
}

#endif
