namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Returns elements from a sequence while a predicate using the supplied context is true.</summary>
        public static ValueEnumerable<TakeWhileWithContext<TEnumerator, TSource, TContext>, TSource> TakeWhile<TEnumerator, TSource, TContext>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TContext, bool> predicate, TContext context)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(predicate), context));
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
    struct TakeWhileWithContext<TEnumerator, TSource, TContext>(TEnumerator source, Func<TSource, TContext, bool> predicate, TContext context) : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;

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

        public bool TryCopyTo(scoped Span<TSource> destination, Index offset) => false;

        public bool TryGetNext(out TSource current)
        {
            if (source.TryGetNext(out current) && predicate(current, context)) return true;
            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose() => source.Dispose();
    }
}
