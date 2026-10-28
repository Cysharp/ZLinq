// This file is generated from FileGen.Command.cs
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

// support for all platforms
// byte/sbyte/ushort/char/short/uint/int/ulong/long
// float/double/decimal/DateTime/DateTimeOffset

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        /// <summary>Generates an infinite sequence that begins with the specified start value and advances by the step value.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="step">The difference between each element and the next one.</param>
        /// <returns>A sequence that contains the infinite sequence of values.</returns>
        public static ValueEnumerable<FromByteInfiniteSequence, Byte> InfiniteSequence(Byte start, Byte step)
        {
            return new(new(start, step));
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromByteInfiniteSequence(Byte start, Byte step) : IValueEnumerator<Byte>
    {
        bool calledGetNext;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<Byte> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<Byte> destination, Index offset)
        {
            return false;
        }

        public bool TryGetNext(out Byte current)
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

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        /// <summary>Generates an infinite sequence that begins with the specified start value and advances by the step value.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="step">The difference between each element and the next one.</param>
        /// <returns>A sequence that contains the infinite sequence of values.</returns>
        public static ValueEnumerable<FromSByteInfiniteSequence, SByte> InfiniteSequence(SByte start, SByte step)
        {
            return new(new(start, step));
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromSByteInfiniteSequence(SByte start, SByte step) : IValueEnumerator<SByte>
    {
        bool calledGetNext;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<SByte> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<SByte> destination, Index offset)
        {
            return false;
        }

        public bool TryGetNext(out SByte current)
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

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        /// <summary>Generates an infinite sequence that begins with the specified start value and advances by the step value.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="step">The difference between each element and the next one.</param>
        /// <returns>A sequence that contains the infinite sequence of values.</returns>
        public static ValueEnumerable<FromUInt16InfiniteSequence, UInt16> InfiniteSequence(UInt16 start, UInt16 step)
        {
            return new(new(start, step));
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromUInt16InfiniteSequence(UInt16 start, UInt16 step) : IValueEnumerator<UInt16>
    {
        bool calledGetNext;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<UInt16> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<UInt16> destination, Index offset)
        {
            return false;
        }

        public bool TryGetNext(out UInt16 current)
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

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        /// <summary>Generates an infinite sequence that begins with the specified start value and advances by the step value.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="step">The difference between each element and the next one.</param>
        /// <returns>A sequence that contains the infinite sequence of values.</returns>
        public static ValueEnumerable<FromInt16InfiniteSequence, Int16> InfiniteSequence(Int16 start, Int16 step)
        {
            return new(new(start, step));
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromInt16InfiniteSequence(Int16 start, Int16 step) : IValueEnumerator<Int16>
    {
        bool calledGetNext;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<Int16> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<Int16> destination, Index offset)
        {
            return false;
        }

        public bool TryGetNext(out Int16 current)
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

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        /// <summary>Generates an infinite sequence that begins with the specified start value and advances by the step value.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="step">The difference between each element and the next one.</param>
        /// <returns>A sequence that contains the infinite sequence of values.</returns>
        public static ValueEnumerable<FromUInt32InfiniteSequence, UInt32> InfiniteSequence(UInt32 start, UInt32 step)
        {
            return new(new(start, step));
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromUInt32InfiniteSequence(UInt32 start, UInt32 step) : IValueEnumerator<UInt32>
    {
        bool calledGetNext;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<UInt32> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<UInt32> destination, Index offset)
        {
            return false;
        }

        public bool TryGetNext(out UInt32 current)
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

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        /// <summary>Generates an infinite sequence that begins with the specified start value and advances by the step value.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="step">The difference between each element and the next one.</param>
        /// <returns>A sequence that contains the infinite sequence of values.</returns>
        public static ValueEnumerable<FromInt32InfiniteSequence, Int32> InfiniteSequence(Int32 start, Int32 step)
        {
            return new(new(start, step));
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromInt32InfiniteSequence(Int32 start, Int32 step) : IValueEnumerator<Int32>
    {
        bool calledGetNext;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<Int32> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<Int32> destination, Index offset)
        {
            return false;
        }

        public bool TryGetNext(out Int32 current)
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

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        /// <summary>Generates an infinite sequence that begins with the specified start value and advances by the step value.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="step">The difference between each element and the next one.</param>
        /// <returns>A sequence that contains the infinite sequence of values.</returns>
        public static ValueEnumerable<FromUInt64InfiniteSequence, UInt64> InfiniteSequence(UInt64 start, UInt64 step)
        {
            return new(new(start, step));
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromUInt64InfiniteSequence(UInt64 start, UInt64 step) : IValueEnumerator<UInt64>
    {
        bool calledGetNext;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<UInt64> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<UInt64> destination, Index offset)
        {
            return false;
        }

        public bool TryGetNext(out UInt64 current)
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

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        /// <summary>Generates an infinite sequence that begins with the specified start value and advances by the step value.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="step">The difference between each element and the next one.</param>
        /// <returns>A sequence that contains the infinite sequence of values.</returns>
        public static ValueEnumerable<FromInt64InfiniteSequence, Int64> InfiniteSequence(Int64 start, Int64 step)
        {
            return new(new(start, step));
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromInt64InfiniteSequence(Int64 start, Int64 step) : IValueEnumerator<Int64>
    {
        bool calledGetNext;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<Int64> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<Int64> destination, Index offset)
        {
            return false;
        }

        public bool TryGetNext(out Int64 current)
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

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        /// <summary>Generates an infinite sequence that begins with the specified start value and advances by the step value.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="step">The difference between each element and the next one.</param>
        /// <returns>A sequence that contains the infinite sequence of values.</returns>
        public static ValueEnumerable<FromCharInfiniteSequence, Char> InfiniteSequence(Char start, Char step)
        {
            return new(new(start, step));
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromCharInfiniteSequence(Char start, Char step) : IValueEnumerator<Char>
    {
        bool calledGetNext;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<Char> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<Char> destination, Index offset)
        {
            return false;
        }

        public bool TryGetNext(out Char current)
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

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        /// <summary>Generates an infinite sequence that begins with the specified start value and advances by the step value.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="step">The difference between each element and the next one.</param>
        /// <returns>A sequence that contains the infinite sequence of values.</returns>
        public static ValueEnumerable<FromSingleInfiniteSequence, Single> InfiniteSequence(Single start, Single step)
        {
            return new(new(start, step));
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromSingleInfiniteSequence(Single start, Single step) : IValueEnumerator<Single>
    {
        bool calledGetNext;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<Single> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<Single> destination, Index offset)
        {
            return false;
        }

        public bool TryGetNext(out Single current)
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

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        /// <summary>Generates an infinite sequence that begins with the specified start value and advances by the step value.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="step">The difference between each element and the next one.</param>
        /// <returns>A sequence that contains the infinite sequence of values.</returns>
        public static ValueEnumerable<FromDoubleInfiniteSequence, Double> InfiniteSequence(Double start, Double step)
        {
            return new(new(start, step));
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromDoubleInfiniteSequence(Double start, Double step) : IValueEnumerator<Double>
    {
        bool calledGetNext;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<Double> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<Double> destination, Index offset)
        {
            return false;
        }

        public bool TryGetNext(out Double current)
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

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        /// <summary>Generates an infinite sequence that begins with the specified start value and advances by the step value.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="step">The difference between each element and the next one.</param>
        /// <returns>A sequence that contains the infinite sequence of values.</returns>
        public static ValueEnumerable<FromDecimalInfiniteSequence, Decimal> InfiniteSequence(Decimal start, Decimal step)
        {
            return new(new(start, step));
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromDecimalInfiniteSequence(Decimal start, Decimal step) : IValueEnumerator<Decimal>
    {
        bool calledGetNext;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<Decimal> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<Decimal> destination, Index offset)
        {
            return false;
        }

        public bool TryGetNext(out Decimal current)
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
