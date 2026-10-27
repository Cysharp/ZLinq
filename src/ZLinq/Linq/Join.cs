namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<Join<TEnumerator, TEnumerator2, TOuter, TInner, TKey, TResult>, TResult> Join<TEnumerator, TEnumerator2, TOuter, TInner, TKey, TResult>(this ValueEnumerable<TEnumerator, TOuter> source, ValueEnumerable<TEnumerator2, TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TInner>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, inner.Enumerator, Throws.IfNull(outerKeySelector), Throws.IfNull(innerKeySelector), Throws.IfNull(resultSelector), null));

        public static ValueEnumerable<Join<TEnumerator, TEnumerator2, TOuter, TInner, TKey, TResult>, TResult> Join<TEnumerator, TEnumerator2, TOuter, TInner, TKey, TResult>(this ValueEnumerable<TEnumerator, TOuter> source, ValueEnumerable<TEnumerator2, TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TInner>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, inner.Enumerator, Throws.IfNull(outerKeySelector), Throws.IfNull(innerKeySelector), Throws.IfNull(resultSelector), comparer));



        public static ValueEnumerable<Join<TEnumerator, FromEnumerable<TInner>, TOuter, TInner, TKey, TResult>, TResult> Join<TEnumerator, TOuter, TInner, TKey, TResult>(this ValueEnumerable<TEnumerator, TOuter> source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(inner).AsValueEnumerable().Enumerator, Throws.IfNull(outerKeySelector), Throws.IfNull(innerKeySelector), Throws.IfNull(resultSelector), null));

        public static ValueEnumerable<Join<TEnumerator, FromEnumerable<TInner>, TOuter, TInner, TKey, TResult>, TResult> Join<TEnumerator, TOuter, TInner, TKey, TResult>(this ValueEnumerable<TEnumerator, TOuter> source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(inner).AsValueEnumerable().Enumerator, Throws.IfNull(outerKeySelector), Throws.IfNull(innerKeySelector), Throws.IfNull(resultSelector), comparer));

        public static ValueEnumerable<Join<TEnumerator, TEnumerator2, TOuter, TInner, TKey>, (TOuter Outer, TInner Inner)> Join<TEnumerator, TEnumerator2, TOuter, TInner, TKey>(this ValueEnumerable<TEnumerator, TOuter> source, ValueEnumerable<TEnumerator2, TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, IEqualityComparer<TKey>? comparer = null)
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TInner>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, inner.Enumerator, Throws.IfNull(outerKeySelector), Throws.IfNull(innerKeySelector), comparer));

        public static ValueEnumerable<Join<TEnumerator, FromEnumerable<TInner>, TOuter, TInner, TKey>, (TOuter Outer, TInner Inner)> Join<TEnumerator, TOuter, TInner, TKey>(this ValueEnumerable<TEnumerator, TOuter> source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, IEqualityComparer<TKey>? comparer = null)
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
    struct Join<TEnumerator, TEnumerator2, TOuter, TInner, TKey, TResult>(TEnumerator source, TEnumerator2 inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
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

        Lookup<TKey, TInner>? innerLookup;
        Grouping<TKey, TInner>? currentGroup;
        int currentGroupIndex;
        TOuter currentOuter = default!;

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
            if (innerLookup == null)
            {
                try
                {
                    innerLookup = Lookup.CreateForJoin(ref inner, innerKeySelector, comparer);
                }
                finally
                {
                    inner.Dispose();
                }
            }

            if (innerLookup.Count == 0)
            {
                goto END;
            }

        // iterating group
        ITERATE:
            if (currentGroup != null)
            {
                if (currentGroupIndex < currentGroup.Count)
                {
                    current = resultSelector(currentOuter, currentGroup[currentGroupIndex]);
                    currentGroupIndex++;
                    return true;
                }
                else
                {
                    currentGroup = null;
                }
            }

            while (source.TryGetNext(out var value))
            {
                var key = outerKeySelector(value);
                if (key is not null) // Enumerable.Join ignores null keys
                {
                    var group = innerLookup.GetGroup(key);
                    if (group != null)
                    {
                        currentOuter = value;
                        currentGroup = group;
                        currentGroupIndex = 0;
                        goto ITERATE;
                    }
                }
            }

        END:
            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (innerLookup == null)
            {
                inner.Dispose();
            }
            source.Dispose();
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct Join<TEnumerator, TEnumerator2, TOuter, TInner, TKey>(TEnumerator source, TEnumerator2 inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerator<(TOuter Outer, TInner Inner)>
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

        Lookup<TKey, TInner>? innerLookup;
        Grouping<TKey, TInner>? currentGroup;
        int currentGroupIndex;
        TOuter currentOuter = default!;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<(TOuter Outer, TInner Inner)> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<(TOuter Outer, TInner Inner)> destination, Index offset) => false;

        public bool TryGetNext(out (TOuter Outer, TInner Inner) current)
        {
            if (innerLookup == null)
            {
                try
                {
                    innerLookup = Lookup.CreateForJoin(ref inner, innerKeySelector, comparer);
                }
                finally
                {
                    inner.Dispose();
                }
            }

            if (innerLookup.Count == 0)
            {
                goto END;
            }

        // iterating group
        ITERATE:
            if (currentGroup != null)
            {
                if (currentGroupIndex < currentGroup.Count)
                {
                    current = (currentOuter, currentGroup[currentGroupIndex]);
                    currentGroupIndex++;
                    return true;
                }
                else
                {
                    currentGroup = null;
                }
            }

            while (source.TryGetNext(out var value))
            {
                var key = outerKeySelector(value);
                if (key is not null) // Enumerable.Join ignores null keys
                {
                    var group = innerLookup.GetGroup(key);
                    if (group != null)
                    {
                        currentOuter = value;
                        currentGroup = group;
                        currentGroupIndex = 0;
                        goto ITERATE;
                    }
                }
            }

        END:
            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (innerLookup == null)
            {
                inner.Dispose();
            }
            source.Dispose();
        }
    }
}
