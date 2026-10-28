namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Returns a sequence that contains the elements from the source sequence with the last specified number of elements omitted.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="count">The number of elements to omit from the end of the sequence.</param>
        /// <returns>A sequence that contains the elements from the source sequence minus the specified number of elements from the end.</returns>
        public static ValueEnumerable<SkipLast<TEnumerator, TSource>, TSource> SkipLast<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Int32 count)
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
    struct SkipLast<TEnumerator, TSource>(TEnumerator source, Int32 count)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        readonly int skipCount = Math.Max(0, count);
        RefBox<ValueQueue<TSource>>? buffer;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (source.TryGetNonEnumeratedCount(out count))
            {
                count = Math.Max(0, count - skipCount);
                return true;
            }

            count = 0;
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
                span = span[..^skipCount]; // Slice from start to (length - skipCount)
                return true;
            }

            span = default;
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

                var sourceOffset = offsetInSkipped;

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

        public bool TryGetNext(out TSource current)
        {
            if (skipCount == 0)
            {
                return source.TryGetNext(out current);
            }

            if (buffer == null)
            {
                buffer = new RefBox<ValueQueue<TSource>>(new ValueQueue<TSource>(4));

                // Fill the buffer with initial items
                while (buffer.GetValueRef().Count < skipCount && source.TryGetNext(out var item))
                {
                    buffer.GetValueRef().Enqueue(item);
                }

                // If we couldn't fill the buffer, there aren't enough items
                if (buffer.GetValueRef().Count < skipCount)
                {
                    Unsafe.SkipInit(out current);
                    return false;
                }
            }

            // Try to get the next item from the source
            if (source.TryGetNext(out var next))
            {
                // Return the oldest item from buffer and add the new one
                current = buffer.GetValueRef().Dequeue();
                buffer.GetValueRef().Enqueue(next);
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            buffer?.Dispose();
            source.Dispose();
        }
    }
}
