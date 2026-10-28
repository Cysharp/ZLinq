using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        /// <summary>Generates a sequence that begins with the specified start value and advances by the step value until reaching the inclusive end value.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="endInclusive">The inclusive bound of the sequence. If step is positive, it is the upper bound; if step is negative, it is the lower bound.</param>
        /// <param name="step">The difference between each element and the next one.</param>
        /// <returns>A sequence that contains the sequence of values.</returns>
        public static ValueEnumerable<FromSequenceDateTime, DateTime> Sequence(DateTime start, DateTime endInclusive, TimeSpan step)
        {
            if (step == TimeSpan.Zero) // (T.IsZero(step))
            {
                if (start != endInclusive)
                {
                    Throws.ArgumentOutOfRange(nameof(step));
                }

                // repeat one
                return new(new(start, endInclusive, step, isIncrement: true));
            }
            else if (step >= TimeSpan.Zero) // (T.IsPositive(step))
            {
                if (endInclusive < start)
                {
                    Throws.ArgumentOutOfRange(nameof(endInclusive));
                }

                // increment pattern
                return new(new(start, endInclusive, step, isIncrement: true));
            }
            else
            {
                if (endInclusive > start)
                {
                    Throws.ArgumentOutOfRange(nameof(endInclusive));
                }

                // decrement pattern
                return new(new(start, endInclusive, step, isIncrement: false));
            }
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromSequenceDateTime(DateTime currentValue, DateTime endInclusive, TimeSpan step, bool isIncrement) : IValueEnumerator<DateTime>
    {
        bool calledGetNext;

        public bool TryGetNext(out DateTime current)
        {
            if (!calledGetNext)
            {
                calledGetNext = true;
                current = currentValue;
                return true;
            }

            if (isIncrement)
            {
                var next = unchecked((DateTime)(currentValue + step));

                if (next >= endInclusive || next <= currentValue)
                {
                    if (next == endInclusive && currentValue != next)
                    {
                        current = currentValue = next;
                        return true;
                    }

                    current = default!;
                    return false;
                }

                current = currentValue = next;
                return true;
            }
            else
            {
                var next = unchecked((DateTime)(currentValue + step));

                if (next <= endInclusive || next >= currentValue)
                {
                    if (next == endInclusive && currentValue != next)
                    {
                        current = currentValue = next;
                        return true;
                    }

                    current = default!;
                    return false;
                }

                current = currentValue = next;
                return true;
            }
        }

        public void Dispose()
        {
        }
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
    }
}

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        /// <summary>Generates a sequence that begins with the specified start value and advances by the step value until reaching the inclusive end value.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="endInclusive">The inclusive bound of the sequence. If step is positive, it is the upper bound; if step is negative, it is the lower bound.</param>
        /// <param name="step">The difference between each element and the next one.</param>
        /// <returns>A sequence that contains the sequence of values.</returns>
        public static ValueEnumerable<FromSequenceDateTimeOffset, DateTimeOffset> Sequence(DateTimeOffset start, DateTimeOffset endInclusive, TimeSpan step)
        {
            if (step == TimeSpan.Zero) // (T.IsZero(step))
            {
                if (start != endInclusive)
                {
                    Throws.ArgumentOutOfRange(nameof(step));
                }

                // repeat one
                return new(new(start, endInclusive, step, isIncrement: true));
            }
            else if (step >= TimeSpan.Zero) // (T.IsPositive(step))
            {
                if (endInclusive < start)
                {
                    Throws.ArgumentOutOfRange(nameof(endInclusive));
                }

                // increment pattern
                return new(new(start, endInclusive, step, isIncrement: true));
            }
            else
            {
                if (endInclusive > start)
                {
                    Throws.ArgumentOutOfRange(nameof(endInclusive));
                }

                // decrement pattern
                return new(new(start, endInclusive, step, isIncrement: false));
            }
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromSequenceDateTimeOffset(DateTimeOffset currentValue, DateTimeOffset endInclusive, TimeSpan step, bool isIncrement) : IValueEnumerator<DateTimeOffset>
    {
        bool calledGetNext;

        public bool TryGetNext(out DateTimeOffset current)
        {
            if (!calledGetNext)
            {
                calledGetNext = true;
                current = currentValue;
                return true;
            }

            if (isIncrement)
            {
                var next = unchecked((DateTimeOffset)(currentValue + step));

                if (next >= endInclusive || next <= currentValue)
                {
                    if (next == endInclusive && currentValue != next)
                    {
                        current = currentValue = next;
                        return true;
                    }

                    current = default!;
                    return false;
                }

                current = currentValue = next;
                return true;
            }
            else
            {
                var next = unchecked((DateTimeOffset)(currentValue + step));

                if (next <= endInclusive || next >= currentValue)
                {
                    if (next == endInclusive && currentValue != next)
                    {
                        current = currentValue = next;
                        return true;
                    }

                    current = default!;
                    return false;
                }

                current = currentValue = next;
                return true;
            }
        }

        public void Dispose()
        {
        }
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
    }
}
