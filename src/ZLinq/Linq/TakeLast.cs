namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<TakeLast<TEnumerator, TSource>, TSource> TakeLast<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Int32 count)
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
    struct TakeLast<TEnumerator, TSource>(TEnumerator source, Int32 count)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        readonly int takeCount = Math.Max(0, count);
        int state = 0; // 0: Initial, 1: Enumerating, 2: Dequeue, 3: Completed
        RentedRingBuffer<TSource>? ringBuffer;


        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (source.TryGetNonEnumeratedCount(out count))
            {
                count = Math.Min(count, takeCount);
                return true;
            }

            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            if (source.TryGetSpan(out span))
            {
                if (span.Length > takeCount)
                {
                    span = span[^takeCount..];
                }
                return true;
            }
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<TSource> destination, Index offset)
        {
            if (source.TryGetNonEnumeratedCount(out var count))
            {
                var actualTakeCount = Math.Min(count, takeCount);
                if (actualTakeCount <= 0)
                {
                    return false;
                }

                var takeLastStartIndex = count - actualTakeCount;
                var offsetInTakeLast = offset.GetOffset(actualTakeCount);

                if (offsetInTakeLast < 0 || offsetInTakeLast >= actualTakeCount)
                {
                    return false;
                }

                var sourceOffset = takeLastStartIndex + offsetInTakeLast;
                var remainingElements = actualTakeCount - offsetInTakeLast;
                var elementsToCopy = Math.Min(remainingElements, destination.Length);

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
            switch (state)
            {
                case 0:
                    return TryGetNextFirstPath(out current);
                case 1:
                    return source.TryGetNext(out current);
                case 2:
                    return ringBuffer!.TryDequeue(out current);
                default:
                    Unsafe.SkipInit(out current);
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        bool TryGetNextFirstPath(out TSource current)
        {
            Unsafe.SkipInit(out current);
            if (takeCount == 0)
            {
                state = 3;
                return false;
            }

            if (source.TryGetNonEnumeratedCount(out var totalCount))
            {
                var skipCount = Math.Max(0, totalCount - takeCount);
                while (source.TryGetNext(out current))
                {
                    if (--skipCount >= 0) continue;
                    state = 1; // Mark as enumerating
                    return true;
                }

                // Source exhausted
                state = 3;
                return false;
            }
            else
            {

                var buffer =  ringBuffer = new(takeCount); // Use a reasonable default capacity;

                while (source.TryGetNext(out var item))
                {
                    buffer.Enqueue(item);
                }

                state = 2; // Mark as collected

                if (buffer.TryDequeue(out current))
                {
                    return true;
                }

                state = 3;
                return false;
            }
        }

        public void Dispose()
        {
            ringBuffer?.Dispose();
            source.Dispose();
        }
    }
}
