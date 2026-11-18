namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Projects each element to a value sequence using the supplied context and flattens the results.</summary>
        public static ValueEnumerable<SelectManyWithContext<TEnumerator, TEnumerator2, TSource, TContext, TResult>, TResult> SelectMany<TEnumerator, TEnumerator2, TSource, TContext, TResult>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TContext, ValueEnumerable<TEnumerator2, TResult>> selector, TContext context)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TResult>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(selector), context));

        /// <summary>Projects each element to an enumerable using the supplied context and flattens the results.</summary>
        public static ValueEnumerable<SelectManyEnumerableWithContext<TEnumerator, TSource, TContext, TResult>, TResult> SelectMany<TEnumerator, TSource, TContext, TResult>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TContext, IEnumerable<TResult>> selector, TContext context)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(selector), context));
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
    struct SelectManyWithContext<TEnumerator, TEnumerator2, TSource, TContext, TResult>(TEnumerator source, Func<TSource, TContext, ValueEnumerable<TEnumerator2, TResult>> selector, TContext context) : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        where TEnumerator2 : struct, IValueEnumerator<TResult>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        TEnumerator2 inner;
        bool hasInner;

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
        BEGIN:
            if (hasInner)
            {
                if (inner.TryGetNext(out current)) return true;
                inner.Dispose();
                hasInner = false;
            }

            if (source.TryGetNext(out var value))
            {
                inner = selector(value, context).Enumerator;
                hasInner = true;
                goto BEGIN;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (hasInner) inner.Dispose();
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
    struct SelectManyEnumerableWithContext<TEnumerator, TSource, TContext, TResult>(TEnumerator source, Func<TSource, TContext, IEnumerable<TResult>> selector, TContext context) : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        FromEnumerable<TResult> inner;
        bool hasInner;

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
        BEGIN:
            if (hasInner)
            {
                if (inner.TryGetNext(out current)) return true;
                inner.Dispose();
                hasInner = false;
            }

            if (source.TryGetNext(out var value))
            {
                inner = selector(value, context).AsValueEnumerable().Enumerator;
                hasInner = true;
                goto BEGIN;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (hasInner) inner.Dispose();
            source.Dispose();
        }
    }
}
