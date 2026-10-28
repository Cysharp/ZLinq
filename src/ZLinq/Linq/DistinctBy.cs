namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Returns distinct elements from a sequence according to a specified key selector function.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <returns>A sequence that contains distinct elements from the source sequence.</returns>
        public static ValueEnumerable<DistinctBy<TEnumerator, TSource, TKey>, TSource> DistinctBy<TEnumerator, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(keySelector), null));

        /// <summary>Returns distinct elements from a sequence according to a specified key selector function.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <param name="comparer">An equality comparer to compare keys, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>A sequence that contains distinct elements from the source sequence.</returns>
        public static ValueEnumerable<DistinctBy<TEnumerator, TSource, TKey>, TSource> DistinctBy<TEnumerator, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(keySelector), comparer));
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
    struct DistinctBy<TEnumerator, TSource, TKey>(TEnumerator source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        HashSetSlim<TKey>? set;

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
            if (set == null)
            {
                set = new HashSetSlim<TKey>(comparer ?? EqualityComparer<TKey>.Default);
            }

            while (source.TryGetNext(out var value))
            {
                if (set.Add(keySelector(value)))
                {
                    current = value;
                    return true;
                }
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            set?.Dispose();
            source.Dispose();
        }
    }
}
