namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Produces the set union of two sequences according to a specified key selector function.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="second">A sequence whose distinct elements form the second set for the union.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <returns>A sequence that contains the elements from both sequences, excluding duplicates.</returns>
        public static ValueEnumerable<UnionBy<TEnumerator, TEnumerator2, TSource, TKey>, TSource> UnionBy<TEnumerator, TEnumerator2, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, ValueEnumerable<TEnumerator2, TSource> second, Func<TSource, TKey> keySelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, second.Enumerator, Throws.IfNull(keySelector), null));

        /// <summary>Produces the set union of two sequences according to a specified key selector function.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="second">A sequence whose distinct elements form the second set for the union.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <param name="comparer">An equality comparer to compare keys, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>A sequence that contains the elements from both sequences, excluding duplicates.</returns>
        public static ValueEnumerable<UnionBy<TEnumerator, TEnumerator2, TSource, TKey>, TSource> UnionBy<TEnumerator, TEnumerator2, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, ValueEnumerable<TEnumerator2, TSource> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, second.Enumerator, Throws.IfNull(keySelector), comparer));

        /// <summary>Produces the set union of two sequences according to a specified key selector function.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="second">A sequence whose distinct elements form the second set for the union.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <returns>A sequence that contains the elements from both sequences, excluding duplicates.</returns>
        public static ValueEnumerable<UnionBy<TEnumerator, FromEnumerable<TSource>, TSource, TKey>, TSource> UnionBy<TEnumerator, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, IEnumerable<TSource> second, Func<TSource, TKey> keySelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(keySelector);
            return new(new(source.Enumerator, Throws.IfNull(second).AsValueEnumerable().Enumerator, Throws.IfNull(keySelector), null));
        }

        /// <summary>Produces the set union of two sequences according to a specified key selector function.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="second">A sequence whose distinct elements form the second set for the union.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <param name="comparer">An equality comparer to compare keys, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>A sequence that contains the elements from both sequences, excluding duplicates.</returns>
        public static ValueEnumerable<UnionBy<TEnumerator, FromEnumerable<TSource>, TSource, TKey>, TSource> UnionBy<TEnumerator, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, IEnumerable<TSource> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(keySelector);
            return new(new(source.Enumerator, Throws.IfNull(second).AsValueEnumerable().Enumerator, Throws.IfNull(keySelector), comparer));
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
    struct UnionBy<TEnumerator, TEnumerator2, TSource, TKey>(TEnumerator source, TEnumerator2 second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        where TEnumerator2 : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        TEnumerator2 second = second;
        HashSetSlim<TKey>? set;
        byte state = 0;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<TSource> destination, Index offset) => false;

        public bool TryGetNext(out TSource current)
        {
            if (state == 0)
            {
                set = new HashSetSlim<TKey>(comparer ?? EqualityComparer<TKey>.Default);
                state = 1;
            }

            if (state == 1)
            {
                while (source.TryGetNext(out var value))
                {
                    if (set!.Add(keySelector(value)))
                    {
                        current = value;
                        return true;
                    }
                }
                state = 2;
            }

            if (state == 2)
            {
                while (second.TryGetNext(out var value))
                {
                    if (set!.Add(keySelector(value)))
                    {
                        current = value;
                        return true;
                    }
                }

                state = 3;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            state = 3;
            set?.Dispose();
            source.Dispose();
            second.Dispose();
        }
    }

}
