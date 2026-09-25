namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<GroupJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey, TResult>, TResult> GroupJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey, TResult>(this ValueEnumerable<TEnumerator, TOuter> source, ValueEnumerable<TEnumerator2, TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TInner>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, inner.Enumerator, Throws.IfNull(outerKeySelector), Throws.IfNull(innerKeySelector), Throws.IfNull(resultSelector), null));

        public static ValueEnumerable<GroupJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey, TResult>, TResult> GroupJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey, TResult>(this ValueEnumerable<TEnumerator, TOuter> source, ValueEnumerable<TEnumerator2, TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TInner>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, inner.Enumerator, Throws.IfNull(outerKeySelector), Throws.IfNull(innerKeySelector), Throws.IfNull(resultSelector), comparer));



        public static ValueEnumerable<GroupJoin<TEnumerator, FromEnumerable<TInner>, TOuter, TInner, TKey, TResult>, TResult> GroupJoin<TEnumerator, TOuter, TInner, TKey, TResult>(this ValueEnumerable<TEnumerator, TOuter> source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(inner).AsValueEnumerable().Enumerator, Throws.IfNull(outerKeySelector), Throws.IfNull(innerKeySelector), Throws.IfNull(resultSelector), null));

        public static ValueEnumerable<GroupJoin<TEnumerator, FromEnumerable<TInner>, TOuter, TInner, TKey, TResult>, TResult> GroupJoin<TEnumerator, TOuter, TInner, TKey, TResult>(this ValueEnumerable<TEnumerator, TOuter> source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(inner).AsValueEnumerable().Enumerator, Throws.IfNull(outerKeySelector), Throws.IfNull(innerKeySelector), Throws.IfNull(resultSelector), comparer));

        public static ValueEnumerable<GroupJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey>, IGrouping<TOuter, TInner>> GroupJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey>(this ValueEnumerable<TEnumerator, TOuter> source, ValueEnumerable<TEnumerator2, TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, IEqualityComparer<TKey>? comparer = null)
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TInner>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, inner.Enumerator, Throws.IfNull(outerKeySelector), Throws.IfNull(innerKeySelector), comparer));

        public static ValueEnumerable<GroupJoin<TEnumerator, FromEnumerable<TInner>, TOuter, TInner, TKey>, IGrouping<TOuter, TInner>> GroupJoin<TEnumerator, TOuter, TInner, TKey>(this ValueEnumerable<TEnumerator, TOuter> source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, IEqualityComparer<TKey>? comparer = null)
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
    struct GroupJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey, TResult>(TEnumerator source, TEnumerator2 inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
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

            while (source.TryGetNext(out var value))
            {
                var key = outerKeySelector(value);
                // Enumerable.GroupJoin allows null unlike Join
                var group = innerLookup.GetGroup(key);
                if (group != null)
                {
                    current = resultSelector(value, group);
                    return true;
                }
                else
                {
                    // return empty grouping
                    current = resultSelector(value, []);
                    return true;
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
    struct GroupJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey>(TEnumerator source, TEnumerator2 inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerator<IGrouping<TOuter, TInner>>
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

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<IGrouping<TOuter, TInner>> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<IGrouping<TOuter, TInner>> destination, Index offset) => false;

        public bool TryGetNext(out IGrouping<TOuter, TInner> current)
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

            if (source.TryGetNext(out var value))
            {
                var key = outerKeySelector(value);
                // Enumerable.GroupJoin allows null unlike Join
                var group = innerLookup.GetGroup(key);
                // return empty grouping if there is no matching group
                current = new GroupJoinGrouping<TOuter, TInner>(value, group != null ? group : []);
                return true;
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

    // IGrouping for GroupJoin without resultSelector, the key is the outer element.
    internal sealed class GroupJoinGrouping<TKey, TElement>(TKey key, IEnumerable<TElement> elements) : IGrouping<TKey, TElement>
    {
        public TKey Key => key;

        public IEnumerator<TElement> GetEnumerator() => elements.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
