#if NET8_0_OR_GREATER

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
        public static ValueEnumerable<FromInfiniteSequence<T>, T> InfiniteSequence<T>(T start, T step)
            where T : IAdditionOperators<T, T, T>
        {
            if (start is null) Throws.Null(nameof(start));
            if (step is null) Throws.Null(nameof(step));

            return new(new(start, step));
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromInt32InfiniteSequence2<T>(int start, int step) : IValueEnumerator<int>
    {
        bool calledGetNext;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<int> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<int> destination, Index offset)
        {
            return false;
        }

        public bool TryGetNext(out int current)
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
    public struct FromInfiniteSequence<T>(T start, T step) : IValueEnumerator<T>
        where T : IAdditionOperators<T, T, T>
    {
        bool calledGetNext;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<T> destination, Index offset)
        {
            return false;
        }

        public bool TryGetNext(out T current)
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

#endif
