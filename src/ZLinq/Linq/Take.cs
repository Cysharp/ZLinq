using System;

namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<Take<TEnumerator, TSource>, TSource> Take<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Int32 count)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, count));

        public static ValueEnumerable<TakeRange<TEnumerator, TSource>, TSource> Take<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Range range)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, range));

        // Optimize

        public static ValueEnumerable<TakeSkip<TEnumerator, TSource>, TSource> Skip<TEnumerator, TSource>(this ValueEnumerable<Take<TEnumerator, TSource>, TSource> source, int count)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source.Enumerator.Skip(count));

        public static ValueEnumerable<TakeSkip<TEnumerator, TSource>, TSource> Skip<TEnumerator, TSource>(this ValueEnumerable<TakeSkip<TEnumerator, TSource>, TSource> source, int count)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source.Enumerator.Skip(count));
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
    struct Take<TEnumerator, TSource>(TEnumerator source, Int32 count)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        internal readonly int takeCount = Math.Max(0, count); // allows negative count
        int index;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (source.TryGetNonEnumeratedCount(out count))
            {
                count = Math.Min(count, takeCount); // take smaller
                return true;
            }

            count = default;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            if (source.TryGetSpan(out span))
            {
                span = span.Slice(0, Math.Min(span.Length, takeCount));
                return true;
            }

            span = default;
            return false;
        }

        public bool TryCopyTo(Span<TSource> destination, Index offset)
        {
            if (source.TryGetNonEnumeratedCount(out var count))
            {
                var actualTakeCount = Math.Min(count, takeCount);

                // When offset IsFromEnd, calculate the source offset from take count.
                var sourceOffset = offset.GetOffset(actualTakeCount);

                if (sourceOffset < 0 || sourceOffset >= actualTakeCount)
                {
                    return false; // out of range mark as fail(for example ElementAt needs failed information).
                }

                // Remaining differs depending on whether it's from the beginning or from the end.
                var remainingElements = offset.IsFromEnd
                    ? offset.Value
                    : actualTakeCount - sourceOffset;

                var elementsToCopy = Math.Min(remainingElements, destination.Length);
                return source.TryCopyTo(destination.Slice(0, elementsToCopy), sourceOffset);
            }

            return false;
        }

        public bool TryGetNext(out TSource current)
        {
            if (index++ < takeCount && source.TryGetNext(out current))
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

        // Optimize
        internal TakeSkip<TEnumerator, TSource> Skip(int skipCount) => new(source, takeCount: takeCount, skipCount: skipCount);
    }

    [StructLayout(LayoutKind.Auto)]
    internal struct RangeProcessor(Range range)
    {
        public readonly Range Range = range;
        public int SkipIndex = 0;
        public int Remains = -2; // -2: uninitialized, -1: unknown count, >=0: known count
        public int Index = 0;
        public int FromEndQueueSize = 0;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Initialize(int? knownCount = null)
        {
            if (Remains != -2) return;

            if (knownCount.HasValue)
            {
                InitializeWithKnownCount(knownCount.Value);
            }
            else if (!Range.Start.IsFromEnd && !Range.End.IsFromEnd)
            {
                InitializeBothFromStart();
            }
            else
            {
                InitializeWithUnknownCount();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        void InitializeBothFromStart()
        {
            SkipIndex = Range.Start.Value;
            var length = Range.End.Value - Range.Start.Value;
            Remains = Math.Max(0, length);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        void InitializeWithKnownCount(int count)
        {
            var start = Range.Start.IsFromEnd
                ? Math.Max(0, count - Range.Start.Value)
                : Range.Start.Value;
            var end = Range.End.IsFromEnd
                ? Math.Max(0, count - Range.End.Value)
                : Range.End.Value;

            SkipIndex = Math.Min(start, count);
            Remains = Math.Max(0, Math.Min(end, count) - SkipIndex);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        void InitializeWithUnknownCount()
        {
            if (!Range.Start.IsFromEnd && Range.End.IsFromEnd)
            {
                SkipIndex = Range.Start.Value;
                Remains = Range.End.Value == 0 ? int.MaxValue : -1;

                if (Range.End.Value > 0)
                {
                    FromEndQueueSize = Range.End.Value;
                }
            }
            else if (Range.Start.IsFromEnd && !Range.End.IsFromEnd)
            {
                SkipIndex = 0;
                Remains = -1;
                FromEndQueueSize = Math.Max(1, Range.Start.Value);
            }
            else
            {
                SkipIndex = 0;
                var maxCount = Range.Start.Value - Range.End.Value;
                Remains = maxCount <= 0 ? 0 : -1;

                if (maxCount > 0)
                {
                    FromEndQueueSize = Math.Max(1, Range.Start.Value);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void CalculateRemainsFromQueue(int totalCount, int queueCount)
        {
            var start = Range.Start.IsFromEnd
                ? Math.Max(0, totalCount - Range.Start.Value)
                : Range.Start.Value;
            var end = Range.End.IsFromEnd
                ? Math.Max(0, totalCount - Range.End.Value)
                : Range.End.Value;

            Remains = Math.Max(0, end - start);

            var currentPosition = totalCount - queueCount;
            var toSkip = Math.Max(0, start - currentPosition);

            Remains = Math.Min(Remains - toSkip, queueCount - toSkip);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public int GetQueueSkipCount(int totalCount, int queueCount)
        {
            var start = Range.Start.IsFromEnd
                ? Math.Max(0, totalCount - Range.Start.Value)
                : Range.Start.Value;

            var currentPosition = totalCount - queueCount;
            return Math.Max(0, start - currentPosition);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public (int offsetInRange, int elementsToCopy) CalculateCopyParameters(
            int totalCount, int destinationLength, Index offset)
        {
            var effectiveRemains = Math.Min(Remains, Math.Max(0, totalCount - SkipIndex));
            if (effectiveRemains <= 0)
                return (-1, 0);

            var offsetInRange = offset.GetOffset(effectiveRemains);
            if (offsetInRange < 0 || offsetInRange >= effectiveRemains)
                return (-1, 0);

            var elementsToCopy = Math.Min(effectiveRemains - offsetInRange, destinationLength);
            return (offsetInRange, elementsToCopy);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void SkipQueue<TSource>(ref ValueQueue<TSource> queue)
        {
            CalculateRemainsFromQueue(Index, queue.Count);

            var toSkip = GetQueueSkipCount(Index, queue.Count);
            while (toSkip > 0 && queue.Count > 0)
            {
                queue.Dequeue();
                toSkip--;
            }
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct TakeRange<TEnumerator, TSource>(TEnumerator source, Range range)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        RangeProcessor rangeProcessor = new(range);
        RefBox<ValueQueue<TSource>>? q;

        [MethodImpl(MethodImplOptions.NoInlining)]
        void Init()
        {
            if (rangeProcessor.Remains != -2) return;

            if (source.TryGetNonEnumeratedCount(out var count))
            {
                rangeProcessor.Initialize(count);
            }
            else
            {
                rangeProcessor.Initialize();
            }

            if (rangeProcessor.FromEndQueueSize > 0)
            {
                q = new(new(Math.Min(rangeProcessor.FromEndQueueSize, 16)));
            }
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (!source.TryGetNonEnumeratedCount(out count))
            {
                return false;
            }

            if (rangeProcessor.Remains == -2)
            {
                rangeProcessor.Initialize(count);
            }

            if (rangeProcessor.Remains >= 0)
            {
                count = rangeProcessor.Remains;
                return true;
            }

            count = default;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            Init();

            if (rangeProcessor.Remains >= 0 && source.TryGetSpan(out span))
            {
                span = span.Slice(rangeProcessor.SkipIndex, rangeProcessor.Remains);
                return true;
            }

            span = default;
            return false;
        }

        public bool TryCopyTo(Span<TSource> destination, Index offset)
        {
            Init();

            if (rangeProcessor.Remains < 0 || !source.TryGetNonEnumeratedCount(out var totalCount))
                return false;

            var (offsetInRange, elementsToCopy) = rangeProcessor.CalculateCopyParameters(
                totalCount, destination.Length, offset);

            if (elementsToCopy <= 0)
                return false;

            return source.TryCopyTo(destination[..elementsToCopy],rangeProcessor.SkipIndex + offsetInRange);
        }

        public bool TryGetNext(out TSource current)
        {
            var remains = rangeProcessor.Remains;
            if (remains == -2)
            {
                Init();
            }

            if (remains == 0)
            {
                Unsafe.SkipInit(out current);
                return false;
            }

            return q == null
                ? TryGetNextSimple(out current)
                : TryGetNextWithQueue(out current);
        }

        bool TryGetNextSimple(out TSource current)
        {
            ref var rangeProcessor = ref this.rangeProcessor;
            while (rangeProcessor.Index < rangeProcessor.SkipIndex)
            {
                if (!source.TryGetNext(out _))
                {
                    rangeProcessor.Remains = 0;
                    Unsafe.SkipInit(out current);
                    return false;
                }

                rangeProcessor.Index++;
            }

            if (rangeProcessor.Remains > 0)
            {
                if (source.TryGetNext(out current))
                {
                    if (rangeProcessor.Remains != int.MaxValue)
                        rangeProcessor.Remains--;
                    return true;
                }
            }

            rangeProcessor.Remains = 0;
            Unsafe.SkipInit(out current);
            return false;
        }

        bool TryGetNextWithQueue(out TSource current)
        {
            ref var rangeProcessor = ref this.rangeProcessor;
            ref var queue = ref q!.GetValueRef();

            // Fast path: dequeue from processed queue
            if (rangeProcessor.Remains > 0 && queue.Count > 0)
            {
                goto DEQUEUE_RETURN;
            }

            // Fill queue from source
            var useSlidingWindow = !rangeProcessor.Range.Start.IsFromEnd && rangeProcessor.Range.End.IsFromEnd;

            while (source.TryGetNext(out current))
            {
                if (rangeProcessor.Index++ < rangeProcessor.SkipIndex)
                    continue;

                if (queue.Count == rangeProcessor.FromEndQueueSize)
                {
                    var result = queue.Dequeue();
                    if (useSlidingWindow)
                    {
                        // Sliding window: return oldest element immediately
                        queue.Enqueue(current);
                        current = result;
                        return true;
                    }
                }
                queue.Enqueue(current);
            }

            if (queue.Count == 0 || useSlidingWindow)
            {
                goto END;
            }

            if (rangeProcessor.Remains == -1)
                rangeProcessor.SkipQueue(ref queue);

        DEQUEUE_RETURN:
            if (rangeProcessor.Remains > 0)
            {
                current = queue.Dequeue();
                rangeProcessor.Remains--;
                return true;
            }

        END:
            rangeProcessor.Remains = 0;
            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            q?.Dispose();
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
        struct TakeSkip<TEnumerator, TSource>(TEnumerator source, Int32 takeCount, Int32 skipCount)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        readonly int takeCount = Math.Max(0, takeCount); // ensure non-negative
        readonly int skipCount = Math.Max(0, skipCount); // ensure non-negative
        int taken;
        int skipped;
        bool reachedTakeLimit;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (source.TryGetNonEnumeratedCount(out count))
            {
                // Apply take limit first
                count = Math.Min(count, takeCount);

                // Then apply skip
                count = Math.Max(0, count - skipCount);

                return true;
            }

            count = default;
            return false;
        }

        public bool TryCopyTo(Span<TSource> destination, Index offset)
        {
            if (source.TryGetNonEnumeratedCount(out var sourceCount))
            {
                // First limit by take
                var afterTake = Math.Min(sourceCount, takeCount);

                // Then apply skip
                var actualSkipCount = Math.Min(afterTake, skipCount);
                var actualCount = afterTake - actualSkipCount;

                if (actualCount <= 0)
                {
                    return false;
                }

                // Calculate offset within the resulting sequence
                var offsetInResult = offset.GetOffset(actualCount);

                if (offsetInResult < 0 || offsetInResult >= actualCount)
                {
                    return false;
                }

                // Calculate the source offset (within the take window, then skip)
                var sourceOffset = actualSkipCount + offsetInResult;

                // Calculate elements available after offset
                var elementsAvailable = actualCount - offsetInResult;

                // Calculate elements to copy
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
                // First apply take limit
                if (span.Length > takeCount)
                {
                    span = span.Slice(0, takeCount);
                }

                // Then skip elements
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
            if (IsResultEmpty())
            {
                Unsafe.SkipInit(out current);
                return false;
            }

            // Once we've reached the take limit, we're done
            if (reachedTakeLimit)
            {
                Unsafe.SkipInit(out current);
                return false;
            }

            // Skip elements that have been emitted from the take portion
            while (skipped < skipCount)
            {
                // First ensure we haven't exceeded the take limit
                if (taken >= takeCount)
                {
                    reachedTakeLimit = true;
                    Unsafe.SkipInit(out current);
                    return false;
                }

                // Get next element from source
                if (!source.TryGetNext(out var _))
                {
                    Unsafe.SkipInit(out current);
                    return false;
                }

                taken++;

                // If we've reached the take limit before finishing skipping,
                // we won't be able to return any elements
                if (taken >= takeCount)
                {
                    reachedTakeLimit = true;
                    Unsafe.SkipInit(out current);
                    return false;
                }

                skipped++;
            }

            // Check if we've reached the take limit
            if (taken >= takeCount)
            {
                reachedTakeLimit = true;
                Unsafe.SkipInit(out current);
                return false;
            }

            // Return elements after taking and skipping
            if (source.TryGetNext(out current))
            {
                taken++;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        bool IsResultEmpty()
        {
            if (takeCount == 0)
            {
                return true;
            }

            if (skipCount >= takeCount)
            {
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }

        // Optimize

        internal TakeSkip<TEnumerator, TSource> Skip(int count)
        {
            if (count <= 0)
            {
                return this; // no changes.
            }

            // check overflow
            int newSkipCount;
            if (count > 0 && skipCount > int.MaxValue - count)
            {
                newSkipCount = int.MaxValue;
            }
            else
            {
                newSkipCount = skipCount + count;
            }

            return new TakeSkip<TEnumerator, TSource>(source, takeCount, newSkipCount);
        }
    }
}
