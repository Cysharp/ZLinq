namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Determines whether two sequences are equal by comparing their elements.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="second">A sequence to compare to the source sequence.</param>
        /// <returns><see langword="true"/> if the two sequences are of equal length and their corresponding elements compare equal; otherwise, <see langword="false"/>.</returns>
        public static Boolean SequenceEqual<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, IEnumerable<TSource> second)
    where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return SequenceEqual(source, Throws.IfNull(second).AsValueEnumerable(), null);
        }

        /// <summary>Determines whether two sequences are equal by comparing their elements.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="second">A sequence to compare to the source sequence.</param>
        /// <param name="comparer">An equality comparer to compare values, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns><see langword="true"/> if the two sequences are of equal length and their corresponding elements compare equal; otherwise, <see langword="false"/>.</returns>
        public static Boolean SequenceEqual<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, IEnumerable<TSource> second, IEqualityComparer<TSource>? comparer) // comparer is nullable
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return SequenceEqual(source, Throws.IfNull(second).AsValueEnumerable(), comparer);
        }

        /// <summary>Determines whether two sequences are equal by comparing their elements.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="second">A sequence to compare to the source sequence.</param>
        /// <returns><see langword="true"/> if the two sequences are of equal length and their corresponding elements compare equal; otherwise, <see langword="false"/>.</returns>
        public static Boolean SequenceEqual<TEnumerator, TEnumerator2, TSource>(this ValueEnumerable<TEnumerator, TSource> source, ValueEnumerable<TEnumerator2, TSource> second)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return SequenceEqual(source, second, null);
        }

        /// <summary>Determines whether two sequences are equal by comparing their elements.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="second">A sequence to compare to the source sequence.</param>
        /// <param name="comparer">An equality comparer to compare values, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns><see langword="true"/> if the two sequences are of equal length and their corresponding elements compare equal; otherwise, <see langword="false"/>.</returns>
        public static Boolean SequenceEqual<TEnumerator, TEnumerator2, TSource>(this ValueEnumerable<TEnumerator, TSource> source, ValueEnumerable<TEnumerator2, TSource> second, IEqualityComparer<TSource>? comparer) // comparer is nullable
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using var e1 = source.Enumerator;
            using var e2 = second.Enumerator;

            if (e1.TryGetNonEnumeratedCount(out var count1) && e2.TryGetNonEnumeratedCount(out var count2) && count1 != count2)
            {
                return false;
            }

#if NET8_0_OR_GREATER
            if (e1.TryGetSpan(out var sourceSpan) && e2.TryGetSpan(out var secondSpan))
            {
                return sourceSpan.SequenceEqual(secondSpan, comparer); // SIMD acceleration
            }
#endif

            comparer ??= EqualityComparer<TSource>.Default;
            while (e1.TryGetNext(out var value1))
            {
                if (!(e2.TryGetNext(out var value2) && comparer.Equals(value1, value2)))
                {
                    return false;
                }
            }

            return !e2.TryGetNext(out _);
        }
    }
}
