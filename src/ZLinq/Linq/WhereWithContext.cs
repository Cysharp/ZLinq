namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Filters an array by using the supplied context.</summary>
        /// <param name="source">The source array.</param>
        /// <param name="predicate">A function that tests an element using the supplied context.</param>
        /// <param name="context">The context passed to the predicate for each element.</param>
        /// <returns>An array-backed sequence that contains the elements that satisfy the predicate.</returns>
        public static ValueEnumerable<ArrayWhereWithContext<TSource, TContext>, TSource> Where<TSource, TContext>(this ValueEnumerable<FromArray<TSource>, TSource> source, Func<TSource, TContext, bool> predicate, TContext context)
            => new(new(source.Enumerator.GetSource(), Throws.IfNull(predicate), context));

        /// <summary>Filters a list by using the supplied context.</summary>
        /// <param name="source">The source list.</param>
        /// <param name="predicate">A function that tests an element using the supplied context.</param>
        /// <param name="context">The context passed to the predicate for each element.</param>
        /// <returns>A list-backed sequence that contains the elements that satisfy the predicate.</returns>
        public static ValueEnumerable<ListWhereWithContext<TSource, TContext>, TSource> Where<TSource, TContext>(this ValueEnumerable<FromList<TSource>, TSource> source, Func<TSource, TContext, bool> predicate, TContext context)
            => new(new(source.Enumerator.GetSource(), Throws.IfNull(predicate), context));

        /// <summary>Filters a sequence of values by using the supplied context.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="predicate">A function that tests an element using the supplied context.</param>
        /// <param name="context">The context passed to the predicate for each element.</param>
        /// <returns>A sequence that contains the elements from the source sequence that satisfy the predicate.</returns>
        public static ValueEnumerable<WhereWithContext<TEnumerator, TSource, TContext>, TSource> Where<TEnumerator, TSource, TContext>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TContext, bool> predicate, TContext context)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(predicate), context));

        /// <summary>Projects the elements that satisfy a context-aware predicate by using a supplied selector context.</summary>
        /// <param name="source">The filtered source sequence.</param>
        /// <param name="selector">A transform function that receives the element and the supplied context.</param>
        /// <param name="context">The context passed to the transform function for each element.</param>
        /// <returns>A sequence whose elements are the result of invoking the transform function on the elements that satisfy the predicate.</returns>
        public static ValueEnumerable<WhereSelectWithContext<TEnumerator, TSource, TResult, TWhereContext, TSelectContext>, TResult> Select<TEnumerator, TSource, TResult, TWhereContext, TSelectContext>(this ValueEnumerable<WhereWithContext<TEnumerator, TSource, TWhereContext>, TSource> source, Func<TSource, TSelectContext, TResult> selector, TSelectContext context)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source.Enumerator.Select(Throws.IfNull(selector), context));

        /// <summary>Projects array elements that satisfy a context-aware predicate using the supplied selector context.</summary>
        public static ValueEnumerable<ArrayWhereSelectWithContext<TSource, TResult, TWhereContext, TSelectContext>, TResult> Select<TSource, TResult, TWhereContext, TSelectContext>(this ValueEnumerable<ArrayWhereWithContext<TSource, TWhereContext>, TSource> source, Func<TSource, TSelectContext, TResult> selector, TSelectContext context)
            => new(source.Enumerator.Select(Throws.IfNull(selector), context));

        /// <summary>Projects list elements that satisfy a context-aware predicate using the supplied selector context.</summary>
        public static ValueEnumerable<ListWhereSelectWithContext<TSource, TResult, TWhereContext, TSelectContext>, TResult> Select<TSource, TResult, TWhereContext, TSelectContext>(this ValueEnumerable<ListWhereWithContext<TSource, TWhereContext>, TSource> source, Func<TSource, TSelectContext, TResult> selector, TSelectContext context)
            => new(source.Enumerator.Select(Throws.IfNull(selector), context));
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
    struct WhereWithContext<TEnumerator, TSource, TContext>(TEnumerator source, Func<TSource, TContext, bool> predicate, TContext context) : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;

        internal WhereSelectWithContext<TEnumerator, TSource, TResult, TContext, TSelectContext> Select<TResult, TSelectContext>(Func<TSource, TSelectContext, TResult> selector, TSelectContext selectContext)
            => new(source, predicate, context, selector, selectContext);

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = default;
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
            while (source.TryGetNext(out var value))
            {
                if (predicate(value, context))
                {
                    current = value;
                    return true;
                }
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose() => source.Dispose();
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct WhereSelectWithContext<TEnumerator, TSource, TResult, TWhereContext, TSelectContext>(TEnumerator source, Func<TSource, TWhereContext, bool> predicate, TWhereContext whereContext, Func<TSource, TSelectContext, TResult> selector, TSelectContext selectContext) : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = default;
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
            while (source.TryGetNext(out var value))
            {
                if (predicate(value, whereContext))
                {
                    current = selector(value, selectContext);
                    return true;
                }
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose() => source.Dispose();
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct ArrayWhereWithContext<TSource, TContext>(TSource[] source, Func<TSource, TContext, bool> predicate, TContext context) : IValueEnumerator<TSource>
    {
        int index;

        internal ArrayWhereSelectWithContext<TSource, TResult, TContext, TSelectContext> Select<TResult, TSelectContext>(Func<TSource, TSelectContext, TResult> selector, TSelectContext selectContext)
            => new(source, predicate, context, selector, selectContext);

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = default;
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
            while ((uint)index < (uint)source.Length)
            {
                var value = source[index++];
                if (predicate(value, context))
                {
                    current = value;
                    return true;
                }
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose() { }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct ArrayWhereSelectWithContext<TSource, TResult, TWhereContext, TSelectContext>(TSource[] source, Func<TSource, TWhereContext, bool> predicate, TWhereContext whereContext, Func<TSource, TSelectContext, TResult> selector, TSelectContext selectContext) : IValueEnumerator<TResult>
    {
        int index;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = default;
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
            while ((uint)index < (uint)source.Length)
            {
                var value = source[index++];
                if (predicate(value, whereContext))
                {
                    current = selector(value, selectContext);
                    return true;
                }
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose() { }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct ListWhereWithContext<TSource, TContext>(List<TSource> source, Func<TSource, TContext, bool> predicate, TContext context) : IValueEnumerator<TSource>
    {
        int index;

        internal ListWhereSelectWithContext<TSource, TResult, TContext, TSelectContext> Select<TResult, TSelectContext>(Func<TSource, TSelectContext, TResult> selector, TSelectContext selectContext)
            => new(source, predicate, context, selector, selectContext);

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = default;
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
            var span = ListMarshal.AsSpan(source);
            while ((uint)index < (uint)span.Length)
            {
                var value = span[index++];
                if (predicate(value, context))
                {
                    current = value;
                    return true;
                }
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose() { }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct ListWhereSelectWithContext<TSource, TResult, TWhereContext, TSelectContext>(List<TSource> source, Func<TSource, TWhereContext, bool> predicate, TWhereContext whereContext, Func<TSource, TSelectContext, TResult> selector, TSelectContext selectContext) : IValueEnumerator<TResult>
    {
        int index;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = default;
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
            var span = ListMarshal.AsSpan(source);
            while ((uint)index < (uint)span.Length)
            {
                var value = span[index++];
                if (predicate(value, whereContext))
                {
                    current = selector(value, selectContext);
                    return true;
                }
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose() { }
    }
}
