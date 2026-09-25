namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<FullJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey, TResult>, TResult> FullJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey, TResult>(this ValueEnumerable<TEnumerator, TOuter> source, ValueEnumerable<TEnumerator2, TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter?, TInner?, TResult> resultSelector, IEqualityComparer<TKey>? comparer = null)
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TInner>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, inner.Enumerator, Throws.IfNull(outerKeySelector), Throws.IfNull(innerKeySelector), Throws.IfNull(resultSelector), comparer));

        public static ValueEnumerable<FullJoin<TEnumerator, FromEnumerable<TInner>, TOuter, TInner, TKey, TResult>, TResult> FullJoin<TEnumerator, TOuter, TInner, TKey, TResult>(this ValueEnumerable<TEnumerator, TOuter> source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter?, TInner?, TResult> resultSelector, IEqualityComparer<TKey>? comparer = null)
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(inner).AsValueEnumerable().Enumerator, Throws.IfNull(outerKeySelector), Throws.IfNull(innerKeySelector), Throws.IfNull(resultSelector), comparer));

        public static ValueEnumerable<FullJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey>, (TOuter? Outer, TInner? Inner)> FullJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey>(this ValueEnumerable<TEnumerator, TOuter> source, ValueEnumerable<TEnumerator2, TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, IEqualityComparer<TKey>? comparer = null)
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TInner>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, inner.Enumerator, Throws.IfNull(outerKeySelector), Throws.IfNull(innerKeySelector), comparer));

        public static ValueEnumerable<FullJoin<TEnumerator, FromEnumerable<TInner>, TOuter, TInner, TKey>, (TOuter? Outer, TInner? Inner)> FullJoin<TEnumerator, TOuter, TInner, TKey>(this ValueEnumerable<TEnumerator, TOuter> source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, IEqualityComparer<TKey>? comparer = null)
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
    struct FullJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey, TResult>(TEnumerator source, TEnumerator2 inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter?, TInner?, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
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
        HashSet<Grouping<TKey, TInner>>? matchedGroups;
        Grouping<TKey, TInner>? currentGroup;
        int currentGroupIndex;
        TOuter currentOuter = default!;
        Grouping<TKey, TInner>? nextUnmatchedGroup;
        bool outerCompleted;
        bool streamingInner;

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
                if (streamingInner || FullJoinHelper.IsEmptyArray<TEnumerator, TOuter>(ref source))
                {
                    streamingInner = true;
                    if (inner.TryGetNext(out var value))
                    {
                        current = resultSelector(default, value);
                        return true;
                    }

                    Unsafe.SkipInit(out current);
                    return false;
                }

                try
                {
                    // FullJoin needs to preserve inner elements with null keys so they can be emitted
                    // as unmatched rows, even though null keys still never participate in matches.
                    innerLookup = Lookup.CreateForFullJoin(ref inner, innerKeySelector, comparer);
                }
                finally
                {
                    inner.Dispose();
                }

                if (innerLookup.Count != 0)
                {
                    matchedGroups = new HashSet<Grouping<TKey, TInner>>();
                }
            }

            if (!outerCompleted)
            {
            // iterating matched group
            ITERATE_OUTER:
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
                    var group = key is null ? null : innerLookup.GetGroup(key);
                    if (group != null)
                    {
                        matchedGroups!.Add(group);
                        currentOuter = value;
                        currentGroup = group;
                        currentGroupIndex = 0;
                        goto ITERATE_OUTER;
                    }
                    else
                    {
                        current = resultSelector(value, default);
                        return true;
                    }
                }

                outerCompleted = true;
                currentOuter = default!;

                // prepare to yield inner elements that had no matching outer element.
                if (matchedGroups != null && matchedGroups.Count < innerLookup.Count)
                {
                    nextUnmatchedGroup = innerLookup.LastGroup!.NextGroupInAddOrder; // as first.
                }
            }

        // iterating unmatched group
        ITERATE_INNER:
            if (currentGroup != null)
            {
                if (currentGroupIndex < currentGroup.Count)
                {
                    current = resultSelector(default, currentGroup[currentGroupIndex]);
                    currentGroupIndex++;
                    return true;
                }
                else
                {
                    currentGroup = null;
                }
            }

            while (nextUnmatchedGroup != null)
            {
                var group = nextUnmatchedGroup;
                nextUnmatchedGroup = (group == innerLookup.LastGroup) ? null : group.NextGroupInAddOrder;

                if (!matchedGroups!.Contains(group))
                {
                    currentGroup = group;
                    currentGroupIndex = 0;
                    goto ITERATE_INNER;
                }
            }

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
    struct FullJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey>(TEnumerator source, TEnumerator2 inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerator<(TOuter? Outer, TInner? Inner)>
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
        HashSet<Grouping<TKey, TInner>>? matchedGroups;
        Grouping<TKey, TInner>? currentGroup;
        int currentGroupIndex;
        TOuter currentOuter = default!;
        Grouping<TKey, TInner>? nextUnmatchedGroup;
        bool outerCompleted;
        bool streamingInner;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<(TOuter? Outer, TInner? Inner)> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<(TOuter? Outer, TInner? Inner)> destination, Index offset) => false;

        public bool TryGetNext(out (TOuter? Outer, TInner? Inner) current)
        {
            if (innerLookup == null)
            {
                if (streamingInner || FullJoinHelper.IsEmptyArray<TEnumerator, TOuter>(ref source))
                {
                    streamingInner = true;
                    if (inner.TryGetNext(out var value))
                    {
                        current = (default, value);
                        return true;
                    }

                    Unsafe.SkipInit(out current);
                    return false;
                }

                try
                {
                    // FullJoin needs to preserve inner elements with null keys so they can be emitted
                    // as unmatched rows, even though null keys still never participate in matches.
                    innerLookup = Lookup.CreateForFullJoin(ref inner, innerKeySelector, comparer);
                }
                finally
                {
                    inner.Dispose();
                }

                if (innerLookup.Count != 0)
                {
                    matchedGroups = new HashSet<Grouping<TKey, TInner>>();
                }
            }

            if (!outerCompleted)
            {
            // iterating matched group
            ITERATE_OUTER:
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
                    var group = key is null ? null : innerLookup.GetGroup(key);
                    if (group != null)
                    {
                        matchedGroups!.Add(group);
                        currentOuter = value;
                        currentGroup = group;
                        currentGroupIndex = 0;
                        goto ITERATE_OUTER;
                    }
                    else
                    {
                        current = (value, default);
                        return true;
                    }
                }

                outerCompleted = true;
                currentOuter = default!;

                // prepare to yield inner elements that had no matching outer element.
                if (matchedGroups != null && matchedGroups.Count < innerLookup.Count)
                {
                    nextUnmatchedGroup = innerLookup.LastGroup!.NextGroupInAddOrder; // as first.
                }
            }

        // iterating unmatched group
        ITERATE_INNER:
            if (currentGroup != null)
            {
                if (currentGroupIndex < currentGroup.Count)
                {
                    current = (default, currentGroup[currentGroupIndex]);
                    currentGroupIndex++;
                    return true;
                }
                else
                {
                    currentGroup = null;
                }
            }

            while (nextUnmatchedGroup != null)
            {
                var group = nextUnmatchedGroup;
                nextUnmatchedGroup = (group == innerLookup.LastGroup) ? null : group.NextGroupInAddOrder;

                if (!matchedGroups!.Contains(group))
                {
                    currentGroup = group;
                    currentGroupIndex = 0;
                    goto ITERATE_INNER;
                }
            }

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

    internal static class FullJoinHelper
    {
        // Enumerable.FullJoin yields inner elements in source order (without building a lookup)
        // only when outer is an empty array, so the same condition is used here to keep the order compatible.
        public static bool IsEmptyArray<TEnumerator, TOuter>(ref TEnumerator source)
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            if (typeof(TEnumerator) == typeof(FromArray<TOuter>))
            {
                return Unsafe.As<TEnumerator, FromArray<TOuter>>(ref source).GetSource().Length == 0;
            }

            if (typeof(TEnumerator) == typeof(FromEnumerable<TOuter>))
            {
                return Unsafe.As<TEnumerator, FromEnumerable<TOuter>>(ref source).GetSource() is TOuter[] { Length: 0 };
            }

            return false;
        }
    }
}
