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

        FullJoinState state;
        Lookup<TKey, TInner>? innerLookup; // non-null once the state has moved to Outer
        HashSet<Grouping<TKey, TInner>>? matchedGroups; // allocated on the first match
        TOuter currentOuter = default!;
        Grouping<TKey, TInner>? currentGroup;
        int currentGroupIndex;
        Grouping<TKey, TInner>? nextUnmatchedGroup;

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
            switch (state)
            {
                case FullJoinState.Initial:
                    if (FullJoinHelper.IsEmptyArray<TEnumerator, TOuter>(ref source))
                    {
                        state = FullJoinState.EmptyOuterArray;
                        goto case FullJoinState.EmptyOuterArray;
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

                    state = FullJoinState.Outer;
                    goto case FullJoinState.Outer;

                case FullJoinState.EmptyOuterArray:
                    if (inner.TryGetNext(out var innerValue))
                    {
                        current = resultSelector(default, innerValue);
                        return true;
                    }

                    state = FullJoinState.Completed;
                    break;

                case FullJoinState.Outer:
                    if (currentGroup != null)
                    {
                        if (currentGroupIndex < currentGroup.Count)
                        {
                            current = resultSelector(currentOuter, currentGroup[currentGroupIndex]);
                            currentGroupIndex++;
                            return true;
                        }

                        currentGroup = null;
                    }

                    while (source.TryGetNext(out var value))
                    {
                        var key = outerKeySelector(value);

                        // the lookup contains the null-key group, which must not be matched.
                        var group = key is null ? null : innerLookup!.GetGroup(key);
                        if (group != null)
                        {
                            (matchedGroups ??= new HashSet<Grouping<TKey, TInner>>()).Add(group);
                            currentOuter = value;
                            currentGroup = group;
                            currentGroupIndex = 0;
                            goto case FullJoinState.Outer;
                        }

                        current = resultSelector(value, default);
                        return true;
                    }

                    currentOuter = default!;

                    // prepare to yield inner elements that had no matching outer element.
                    if ((matchedGroups?.Count ?? 0) < innerLookup!.Count)
                    {
                        nextUnmatchedGroup = innerLookup.FirstGroup;
                    }

                    state = FullJoinState.UnmatchedInner;
                    goto case FullJoinState.UnmatchedInner;

                case FullJoinState.UnmatchedInner:
                    if (currentGroup != null)
                    {
                        if (currentGroupIndex < currentGroup.Count)
                        {
                            current = resultSelector(default, currentGroup[currentGroupIndex]);
                            currentGroupIndex++;
                            return true;
                        }

                        currentGroup = null;
                    }

                    while (nextUnmatchedGroup != null)
                    {
                        var group = nextUnmatchedGroup;
                        nextUnmatchedGroup = (group == innerLookup!.LastGroup) ? null : group.NextGroupInAddOrder;

                        if (matchedGroups == null || !matchedGroups.Contains(group))
                        {
                            currentGroup = group;
                            currentGroupIndex = 0;
                            goto case FullJoinState.UnmatchedInner;
                        }
                    }

                    state = FullJoinState.Completed;
                    break;
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

        FullJoinState state;
        Lookup<TKey, TInner>? innerLookup; // non-null once the state has moved to Outer
        HashSet<Grouping<TKey, TInner>>? matchedGroups; // allocated on the first match
        TOuter currentOuter = default!;
        Grouping<TKey, TInner>? currentGroup;
        int currentGroupIndex;
        Grouping<TKey, TInner>? nextUnmatchedGroup;

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
            switch (state)
            {
                case FullJoinState.Initial:
                    if (FullJoinHelper.IsEmptyArray<TEnumerator, TOuter>(ref source))
                    {
                        state = FullJoinState.EmptyOuterArray;
                        goto case FullJoinState.EmptyOuterArray;
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

                    state = FullJoinState.Outer;
                    goto case FullJoinState.Outer;

                case FullJoinState.EmptyOuterArray:
                    if (inner.TryGetNext(out var innerValue))
                    {
                        current = (default, innerValue);
                        return true;
                    }

                    state = FullJoinState.Completed;
                    break;

                case FullJoinState.Outer:
                    if (currentGroup != null)
                    {
                        if (currentGroupIndex < currentGroup.Count)
                        {
                            current = (currentOuter, currentGroup[currentGroupIndex]);
                            currentGroupIndex++;
                            return true;
                        }

                        currentGroup = null;
                    }

                    while (source.TryGetNext(out var value))
                    {
                        var key = outerKeySelector(value);

                        // the lookup contains the null-key group, which must not be matched.
                        var group = key is null ? null : innerLookup!.GetGroup(key);
                        if (group != null)
                        {
                            (matchedGroups ??= new HashSet<Grouping<TKey, TInner>>()).Add(group);
                            currentOuter = value;
                            currentGroup = group;
                            currentGroupIndex = 0;
                            goto case FullJoinState.Outer;
                        }

                        current = (value, default);
                        return true;
                    }

                    currentOuter = default!;

                    // prepare to yield inner elements that had no matching outer element.
                    if ((matchedGroups?.Count ?? 0) < innerLookup!.Count)
                    {
                        nextUnmatchedGroup = innerLookup.FirstGroup;
                    }

                    state = FullJoinState.UnmatchedInner;
                    goto case FullJoinState.UnmatchedInner;

                case FullJoinState.UnmatchedInner:
                    if (currentGroup != null)
                    {
                        if (currentGroupIndex < currentGroup.Count)
                        {
                            current = (default, currentGroup[currentGroupIndex]);
                            currentGroupIndex++;
                            return true;
                        }

                        currentGroup = null;
                    }

                    while (nextUnmatchedGroup != null)
                    {
                        var group = nextUnmatchedGroup;
                        nextUnmatchedGroup = (group == innerLookup!.LastGroup) ? null : group.NextGroupInAddOrder;

                        if (matchedGroups == null || !matchedGroups.Contains(group))
                        {
                            currentGroup = group;
                            currentGroupIndex = 0;
                            goto case FullJoinState.UnmatchedInner;
                        }
                    }

                    state = FullJoinState.Completed;
                    break;
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

    internal enum FullJoinState : byte
    {
        Initial,
        EmptyOuterArray, // outer is an empty array, so inner is yielded in source order without building a lookup
        Outer, // yielding outer elements, each paired with the elements of its matched group or with default
        UnmatchedInner, // yielding inner elements whose group matched no outer element
        Completed,
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
