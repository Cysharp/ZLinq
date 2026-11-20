namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Projects each element of a sequence into a new form by using the supplied context.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="selector">A transform function that receives the element and the supplied context.</param>
        /// <param name="context">The context passed to the transform function for each element.</param>
        /// <returns>A sequence whose elements are the result of invoking the transform function on each source element and the context.</returns>
        public static ValueEnumerable<SelectWithContext<TEnumerator, TSource, TResult, TContext>, TResult> Select<TEnumerator, TSource, TResult, TContext>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TContext, TResult> selector, TContext context)
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
    struct SelectWithContext<TEnumerator, TSource, TResult, TContext>(TEnumerator source, Func<TSource, TContext, TResult> selector, TContext context) : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;

        public bool TryGetNonEnumeratedCount(out int count) => source.TryGetNonEnumeratedCount(out count);

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<TResult> destination, Index offset)
        {
            if (source.TryGetSpan(out var span))
            {
                if (EnumeratorHelper.TryGetSlice(span, offset, destination.Length, out var slice))
                {
                    for (var i = 0; (uint)i < (uint)slice.Length; i++)
                    {
                        destination[i] = selector(slice[i], context);
                    }
                    return true;
                }
            }

            if (destination.Length == 1)
            {
#if NETSTANDARD2_0
                var singleSpan = SingleSpan.Create<TSource>();
                if (source.TryCopyTo(singleSpan, offset))
                {
                    try
                    {
                        destination[0] = selector(singleSpan[0], context);
                    }
                    finally
                    {
                        singleSpan.Clear();
                    }
                    return true;
                }
#else
                var current = default(TSource)!;
                if (source.TryCopyTo(SingleSpan.Create(ref current), offset))
                {
                    destination[0] = selector(current, context);
                    return true;
                }
#endif

                if (EnumeratorHelper.TryConsumeGetAt(ref source, offset, out TSource value))
                {
                    destination[0] = selector(value, context);
                    return true;
                }
            }

            return false;
        }

        public bool TryGetNext(out TResult current)
        {
            if (source.TryGetNext(out var value))
            {
                current = selector(value, context);
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose() => source.Dispose();
    }
}
