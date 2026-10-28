namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Produces the set intersection of two sequences.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="second">A sequence whose distinct elements that also appear in the source sequence will be returned.</param>
        /// <returns>A sequence that contains the elements that form the set intersection of two sequences.</returns>
        public static ValueEnumerable<Intersect<TEnumerator, TEnumerator2, TSource>, TSource> Intersect<TEnumerator, TEnumerator2, TSource>(this ValueEnumerable<TEnumerator, TSource> source, ValueEnumerable<TEnumerator2, TSource> second)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, second, null));

        /// <summary>Produces the set intersection of two sequences.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="second">A sequence whose distinct elements that also appear in the source sequence will be returned.</param>
        /// <param name="comparer">An equality comparer to compare values, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>A sequence that contains the elements that form the set intersection of two sequences.</returns>
        public static ValueEnumerable<Intersect<TEnumerator, TEnumerator2, TSource>, TSource> Intersect<TEnumerator, TEnumerator2, TSource>(this ValueEnumerable<TEnumerator, TSource> source, ValueEnumerable<TEnumerator2, TSource> second, IEqualityComparer<TSource>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, second, comparer));



        /// <summary>Produces the set intersection of two sequences.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="second">A sequence whose distinct elements that also appear in the source sequence will be returned.</param>
        /// <returns>A sequence that contains the elements that form the set intersection of two sequences.</returns>
        public static ValueEnumerable<Intersect<TEnumerator, FromEnumerable<TSource>, TSource>, TSource> Intersect<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, IEnumerable<TSource> second)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(second).AsValueEnumerable(), null));

        /// <summary>Produces the set intersection of two sequences.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="second">A sequence whose distinct elements that also appear in the source sequence will be returned.</param>
        /// <param name="comparer">An equality comparer to compare values, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>A sequence that contains the elements that form the set intersection of two sequences.</returns>
        public static ValueEnumerable<Intersect<TEnumerator, FromEnumerable<TSource>, TSource>, TSource> Intersect<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, IEnumerable<TSource> second, IEqualityComparer<TSource>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(second).AsValueEnumerable(), comparer));
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
    struct Intersect<TEnumerator, TEnumerator2, TSource>(TEnumerator source, ValueEnumerable<TEnumerator2, TSource> second, IEqualityComparer<TSource>? comparer)
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
        ValueEnumerable<TEnumerator2, TSource> second = second;
        HashSetSlim<TSource>? set;

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
                set = second.ToHashSetSlim(comparer);
            }

            while (source.TryGetNext(out var value))
            {
                if (set.Remove(value))
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
