namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Groups the elements of a sequence according to a specified key selector function.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <returns>A sequence of groups, where each group contains a sequence of objects and a key.</returns>
        public static ValueEnumerable<GroupBy<TEnumerator, TSource, TKey>, IGrouping<TKey, TSource>> GroupBy<TEnumerator, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(keySelector), null));

        /// <summary>Groups the elements of a sequence according to a specified key selector function.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <param name="comparer">An equality comparer to compare keys, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>A sequence of groups, where each group contains a sequence of objects and a key.</returns>
        public static ValueEnumerable<GroupBy<TEnumerator, TSource, TKey>, IGrouping<TKey, TSource>> GroupBy<TEnumerator, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(keySelector), comparer));

        /// <summary>Groups the elements of a sequence according to a specified key selector function.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>A sequence of groups, where each group contains a sequence of objects and a key.</returns>
        public static ValueEnumerable<GroupBy2<TEnumerator, TSource, TKey, TElement>, IGrouping<TKey, TElement>> GroupBy<TEnumerator, TSource, TKey, TElement>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(keySelector), Throws.IfNull(elementSelector), null));

        /// <summary>Groups the elements of a sequence according to a specified key selector function.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="comparer">An equality comparer to compare keys, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>A sequence of groups, where each group contains a sequence of objects and a key.</returns>
        public static ValueEnumerable<GroupBy2<TEnumerator, TSource, TKey, TElement>, IGrouping<TKey, TElement>> GroupBy<TEnumerator, TSource, TKey, TElement>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(keySelector), Throws.IfNull(elementSelector), comparer));

        /// <summary>Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <param name="resultSelector">A function to create a result value from each group.</param>
        /// <returns>A sequence of elements of the result type where each element represents a projection over a group and its key.</returns>
        public static ValueEnumerable<GroupBy3<TEnumerator, TSource, TKey, TResult>, TResult> GroupBy<TEnumerator, TSource, TKey, TResult>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(keySelector), Throws.IfNull(resultSelector), null));

        /// <summary>Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <param name="resultSelector">A function to create a result value from each group.</param>
        /// <param name="comparer">An equality comparer to compare keys, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>A sequence of elements of the result type where each element represents a projection over a group and its key.</returns>
        public static ValueEnumerable<GroupBy3<TEnumerator, TSource, TKey, TResult>, TResult> GroupBy<TEnumerator, TSource, TKey, TResult>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(keySelector), Throws.IfNull(resultSelector), comparer));

        /// <summary>Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="resultSelector">A function to create a result value from each group.</param>
        /// <returns>A sequence of elements of the result type where each element represents a projection over a group and its key.</returns>
        public static ValueEnumerable<GroupBy4<TEnumerator, TSource, TKey, TElement, TResult>, TResult> GroupBy<TEnumerator, TSource, TKey, TElement, TResult>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(keySelector), Throws.IfNull(elementSelector), Throws.IfNull(resultSelector), null));

        /// <summary>Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="resultSelector">A function to create a result value from each group.</param>
        /// <param name="comparer">An equality comparer to compare keys, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>A sequence of elements of the result type where each element represents a projection over a group and its key.</returns>
        public static ValueEnumerable<GroupBy4<TEnumerator, TSource, TKey, TElement, TResult>, TResult> GroupBy<TEnumerator, TSource, TKey, TElement, TResult>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(keySelector), Throws.IfNull(elementSelector), Throws.IfNull(resultSelector), comparer));

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
    struct GroupBy<TEnumerator, TSource, TKey>(TEnumerator source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerator<IGrouping<TKey, TSource>>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        bool init;
        Grouping<TKey, TSource>? rootGrouping;
        Grouping<TKey, TSource>? currentGrouping;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<IGrouping<TKey, TSource>> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<IGrouping<TKey, TSource>> destination, Index offset) => false;

        public bool TryGetNext(out IGrouping<TKey, TSource> current)
        {
            if (!init)
            {
                init = true;
                rootGrouping = BuildRoot();
                if (rootGrouping != null)
                {
                    current = rootGrouping;
                    currentGrouping = rootGrouping;
                    return true;
                }
            }

            currentGrouping = currentGrouping?.NextGroupInAddOrder;

            if (currentGrouping == null || currentGrouping == rootGrouping)
            {
                current = default!;
                return false;
            }
            else
            {
                current = currentGrouping;
                return true;
            }
        }

        public void Dispose()
        {
            source.Dispose();
        }

        Grouping<TKey, TSource>? BuildRoot()
        {
            var lookupBuilder = new LookupBuilder<TKey, TSource>(comparer ?? EqualityComparer<TKey>.Default);

            if (source.TryGetSpan(out var span))
            {
                foreach (var item in span)
                {
                    lookupBuilder.Add(keySelector(item), item);
                }
            }
            else
            {
                using (source)
                {
                    while (source.TryGetNext(out var item))
                    {
                        lookupBuilder.Add(keySelector(item), item);
                    }
                }
            }

            return lookupBuilder.GetRootGroupAndClear();
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct GroupBy2<TEnumerator, TSource, TKey, TElement>(TEnumerator source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerator<IGrouping<TKey, TElement>>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        bool init;
        Grouping<TKey, TElement>? rootGrouping;
        Grouping<TKey, TElement>? currentGrouping;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<IGrouping<TKey, TElement>> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<IGrouping<TKey, TElement>> destination, Index offset) => false;

        public bool TryGetNext(out IGrouping<TKey, TElement> current)
        {
            if (!init)
            {
                init = true;
                rootGrouping = BuildRoot();
                if (rootGrouping != null)
                {
                    current = rootGrouping;
                    currentGrouping = rootGrouping;
                    return true;
                }
            }

            currentGrouping = currentGrouping?.NextGroupInAddOrder;

            if (currentGrouping == null || currentGrouping == rootGrouping)
            {
                current = default!;
                return false;
            }
            else
            {
                current = currentGrouping;
                return true;
            }
        }

        public void Dispose()
        {
            source.Dispose();
        }

        Grouping<TKey, TElement>? BuildRoot()
        {
            var lookupBuilder = new LookupBuilder<TKey, TElement>(comparer ?? EqualityComparer<TKey>.Default);

            if (source.TryGetSpan(out var span))
            {
                foreach (var item in span)
                {
                    lookupBuilder.Add(keySelector(item), elementSelector(item));
                }
            }
            else
            {
                using (source)
                {
                    while (source.TryGetNext(out var item))
                    {
                        lookupBuilder.Add(keySelector(item), elementSelector(item));
                    }
                }
            }

            return lookupBuilder.GetRootGroupAndClear();
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct GroupBy3<TEnumerator, TSource, TKey, TResult>(TEnumerator source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        bool init;
        Grouping<TKey, TSource>? rootGrouping;
        Grouping<TKey, TSource>? currentGrouping;

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
            if (!init)
            {
                init = true;
                rootGrouping = BuildRoot();
                if (rootGrouping != null)
                {
                    current = resultSelector(rootGrouping.Key, rootGrouping);
                    currentGrouping = rootGrouping;
                    return true;
                }
            }

            currentGrouping = currentGrouping?.NextGroupInAddOrder;

            if (currentGrouping == null || currentGrouping == rootGrouping)
            {
                current = default!;
                return false;
            }
            else
            {
                current = resultSelector(currentGrouping.Key, currentGrouping);
                return true;
            }
        }

        public void Dispose()
        {
            source.Dispose();
        }

        Grouping<TKey, TSource>? BuildRoot()
        {
            var lookupBuilder = new LookupBuilder<TKey, TSource>(comparer ?? EqualityComparer<TKey>.Default);

            if (source.TryGetSpan(out var span))
            {
                foreach (var item in span)
                {
                    lookupBuilder.Add(keySelector(item), item);
                }
            }
            else
            {
                using (source)
                {
                    while (source.TryGetNext(out var item))
                    {
                        lookupBuilder.Add(keySelector(item), item);
                    }
                }
            }

            return lookupBuilder.GetRootGroupAndClear();
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct GroupBy4<TEnumerator, TSource, TKey, TElement, TResult>(TEnumerator source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        bool init;
        Grouping<TKey, TElement>? rootGrouping;
        Grouping<TKey, TElement>? currentGrouping;

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
            if (!init)
            {
                init = true;
                rootGrouping = BuildRoot();
                if (rootGrouping != null)
                {
                    current = resultSelector(rootGrouping.Key, rootGrouping);
                    currentGrouping = rootGrouping;
                    return true;
                }
            }

            currentGrouping = currentGrouping?.NextGroupInAddOrder;

            if (currentGrouping == null || currentGrouping == rootGrouping)
            {
                current = default!;
                return false;
            }
            else
            {
                current = resultSelector(currentGrouping.Key, currentGrouping);
                return true;
            }
        }

        public void Dispose()
        {
            source.Dispose();
        }

        Grouping<TKey, TElement>? BuildRoot()
        {
            var lookupBuilder = new LookupBuilder<TKey, TElement>(comparer ?? EqualityComparer<TKey>.Default);

            if (source.TryGetSpan(out var span))
            {
                foreach (var item in span)
                {
                    lookupBuilder.Add(keySelector(item), elementSelector(item));
                }
            }
            else
            {
                using (source)
                {
                    while (source.TryGetNext(out var item))
                    {
                        lookupBuilder.Add(keySelector(item), elementSelector(item));
                    }
                }
            }

            return lookupBuilder.GetRootGroupAndClear();
        }
    }
}
