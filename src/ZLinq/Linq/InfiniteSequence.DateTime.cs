using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        /// <summary>Generates an infinite sequence that begins with the specified start value and advances by the step value.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="step">The difference between each element and the next one.</param>
        /// <returns>A sequence that contains the infinite sequence of values.</returns>
        public static ValueEnumerable<FromInfiniteSequenceDateTime, DateTime> InfiniteSequence(DateTime start, TimeSpan step)
        {
            return new(new(start, step));
        }

        /// <summary>Generates an infinite sequence that begins with the specified start value and advances by the step value.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="step">The difference between each element and the next one.</param>
        /// <returns>A sequence that contains the infinite sequence of values.</returns>
        public static ValueEnumerable<FromInfiniteSequenceDateTimeOffset, DateTimeOffset> InfiniteSequence(DateTimeOffset start, TimeSpan step)
        {
            return new(new(start, step));
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromInfiniteSequenceDateTime(DateTime start, TimeSpan step) : IValueEnumerator<DateTime>
    {
        bool calledGetNext;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<DateTime> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<DateTime> destination, Index offset)
        {
            return false;
        }

        public bool TryGetNext(out DateTime current)
        {
            if (!calledGetNext)
            {
                calledGetNext = true;
                current = start;
                return true;
            }

            current = start += step;
            return true;
        }

        public void Dispose()
        {
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromInfiniteSequenceDateTimeOffset(DateTimeOffset start, TimeSpan step) : IValueEnumerator<DateTimeOffset>
    {
        bool calledGetNext;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<DateTimeOffset> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<DateTimeOffset> destination, Index offset)
        {
            return false;
        }

        public bool TryGetNext(out DateTimeOffset current)
        {
            if (!calledGetNext)
            {
                calledGetNext = true;
                current = start;
                return true;
            }

            current = start += step;
            return true;
        }

        public void Dispose()
        {
        }
    }
}
