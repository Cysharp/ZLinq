namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Correlates the elements of two sequences based on matching keys. The elements of the inner sequence without a match are also returned.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="inner">The sequence to join to the source sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the source sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the inner sequence.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements, or from the default value of the outer element and an element of the inner sequence if it has no match.</param>
        /// <returns>A sequence that contains elements of the result type that are obtained by performing a right outer join on two sequences.</returns>
        public static ValueEnumerable<RightJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey, TResult>, TResult> RightJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey, TResult>(this ValueEnumerable<TEnumerator, TOuter> source, ValueEnumerable<TEnumerator2, TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter?, TInner, TResult> resultSelector)
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TInner>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, inner.Enumerator, Throws.IfNull(outerKeySelector), Throws.IfNull(innerKeySelector), Throws.IfNull(resultSelector), null));

        /// <summary>Correlates the elements of two sequences based on matching keys. The elements of the inner sequence without a match are also returned.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="inner">The sequence to join to the source sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the source sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the inner sequence.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements, or from the default value of the outer element and an element of the inner sequence if it has no match.</param>
        /// <param name="comparer">An equality comparer to compare keys, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>A sequence that contains elements of the result type that are obtained by performing a right outer join on two sequences.</returns>
        public static ValueEnumerable<RightJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey, TResult>, TResult> RightJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey, TResult>(this ValueEnumerable<TEnumerator, TOuter> source, ValueEnumerable<TEnumerator2, TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter?, TInner, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TInner>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, inner.Enumerator, Throws.IfNull(outerKeySelector), Throws.IfNull(innerKeySelector), Throws.IfNull(resultSelector), comparer));


        /// <summary>Correlates the elements of two sequences based on matching keys. The elements of the inner sequence without a match are also returned.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="inner">The sequence to join to the source sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the source sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the inner sequence.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements, or from the default value of the outer element and an element of the inner sequence if it has no match.</param>
        /// <returns>A sequence that contains elements of the result type that are obtained by performing a right outer join on two sequences.</returns>
        public static ValueEnumerable<RightJoin<TEnumerator, FromEnumerable<TInner>, TOuter, TInner, TKey, TResult>, TResult> RightJoin<TEnumerator, TOuter, TInner, TKey, TResult>(this ValueEnumerable<TEnumerator, TOuter> source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter?, TInner, TResult> resultSelector)
    where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
    => new(new(source.Enumerator, Throws.IfNull(inner).AsValueEnumerable().Enumerator, Throws.IfNull(outerKeySelector), Throws.IfNull(innerKeySelector), Throws.IfNull(resultSelector), null));

        /// <summary>Correlates the elements of two sequences based on matching keys. The elements of the inner sequence without a match are also returned.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="inner">The sequence to join to the source sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the source sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the inner sequence.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements, or from the default value of the outer element and an element of the inner sequence if it has no match.</param>
        /// <param name="comparer">An equality comparer to compare keys, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>A sequence that contains elements of the result type that are obtained by performing a right outer join on two sequences.</returns>
        public static ValueEnumerable<RightJoin<TEnumerator, FromEnumerable<TInner>, TOuter, TInner, TKey, TResult>, TResult> RightJoin<TEnumerator, TOuter, TInner, TKey, TResult>(this ValueEnumerable<TEnumerator, TOuter> source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter?, TInner, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(inner).AsValueEnumerable().Enumerator, Throws.IfNull(outerKeySelector), Throws.IfNull(innerKeySelector), Throws.IfNull(resultSelector), comparer));

        /// <summary>Correlates the elements of two sequences based on matching keys. The elements of the inner sequence without a match are also returned.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="inner">The sequence to join to the source sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the source sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the inner sequence.</param>
        /// <param name="comparer">An equality comparer to compare keys, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>A sequence that contains tuples of elements, obtained by performing a right outer join on two sequences. The outer element is the default value if it has no match.</returns>
        public static ValueEnumerable<RightJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey>, (TOuter? Outer, TInner Inner)> RightJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey>(this ValueEnumerable<TEnumerator, TOuter> source, ValueEnumerable<TEnumerator2, TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, IEqualityComparer<TKey>? comparer = null)
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TInner>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, inner.Enumerator, Throws.IfNull(outerKeySelector), Throws.IfNull(innerKeySelector), comparer));

        /// <summary>Correlates the elements of two sequences based on matching keys. The elements of the inner sequence without a match are also returned.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="inner">The sequence to join to the source sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the source sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the inner sequence.</param>
        /// <param name="comparer">An equality comparer to compare keys, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>A sequence that contains tuples of elements, obtained by performing a right outer join on two sequences. The outer element is the default value if it has no match.</returns>
        public static ValueEnumerable<RightJoin<TEnumerator, FromEnumerable<TInner>, TOuter, TInner, TKey>, (TOuter? Outer, TInner Inner)> RightJoin<TEnumerator, TOuter, TInner, TKey>(this ValueEnumerable<TEnumerator, TOuter> source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, IEqualityComparer<TKey>? comparer = null)
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(inner).AsValueEnumerable().Enumerator, Throws.IfNull(outerKeySelector), Throws.IfNull(innerKeySelector), comparer));
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
    struct RightJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey, TResult>(TEnumerator source, TEnumerator2 inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter?, TInner, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerator<TResult>
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TInner>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
    {
        TEnumerator source = source;
        TEnumerator2 inner = inner;

        Lookup<TKey, TOuter>? outerLookup;
        Grouping<TKey, TOuter>? currentGroup;
        int currentGroupIndex;
        TInner currentInner = default!;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<TResult> destination, Index offset) => false;

        public bool TryGetNext(out TResult current)
        {
            if (outerLookup == null)
            {
                try
                {
                    // CreateForJoin excludes null.
                    // If we want to accept null keys, we should create a lookup with null keys and join them.
                    // However, this behavior matches the official dotnet implementation.
                    outerLookup = Lookup.CreateForJoin(ref source, outerKeySelector, comparer);
                }
                finally
                {
                    source.Dispose();
                }
            }

        // iterating group
        ITERATE:
            if (currentGroup != null)
            {
                if (currentGroupIndex < currentGroup.Count)
                {
                    current = resultSelector(currentGroup[currentGroupIndex], currentInner);
                    currentGroupIndex++;
                    return true;
                }
                else
                {
                    currentGroup = null;
                }
            }

            while (inner.TryGetNext(out var value))
            {
                var key = innerKeySelector(value);
                var group = outerLookup.GetGroup(key);
                if (group != null)
                {
                    currentInner = value;
                    currentGroup = group;
                    currentGroupIndex = 0;
                    goto ITERATE;
                }
                else
                {
                    // The difference with Join is how nulls are handled.
                    current = resultSelector(default, value);
                    return true;
                }
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (outerLookup == null) // source is outer
            {
                source.Dispose();
            }
            inner.Dispose();
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct RightJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey>(TEnumerator source, TEnumerator2 inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerator<(TOuter? Outer, TInner Inner)>
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TInner>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
    {
        TEnumerator source = source;
        TEnumerator2 inner = inner;

        Lookup<TKey, TOuter>? outerLookup;
        Grouping<TKey, TOuter>? currentGroup;
        int currentGroupIndex;
        TInner currentInner = default!;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<(TOuter? Outer, TInner Inner)> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<(TOuter? Outer, TInner Inner)> destination, Index offset) => false;

        public bool TryGetNext(out (TOuter? Outer, TInner Inner) current)
        {
            if (outerLookup == null)
            {
                try
                {
                    // CreateForJoin excludes null.
                    // If we want to accept null keys, we should create a lookup with null keys and join them.
                    // However, this behavior matches the official dotnet implementation.
                    outerLookup = Lookup.CreateForJoin(ref source, outerKeySelector, comparer);
                }
                finally
                {
                    source.Dispose();
                }
            }

        // iterating group
        ITERATE:
            if (currentGroup != null)
            {
                if (currentGroupIndex < currentGroup.Count)
                {
                    current = (currentGroup[currentGroupIndex], currentInner);
                    currentGroupIndex++;
                    return true;
                }
                else
                {
                    currentGroup = null;
                }
            }

            while (inner.TryGetNext(out var value))
            {
                var key = innerKeySelector(value);
                var group = outerLookup.GetGroup(key);
                if (group != null)
                {
                    currentInner = value;
                    currentGroup = group;
                    currentGroupIndex = 0;
                    goto ITERATE;
                }
                else
                {
                    // The difference with Join is how nulls are handled.
                    current = (default, value);
                    return true;
                }
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (outerLookup == null) // source is outer
            {
                source.Dispose();
            }
            inner.Dispose();
        }
    }
}
