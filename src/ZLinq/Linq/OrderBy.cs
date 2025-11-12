using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<OrderBy<TEnumerator, TSource, TSource>, TSource> Order<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                , allows ref struct
#endif
            => new(new(source.Enumerator, UnsafeFunctions<TSource, TSource>.Identity, null, null, descending: false));

        public static ValueEnumerable<OrderBy<TEnumerator, TSource, TSource>, TSource> Order<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, IComparer<TSource>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                , allows ref struct
#endif
            => new(new(source.Enumerator, UnsafeFunctions<TSource, TSource>.Identity, comparer, null, descending: false));

        public static ValueEnumerable<OrderBy<TEnumerator, TSource, TSource>, TSource> OrderDescending<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                , allows ref struct
#endif
            => new(new(source.Enumerator, UnsafeFunctions<TSource, TSource>.Identity, null, null, descending: true));

        public static ValueEnumerable<OrderBy<TEnumerator, TSource, TSource>, TSource> OrderDescending<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, IComparer<TSource>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                , allows ref struct
#endif
            => new(new(source.Enumerator, UnsafeFunctions<TSource, TSource>.Identity, comparer, null, descending: true));

        public static ValueEnumerable<OrderBy<TEnumerator, TSource, TKey>, TSource> OrderBy<TEnumerator, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(keySelector), null, null, false));

        public static ValueEnumerable<OrderBy<TEnumerator, TSource, TKey>, TSource> OrderBy<TEnumerator, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(keySelector);
            return new(new(source.Enumerator, keySelector, comparer, null, descending: false));
        }

        public static ValueEnumerable<OrderBy<TEnumerator, TSource, TKey>, TSource> OrderByDescending<TEnumerator, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(keySelector), null, null, true));

        public static ValueEnumerable<OrderBy<TEnumerator, TSource, TKey>, TSource> OrderByDescending<TEnumerator, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(keySelector);
            return new(new(source.Enumerator, keySelector, comparer, null, descending: true));
        }

        // ThenBy

        public static ValueEnumerable<OrderBy<TEnumerator, TSource, TSecondKey>, TSource> ThenBy<TEnumerator, TSource, TKey, TSecondKey>(this ValueEnumerable<OrderBy<TEnumerator, TSource, TKey>, TSource> source, Func<TSource, TSecondKey> keySelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                , allows ref struct
#endif
            => new(source.Enumerator.ThenBy(Throws.IfNull(keySelector)));

        public static ValueEnumerable<OrderBy<TEnumerator, TSource, TSecondKey>, TSource> ThenBy<TEnumerator, TSource, TKey, TSecondKey>(this ValueEnumerable<OrderBy<TEnumerator, TSource, TKey>, TSource> source, Func<TSource, TSecondKey> keySelector, IComparer<TSecondKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                , allows ref struct
#endif
            => new(source.Enumerator.ThenBy(Throws.IfNull(keySelector), comparer));

        public static ValueEnumerable<OrderBy<TEnumerator, TSource, TSecondKey>, TSource> ThenByDescending<TEnumerator, TSource, TKey, TSecondKey>(this ValueEnumerable<OrderBy<TEnumerator, TSource, TKey>, TSource> source, Func<TSource, TSecondKey> keySelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                , allows ref struct
#endif
            => new(source.Enumerator.ThenByDescending(Throws.IfNull(keySelector)));

        public static ValueEnumerable<OrderBy<TEnumerator, TSource, TSecondKey>, TSource> ThenByDescending<TEnumerator, TSource, TKey, TSecondKey>(this ValueEnumerable<OrderBy<TEnumerator, TSource, TKey>, TSource> source, Func<TSource, TSecondKey> keySelector, IComparer<TSecondKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                , allows ref struct
#endif
            => new(source.Enumerator.ThenByDescending(Throws.IfNull(keySelector), comparer));

        // Skip Take Optimization

        public static ValueEnumerable<OrderBySkipTake<TEnumerator, TSource, TKey>, TSource> Skip<TEnumerator, TSource, TKey>(this ValueEnumerable<OrderBy<TEnumerator, TSource, TKey>, TSource> source, int count)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return new(new(source.Enumerator, Math.Max(0, count), int.MaxValue));
        }

        public static ValueEnumerable<OrderBySkipTake<TEnumerator, TSource, TKey>, TSource> Take<TEnumerator, TSource, TKey>(this ValueEnumerable<OrderBy<TEnumerator, TSource, TKey>, TSource> source, int count)
    where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            count = Math.Max(0, count);
            if (count == 0)
            {
                // Empty case: return an OrderBySkipTake that represents an empty result
                return new(new(source.Enumerator, 0, -1));
            }
            return new(new(source.Enumerator, 0, count - 1));
        }

        public static ValueEnumerable<OrderBySkipTake<TEnumerator, TSource, TKey>, TSource> Skip<TEnumerator, TSource, TKey>(this ValueEnumerable<OrderBySkipTake<TEnumerator, TSource, TKey>, TSource> source, int count)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return new(source.Enumerator.Skip(Math.Max(0, count)));
        }

        public static ValueEnumerable<OrderBySkipTake<TEnumerator, TSource, TKey>, TSource> Take<TEnumerator, TSource, TKey>(this ValueEnumerable<OrderBySkipTake<TEnumerator, TSource, TKey>, TSource> source, int count)
    where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            count = Math.Max(0, count);
            return new(source.Enumerator.Take(count));
        }
    }
}

namespace ZLinq.Linq
{
    // OrderBy has ThenBy/ThenByDescending instance methods.

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct OrderBy<TEnumerator, TSource, TKey>(TEnumerator source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer, IOrderByComparable<TSource>? parent, bool descending)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
    {
        internal TEnumerator source = source;
        internal OrderByComparable<TSource, TKey> comparable = new(keySelector, comparer, parent, descending); // boxed, this is safe to copy as struct state

        RentedArrayBox<TSource>? buffer;
        int index;

        public bool TryGetNonEnumeratedCount(out int count) => source.TryGetNonEnumeratedCount(out count);

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            InitBuffer();
            span = buffer.Span;
            return true;
        }

        public bool TryCopyTo(scoped Span<TSource> destination, Index offset)
        {
            // in-place sort needs full src buffer(no offset)
            if (source.TryGetNonEnumeratedCount(out var count) && offset.GetOffset(count) == 0)
            {
                // destination must be larger than source
                if (destination.Length >= count)
                {
                    // and ok to copy then sort.
                    if (source.TryCopyTo(destination, 0))
                    {
                        Sort(destination.Slice(0, count));
                        return true;
                    }
                }
            }

            if (destination.Length == 1)
            {
                // Try to use quickselect algorithm instead of full sort
                var (elementArray, elementCount) = new ValueEnumerable<TEnumerator, TSource>(source).ToArrayPool();

                var elementAt = offset.GetOffset(elementCount);
                if (elementCount == 0)
                {
                    buffer = RentedArrayBox<TSource>.Empty;
                    return false;
                }
                else if (unchecked((uint)elementAt) >= elementCount)
                {
                    // same as InitBuffer()
                    buffer = new RentedArrayBox<TSource>(elementArray, elementCount); // keep rental array(don't return in this method)
                    Sort(buffer.Span);
                    return false;
                }

                try
                {
                    var span = elementArray.AsSpan(0, elementCount);
                    var (indexMap, size) = ValueEnumerable.Range(0, span.Length).ToArrayPool();

                    using var comparer = comparable.GetComparer(span, null!);

                    int elementIndex;
                    if (elementAt == 0)
                    {
                        elementIndex = OrderByHelper.Min(indexMap, comparer, count: size); // First
                    }
                    else if (elementAt == size - 1)
                    {
                        elementIndex = OrderByHelper.Max(indexMap, comparer, count: size); // Last
                    }
                    else
                    {
                        elementIndex = OrderByHelper.QuickSelect(indexMap, comparer, right: size - 1, idx: elementAt); // ElementAt
                    }

                    destination[0] = span[elementIndex];

                    ArrayPool<int>.Shared.Return(indexMap, clearArray: false);
                }
                finally
                {
                    ArrayPool<TSource>.Shared.Return(elementArray, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<TSource>());
                }

                return true;
            }

            InitBuffer();
            if (EnumeratorHelper.TryGetSlice<TSource>(buffer.Span, offset, destination.Length, out var slice))
            {
                slice.CopyTo(destination);
                return true;
            }

            return false;
        }

        public bool TryGetNext(out TSource current)
        {
            InitBuffer();

            if ((uint)index < (uint)buffer.Length)
            {
                current = buffer.UnsafeGetAt(index);
                index++;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            buffer?.Dispose();
            source.Dispose();
        }

        [MemberNotNull(nameof(buffer))]
        void InitBuffer()
        {
            if (buffer == null)
            {
                var (array, count) = new ValueEnumerable<TEnumerator, TSource>(source).ToArrayPool();
                buffer = new RentedArrayBox<TSource>(array, count);
                Sort(buffer.Span);
            }
        }

        void Sort(scoped Span<TSource> span)
        {
            if (IsAllowDirectSort())
            {
                if (descending)
                {
                    span.Sort(DescendingDefaultComparer<TSource>.Default);
                }
                else
                {
                    span.Sort();
                }
            }
            else
            {
                // faster(using SIMD) index map creation
                var (indexMap, size) = ValueEnumerable.Range(0, span.Length).ToArrayPool();

                using var comparer = comparable.GetComparer(span, null!);
                indexMap.AsSpan(0, size).Sort(span, comparer);
                ArrayPool<int>.Shared.Return(indexMap, clearArray: false);
            }
        }

        public OrderBy<TEnumerator, TSource, TSecondKey> ThenBy<TSecondKey>(Func<TSource, TSecondKey> keySelector, IComparer<TSecondKey>? comparer = null)
        {
            ArgumentNullException.ThrowIfNull(keySelector);
            return new OrderBy<TEnumerator, TSource, TSecondKey>(source, keySelector, comparer, this.comparable, descending: false);
        }

        public OrderBy<TEnumerator, TSource, TSecondKey> ThenByDescending<TSecondKey>(Func<TSource, TSecondKey> keySelector, IComparer<TSecondKey>? comparer = null)
        {
            ArgumentNullException.ThrowIfNull(keySelector);
            return new OrderBy<TEnumerator, TSource, TSecondKey>(source, keySelector, comparer, this.comparable, descending: true);
        }

        bool IsAllowDirectSort()
        {
            if (parent == null && keySelector == UnsafeFunctions<TSource, TKey>.Identity && OrderByHelper.TypeIsImplicitlyStable<TSource>() && (comparer is null || comparer == Comparer<TSource>.Default))
            {
                return true;
            }
            return false;
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct OrderBySkipTake<TEnumerator, TSource, TKey>
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
    {
        TEnumerator source;
        OrderByComparable<TSource, TKey> comparable;
        int minIndexInclusive;
        int maxIndexInclusive;

        RentedArrayBox<TSource>? buffer;
        RentedArrayBox<int>? indexMap;
        int maxIndex;
        int index;

        public OrderBySkipTake(OrderBy<TEnumerator, TSource, TKey> orderBy, int minIndexInclusive, int maxIndexInclusive)
        {
            this.source = orderBy.source;
            this.comparable = orderBy.comparable;
            this.minIndexInclusive = minIndexInclusive;
            this.maxIndexInclusive = maxIndexInclusive;
        }

        OrderBySkipTake(TEnumerator source, OrderByComparable<TSource, TKey> comparable, int minIndexInclusive, int maxIndexInclusive)
        {
            this.source = source;
            this.comparable = comparable;
            this.minIndexInclusive = minIndexInclusive;
            this.maxIndexInclusive = maxIndexInclusive;
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (source.TryGetNonEnumeratedCount(out count))
            {
                if (count <= minIndexInclusive || maxIndexInclusive < minIndexInclusive)
                {
                    count = 0;
                    return true;
                }

                count = (count <= maxIndexInclusive ? count : maxIndexInclusive + 1) - minIndexInclusive;
                return true;
            }

            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<TSource> destination, Index offset)
        {
            if (maxIndexInclusive < minIndexInclusive)
            {
                // Empty case: no elements to copy
                return destination.Length == 0;
            }

            InitBuffer();
            if (indexMap != null)
            {
                var indexSlice = indexMap.Span.Slice(minIndexInclusive, (int)Math.Min((uint)maxIndexInclusive - minIndexInclusive + 1, (uint)indexMap.Span.Length - minIndexInclusive));

                if (EnumeratorHelper.TryGetSlice<int>(indexSlice, offset, destination.Length, out var slice))
                {
                    for (var i = 0; (uint)i < (uint)slice.Length; i++)
                    {
                        destination[i] = buffer.UnsafeGetAt(slice[i]);
                    }
                    return true;
                }
            }
            return false;
        }

        public bool TryGetNext(out TSource current)
        {
            if (maxIndexInclusive < minIndexInclusive)
            {
                // Empty case: no elements to return
                Unsafe.SkipInit(out current);
                return false;
            }

            var buf = buffer;
            if (buf == null)
            {
                InitBuffer();
                buf = buffer;
            }

            var i = index + minIndexInclusive;
            var map = indexMap;
            if (map != null && i <= maxIndex)
            {
                current = buf.UnsafeGetAt(map!.UnsafeGetAt(i));
                index++;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            buffer?.Dispose();
            indexMap?.Dispose();
            source.Dispose();
        }

        [MemberNotNull(nameof(buffer))]
        void InitBuffer()
        {
            if (buffer == null)
            {
                var (array, count) = new ValueEnumerable<TEnumerator, TSource>(source).ToArrayPool();
                buffer = new RentedArrayBox<TSource>(array, count);
                Sort(buffer.Span);
            }
        }

        void Sort(ReadOnlySpan<TSource> span) // not sort element, only indexes
        {
            var count = span.Length;
            if (count > minIndexInclusive && maxIndexInclusive >= minIndexInclusive)
            {
                maxIndex = maxIndexInclusive;
                if (count <= maxIndex)
                {
                    maxIndex = count - 1;
                }

                var (indexMapSource, size) = ValueEnumerable.Range(0, span.Length).ToArrayPool();
                indexMap = new RentedArrayBox<int>(indexMapSource, size);

                using var comparer = comparable.GetComparer(span, null!);
                OrderByHelper.PartialQuickSort(indexMapSource, comparer, left: 0, right: size - 1, minIdx: minIndexInclusive, maxIdx: maxIndex);
            }
        }

        internal OrderBySkipTake<TEnumerator, TSource, TKey> Skip(int count)
        {
            var minIndex = minIndexInclusive + count;
            if ((uint)minIndex > (uint)maxIndexInclusive)
            {
                return new OrderBySkipTake<TEnumerator, TSource, TKey>(source, comparable, 0, -1); // Empty
            }
            else
            {
                return new OrderBySkipTake<TEnumerator, TSource, TKey>(source, comparable, minIndex, maxIndexInclusive);
            }
        }

        internal OrderBySkipTake<TEnumerator, TSource, TKey> Take(int count)
        {
            count = Math.Max(0, count);
            if (count == 0)
            {
                // Empty case: return OrderBySkipTake that represents empty result
                return new OrderBySkipTake<TEnumerator, TSource, TKey>(source, comparable, 0, -1);
            }

            var maxIndex = minIndexInclusive + count - 1;
            if ((uint)maxIndex >= (uint)maxIndexInclusive)
            {
                return this;
            }

            return new OrderBySkipTake<TEnumerator, TSource, TKey>(source, comparable, minIndexInclusive, maxIndex);
        }
    }

    // my previously implementation in js: https://github.com/neuecc/linq.js/blob/v3/linq.js#L2672-L2760

    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IOrderByComparable<TSource>
    {
        IOrderByComparer GetComparer(ReadOnlySpan<TSource> source, IOrderByComparer? childComparer);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IOrderByComparer : IComparer<int>, IDisposable
    {
    }

    internal sealed class OrderByComparable<TSource, TKey>(Func<TSource, TKey> keySelector, IComparer<TKey>? comparer, IOrderByComparable<TSource>? parent, bool descending) : IOrderByComparable<TSource>
    {
        IComparer<TKey> comparer = comparer ?? Comparer<TKey>.Default;

        public IOrderByComparer GetComparer(ReadOnlySpan<TSource> source, IOrderByComparer? childComparer)
        {
            var nextComparer = new OrderByComparer<TSource, TKey>(source, keySelector, comparer, childComparer, descending);
            return parent?.GetComparer(source, nextComparer) ?? nextComparer;
        }
    }

    internal sealed class OrderByComparer<TSource, TKey> : IOrderByComparer
    {
        TKey[] keys = default!; // must call Initialize before Compare
        IComparer<TKey> comparer;
        IOrderByComparer? childComparer;
        bool descending;

        public OrderByComparer(ReadOnlySpan<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer, IOrderByComparer? childComparer, bool descending)
        {
            var tempArray = ArrayPool<TKey>.Shared.Rent(source.Length);
            for (var i = 0; (uint)i < (uint)source.Length; i++)
            {
                tempArray[i] = keySelector(source[i]);
            }
            this.keys = tempArray; // Compare range is [0, source.Length).
            this.comparer = comparer;
            this.childComparer = childComparer;
            this.descending = descending;
        }

        // index based sorting
        public int Compare(int index1, int index2)
        {
            var compareResult = comparer.Compare(keys[index1], keys[index2]);
            if (compareResult != 0)
            {
                // -int.MinValue does not get its sign flipped and remains as int.MinValue.
                // Since only -1, 1 are needed for comparison, normalization is done here.
                var sign = (compareResult > 0) ? 1 : -1;
                return descending ? -sign : sign;
            }

            // same result => thenBy
            if (childComparer != null)
            {
                return childComparer.Compare(index1, index2);
            }

            // finally compare index to ensure stable sort
            if (index1 == index2) return 0;
            return (index1 < index2) ? -1 : 1;
        }

        public void Dispose()
        {
            if (keys != null)
            {
                ArrayPool<TKey>.Shared.Return(keys, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<TKey>());
                keys = null!;
                if (childComparer != null)
                {
                    childComparer.Dispose();
                    childComparer = null;
                }
            }
        }
    }

    file static class UnsafeFunctions<T, U>
    {
        public static readonly Func<T, U> Identity = static x => Unsafe.As<T, U>(ref x);
    }

    file class DescendingDefaultComparer<T> : IComparer<T>
    {
        public static IComparer<T> Default = new DescendingDefaultComparer<T>();

        DescendingDefaultComparer()
        {

        }

        public int Compare(T? x, T? y)
        {
            return Comparer<T?>.Default.Compare(y, x);
        }
    }

    file static class OrderByHelper
    {
        // dotnet/runtime optimized types
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool TypeIsImplicitlyStable<T>()
        {
            Type t = typeof(T);
            if (typeof(T).IsEnum)
            {
                t = typeof(T).GetEnumUnderlyingType();
            }

            return
                t == typeof(sbyte) || t == typeof(byte) || t == typeof(bool) ||
                t == typeof(short) || t == typeof(ushort) || t == typeof(char) ||
                t == typeof(int) || t == typeof(uint) ||
                t == typeof(long) || t == typeof(ulong) ||
#if NET8_0_OR_GREATER
                t == typeof(Int128) || t == typeof(UInt128) ||
#endif
                t == typeof(nint) || t == typeof(nuint);
        }

        // based dotnet/runtime implementation

        internal static int QuickSelect(int[] map, IComparer<int> comparer, int right, int idx)
        {
            int left = 0;
            do
            {
                int i = left;
                int j = right;
                int x = map[i + ((j - i) >> 1)];
                do
                {
                    while (i < map.Length && comparer.Compare(x, map[i]) > 0)
                    {
                        i++;
                    }

                    while (j >= 0 && comparer.Compare(x, map[j]) < 0)
                    {
                        j--;
                    }

                    if (i > j)
                    {
                        break;
                    }

                    if (i < j)
                    {
                        int temp = map[i];
                        map[i] = map[j];
                        map[j] = temp;
                    }

                    i++;
                    j--;
                }
                while (i <= j);

                if (i <= idx)
                {
                    left = i + 1;
                }
                else
                {
                    right = j - 1;
                }

                if (j - left <= right - i)
                {
                    if (left < j)
                    {
                        right = j;
                    }

                    left = i;
                }
                else
                {
                    if (i < right)
                    {
                        left = i;
                    }

                    right = j;
                }
            }
            while (left < right);

            return map[idx];
        }

        internal static void PartialQuickSort(int[] map, IComparer<int> comparer, int left, int right, int minIdx, int maxIdx)
        {
            do
            {
                int i = left;
                int j = right;
                int x = map[i + ((j - i) >> 1)];
                do
                {
                    while (i < map.Length && comparer.Compare(x, map[i]) > 0)
                    {
                        i++;
                    }

                    while (j >= 0 && comparer.Compare(x, map[j]) < 0)
                    {
                        j--;
                    }

                    if (i > j)
                    {
                        break;
                    }

                    if (i < j)
                    {
                        int temp = map[i];
                        map[i] = map[j];
                        map[j] = temp;
                    }

                    i++;
                    j--;
                }
                while (i <= j);

                if (minIdx >= i)
                {
                    left = i + 1;
                }
                else if (maxIdx <= j)
                {
                    right = j - 1;
                }

                if (j - left <= right - i)
                {
                    if (left < j)
                    {
                        PartialQuickSort(map, comparer, left, j, minIdx, maxIdx);
                    }

                    left = i;
                }
                else
                {
                    if (i < right)
                    {
                        PartialQuickSort(map, comparer, i, right, minIdx, maxIdx);
                    }

                    right = j;
                }
            }
            while (left < right);
        }

        internal static int Min(int[] map, IComparer<int> comparer, int count)
        {
            int index = 0;
            for (int i = 1; i < count; i++)
            {
                if (comparer.Compare(map[i], map[index]) < 0)
                {
                    index = i;
                }
            }
            return map[index];
        }

        internal static int Max(int[] map, IComparer<int> comparer, int count)
        {
            int index = 0;
            for (int i = 1; i < count; i++)
            {
                if (comparer.Compare(map[i], map[index]) >= 0)
                {
                    index = i;
                }
            }
            return map[index];
        }
    }
}
