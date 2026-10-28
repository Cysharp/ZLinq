namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Bypasses a specified number of elements in a sequence and then returns the remaining elements.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="count">The number of elements to skip before returning the remaining elements.</param>
        /// <returns>A sequence that contains the elements that occur after the specified index in the source sequence.</returns>
        public static ValueEnumerable<Skip<TEnumerator, TSource>, TSource> Skip<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Int32 count)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, count));
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
    struct Skip<TEnumerator, TSource>(TEnumerator source, Int32 count)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        readonly int skipCount = Math.Max(0, count); // ensure non-negative
        int skipped;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (source.TryGetNonEnumeratedCount(out count))
            {
                count = Math.Max(0, count - skipCount); // subtract skip count, ensure non-negative
                return true;
            }

            count = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<TSource> destination, Index offset)
        {
            if (source.TryGetNonEnumeratedCount(out var count))
            {
                var actualSkipCount = Math.Min(count, skipCount);

                var remainingCount = count - actualSkipCount;

                if (remainingCount <= 0)
                {
                    return false;
                }

                var offsetInSkipped = offset.GetOffset(remainingCount);

                if (offsetInSkipped < 0 || offsetInSkipped >= remainingCount)
                {
                    return false;
                }

                var sourceOffset = actualSkipCount + offsetInSkipped;

                var elementsAvailable = remainingCount - offsetInSkipped;

                var elementsToCopy = Math.Min(elementsAvailable, destination.Length);

                if (elementsToCopy <= 0)
                {
                    return false;
                }

                return source.TryCopyTo(destination.Slice(0, elementsToCopy), sourceOffset);
            }

            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            if (source.TryGetSpan(out span))
            {
                if (span.Length <= skipCount)
                {
                    span = default;
                    return true;
                }
                span = span.Slice(skipCount);
                return true;
            }

            span = default;
            return false;
        }

        public bool TryGetNext(out TSource current)
        {
            // Skip elements if not already skipped
            while (skipped < skipCount)
            {
                if (!source.TryGetNext(out var _))
                {
                    Unsafe.SkipInit(out current);
                    return false;
                }
                skipped++;
            }

            // Return elements after skipping
            if (source.TryGetNext(out current))
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
