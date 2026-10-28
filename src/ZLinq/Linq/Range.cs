#if NET8_0_OR_GREATER
using System.Numerics;
#endif

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        /// <summary>Generates a sequence of sequential values within a specified range.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="count">The number of elements to generate.</param>
        /// <returns>A sequence that contains a range of sequential values.</returns>
        public static ValueEnumerable<FromRange, int> Range(int start, int count)
        {
            long max = ((long)start) + count - 1;
            if (count < 0 || max > int.MaxValue)
            {
                Throws.ArgumentOutOfRange(nameof(count));
            }

            return new(new(start, count));
        }

        /// <summary>Generates a sequence of sequential values within a specified range.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="count">The number of elements to generate.</param>
        /// <param name="step">The difference between each element and the next one.</param>
        /// <returns>A sequence that contains a range of sequential values.</returns>
        public static ValueEnumerable<FromRangeDateTime, DateTime> Range(DateTime start, int count, TimeSpan step) => new(new(start, count, step));

        /// <summary>Generates a sequence of sequential values within a specified range.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="count">The number of elements to generate.</param>
        /// <param name="step">The difference between each element and the next one.</param>
        /// <returns>A sequence that contains a range of sequential values.</returns>
        public static ValueEnumerable<FromRangeDateTimeOffset, DateTimeOffset> Range(DateTimeOffset start, int count, TimeSpan step) => new(new(start, count, step));

#if NET8_0_OR_GREATER

        /// <summary>Generates a sequence of sequential values within a specified range.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="count">The number of elements to generate.</param>
        /// <returns>A sequence that contains a range of sequential values.</returns>
        public static ValueEnumerable<FromRange<T, T>, T> Range<T>(T start, int count)
            where T : INumberBase<T>
        {
            if (count < 0)
            {
                Throws.ArgumentOutOfRange(nameof(count));
            }

            return new(new(start, count, T.One));
        }

        /// <summary>Generates a sequence of sequential values within a specified range.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="count">The number of elements to generate.</param>
        /// <param name="step">The difference between each element and the next one.</param>
        /// <returns>A sequence that contains a range of sequential values.</returns>
        public static ValueEnumerable<FromRange<T, TStep>, T> Range<T, TStep>(T start, int count, TStep step)
            where T : IAdditionOperators<T, TStep, T>
        {
            if (count < 0)
            {
                Throws.ArgumentOutOfRange(nameof(count));
            }

            return new(new(start, count, step));
        }

#endif

        #region Obsolete

        /// <summary>Generates a sequence of sequential values within a specified range.</summary>
        /// <param name="range">The range of integers to generate. The start must be an index from the start; the end can be <c>^0</c> to generate an infinite sequence.</param>
        /// <param name="rightBound">Whether the end of the range is included in the sequence.</param>
        /// <returns>A sequence that contains a range of sequential values.</returns>
        [Obsolete("Use ValueEnumerable.Sequence instead. This will be removed in a future version.")]
        public static ValueEnumerable<FromRange2, int> Range(Range range, RightBound rightBound = RightBound.Exclusive)
        {
            if (range.Start.IsFromEnd)
            {
                Throws.IsFromEnd(nameof(range));
            }

            if (range.End.IsFromEnd)
            {
                if (range.End.Value == 0)
                {
                    return new(new(range.Start.Value, 0, true)); // infinite
                }
                Throws.IsFromEnd(nameof(range));
            }

            var start = range.Start.Value;
            var count = range.End.Value - range.Start.Value;
            if (rightBound == RightBound.Inclusive)
            {
                count++;
            }

            long max = ((long)start) + count - 1;
            if (count < 0 || max > int.MaxValue)
            {
                Throws.ArgumentOutOfRange(nameof(range));
            }

            return new(new(start, count, false));
        }

#if NET8_0_OR_GREATER

        /// <summary>Generates a sequence of sequential values within a specified range.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="end">The end value of the range.</param>
        /// <param name="rightBound">Whether the end value is included in the sequence.</param>
        /// <returns>A sequence that contains a range of sequential values.</returns>
        [Obsolete("Use ValueEnumerable.Sequence instead. This will be removed in a future version.")]
        public static ValueEnumerable<FromRangeTo<T, T>, T> Range<T>(T start, T end, RightBound rightBound)
            where T : INumberBase<T>, IComparisonOperators<T, T, bool>
        {
            var step = start < end ? T.One : -T.One;
            return new(new(start, end, step, rightBound));
        }

        /// <summary>Generates a sequence of sequential values within a specified range.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="end">The end value of the range.</param>
        /// <param name="step">The difference between each element and the next one.</param>
        /// <param name="rightBound">Whether the end value is included in the sequence.</param>
        /// <returns>A sequence that contains a range of sequential values.</returns>
        [Obsolete("Use ValueEnumerable.Sequence instead. This will be removed in a future version.")]
        public static ValueEnumerable<FromRangeTo<T, TStep>, T> Range<T, TStep>(T start, T end, TStep step, RightBound rightBound)
            where T : IAdditionOperators<T, TStep, T>, IComparisonOperators<T, T, bool>
        {
            return new(new(start, end, step, rightBound));
        }

#endif

        /// <summary>Generates a sequence of sequential values within a specified range.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="end">The end value of the range.</param>
        /// <param name="step">The difference between each element and the next one.</param>
        /// <param name="rightBound">Whether the end value is included in the sequence.</param>
        /// <returns>A sequence that contains a range of sequential values.</returns>
        [Obsolete("Use ValueEnumerable.Sequence instead. This will be removed in a future version.")]
        public static ValueEnumerable<FromRangeDateTimeTo, DateTime> Range(DateTime start, DateTime end, TimeSpan step, RightBound rightBound) => new(new(start, end, step, rightBound));

        /// <summary>Generates a sequence of sequential values within a specified range.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="end">The end value of the range.</param>
        /// <param name="step">The difference between each element and the next one.</param>
        /// <param name="rightBound">Whether the end value is included in the sequence.</param>
        /// <returns>A sequence that contains a range of sequential values.</returns>
        [Obsolete("Use ValueEnumerable.Sequence instead. This will be removed in a future version.")]
        public static ValueEnumerable<FromRangeDateTimeOffsetTo, DateTimeOffset> Range(DateTimeOffset start, DateTimeOffset end, TimeSpan step, RightBound rightBound) => new(new(start, end, step, rightBound));

        #endregion
    }

    [Obsolete("Use ValueEnumerable.Sequence instead. This will be removed in a future version.")]
    public enum RightBound
    {
        Inclusive,
        Exclusive
    }
}

namespace ZLinq.Linq
{
    // Standard Range implementation

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromRange(int start, int count) : IValueEnumerator<int>
    {
        internal readonly int count = count;
        internal readonly int start = start;
        internal readonly int to = start + count; // wraps to int.MinValue when the last element is int.MaxValue, so compare with != instead of <
        int value = start;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = this.count;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<int> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<int> destination, Index offset)
        {
            if (EnumeratorHelper.TryGetSliceRange(count, offset, destination.Length, out var fillStart, out var fillCount))
            {
                FillIncremental(destination.Slice(0, fillCount), start + fillStart);
                return true;
            }

            return false;
        }

        public bool TryGetNext(out int current)
        {
            if (value != to)
            {
                current = value;
                value++;
                return true;
            }

            current = 0;
            return false;
        }

        public void Dispose()
        {
        }

        // borrowed from .NET Enumerable.Range vectorized fill, originally implemented by @neon-sunset.
        internal static void FillIncremental(Span<int> span, int start)
        {
            ref var current = ref MemoryMarshal.GetReference(span);
            ref var end = ref Unsafe.Add(ref current, span.Length);

#if NET8_0_OR_GREATER
            if (Vector.IsHardwareAccelerated
                && Vector<int>.IsSupported
#if NET8_0
                && Vector<int>.Count <= 16
#endif
                && span.Length >= Vector<int>.Count)
            {
#if NET9_0_OR_GREATER
                var indices = Vector<int>.Indices;                   // <0, 1, 2, 3, 4, 5, 6, 7>...
#else
                var indices = new Vector<int>((ReadOnlySpan<int>)new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
#endif
                // for example, start = 5, Vector<int>.Count = 8
                var data = indices + new Vector<int>(start);         // <5, 6, 7, 8, 9, 10, 11, 12>...
                var increment = new Vector<int>(Vector<int>.Count);  // <8, 8, 8, 8, 8, 8, 8, 8>...

                ref var to = ref Unsafe.Subtract(ref end, Vector<int>.Count);
                do
                {
                    data.StoreUnsafe(ref current);                              // copy vectorized data to Span pointer
                    data += increment;                                          // <13, 14, 15, 16, 17, 18, 19, 20>...
                    current = ref Unsafe.Add(ref current, Vector<int>.Count);   // move pointer++

                    // available space for vectorized copy
                    // (current <= to) -> !(current > to)
                } while (!Unsafe.IsAddressGreaterThan(ref current, ref to));

                start = data[0]; // next value for fill
            }
#endif

            // fill rest
            while (Unsafe.IsAddressLessThan(ref current, ref end))
            {
                current = start++; // reuse local variable
                current = ref Unsafe.Add(ref current, 1);
            }
        }
    }

#if NET8_0_OR_GREATER

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromRange<T, TStep>(T start, int count, TStep step) : IValueEnumerator<T>
        where T : IAdditionOperators<T, TStep, T>
    {
        readonly int count = count;
        readonly TStep step = step;

        T value = start;
        int index = 0;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = this.count;
            return true;
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
            if (index < count)
            {
                if (index != 0)
                {
                    checked
                    {
                        value += step;
                    }
                }
                current = value;
                index++;
                return true;
            }

            current = default(T)!;
            return false;
        }

        public void Dispose()
        {
        }
    }

#endif

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromRangeDateTime(DateTime start, int count, TimeSpan step) : IValueEnumerator<DateTime>
    {
        readonly int count = count;
        readonly TimeSpan timeSpan = step;

        int index = 0;
        DateTime value = start;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = this.count;
            return true;
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
            if (index < count)
            {
                if (index != 0)
                {
                    checked
                    {
                        value += step;
                    }
                }
                current = value;
                index++;
                return true;
            }

            current = default(DateTime)!;
            return false;
        }

        public void Dispose()
        {
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromRangeDateTimeOffset(DateTimeOffset start, int count, TimeSpan step) : IValueEnumerator<DateTimeOffset>
    {
        readonly int count = count;
        readonly TimeSpan timeSpan = step;

        int index = 0;
        DateTimeOffset value = start;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = this.count;
            return true;
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
            if (index < count)
            {
                if (index != 0)
                {
                    checked
                    {
                        value += step;
                    }
                }
                current = value;
                index++;
                return true;
            }

            current = default(DateTimeOffset)!;
            return false;
        }

        public void Dispose()
        {
        }
    }

    #region Obsolete

    // for `System.Range`

    [Obsolete("Use ValueEnumerable.Sequence instead. This will be removed in a future version.")]
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromRange2(int start, int count, bool isInfinite) : IValueEnumerator<int>
    {
        readonly int count = count;
        readonly int start = start;
        readonly int to = isInfinite ? int.MaxValue - start : start + count;
        readonly bool isInfinite = isInfinite;
        int value = start;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (isInfinite)
            {
                count = 0;
                return false;
            }

            count = this.count;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<int> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<int> destination, Index offset)
        {
            if (isInfinite) return false;

            if (EnumeratorHelper.TryGetSliceRange(count, offset, destination.Length, out var fillStart, out var fillCount))
            {
                FromRange.FillIncremental(destination.Slice(0, fillCount), start + fillStart);
                return true;
            }

            return false;
        }

        public bool TryGetNext(out int current)
        {
            if (value < to)
            {
                current = value;
                checked
                {
                    value++;
                }
                return true;
            }

            current = 0;
            return false;
        }

        public void Dispose()
        {
        }
    }

#if NET8_0_OR_GREATER

    [Obsolete("Use ValueEnumerable.Sequence instead. This will be removed in a future version.")]
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromRangeTo<T, TStep>(T start, T end, TStep step, RightBound rightBound) : IValueEnumerator<T>
        where T : IAdditionOperators<T, TStep, T>, IComparisonOperators<T, T, bool>
    {
        readonly T end = end;
        readonly TStep step = step;
        readonly RightBound rightBound = rightBound;
        bool forward = start < end;

        T value = start;
        bool first = true;

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
            if (first)
            {
                current = value;
                first = false;
                return true;
            }

            checked
            {
                value += step;
            }

            if (forward)
            {
                if (value < end || (rightBound == RightBound.Inclusive && value <= end))
                {
                    current = value;
                    return true;
                }
            }
            else
            {
                if (value > end || (rightBound == RightBound.Inclusive && value >= end))
                {
                    current = value;
                    return true;
                }
            }

            current = default(T)!;
            return false;
        }

        public void Dispose()
        {
        }
    }

#endif

    [Obsolete("Use ValueEnumerable.Sequence instead. This will be removed in a future version.")]
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromRangeDateTimeTo(DateTime start, DateTime end, TimeSpan step, RightBound rightBound) : IValueEnumerator<DateTime>
    {
        readonly DateTime end = end;
        readonly TimeSpan step = step;
        readonly RightBound rightBound = rightBound;
        bool forward = start < end;

        DateTime value = start;
        bool first = true;

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
            if (first)
            {
                current = value;
                first = false;
                return true;
            }

            checked
            {
                value += step;
            }

            if (forward)
            {
                if (value < end || (rightBound == RightBound.Inclusive && value <= end))
                {
                    current = value;
                    return true;
                }
            }
            else
            {
                if (value > end || (rightBound == RightBound.Inclusive && value >= end))
                {
                    current = value;
                    return true;
                }
            }

            current = default(DateTime)!;
            return false;
        }

        public void Dispose()
        {
        }
    }

    [Obsolete("Use ValueEnumerable.Sequence instead. This will be removed in a future version.")]
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromRangeDateTimeOffsetTo(DateTimeOffset start, DateTimeOffset end, TimeSpan step, RightBound rightBound) : IValueEnumerator<DateTimeOffset>
    {
        readonly DateTimeOffset end = end;
        readonly TimeSpan step = step;
        readonly RightBound rightBound = rightBound;

        DateTimeOffset value = start;
        bool first = true;
        bool forward = start < end;

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
            if (first)
            {
                current = value;
                first = false;
                return true;
            }

            checked
            {
                value += step;
            }

            if (forward)
            {
                if (value < end || (rightBound == RightBound.Inclusive && value <= end))
                {
                    current = value;
                    return true;
                }
            }
            else
            {
                if (value > end || (rightBound == RightBound.Inclusive && value >= end))
                {
                    current = value;
                    return true;
                }
            }

            current = default(DateTimeOffset)!;
            return false;
        }

        public void Dispose()
        {
        }
    }

    #endregion
}
