using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Numerics;
#endif

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        public static ValueEnumerable<FromNonGenericEnumerable<object>, object> AsValueEnumerable(this IEnumerable source)
        {
            return new(new(Throws.IfNull(source)));
        }

        public static ValueEnumerable<FromNonGenericEnumerable<T>, T> AsValueEnumerable<T>(this IEnumerable source)
        {
            return new(new(Throws.IfNull(source)));
        }

        public static ValueEnumerable<FromEnumerable<T>, T> AsValueEnumerable<T>(this IEnumerable<T> source)
        {
            return new(new(Throws.IfNull(source)));
        }

        public static ValueEnumerable<FromArray<T>, T> AsValueEnumerable<T>(this T[] source)
        {
            return new(new(Throws.IfNull(source)));
        }

        public static ValueEnumerable<FromList<T>, T> AsValueEnumerable<T>(this List<T> source)
        {
            return new(new(Throws.IfNull(source)));
        }

        public static ValueEnumerable<FromMemory<T>, T> AsValueEnumerable<T>(this ArraySegment<T> source)
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromMemory<T>, T> AsValueEnumerable<T>(this Memory<T> source)
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromMemory<T>, T> AsValueEnumerable<T>(this ReadOnlyMemory<T> source)
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromReadOnlySequence<T>, T> AsValueEnumerable<T>(this ReadOnlySequence<T> source)
        {
            return new(new(source));
        }

        // for System.Collections.Generic

        public static ValueEnumerable<FromDictionary<TKey, TValue>, KeyValuePair<TKey, TValue>> AsValueEnumerable<TKey, TValue>(this Dictionary<TKey, TValue> source)
            where TKey : notnull
        {
            return new(new(Throws.IfNull(source)));
        }

        public static ValueEnumerable<FromSortedDictionary<TKey, TValue>, KeyValuePair<TKey, TValue>> AsValueEnumerable<TKey, TValue>(this SortedDictionary<TKey, TValue> source)
            where TKey : notnull
        {
            return new(new(Throws.IfNull(source)));
        }

        public static ValueEnumerable<FromQueue<T>, T> AsValueEnumerable<T>(this Queue<T> source)
        {
            return new(new(Throws.IfNull(source)));
        }

        public static ValueEnumerable<FromStack<T>, T> AsValueEnumerable<T>(this Stack<T> source)
        {
            return new(new(Throws.IfNull(source)));
        }

        public static ValueEnumerable<FromLinkedList<T>, T> AsValueEnumerable<T>(this LinkedList<T> source)
        {
            return new(new(Throws.IfNull(source)));
        }

        public static ValueEnumerable<FromHashSet<T>, T> AsValueEnumerable<T>(this HashSet<T> source)
        {
            return new(new(Throws.IfNull(source)));
        }

        public static ValueEnumerable<FromSortedSet<T>, T> AsValueEnumerable<T>(this SortedSet<T> source)
        {
            return new(new(Throws.IfNull(source)));
        }

#if NET8_0_OR_GREATER

        public static ValueEnumerable<FromImmutableArray<T>, T> AsValueEnumerable<T>(this ImmutableArray<T> source)
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromImmutableHashSet<T>, T> AsValueEnumerable<T>(this ImmutableHashSet<T> source)
        {
            return new(new(Throws.IfNull(source)));
        }

#endif

#if NET9_0_OR_GREATER

        public static ValueEnumerable<FromSpan<T>, T> AsValueEnumerable<T>(this Span<T> source)
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromSpan<T>, T> AsValueEnumerable<T>(this ReadOnlySpan<T> source)
        {
            return new(new(source));
        }

#endif

    }
}

namespace ZLinq.Linq
{
    internal struct FromEnumerableContent(object source)
    {
        public object Source = source;
        public int Index;
        [MethodImpl]
        public void ThrowIfNoEnumerable()
        {
            if (Index >= 0) return;
            Throw();
            return;

            void Throw()
            {
                throw new InvalidOperationException("The enumerable is no longer available.");
            }
        }
    }

    [StructLayout(LayoutKind.Auto)]
    public struct FromEnumerable<T> : IValueEnumerator<T>
    {
        readonly CollectionIterator<T> iterator;
        FromEnumerableContent content;

        public FromEnumerable(IEnumerable<T> source)
        {
            if (source.GetType() == typeof(T[]))
            {
                this.iterator = ArrayIterator<T>.Instance;
            }
            else if (source.GetType() == typeof(List<T>))
            {
                this.iterator = ListIterator<T>.Instance;
            }
            else if (source is IReadOnlyList<T>)
            {
                this.iterator = IReadOnlyListIterator<T>.Instance;
            }
            else if (source is IList<T>)
            {
                this.iterator = IListIterator<T>.Instance;
            }
            else
            {
                this.iterator = EnumerableIterator<T>.Instance;
            }

            this.content = new(source);
        }

        // for Contains, need to check ICollection of IEqualityComparer due to compatibility
        internal IEnumerable<T> GetSource()
        {
            content.ThrowIfNoEnumerable();
            return Unsafe.As<IEnumerable<T>>(content.Source);
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            content.ThrowIfNoEnumerable();
            return iterator.TryGetNonEnumeratedCount(Unsafe.As<IEnumerable<T>>(content.Source), out count);
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            content.ThrowIfNoEnumerable();
            return iterator.TryGetSpan(Unsafe.As<IEnumerable<T>>(content.Source), out span);
        }

        public bool TryCopyTo(Span<T> destination, Index offset)
        {
            content.ThrowIfNoEnumerable();
            return iterator.TryCopyTo(Unsafe.As<IEnumerable<T>>(content.Source), destination, offset);
        }

        public bool TryGetNext(out T current) => iterator.TryGetNext(ref content, out current);

        public void Dispose()
        {
            if (content.Index >= 0) return;
            Unsafe.As<IEnumerator<T>>(content.Source).Dispose();
        }
    }

    // I've tried multiple implementations(such as switch direct call and managed function pointer),
    // but using virtual methods showed the best performance.
    internal abstract class CollectionIterator<T>
    {
        public abstract bool TryGetNonEnumeratedCount(IEnumerable<T> source, out int count);

        public virtual bool TryGetSpan(IEnumerable<T> source, out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }

        public virtual bool TryCopyTo(IEnumerable<T> source, Span<T> destination, Index offset)
        {
            return false;
        }

        public abstract bool TryGetNext(ref FromEnumerableContent content, out T current);
    }

    internal sealed class ArrayIterator<T> : CollectionIterator<T>
    {
        public static readonly ArrayIterator<T> Instance = new();

        ArrayIterator() { }

        public override bool TryGetNonEnumeratedCount(IEnumerable<T> source, out int count)
        {
            count = Unsafe.As<IEnumerable<T>, T[]>(ref source).Length;
            return true;
        }

        public override bool TryGetSpan(IEnumerable<T> source, out ReadOnlySpan<T> span)
        {
            span = Unsafe.As<IEnumerable<T>, T[]>(ref source);
            return true;
        }

        public override bool TryCopyTo(IEnumerable<T> source, Span<T> destination, Index offset)
        {
            if (EnumeratorHelper.TryGetSlice<T>(Unsafe.As<IEnumerable<T>, T[]>(ref source), offset, destination.Length, out var slice))
            {
                slice.CopyTo(destination);
                return true;
            }
            return false;
        }

        public override bool TryGetNext(ref FromEnumerableContent content, out T current)
        {
            var index = content.Index;
            var src = Unsafe.As<T[]>(content.Source);
            if ((uint)index < (uint)src.Length)
            {
                current = src[index];
                content.Index = index + 1;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }
    }

    internal sealed class ListIterator<T> : CollectionIterator<T>
    {
        public static readonly ListIterator<T> Instance = new();

        ListIterator() { }

        public override bool TryGetNonEnumeratedCount(IEnumerable<T> source, out int count)
        {
            count = Unsafe.As<IEnumerable<T>, List<T>>(ref source).Count;
            return true;
        }

        public override bool TryGetSpan(IEnumerable<T> source, out ReadOnlySpan<T> span)
        {
            span = CollectionsMarshal.AsSpan(Unsafe.As<IEnumerable<T>, List<T>>(ref source));
            return true;
        }

        public override bool TryCopyTo(IEnumerable<T> source, Span<T> destination, Index offset)
        {
            if (EnumeratorHelper.TryGetSlice<T>(CollectionsMarshal.AsSpan(Unsafe.As<IEnumerable<T>, List<T>>(ref source)), offset, destination.Length, out var slice))
            {
                slice.CopyTo(destination);
                return true;
            }
            return false;
        }

        public override bool TryGetNext(ref FromEnumerableContent content, out T current)
        {
            var index = content.Index;
            var src = Unsafe.As<List<T>>(content.Source);
            if (index < src.Count)
            {
                current = src[index];
                content.Index = index + 1;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }
    }

    internal sealed class IListIterator<T> : CollectionIterator<T>
    {
        public static readonly IListIterator<T> Instance = new();

        IListIterator() { }

        public override bool TryGetNonEnumeratedCount(IEnumerable<T> source, out int count)
        {
            count = Unsafe.As<IEnumerable<T>, IList<T>>(ref source).Count;
            return true;
        }

        public override bool TryGetNext(ref FromEnumerableContent content, out T current)
        {
            var index = content.Index;
            var src = Unsafe.As<IList<T>>(content.Source);
            if (index < src.Count)
            {
                current = src[index];
                content.Index = index + 1;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }
    }

    internal sealed class IReadOnlyListIterator<T> : CollectionIterator<T>
    {
        public static readonly IReadOnlyListIterator<T> Instance = new();

        IReadOnlyListIterator() { }

        public override bool TryGetNonEnumeratedCount(IEnumerable<T> source, out int count)
        {
            count = Unsafe.As<IEnumerable<T>, IReadOnlyList<T>>(ref source).Count;
            return true;
        }

        public override bool TryGetNext(ref FromEnumerableContent content, out T current)
        {
            var index = content.Index;
            var src = Unsafe.As<IReadOnlyList<T>>(content.Source);
            if (index < src.Count)
            {
                current = src[index];
                content.Index = index + 1;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }
    }

    internal sealed class EnumerableIterator<T> : CollectionIterator<T>
    {
        public static readonly EnumerableIterator<T> Instance = new();

        EnumerableIterator() { }

        public override bool TryGetNonEnumeratedCount(IEnumerable<T> source, out int count)
        {
#if NET8_0_OR_GREATER
            if (source.TryGetNonEnumeratedCount(out count)) // call System.Linq.Enumerable.TryGetNonEnumeratedCount
            {
                return true;
            }
#else
            if (source is ICollection<T> c)
            {
                count = c.Count;
                return true;
            }
#endif
            else if (source is IReadOnlyCollection<T> rc) // Enumerable.TryGetNonEnumeratedCount does not check IReadOnlyCollection
            {
                count = rc.Count;
                return true;
            }
            count = 0;
            return false;
        }

        public override bool TryGetNext(ref FromEnumerableContent content, out T current)
        {
            var enumerator = Unsafe.As<IEnumerator<T>>(content.Source);
            if (content.Index == 0)
            {
                enumerator = Initialize(ref content);
            }

            if (enumerator.MoveNext())
            {
                current = enumerator.Current;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;

            [MethodImpl(MethodImplOptions.NoInlining)]
            static IEnumerator<T> Initialize(ref FromEnumerableContent content)
            {
                var enumerator = Unsafe.As<IEnumerable<T>>(content.Source).GetEnumerator();
                content.Source = enumerator;
                content.Index = -1;

                // ReSharper disable once NotDisposedResourceIsReturned
                return enumerator;
            }
        }
    }

    [StructLayout(LayoutKind.Auto)]
    public struct FromNonGenericEnumerable<T>(IEnumerable source) : IValueEnumerator<T>
    {
        IEnumerator? enumerator;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (source is ICollection c)
            {
                count = c.Count;
                return true;
            }
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<T> destination, Index offset)
        {
            return false;
        }

        public bool TryGetNext(out T current)
        {
            if (enumerator == null)
            {
                enumerator = source.GetEnumerator();
            }

            if (enumerator.MoveNext())
            {
                var value = enumerator.Current;
                current = (T)value;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (enumerator is IDisposable d)
            {
                d.Dispose();
            }
        }
    }

    [StructLayout(LayoutKind.Auto)]
    public struct FromArray<T>(T[] source) : IValueEnumerator<T>
    {
        int index;

        // becareful, don't call AsSpan
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal T[] GetSource() => source;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Length;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            // AsSpan is failed by array variance
            if (source.GetType() != typeof(T[]))
            {
                span = default;
                return false;
            }

            span = source.AsSpan();
            return true;
        }

        public bool TryCopyTo(Span<T> destination, Index offset)
        {
            if (EnumeratorHelper.TryGetSlice<T>(source, offset, destination.Length, out var slice))
            {
                slice.CopyTo(destination);
                return true;
            }
            return false;
        }

        public bool TryGetNext(out T current)
        {
            var length = source.Length;
#if NET8_0_OR_GREATER
            ref var firstElement = ref MemoryMarshal.GetArrayDataReference(source);

            if ((uint)index < (uint)length)
            {
                current = Unsafe.Add(ref firstElement, (nint)(uint)index);
                index++;
                return true;
            }
#else
            if ((uint)index < (uint)length)
            {
                current = source[index];
                index++;
                return true;
            }
#endif

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
        }
    }

    [StructLayout(LayoutKind.Auto)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct FromMemory<T>(ReadOnlyMemory<T> source) : IValueEnumerator<T>
    {
#if NET9_0_OR_GREATER
        ReadOnlySpan<T> source = source.Span;
#endif

        int index;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Length;
            return true;
        }

        public bool TryCopyTo(scoped Span<T> destination, Index offset)
        {
#if NET9_0_OR_GREATER
            var span = source;
#else
            var span = source.Span;
#endif
            if (EnumeratorHelper.TryGetSlice<T>(span, offset, destination.Length, out var slice))
            {
                slice.CopyTo(destination);
                return true;
            }
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
#if NET9_0_OR_GREATER
            span = source;
#else
            span = source.Span;
#endif
            return true;
        }

        public bool TryGetNext(out T current)
        {
            if (index < source.Length)
            {
#if NET9_0_OR_GREATER
                current = source[index];
#else
                current = source.Span[index];
#endif
                index++;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
        }
    }

    [StructLayout(LayoutKind.Auto)]
    public struct FromList<T>(List<T> source) : IValueEnumerator<T>
    {
        int index;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal List<T> GetSource() => source;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Count;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = CollectionsMarshal.AsSpan(source);
            return true;
        }

        public bool TryCopyTo(Span<T> destination, Index offset)
        {
            var span = CollectionsMarshal.AsSpan(source);
            if (EnumeratorHelper.TryGetSlice<T>(span, offset, destination.Length, out var slice))
            {
                slice.CopyTo(destination);
                return true;
            }
            return false;
        }

        public bool TryGetNext(out T current)
        {
            if (index < source.Count)
            {
                current = source[index];
                index++;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
        }
    }

    [StructLayout(LayoutKind.Auto)]
    public struct FromDictionary<TKey, TValue>(Dictionary<TKey, TValue> source) : IValueEnumerator<KeyValuePair<TKey, TValue>>
        where TKey : notnull
    {
        bool isInit = false;
        Dictionary<TKey, TValue>.Enumerator enumerator;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Count;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<KeyValuePair<TKey, TValue>> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<KeyValuePair<TKey, TValue>> destination, Index offset) => false;

        public bool TryGetNext(out KeyValuePair<TKey, TValue> current)
        {
            if (!isInit)
            {
                isInit = true;
                enumerator = source.GetEnumerator();
            }

            if (enumerator.MoveNext())
            {
                current = enumerator.Current;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (isInit)
            {
                enumerator.Dispose();
            }
        }
    }

    [StructLayout(LayoutKind.Auto)]
    public struct FromSortedDictionary<TKey, TValue>(SortedDictionary<TKey, TValue> source) : IValueEnumerator<KeyValuePair<TKey, TValue>>
        where TKey : notnull
    {
        bool isInit = false;
        SortedDictionary<TKey, TValue>.Enumerator enumerator;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Count;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<KeyValuePair<TKey, TValue>> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<KeyValuePair<TKey, TValue>> destination, Index offset) => false;

        public bool TryGetNext(out KeyValuePair<TKey, TValue> current)
        {
            if (!isInit)
            {
                isInit = true;
                enumerator = source.GetEnumerator();
            }

            if (enumerator.MoveNext())
            {
                current = enumerator.Current;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (isInit)
            {
                enumerator.Dispose();
            }
        }
    }

    [StructLayout(LayoutKind.Auto)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct FromReadOnlySequence<T>(ReadOnlySequence<T> source) : IValueEnumerator<T>
    {
        bool isInit = false;
        ReadOnlySequence<T>.Enumerator sequenceEnumerator;
        FromMemory<T> enumerator;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            const int ArrayMaxLength = 0X7FFFFFC7;
            if (source.Length <= ArrayMaxLength)
            {
                count = checked((int)source.Length);
                return true;
            }
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            if (source.IsSingleSegment)
            {
                span = source.First.Span;
                return true;
            }

            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<T> destination, Index offset)
        {
            if (source.IsSingleSegment)
            {
                var span = source.First.Span;
                if (EnumeratorHelper.TryGetSlice<T>(span, offset, destination.Length, out var slice))
                {
                    slice.CopyTo(destination);
                    return true;
                }
            }
            else
            {
                if (EnumeratorHelper.TryGetSliceRange(checked((int)source.Length), offset, destination.Length, out var start, out var count))
                {
                    source.Slice(start, count).CopyTo(destination);
                    return true;
                }
            }

            return false;
        }

        public bool TryGetNext(out T current)
        {
            if (!isInit)
            {
                isInit = true;
                sequenceEnumerator = source.GetEnumerator();
            }

        MOVE_NEXT:
            if (enumerator.TryGetNext(out current))
            {
                return true;
            }

            if (sequenceEnumerator.MoveNext())
            {
                enumerator = sequenceEnumerator.Current.AsValueEnumerable().Enumerator;
                goto MOVE_NEXT;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            // no needs FromMemory<T>.Dispose
        }
    }

    [StructLayout(LayoutKind.Auto)]
    public struct FromQueue<T>(Queue<T> source) : IValueEnumerator<T>
    {
        bool isInit;
        Queue<T>.Enumerator enumerator;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Count;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<T> destination, Index offset) => false;

        public bool TryGetNext(out T current)
        {
            if (!isInit)
            {
                isInit = true;
                enumerator = source.GetEnumerator();
            }

            if (enumerator.MoveNext())
            {
                current = enumerator.Current;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (isInit)
            {
                enumerator.Dispose();
            }
        }
    }

    [StructLayout(LayoutKind.Auto)]
    public struct FromStack<T>(Stack<T> source) : IValueEnumerator<T>
    {
        bool isInit;
        Stack<T>.Enumerator enumerator;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Count;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<T> destination, Index offset) => false;

        public bool TryGetNext(out T current)
        {
            if (!isInit)
            {
                isInit = true;
                enumerator = source.GetEnumerator();
            }

            if (enumerator.MoveNext())
            {
                current = enumerator.Current;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (isInit)
            {
                enumerator.Dispose();
            }
        }
    }

    [StructLayout(LayoutKind.Auto)]
    public struct FromLinkedList<T>(LinkedList<T> source) : IValueEnumerator<T>
    {
        bool isInit;
        LinkedList<T>.Enumerator enumerator;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Count;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<T> destination, Index offset) => false;

        public bool TryGetNext(out T current)
        {
            if (!isInit)
            {
                isInit = true;
                enumerator = source.GetEnumerator();
            }

            if (enumerator.MoveNext())
            {
                current = enumerator.Current;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (isInit)
            {
                enumerator.Dispose();
            }
        }
    }

    [StructLayout(LayoutKind.Auto)]
    public struct FromHashSet<T>(HashSet<T> source) : IValueEnumerator<T>
    {
        bool isInit;
        HashSet<T>.Enumerator enumerator;

        // for Contains, need to check ICollection of IEqualityComparer due to compatibility
        internal HashSet<T> GetSource() => source;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Count;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<T> destination, Index offset) => false;

        public bool TryGetNext(out T current)
        {
            if (!isInit)
            {
                isInit = true;
                enumerator = source.GetEnumerator();
            }

            if (enumerator.MoveNext())
            {
                current = enumerator.Current;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (isInit)
            {
                enumerator.Dispose();
            }
        }
    }

    [StructLayout(LayoutKind.Auto)]
    public struct FromSortedSet<T>(SortedSet<T> source) : IValueEnumerator<T>
    {
        bool isInit;
        SortedSet<T>.Enumerator enumerator;

        // for Contains, need to check ICollection of IEqualityComparer due to compatibility
        internal SortedSet<T> GetSource() => source;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Count;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<T> destination, Index offset) => false;

        public bool TryGetNext(out T current)
        {
            if (!isInit)
            {
                isInit = true;
                enumerator = source.GetEnumerator();
            }

            if (enumerator.MoveNext())
            {
                current = enumerator.Current;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (isInit)
            {
                enumerator.Dispose();
            }
        }
    }

#if NET8_0_OR_GREATER
    [StructLayout(LayoutKind.Auto)]
    public struct FromImmutableArray<T>(ImmutableArray<T> source) : IValueEnumerator<T>
    {
        int index;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Length;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = source.AsSpan();
            return true;
        }

        public bool TryCopyTo(Span<T> destination, Index offset)
        {
            if (EnumeratorHelper.TryGetSlice<T>(source.AsSpan(), offset, destination.Length, out var slice))
            {
                slice.CopyTo(destination);
                return true;
            }
            return false;
        }

        public bool TryGetNext(out T current)
        {
            if (index < source.Length)
            {
                current = source[index];
                index++;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
        }
    }

    [StructLayout(LayoutKind.Auto)]
    public struct FromImmutableHashSet<T>(ImmutableHashSet<T> source) : IValueEnumerator<T>
    {
        bool isInit;
        ImmutableHashSet<T>.Enumerator enumerator;

        internal ImmutableHashSet<T> GetSource() => source;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Count;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<T> destination, Index offset) => false;

        public bool TryGetNext(out T current)
        {
            if (!isInit)
            {
                isInit = true;
                enumerator = source.GetEnumerator();
            }

            if (enumerator.MoveNext())
            {
                current = enumerator.Current;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (isInit)
            {
                enumerator.Dispose();
            }
        }
    }

#endif


#if NET9_0_OR_GREATER

    [StructLayout(LayoutKind.Auto)]
    public ref struct FromSpan<T>(ReadOnlySpan<T> source) : IValueEnumerator<T>
    {
        ReadOnlySpan<T> source = source;
        int index;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Length;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = source;
            return true;
        }

        public bool TryCopyTo(scoped Span<T> destination, Index offset)
        {
            if (EnumeratorHelper.TryGetSlice<T>(source, offset, destination.Length, out var slice))
            {
                slice.CopyTo(destination);
                return true;
            }
            return false;
        }

        public bool TryGetNext(out T current)
        {
            if (index < source.Length)
            {
                current = source[index];
                index++;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
        }
    }
#endif
}
