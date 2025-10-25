#if ZLINQ_UNITY_COLLECTIONS_SUPPORT

#pragma warning disable CS9074
#nullable enable

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using ZLinq.Internal;
using ZLinq.Linq;

namespace ZLinq
{
    public static class UnityCollectionsExtensions
    {
        public static ValueEnumerable<FromNativeList<T>, T> AsValueEnumerable<T>(this NativeList<T> source)
            where T : unmanaged
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromNativeQueue<T>, T> AsValueEnumerable<T>(this NativeQueue<T>.ReadOnly source)
            where T : unmanaged
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromNativeHashSet<T>, T> AsValueEnumerable<T>(this NativeHashSet<T> source)
            where T : unmanaged, IEquatable<T>
        {
            return new(new(source.AsReadOnly()));
        }

        public static ValueEnumerable<FromNativeHashSet<T>, T> AsValueEnumerable<T>(this NativeHashSet<T>.ReadOnly source)
            where T : unmanaged, IEquatable<T>
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromNativeHashMap<TKey, TValue>, KVPair<TKey, TValue>> AsValueEnumerable<TKey, TValue>(this NativeHashMap<TKey, TValue> source)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : unmanaged
        {
            return new(new(source.AsReadOnly()));
        }

        public static ValueEnumerable<FromNativeHashMap<TKey, TValue>, KVPair<TKey, TValue>> AsValueEnumerable<TKey, TValue>(this NativeHashMap<TKey, TValue>.ReadOnly source)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : unmanaged
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromNativeText, Unicode.Rune> AsValueEnumerable(this NativeText source)
        {
            return new(new(source.AsReadOnly()));
        }

        public static ValueEnumerable<FromNativeText, Unicode.Rune> AsValueEnumerable(this NativeText.ReadOnly source)
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromFixedList32Bytes<T>, T> AsValueEnumerable<T>(this FixedList32Bytes<T> source)
            where T : unmanaged
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromFixedList64Bytes<T>, T> AsValueEnumerable<T>(this FixedList64Bytes<T> source)
            where T : unmanaged
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromFixedList128Bytes<T>, T> AsValueEnumerable<T>(this FixedList128Bytes<T> source)
            where T : unmanaged
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromFixedList512Bytes<T>, T> AsValueEnumerable<T>(this FixedList512Bytes<T> source)
            where T : unmanaged
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromFixedList4096Bytes<T>, T> AsValueEnumerable<T>(this FixedList4096Bytes<T> source)
            where T : unmanaged
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromFixedString32Bytes, Unicode.Rune> AsValueEnumerable(this FixedString32Bytes source)
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromFixedString64Bytes, Unicode.Rune> AsValueEnumerable(this FixedString64Bytes source)
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromFixedString128Bytes, Unicode.Rune> AsValueEnumerable(this FixedString128Bytes source)
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromFixedString512Bytes, Unicode.Rune> AsValueEnumerable(this FixedString512Bytes source)
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromFixedString4096Bytes, Unicode.Rune> AsValueEnumerable(this FixedString4096Bytes source)
        {
            return new(new(source));
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromNativeList<T> : IValueEnumerator<T>
        where T : unmanaged
    {
        NativeList<T> source;
        int index;

        public FromNativeList(NativeList<T> source)
        {
            this.source = source;
            this.index = 0;
        }

        public void Dispose()
        {
        }

        public unsafe bool TryCopyTo(Span<T> destination, Index offset)
        {
            if (EnumeratorHelper.TryGetSlice(new ReadOnlySpan<T>(source.GetUnsafePtr(), source.Length), offset, destination.Length, out var slice))
            {
                slice.CopyTo(destination);
                return true;
            }
            return false;
        }

        public bool TryGetNext(out T current)
        {
            if ((uint)index < (uint)source.Length)
            {
                current = source[index++];
                return true;
            }

            current = default!;
            return false;
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Length;
            return true;
        }

        public unsafe bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = new ReadOnlySpan<T>(source.GetUnsafePtr(), source.Length);
            return true;
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromNativeQueue<T> : IValueEnumerator<T>
        where T : unmanaged
    {
        NativeQueue<T>.ReadOnly source;
        NativeQueue<T>.Enumerator enumerator;

        public FromNativeQueue(NativeQueue<T>.ReadOnly source)
        {
            this.source = source;
            this.enumerator = source.GetEnumerator();
        }

        public void Dispose()
        {
        }

        public unsafe bool TryCopyTo(Span<T> destination, Index offset) => false;

        public bool TryGetNext(out T current)
        {
            if (enumerator.MoveNext())
            {
                current = enumerator.Current;
                return true;
            }

            current = default!;
            return false;
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Count;
            return true;
        }

        public unsafe bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromNativeHashSet<T> : IValueEnumerator<T>
        where T : unmanaged, IEquatable<T>
    {
        NativeHashSet<T>.ReadOnly source;
        NativeHashSet<T>.Enumerator enumerator;

        public FromNativeHashSet(NativeHashSet<T>.ReadOnly source)
        {
            this.source = source;
            this.enumerator = source.GetEnumerator();
        }

        public void Dispose()
        {
        }

        public unsafe bool TryCopyTo(Span<T> destination, Index offset) => false;

        public bool TryGetNext(out T current)
        {
            if (enumerator.MoveNext())
            {
                current = enumerator.Current;
                return true;
            }

            current = default!;
            return false;
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Count;
            return true;
        }

        public unsafe bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromNativeHashMap<TKey, TValue> : IValueEnumerator<KVPair<TKey, TValue>>
        where TKey : unmanaged, IEquatable<TKey>
        where TValue : unmanaged
    {
        NativeHashMap<TKey, TValue>.ReadOnly source;
        NativeHashMap<TKey, TValue>.Enumerator enumerator;

        public FromNativeHashMap(NativeHashMap<TKey, TValue>.ReadOnly source)
        {
            this.source = source;
            this.enumerator = source.GetEnumerator();
        }

        public void Dispose()
        {
        }

        public unsafe bool TryCopyTo(Span<KVPair<TKey, TValue>> destination, Index offset) => false;

        public bool TryGetNext(out KVPair<TKey, TValue> current)
        {
            if (enumerator.MoveNext())
            {
                current = enumerator.Current;
                return true;
            }

            current = default!;
            return false;
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Count;
            return true;
        }

        public unsafe bool TryGetSpan(out ReadOnlySpan<KVPair<TKey, TValue>> span)
        {
            span = default;
            return false;
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromNativeText : IValueEnumerator<Unicode.Rune>
    {
        NativeText.Enumerator enumerator;

        public FromNativeText(NativeText.ReadOnly source)
        {
            this.enumerator = source.GetEnumerator();
        }

        public void Dispose()
        {
        }

        public unsafe bool TryCopyTo(Span<Unicode.Rune> destination, Index offset) => false;

        public bool TryGetNext(out Unicode.Rune current)
        {
            if (enumerator.MoveNext())
            {
                current = enumerator.Current;
                return true;
            }

            current = default!;
            return false;
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = default;
            return false;
        }

        public unsafe bool TryGetSpan(out ReadOnlySpan<Unicode.Rune> span)
        {
            span = default;
            return false;
        }
    }

    public struct FromFixedList32Bytes<T> : IValueEnumerator<T>
        where T : unmanaged
    {
        FixedList32Bytes<T> source;
        int index;

        public FromFixedList32Bytes(FixedList32Bytes<T> source)
        {
            this.source = source;
            this.index = 0;
        }

        public void Dispose()
        {
        }

        public unsafe bool TryCopyTo(Span<T> destination, Index offset) => false;

        public bool TryGetNext(out T current)
        {
            if ((uint)index < (uint)source.Length)
            {
                current = source[index++];
                return true;
            }

            current = default!;
            return false;
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Length;
            return true;
        }

        public unsafe bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }
    }

    public struct FromFixedList64Bytes<T> : IValueEnumerator<T>
        where T : unmanaged
    {
        FixedList64Bytes<T> source;
        int index;

        public FromFixedList64Bytes(FixedList64Bytes<T> source)
        {
            this.source = source;
            this.index = 0;
        }

        public void Dispose()
        {
        }

        public unsafe bool TryCopyTo(Span<T> destination, Index offset) => false;

        public bool TryGetNext(out T current)
        {
            if ((uint)index < (uint)source.Length)
            {
                current = source[index++];
                return true;
            }

            current = default!;
            return false;
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Length;
            return true;
        }

        public unsafe bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }
    }

    public struct FromFixedList128Bytes<T> : IValueEnumerator<T>
        where T : unmanaged
    {
        FixedList128Bytes<T> source;
        int index;

        public FromFixedList128Bytes(FixedList128Bytes<T> source)
        {
            this.source = source;
            this.index = 0;
        }

        public void Dispose()
        {
        }

        public unsafe bool TryCopyTo(Span<T> destination, Index offset) => false;

        public bool TryGetNext(out T current)
        {
            if ((uint)index < (uint)source.Length)
            {
                current = source[index++];
                return true;
            }

            current = default!;
            return false;
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Length;
            return true;
        }

        public unsafe bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }
    }

    public struct FromFixedList512Bytes<T> : IValueEnumerator<T>
        where T : unmanaged
    {
        FixedList512Bytes<T> source;
        int index;

        public FromFixedList512Bytes(FixedList512Bytes<T> source)
        {
            this.source = source;
            this.index = 0;
        }

        public void Dispose()
        {
        }

        public unsafe bool TryCopyTo(Span<T> destination, Index offset) => false;

        public bool TryGetNext(out T current)
        {
            if ((uint)index < (uint)source.Length)
            {
                current = source[index++];
                return true;
            }

            current = default!;
            return false;
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Length;
            return true;
        }

        public unsafe bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }
    }

    public struct FromFixedList4096Bytes<T> : IValueEnumerator<T>
        where T : unmanaged
    {
        FixedList4096Bytes<T> source;
        int index;

        public FromFixedList4096Bytes(FixedList4096Bytes<T> source)
        {
            this.source = source;
            this.index = 0;
        }

        public void Dispose()
        {
        }

        public unsafe bool TryCopyTo(Span<T> destination, Index offset) => false;

        public bool TryGetNext(out T current)
        {
            if ((uint)index < (uint)source.Length)
            {
                current = source[index++];
                return true;
            }

            current = default!;
            return false;
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Length;
            return true;
        }

        public unsafe bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromFixedString32Bytes : IValueEnumerator<Unicode.Rune>
    {
        FixedString32Bytes.Enumerator enumerator;

        public FromFixedString32Bytes(FixedString32Bytes source)
        {
            this.enumerator = source.GetEnumerator();
        }

        public void Dispose()
        {
        }

        public unsafe bool TryCopyTo(Span<Unicode.Rune> destination, Index offset) => false;

        public bool TryGetNext(out Unicode.Rune current)
        {
            if (enumerator.MoveNext())
            {
                current = enumerator.Current;
                return true;
            }

            current = default!;
            return false;
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = default;
            return false;
        }

        public unsafe bool TryGetSpan(out ReadOnlySpan<Unicode.Rune> span)
        {
            span = default;
            return false;
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromFixedString64Bytes : IValueEnumerator<Unicode.Rune>
    {
        FixedString64Bytes.Enumerator enumerator;

        public FromFixedString64Bytes(FixedString64Bytes source)
        {
            this.enumerator = source.GetEnumerator();
        }

        public void Dispose()
        {
        }

        public unsafe bool TryCopyTo(Span<Unicode.Rune> destination, Index offset) => false;

        public bool TryGetNext(out Unicode.Rune current)
        {
            if (enumerator.MoveNext())
            {
                current = enumerator.Current;
                return true;
            }

            current = default!;
            return false;
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = default;
            return false;
        }

        public unsafe bool TryGetSpan(out ReadOnlySpan<Unicode.Rune> span)
        {
            span = default;
            return false;
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromFixedString128Bytes : IValueEnumerator<Unicode.Rune>
    {
        FixedString128Bytes.Enumerator enumerator;

        public FromFixedString128Bytes(FixedString128Bytes source)
        {
            this.enumerator = source.GetEnumerator();
        }

        public void Dispose()
        {
        }

        public unsafe bool TryCopyTo(Span<Unicode.Rune> destination, Index offset) => false;

        public bool TryGetNext(out Unicode.Rune current)
        {
            if (enumerator.MoveNext())
            {
                current = enumerator.Current;
                return true;
            }

            current = default!;
            return false;
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = default;
            return false;
        }

        public unsafe bool TryGetSpan(out ReadOnlySpan<Unicode.Rune> span)
        {
            span = default;
            return false;
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromFixedString512Bytes : IValueEnumerator<Unicode.Rune>
    {
        FixedString512Bytes.Enumerator enumerator;

        public FromFixedString512Bytes(FixedString512Bytes source)
        {
            this.enumerator = source.GetEnumerator();
        }

        public void Dispose()
        {
        }

        public unsafe bool TryCopyTo(Span<Unicode.Rune> destination, Index offset) => false;

        public bool TryGetNext(out Unicode.Rune current)
        {
            if (enumerator.MoveNext())
            {
                current = enumerator.Current;
                return true;
            }

            current = default!;
            return false;
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = default;
            return false;
        }

        public unsafe bool TryGetSpan(out ReadOnlySpan<Unicode.Rune> span)
        {
            span = default;
            return false;
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromFixedString4096Bytes : IValueEnumerator<Unicode.Rune>
    {
        FixedString4096Bytes.Enumerator enumerator;

        public FromFixedString4096Bytes(FixedString4096Bytes source)
        {
            this.enumerator = source.GetEnumerator();
        }

        public void Dispose()
        {
        }

        public unsafe bool TryCopyTo(Span<Unicode.Rune> destination, Index offset) => false;

        public bool TryGetNext(out Unicode.Rune current)
        {
            if (enumerator.MoveNext())
            {
                current = enumerator.Current;
                return true;
            }

            current = default!;
            return false;
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = default;
            return false;
        }

        public unsafe bool TryGetSpan(out ReadOnlySpan<Unicode.Rune> span)
        {
            span = default;
            return false;
        }
    }
}

#pragma warning restore CS9074
#endif
