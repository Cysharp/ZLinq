namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Produces the set union of two sequences.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="second">A sequence whose distinct elements form the second set for the union.</param>
        /// <returns>A sequence that contains the elements from both sequences, excluding duplicates.</returns>
        public static ValueEnumerable<Union<TEnumerator, TEnumerator2, TSource>, TSource> Union<TEnumerator, TEnumerator2, TSource>(this ValueEnumerable<TEnumerator, TSource> source, ValueEnumerable<TEnumerator2, TSource> second)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, second.Enumerator, null));

        /// <summary>Produces the set union of two sequences.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="second">A sequence whose distinct elements form the second set for the union.</param>
        /// <param name="comparer">An equality comparer to compare values, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>A sequence that contains the elements from both sequences, excluding duplicates.</returns>
        public static ValueEnumerable<Union<TEnumerator, TEnumerator2, TSource>, TSource> Union<TEnumerator, TEnumerator2, TSource>(this ValueEnumerable<TEnumerator, TSource> source, ValueEnumerable<TEnumerator2, TSource> second, IEqualityComparer<TSource>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, second.Enumerator, comparer));


        /// <summary>Produces the set union of two sequences.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="second">A sequence whose distinct elements form the second set for the union.</param>
        /// <returns>A sequence that contains the elements from both sequences, excluding duplicates.</returns>
        public static ValueEnumerable<Union<TEnumerator, FromEnumerable<TSource>, TSource>, TSource> Union<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, IEnumerable<TSource> second)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(second).AsValueEnumerable().Enumerator, null));

        /// <summary>Produces the set union of two sequences.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="second">A sequence whose distinct elements form the second set for the union.</param>
        /// <param name="comparer">An equality comparer to compare values, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>A sequence that contains the elements from both sequences, excluding duplicates.</returns>
        public static ValueEnumerable<Union<TEnumerator, FromEnumerable<TSource>, TSource>, TSource> Union<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, IEnumerable<TSource> second, IEqualityComparer<TSource>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(second).AsValueEnumerable().Enumerator, comparer));
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
   struct Union<TEnumerator, TEnumerator2, TSource>(TEnumerator source, TEnumerator2 second, IEqualityComparer<TSource>? comparer)
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
        HashSetSlim<TSource>? set;
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
            if (state == 0)
            {
                set = new HashSetSlim<TSource>(comparer ?? EqualityComparer<TSource>.Default);
                state = 1;
            }

            if (state == 1)
            {
                while (source.TryGetNext(out var value))
                {
                    if (set!.Add(value))
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
                    if (set!.Add(value))
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
