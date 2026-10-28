using System;

namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Concatenates two sequences.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="second">The sequence to concatenate to the source sequence.</param>
        /// <returns>A sequence that contains the concatenated elements of the two sequences.</returns>
        public static ValueEnumerable<Concat<TEnumerator1, TEnumerator2, TSource>, TSource> Concat<TEnumerator1, TEnumerator2, TSource>(this ValueEnumerable<TEnumerator1, TSource> source, ValueEnumerable<TEnumerator2, TSource> second)
            where TEnumerator1 : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, second.Enumerator));


        /// <summary>Concatenates two sequences.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="second">The sequence to concatenate to the source sequence.</param>
        /// <returns>A sequence that contains the concatenated elements of the two sequences.</returns>
        public static ValueEnumerable<Concat<TEnumerator1, FromEnumerable<TSource>, TSource>, TSource> Concat<TEnumerator1, TSource>(this ValueEnumerable<TEnumerator1, TSource> source, IEnumerable<TSource> second)
            where TEnumerator1 : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(second);
            return Concat(source, second.AsValueEnumerable());
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct Concat<TEnumerator1, TEnumerator2, TSource>(TEnumerator1 first, TEnumerator2 second)
        : IValueEnumerator<TSource>
        where TEnumerator1 : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        where TEnumerator2 : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator1 first = first;
        TEnumerator2 second = second;
        bool firstCompleted;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (first.TryGetNonEnumeratedCount(out var count1) && second.TryGetNonEnumeratedCount(out var count2))
            {
                count = checked(count1 + count2);
                return true;
            }
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<TSource> destination, Index offset)
        {
            // return false because source1 succeeded but source2 failed, it is high-cost operation
            // Also, if source1 succeeds and source2 fails, there is a possibility that source1's TryCopyTo completes and TryGetNext stops working.
            // ZLinq's semantics do not guarantee the operation of other operations if TryCopyTo succeeds.
            // Therefore, it must be set to false.
            return false;
        }

        public bool TryGetNext(out TSource current)
        {
            if (!firstCompleted)
            {
                // iterate first
                if (first.TryGetNext(out current))
                {
                    return true;
                }
                first.Dispose();
                firstCompleted = true;
            }

            // iterate second
            if (second.TryGetNext(out current))
            {
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (!firstCompleted)
            {
                first.Dispose();
            }
            second.Dispose();
        }
    }
}
