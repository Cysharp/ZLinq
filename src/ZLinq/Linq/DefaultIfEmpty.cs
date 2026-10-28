namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Returns the elements of the specified sequence or the type parameter's default value in a singleton collection if the sequence is empty.</summary>
        /// <param name="source">The source sequence.</param>
        /// <returns>A sequence that contains the default value if the source sequence is empty; otherwise, the source sequence.</returns>
        public static ValueEnumerable<DefaultIfEmpty<TEnumerator, TSource>, TSource> DefaultIfEmpty<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, default!));

        /// <summary>Returns the elements of the specified sequence or the specified value in a singleton collection if the sequence is empty.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="defaultValue">The value to return if the sequence is empty.</param>
        /// <returns>A sequence that contains the specified value if the source sequence is empty; otherwise, the source sequence.</returns>
        public static ValueEnumerable<DefaultIfEmpty<TEnumerator, TSource>, TSource> DefaultIfEmpty<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, TSource defaultValue)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, defaultValue));

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
    struct DefaultIfEmpty<TEnumerator, TSource>(TEnumerator source, TSource defaultValue)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        bool iterateDefault = true;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (source.TryGetNonEnumeratedCount(out count))
            {
                if (count == 0)
                {
                    // For empty collection, we'll return 1 item (the default value)
                    count = 1;
                }
                return true;
            }
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            if (source.TryGetSpan(out span))
            {
                if (span.Length == 0)
                {
                    // We can't create a span with custom default value on the fly
                    return false;
                }
                return true;
            }
            return false;
        }

        public bool TryCopyTo(scoped Span<TSource> destination, Index offset)
        {
            if (destination.Length == 0) return true;

            // ignore offset when checking
            if (source.TryGetNonEnumeratedCount(out var sourceCount) && sourceCount == 0 && destination.Length >= 1)
            {
                // check offset
                if (offset.GetOffset(1) == 0) // the result has exactly one element(the default value)
                {
                    destination[0] = defaultValue;
                    return true;
                }
                return false;
            }
            else
            {
                return source.TryCopyTo(destination, offset);
            }
        }

        public bool TryGetNext(out TSource current)
        {
            if (source.TryGetNext(out current))
            {
                iterateDefault = false;
                return true;
            }

            if (iterateDefault)
            {
                iterateDefault = false;
                current = defaultValue;
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
