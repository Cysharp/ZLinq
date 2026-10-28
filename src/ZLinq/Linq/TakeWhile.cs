namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Returns elements from a sequence as long as a specified condition is true.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A sequence that contains the elements from the source sequence that occur before the element at which the test no longer passes.</returns>
        public static ValueEnumerable<TakeWhile<TEnumerator, TSource>, TSource> TakeWhile<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Boolean> predicate)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(predicate)));

        /// <summary>Returns elements from a sequence as long as a specified condition is true. The element's index is used in the logic of the predicate function.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="predicate">A function to test each source element for a condition; the second parameter of the function represents the index of the source element.</param>
        /// <returns>A sequence that contains the elements from the source sequence that occur before the element at which the test no longer passes.</returns>
        public static ValueEnumerable<TakeWhile2<TEnumerator, TSource>, TSource> TakeWhile<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Int32, Boolean> predicate)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(predicate)));

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
    struct TakeWhile<TEnumerator, TSource>(TEnumerator source, Func<TSource, Boolean> predicate)
        : IValueEnumerator<TSource>
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
            if (source.TryGetNext(out current) && predicate(current))
            {
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
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
    struct TakeWhile2<TEnumerator, TSource>(TEnumerator source, Func<TSource, Int32, Boolean> predicate)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        int index = 0;

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
            if (source.TryGetNext(out current) && predicate(current, index++))
            {
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }
}
