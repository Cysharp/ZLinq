namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Returns distinct elements from a sequence.</summary>
        /// <param name="source">The source sequence.</param>
        /// <returns>A sequence that contains distinct elements from the source sequence.</returns>
        public static ValueEnumerable<Distinct<TEnumerator, TSource>, TSource> Distinct<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, null!));

        /// <summary>Returns distinct elements from a sequence.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="comparer">An equality comparer to compare values, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>A sequence that contains distinct elements from the source sequence.</returns>
        public static ValueEnumerable<Distinct<TEnumerator, TSource>, TSource> Distinct<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, IEqualityComparer<TSource>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, comparer));

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
    struct Distinct<TEnumerator, TSource>(TEnumerator source, IEqualityComparer<TSource>? comparer)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
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

        public bool TryCopyTo(scoped Span<TSource> destination, Index offset)
        {
            if (destination.Length == 1 && offset.Value == 0) // as TryGetFirst
            {
                return source.TryCopyTo(destination, offset);
            }

            return false;
        }

        public bool TryGetNext(out TSource current)
        {
            if (set == null)
            {
                set = new HashSetSlim<TSource>(comparer ?? EqualityComparer<TSource>.Default);
            }

            while (source.TryGetNext(out var value))
            {
                if (set.Add(value))
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
