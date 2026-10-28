// This file is generated from FileGen.Command.cs
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

// support for all platforms
// byte/sbyte/ushort/char/short/uint/int/ulong/long (+SIMD optimize)
// float/double/decimal/DateTime/DateTimeOffset

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        /// <summary>Generates a sequence that begins with the specified start value and advances by the step value until reaching the inclusive end value.</summary>
        /// <param name="start">The value of the first element in the sequence.</param>
        /// <param name="endInclusive">The inclusive bound of the sequence. If step is positive, it is the upper bound; if step is negative, it is the lower bound.</param>
        /// <param name="step">The difference between each element and the next one.</param>
        /// <returns>A sequence that contains the sequence of values.</returns>
        public static ValueEnumerable<FromByteSequence, Byte> Sequence(Byte start, Byte endInclusive, Byte step)
        {


            if (step == 0) // (T.IsZero(step))
            {
                if (start != endInclusive)
                {
                    Throws.ArgumentOutOfRange(nameof(step));
                }

                // repeat one
                return new(new(start, endInclusive, step, isIncrement: true));
            }
            else if (step >= 0) // (T.IsPositive(step))
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
    public struct FromByteSequence(Byte currentValue, Byte endInclusive, Byte step, bool isIncrement) : IValueEnumerator<Byte>
    {
        bool calledGetNext;

        public bool TryGetNext(out Byte current)
        {
            if (!calledGetNext)
            {
                calledGetNext = true;
                current = currentValue;
                return true;
            }

            if (isIncrement)
            {
                var next = unchecked((Byte)(currentValue + step));

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
                var next = unchecked((Byte)(currentValue + step));

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
            if (CanOptimize())
            {
                count = (int)(endInclusive - currentValue + 1); // currentValue is start if not strated yet.
                return true;
            }

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
            if (TryGetNonEnumeratedCount(out var count))
            {
                if (EnumeratorHelper.TryGetSliceRange(count, offset, destination.Length, out var fillStart, out var fillCount))
                {
                    FillIncremental(destination.Slice(0, fillCount), (Byte)(currentValue + (Byte)fillStart));
                    return true;
                }
            }

            return false;
        }

        bool CanOptimize()
        {
            if (step == 1 && endInclusive - currentValue + 1 <= Byte.MaxValue)
            {
                return true;
            }
            return false;
        }

        static void FillIncremental(Span<Byte> span, Byte start)
        {
            ref var current = ref MemoryMarshal.GetReference(span);
            ref var end = ref Unsafe.Add(ref current, span.Length);

#if NET8_0_OR_GREATER
            if (Vector.IsHardwareAccelerated
                && Vector<Byte>.IsSupported
#if NET8_0
            && Vector<Byte>.Count <= 16
#endif
                && span.Length >= Vector<Byte>.Count)
            {
#if NET9_0_OR_GREATER
                var indices = Vector<Byte>.Indices;                   // <0, 1, 2, 3, 4, 5, 6, 7>...
#else
            var indices = new Vector<Byte>((ReadOnlySpan<Byte>)new Byte[] { (Byte)0, (Byte)1, (Byte)2, (Byte)3, (Byte)4, (Byte)5, (Byte)6, (Byte)7, (Byte)8, (Byte)9, (Byte)10, (Byte)11, (Byte)12, (Byte)13, (Byte)14, (Byte)15 });
#endif
                // for example, start = 5, Vector<Byte>.Count = 8
                var data = indices + new Vector<Byte>(start);         // <5, 6, 7, 8, 9, 10, 11, 12>...
                var increment = new Vector<Byte>((Byte)Vector<Byte>.Count);  // <8, 8, 8, 8, 8, 8, 8, 8>...

                ref var to = ref Unsafe.Subtract(ref end, Vector<Byte>.Count);
                do
                {
                    data.StoreUnsafe(ref current);                              // copy vectorized data to Span pointer
                    data += increment;                                          // <13, 14, 15, 16, 17, 18, 19, 20>...
                    current = ref Unsafe.Add(ref current, Vector<Byte>.Count);   // move pointer++

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
        public static ValueEnumerable<FromSByteSequence, SByte> Sequence(SByte start, SByte endInclusive, SByte step)
        {


            if (step == 0) // (T.IsZero(step))
            {
                if (start != endInclusive)
                {
                    Throws.ArgumentOutOfRange(nameof(step));
                }

                // repeat one
                return new(new(start, endInclusive, step, isIncrement: true));
            }
            else if (step >= 0) // (T.IsPositive(step))
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
    public struct FromSByteSequence(SByte currentValue, SByte endInclusive, SByte step, bool isIncrement) : IValueEnumerator<SByte>
    {
        bool calledGetNext;

        public bool TryGetNext(out SByte current)
        {
            if (!calledGetNext)
            {
                calledGetNext = true;
                current = currentValue;
                return true;
            }

            if (isIncrement)
            {
                var next = unchecked((SByte)(currentValue + step));

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
                var next = unchecked((SByte)(currentValue + step));

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
            if (CanOptimize())
            {
                count = (int)(endInclusive - currentValue + 1); // currentValue is start if not strated yet.
                return true;
            }

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
            if (TryGetNonEnumeratedCount(out var count))
            {
                if (EnumeratorHelper.TryGetSliceRange(count, offset, destination.Length, out var fillStart, out var fillCount))
                {
                    FillIncremental(destination.Slice(0, fillCount), (SByte)(currentValue + (SByte)fillStart));
                    return true;
                }
            }

            return false;
        }

        bool CanOptimize()
        {
            if (step == 1 && endInclusive - currentValue + 1 <= SByte.MaxValue)
            {
                return true;
            }
            return false;
        }

        static void FillIncremental(Span<SByte> span, SByte start)
        {
            ref var current = ref MemoryMarshal.GetReference(span);
            ref var end = ref Unsafe.Add(ref current, span.Length);

#if NET8_0_OR_GREATER
            if (Vector.IsHardwareAccelerated
                && Vector<SByte>.IsSupported
#if NET8_0
            && Vector<SByte>.Count <= 16
#endif
                && span.Length >= Vector<SByte>.Count)
            {
#if NET9_0_OR_GREATER
                var indices = Vector<SByte>.Indices;                   // <0, 1, 2, 3, 4, 5, 6, 7>...
#else
            var indices = new Vector<SByte>((ReadOnlySpan<SByte>)new SByte[] { (SByte)0, (SByte)1, (SByte)2, (SByte)3, (SByte)4, (SByte)5, (SByte)6, (SByte)7, (SByte)8, (SByte)9, (SByte)10, (SByte)11, (SByte)12, (SByte)13, (SByte)14, (SByte)15 });
#endif
                // for example, start = 5, Vector<SByte>.Count = 8
                var data = indices + new Vector<SByte>(start);         // <5, 6, 7, 8, 9, 10, 11, 12>...
                var increment = new Vector<SByte>((SByte)Vector<SByte>.Count);  // <8, 8, 8, 8, 8, 8, 8, 8>...

                ref var to = ref Unsafe.Subtract(ref end, Vector<SByte>.Count);
                do
                {
                    data.StoreUnsafe(ref current);                              // copy vectorized data to Span pointer
                    data += increment;                                          // <13, 14, 15, 16, 17, 18, 19, 20>...
                    current = ref Unsafe.Add(ref current, Vector<SByte>.Count);   // move pointer++

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
        public static ValueEnumerable<FromUInt16Sequence, UInt16> Sequence(UInt16 start, UInt16 endInclusive, UInt16 step)
        {


            if (step == 0) // (T.IsZero(step))
            {
                if (start != endInclusive)
                {
                    Throws.ArgumentOutOfRange(nameof(step));
                }

                // repeat one
                return new(new(start, endInclusive, step, isIncrement: true));
            }
            else if (step >= 0) // (T.IsPositive(step))
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
    public struct FromUInt16Sequence(UInt16 currentValue, UInt16 endInclusive, UInt16 step, bool isIncrement) : IValueEnumerator<UInt16>
    {
        bool calledGetNext;

        public bool TryGetNext(out UInt16 current)
        {
            if (!calledGetNext)
            {
                calledGetNext = true;
                current = currentValue;
                return true;
            }

            if (isIncrement)
            {
                var next = unchecked((UInt16)(currentValue + step));

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
                var next = unchecked((UInt16)(currentValue + step));

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
            if (CanOptimize())
            {
                count = (int)(endInclusive - currentValue + 1); // currentValue is start if not strated yet.
                return true;
            }

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
            if (TryGetNonEnumeratedCount(out var count))
            {
                if (EnumeratorHelper.TryGetSliceRange(count, offset, destination.Length, out var fillStart, out var fillCount))
                {
                    FillIncremental(destination.Slice(0, fillCount), (UInt16)(currentValue + (UInt16)fillStart));
                    return true;
                }
            }

            return false;
        }

        bool CanOptimize()
        {
            if (step == 1 && endInclusive - currentValue + 1 <= UInt16.MaxValue)
            {
                return true;
            }
            return false;
        }

        static void FillIncremental(Span<UInt16> span, UInt16 start)
        {
            ref var current = ref MemoryMarshal.GetReference(span);
            ref var end = ref Unsafe.Add(ref current, span.Length);

#if NET8_0_OR_GREATER
            if (Vector.IsHardwareAccelerated
                && Vector<UInt16>.IsSupported
#if NET8_0
            && Vector<UInt16>.Count <= 16
#endif
                && span.Length >= Vector<UInt16>.Count)
            {
#if NET9_0_OR_GREATER
                var indices = Vector<UInt16>.Indices;                   // <0, 1, 2, 3, 4, 5, 6, 7>...
#else
            var indices = new Vector<UInt16>((ReadOnlySpan<UInt16>)new UInt16[] { (UInt16)0, (UInt16)1, (UInt16)2, (UInt16)3, (UInt16)4, (UInt16)5, (UInt16)6, (UInt16)7, (UInt16)8, (UInt16)9, (UInt16)10, (UInt16)11, (UInt16)12, (UInt16)13, (UInt16)14, (UInt16)15 });
#endif
                // for example, start = 5, Vector<UInt16>.Count = 8
                var data = indices + new Vector<UInt16>(start);         // <5, 6, 7, 8, 9, 10, 11, 12>...
                var increment = new Vector<UInt16>((UInt16)Vector<UInt16>.Count);  // <8, 8, 8, 8, 8, 8, 8, 8>...

                ref var to = ref Unsafe.Subtract(ref end, Vector<UInt16>.Count);
                do
                {
                    data.StoreUnsafe(ref current);                              // copy vectorized data to Span pointer
                    data += increment;                                          // <13, 14, 15, 16, 17, 18, 19, 20>...
                    current = ref Unsafe.Add(ref current, Vector<UInt16>.Count);   // move pointer++

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
        public static ValueEnumerable<FromInt16Sequence, Int16> Sequence(Int16 start, Int16 endInclusive, Int16 step)
        {


            if (step == 0) // (T.IsZero(step))
            {
                if (start != endInclusive)
                {
                    Throws.ArgumentOutOfRange(nameof(step));
                }

                // repeat one
                return new(new(start, endInclusive, step, isIncrement: true));
            }
            else if (step >= 0) // (T.IsPositive(step))
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
    public struct FromInt16Sequence(Int16 currentValue, Int16 endInclusive, Int16 step, bool isIncrement) : IValueEnumerator<Int16>
    {
        bool calledGetNext;

        public bool TryGetNext(out Int16 current)
        {
            if (!calledGetNext)
            {
                calledGetNext = true;
                current = currentValue;
                return true;
            }

            if (isIncrement)
            {
                var next = unchecked((Int16)(currentValue + step));

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
                var next = unchecked((Int16)(currentValue + step));

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
            if (CanOptimize())
            {
                count = (int)(endInclusive - currentValue + 1); // currentValue is start if not strated yet.
                return true;
            }

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
            if (TryGetNonEnumeratedCount(out var count))
            {
                if (EnumeratorHelper.TryGetSliceRange(count, offset, destination.Length, out var fillStart, out var fillCount))
                {
                    FillIncremental(destination.Slice(0, fillCount), (Int16)(currentValue + (Int16)fillStart));
                    return true;
                }
            }

            return false;
        }

        bool CanOptimize()
        {
            if (step == 1 && endInclusive - currentValue + 1 <= Int16.MaxValue)
            {
                return true;
            }
            return false;
        }

        static void FillIncremental(Span<Int16> span, Int16 start)
        {
            ref var current = ref MemoryMarshal.GetReference(span);
            ref var end = ref Unsafe.Add(ref current, span.Length);

#if NET8_0_OR_GREATER
            if (Vector.IsHardwareAccelerated
                && Vector<Int16>.IsSupported
#if NET8_0
            && Vector<Int16>.Count <= 16
#endif
                && span.Length >= Vector<Int16>.Count)
            {
#if NET9_0_OR_GREATER
                var indices = Vector<Int16>.Indices;                   // <0, 1, 2, 3, 4, 5, 6, 7>...
#else
            var indices = new Vector<Int16>((ReadOnlySpan<Int16>)new Int16[] { (Int16)0, (Int16)1, (Int16)2, (Int16)3, (Int16)4, (Int16)5, (Int16)6, (Int16)7, (Int16)8, (Int16)9, (Int16)10, (Int16)11, (Int16)12, (Int16)13, (Int16)14, (Int16)15 });
#endif
                // for example, start = 5, Vector<Int16>.Count = 8
                var data = indices + new Vector<Int16>(start);         // <5, 6, 7, 8, 9, 10, 11, 12>...
                var increment = new Vector<Int16>((Int16)Vector<Int16>.Count);  // <8, 8, 8, 8, 8, 8, 8, 8>...

                ref var to = ref Unsafe.Subtract(ref end, Vector<Int16>.Count);
                do
                {
                    data.StoreUnsafe(ref current);                              // copy vectorized data to Span pointer
                    data += increment;                                          // <13, 14, 15, 16, 17, 18, 19, 20>...
                    current = ref Unsafe.Add(ref current, Vector<Int16>.Count);   // move pointer++

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
        public static ValueEnumerable<FromUInt32Sequence, UInt32> Sequence(UInt32 start, UInt32 endInclusive, UInt32 step)
        {


            if (step == 0) // (T.IsZero(step))
            {
                if (start != endInclusive)
                {
                    Throws.ArgumentOutOfRange(nameof(step));
                }

                // repeat one
                return new(new(start, endInclusive, step, isIncrement: true));
            }
            else if (step >= 0) // (T.IsPositive(step))
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
    public struct FromUInt32Sequence(UInt32 currentValue, UInt32 endInclusive, UInt32 step, bool isIncrement) : IValueEnumerator<UInt32>
    {
        bool calledGetNext;

        public bool TryGetNext(out UInt32 current)
        {
            if (!calledGetNext)
            {
                calledGetNext = true;
                current = currentValue;
                return true;
            }

            if (isIncrement)
            {
                var next = unchecked((UInt32)(currentValue + step));

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
                var next = unchecked((UInt32)(currentValue + step));

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
            if (CanOptimize())
            {
                count = (int)(endInclusive - currentValue + 1); // currentValue is start if not strated yet.
                return true;
            }

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
            if (TryGetNonEnumeratedCount(out var count))
            {
                if (EnumeratorHelper.TryGetSliceRange(count, offset, destination.Length, out var fillStart, out var fillCount))
                {
                    FillIncremental(destination.Slice(0, fillCount), (UInt32)(currentValue + (UInt32)fillStart));
                    return true;
                }
            }

            return false;
        }

        bool CanOptimize()
        {
            if (step == 1 && endInclusive - currentValue + 1 <= UInt32.MaxValue)
            {
                return true;
            }
            return false;
        }

        static void FillIncremental(Span<UInt32> span, UInt32 start)
        {
            ref var current = ref MemoryMarshal.GetReference(span);
            ref var end = ref Unsafe.Add(ref current, span.Length);

#if NET8_0_OR_GREATER
            if (Vector.IsHardwareAccelerated
                && Vector<UInt32>.IsSupported
#if NET8_0
            && Vector<UInt32>.Count <= 16
#endif
                && span.Length >= Vector<UInt32>.Count)
            {
#if NET9_0_OR_GREATER
                var indices = Vector<UInt32>.Indices;                   // <0, 1, 2, 3, 4, 5, 6, 7>...
#else
            var indices = new Vector<UInt32>((ReadOnlySpan<UInt32>)new UInt32[] { (UInt32)0, (UInt32)1, (UInt32)2, (UInt32)3, (UInt32)4, (UInt32)5, (UInt32)6, (UInt32)7, (UInt32)8, (UInt32)9, (UInt32)10, (UInt32)11, (UInt32)12, (UInt32)13, (UInt32)14, (UInt32)15 });
#endif
                // for example, start = 5, Vector<UInt32>.Count = 8
                var data = indices + new Vector<UInt32>(start);         // <5, 6, 7, 8, 9, 10, 11, 12>...
                var increment = new Vector<UInt32>((UInt32)Vector<UInt32>.Count);  // <8, 8, 8, 8, 8, 8, 8, 8>...

                ref var to = ref Unsafe.Subtract(ref end, Vector<UInt32>.Count);
                do
                {
                    data.StoreUnsafe(ref current);                              // copy vectorized data to Span pointer
                    data += increment;                                          // <13, 14, 15, 16, 17, 18, 19, 20>...
                    current = ref Unsafe.Add(ref current, Vector<UInt32>.Count);   // move pointer++

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
        public static ValueEnumerable<FromInt32Sequence, Int32> Sequence(Int32 start, Int32 endInclusive, Int32 step)
        {


            if (step == 0) // (T.IsZero(step))
            {
                if (start != endInclusive)
                {
                    Throws.ArgumentOutOfRange(nameof(step));
                }

                // repeat one
                return new(new(start, endInclusive, step, isIncrement: true));
            }
            else if (step >= 0) // (T.IsPositive(step))
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
    public struct FromInt32Sequence(Int32 currentValue, Int32 endInclusive, Int32 step, bool isIncrement) : IValueEnumerator<Int32>
    {
        bool calledGetNext;

        public bool TryGetNext(out Int32 current)
        {
            if (!calledGetNext)
            {
                calledGetNext = true;
                current = currentValue;
                return true;
            }

            if (isIncrement)
            {
                var next = unchecked((Int32)(currentValue + step));

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
                var next = unchecked((Int32)(currentValue + step));

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
            if (CanOptimize())
            {
                count = (int)(endInclusive - currentValue + 1); // currentValue is start if not strated yet.
                return true;
            }

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
            if (TryGetNonEnumeratedCount(out var count))
            {
                if (EnumeratorHelper.TryGetSliceRange(count, offset, destination.Length, out var fillStart, out var fillCount))
                {
                    FillIncremental(destination.Slice(0, fillCount), (Int32)(currentValue + (Int32)fillStart));
                    return true;
                }
            }

            return false;
        }

        bool CanOptimize()
        {
            if (step == 1 && endInclusive - currentValue + 1 <= Int32.MaxValue)
            {
                return true;
            }
            return false;
        }

        static void FillIncremental(Span<Int32> span, Int32 start)
        {
            ref var current = ref MemoryMarshal.GetReference(span);
            ref var end = ref Unsafe.Add(ref current, span.Length);

#if NET8_0_OR_GREATER
            if (Vector.IsHardwareAccelerated
                && Vector<Int32>.IsSupported
#if NET8_0
            && Vector<Int32>.Count <= 16
#endif
                && span.Length >= Vector<Int32>.Count)
            {
#if NET9_0_OR_GREATER
                var indices = Vector<Int32>.Indices;                   // <0, 1, 2, 3, 4, 5, 6, 7>...
#else
            var indices = new Vector<Int32>((ReadOnlySpan<Int32>)new Int32[] { (Int32)0, (Int32)1, (Int32)2, (Int32)3, (Int32)4, (Int32)5, (Int32)6, (Int32)7, (Int32)8, (Int32)9, (Int32)10, (Int32)11, (Int32)12, (Int32)13, (Int32)14, (Int32)15 });
#endif
                // for example, start = 5, Vector<Int32>.Count = 8
                var data = indices + new Vector<Int32>(start);         // <5, 6, 7, 8, 9, 10, 11, 12>...
                var increment = new Vector<Int32>((Int32)Vector<Int32>.Count);  // <8, 8, 8, 8, 8, 8, 8, 8>...

                ref var to = ref Unsafe.Subtract(ref end, Vector<Int32>.Count);
                do
                {
                    data.StoreUnsafe(ref current);                              // copy vectorized data to Span pointer
                    data += increment;                                          // <13, 14, 15, 16, 17, 18, 19, 20>...
                    current = ref Unsafe.Add(ref current, Vector<Int32>.Count);   // move pointer++

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
        public static ValueEnumerable<FromUInt64Sequence, UInt64> Sequence(UInt64 start, UInt64 endInclusive, UInt64 step)
        {


            if (step == 0) // (T.IsZero(step))
            {
                if (start != endInclusive)
                {
                    Throws.ArgumentOutOfRange(nameof(step));
                }

                // repeat one
                return new(new(start, endInclusive, step, isIncrement: true));
            }
            else if (step >= 0) // (T.IsPositive(step))
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
    public struct FromUInt64Sequence(UInt64 currentValue, UInt64 endInclusive, UInt64 step, bool isIncrement) : IValueEnumerator<UInt64>
    {
        bool calledGetNext;

        public bool TryGetNext(out UInt64 current)
        {
            if (!calledGetNext)
            {
                calledGetNext = true;
                current = currentValue;
                return true;
            }

            if (isIncrement)
            {
                var next = unchecked((UInt64)(currentValue + step));

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
                var next = unchecked((UInt64)(currentValue + step));

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
            if (CanOptimize())
            {
                count = (int)(endInclusive - currentValue + 1); // currentValue is start if not strated yet.
                return true;
            }

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
            if (TryGetNonEnumeratedCount(out var count))
            {
                if (EnumeratorHelper.TryGetSliceRange(count, offset, destination.Length, out var fillStart, out var fillCount))
                {
                    FillIncremental(destination.Slice(0, fillCount), (UInt64)(currentValue + (UInt64)fillStart));
                    return true;
                }
            }

            return false;
        }

        bool CanOptimize()
        {
            if (step == 1 && endInclusive - currentValue + 1 <= UInt64.MaxValue)
            {
                return true;
            }
            return false;
        }

        static void FillIncremental(Span<UInt64> span, UInt64 start)
        {
            ref var current = ref MemoryMarshal.GetReference(span);
            ref var end = ref Unsafe.Add(ref current, span.Length);

#if NET8_0_OR_GREATER
            if (Vector.IsHardwareAccelerated
                && Vector<UInt64>.IsSupported
#if NET8_0
            && Vector<UInt64>.Count <= 16
#endif
                && span.Length >= Vector<UInt64>.Count)
            {
#if NET9_0_OR_GREATER
                var indices = Vector<UInt64>.Indices;                   // <0, 1, 2, 3, 4, 5, 6, 7>...
#else
            var indices = new Vector<UInt64>((ReadOnlySpan<UInt64>)new UInt64[] { (UInt64)0, (UInt64)1, (UInt64)2, (UInt64)3, (UInt64)4, (UInt64)5, (UInt64)6, (UInt64)7, (UInt64)8, (UInt64)9, (UInt64)10, (UInt64)11, (UInt64)12, (UInt64)13, (UInt64)14, (UInt64)15 });
#endif
                // for example, start = 5, Vector<UInt64>.Count = 8
                var data = indices + new Vector<UInt64>(start);         // <5, 6, 7, 8, 9, 10, 11, 12>...
                var increment = new Vector<UInt64>((UInt64)Vector<UInt64>.Count);  // <8, 8, 8, 8, 8, 8, 8, 8>...

                ref var to = ref Unsafe.Subtract(ref end, Vector<UInt64>.Count);
                do
                {
                    data.StoreUnsafe(ref current);                              // copy vectorized data to Span pointer
                    data += increment;                                          // <13, 14, 15, 16, 17, 18, 19, 20>...
                    current = ref Unsafe.Add(ref current, Vector<UInt64>.Count);   // move pointer++

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
        public static ValueEnumerable<FromInt64Sequence, Int64> Sequence(Int64 start, Int64 endInclusive, Int64 step)
        {


            if (step == 0) // (T.IsZero(step))
            {
                if (start != endInclusive)
                {
                    Throws.ArgumentOutOfRange(nameof(step));
                }

                // repeat one
                return new(new(start, endInclusive, step, isIncrement: true));
            }
            else if (step >= 0) // (T.IsPositive(step))
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
    public struct FromInt64Sequence(Int64 currentValue, Int64 endInclusive, Int64 step, bool isIncrement) : IValueEnumerator<Int64>
    {
        bool calledGetNext;

        public bool TryGetNext(out Int64 current)
        {
            if (!calledGetNext)
            {
                calledGetNext = true;
                current = currentValue;
                return true;
            }

            if (isIncrement)
            {
                var next = unchecked((Int64)(currentValue + step));

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
                var next = unchecked((Int64)(currentValue + step));

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
            if (CanOptimize())
            {
                count = (int)(endInclusive - currentValue + 1); // currentValue is start if not strated yet.
                return true;
            }

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
            if (TryGetNonEnumeratedCount(out var count))
            {
                if (EnumeratorHelper.TryGetSliceRange(count, offset, destination.Length, out var fillStart, out var fillCount))
                {
                    FillIncremental(destination.Slice(0, fillCount), (Int64)(currentValue + (Int64)fillStart));
                    return true;
                }
            }

            return false;
        }

        bool CanOptimize()
        {
            if (step == 1 && endInclusive - currentValue + 1 <= Int64.MaxValue)
            {
                return true;
            }
            return false;
        }

        static void FillIncremental(Span<Int64> span, Int64 start)
        {
            ref var current = ref MemoryMarshal.GetReference(span);
            ref var end = ref Unsafe.Add(ref current, span.Length);

#if NET8_0_OR_GREATER
            if (Vector.IsHardwareAccelerated
                && Vector<Int64>.IsSupported
#if NET8_0
            && Vector<Int64>.Count <= 16
#endif
                && span.Length >= Vector<Int64>.Count)
            {
#if NET9_0_OR_GREATER
                var indices = Vector<Int64>.Indices;                   // <0, 1, 2, 3, 4, 5, 6, 7>...
#else
            var indices = new Vector<Int64>((ReadOnlySpan<Int64>)new Int64[] { (Int64)0, (Int64)1, (Int64)2, (Int64)3, (Int64)4, (Int64)5, (Int64)6, (Int64)7, (Int64)8, (Int64)9, (Int64)10, (Int64)11, (Int64)12, (Int64)13, (Int64)14, (Int64)15 });
#endif
                // for example, start = 5, Vector<Int64>.Count = 8
                var data = indices + new Vector<Int64>(start);         // <5, 6, 7, 8, 9, 10, 11, 12>...
                var increment = new Vector<Int64>((Int64)Vector<Int64>.Count);  // <8, 8, 8, 8, 8, 8, 8, 8>...

                ref var to = ref Unsafe.Subtract(ref end, Vector<Int64>.Count);
                do
                {
                    data.StoreUnsafe(ref current);                              // copy vectorized data to Span pointer
                    data += increment;                                          // <13, 14, 15, 16, 17, 18, 19, 20>...
                    current = ref Unsafe.Add(ref current, Vector<Int64>.Count);   // move pointer++

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
        public static ValueEnumerable<FromCharSequence, Char> Sequence(Char start, Char endInclusive, Char step)
        {


            if (step == 0) // (T.IsZero(step))
            {
                if (start != endInclusive)
                {
                    Throws.ArgumentOutOfRange(nameof(step));
                }

                // repeat one
                return new(new(start, endInclusive, step, isIncrement: true));
            }
            else if (step >= 0) // (T.IsPositive(step))
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
    public struct FromCharSequence(Char currentValue, Char endInclusive, Char step, bool isIncrement) : IValueEnumerator<Char>
    {
        bool calledGetNext;

        public bool TryGetNext(out Char current)
        {
            if (!calledGetNext)
            {
                calledGetNext = true;
                current = currentValue;
                return true;
            }

            if (isIncrement)
            {
                var next = unchecked((Char)(currentValue + step));

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
                var next = unchecked((Char)(currentValue + step));

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
            if (CanOptimize())
            {
                count = (int)(endInclusive - currentValue + 1); // currentValue is start if not strated yet.
                return true;
            }

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
            if (TryGetNonEnumeratedCount(out var count))
            {
                if (EnumeratorHelper.TryGetSliceRange(count, offset, destination.Length, out var fillStart, out var fillCount))
                {
                    FillIncremental(destination.Slice(0, fillCount), (Char)(currentValue + (Char)fillStart));
                    return true;
                }
            }

            return false;
        }

        bool CanOptimize()
        {
            if (step == 1 && endInclusive - currentValue + 1 <= Char.MaxValue)
            {
                return true;
            }
            return false;
        }

        static void FillIncremental(Span<Char> span, Char start)
        {
            ref var current = ref MemoryMarshal.GetReference(span);
            ref var end = ref Unsafe.Add(ref current, span.Length);

#if NET8_0_OR_GREATER
            if (Vector.IsHardwareAccelerated
                && Vector<Char>.IsSupported
#if NET8_0
            && Vector<Char>.Count <= 16
#endif
                && span.Length >= Vector<Char>.Count)
            {
#if NET9_0_OR_GREATER
                var indices = Vector<Char>.Indices;                   // <0, 1, 2, 3, 4, 5, 6, 7>...
#else
            var indices = new Vector<Char>((ReadOnlySpan<Char>)new Char[] { (Char)0, (Char)1, (Char)2, (Char)3, (Char)4, (Char)5, (Char)6, (Char)7, (Char)8, (Char)9, (Char)10, (Char)11, (Char)12, (Char)13, (Char)14, (Char)15 });
#endif
                // for example, start = 5, Vector<Char>.Count = 8
                var data = indices + new Vector<Char>(start);         // <5, 6, 7, 8, 9, 10, 11, 12>...
                var increment = new Vector<Char>((Char)Vector<Char>.Count);  // <8, 8, 8, 8, 8, 8, 8, 8>...

                ref var to = ref Unsafe.Subtract(ref end, Vector<Char>.Count);
                do
                {
                    data.StoreUnsafe(ref current);                              // copy vectorized data to Span pointer
                    data += increment;                                          // <13, 14, 15, 16, 17, 18, 19, 20>...
                    current = ref Unsafe.Add(ref current, Vector<Char>.Count);   // move pointer++

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
        public static ValueEnumerable<FromSingleSequence, Single> Sequence(Single start, Single endInclusive, Single step)
        {
            if (Single.IsNaN(start)) Throws.ArgumentOutOfRange(nameof(start));
            if (Single.IsNaN(endInclusive)) Throws.ArgumentOutOfRange(nameof(endInclusive));
            if (Single.IsNaN(step)) Throws.ArgumentOutOfRange(nameof(step));

            if (step == 0) // (T.IsZero(step))
            {
                if (start != endInclusive)
                {
                    Throws.ArgumentOutOfRange(nameof(step));
                }

                // repeat one
                return new(new(start, endInclusive, step, isIncrement: true));
            }
            else if (step >= 0) // (T.IsPositive(step))
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
    public struct FromSingleSequence(Single currentValue, Single endInclusive, Single step, bool isIncrement) : IValueEnumerator<Single>
    {
        bool calledGetNext;

        public bool TryGetNext(out Single current)
        {
            if (!calledGetNext)
            {
                calledGetNext = true;
                current = currentValue;
                return true;
            }

            if (isIncrement)
            {
                var next = unchecked((Single)(currentValue + step));

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
                var next = unchecked((Single)(currentValue + step));

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

        public bool TryGetSpan(out ReadOnlySpan<Single> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<Single> destination, Index offset)
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
        public static ValueEnumerable<FromDoubleSequence, Double> Sequence(Double start, Double endInclusive, Double step)
        {
            if (Double.IsNaN(start)) Throws.ArgumentOutOfRange(nameof(start));
            if (Double.IsNaN(endInclusive)) Throws.ArgumentOutOfRange(nameof(endInclusive));
            if (Double.IsNaN(step)) Throws.ArgumentOutOfRange(nameof(step));

            if (step == 0) // (T.IsZero(step))
            {
                if (start != endInclusive)
                {
                    Throws.ArgumentOutOfRange(nameof(step));
                }

                // repeat one
                return new(new(start, endInclusive, step, isIncrement: true));
            }
            else if (step >= 0) // (T.IsPositive(step))
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
    public struct FromDoubleSequence(Double currentValue, Double endInclusive, Double step, bool isIncrement) : IValueEnumerator<Double>
    {
        bool calledGetNext;

        public bool TryGetNext(out Double current)
        {
            if (!calledGetNext)
            {
                calledGetNext = true;
                current = currentValue;
                return true;
            }

            if (isIncrement)
            {
                var next = unchecked((Double)(currentValue + step));

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
                var next = unchecked((Double)(currentValue + step));

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

        public bool TryGetSpan(out ReadOnlySpan<Double> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<Double> destination, Index offset)
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
        public static ValueEnumerable<FromDecimalSequence, Decimal> Sequence(Decimal start, Decimal endInclusive, Decimal step)
        {


            if (step == 0) // (T.IsZero(step))
            {
                if (start != endInclusive)
                {
                    Throws.ArgumentOutOfRange(nameof(step));
                }

                // repeat one
                return new(new(start, endInclusive, step, isIncrement: true));
            }
            else if (step >= 0) // (T.IsPositive(step))
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
    public struct FromDecimalSequence(Decimal currentValue, Decimal endInclusive, Decimal step, bool isIncrement) : IValueEnumerator<Decimal>
    {
        bool calledGetNext;

        public bool TryGetNext(out Decimal current)
        {
            if (!calledGetNext)
            {
                calledGetNext = true;
                current = currentValue;
                return true;
            }

            if (isIncrement)
            {
                var next = unchecked((Decimal)(currentValue + step));

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
                var next = unchecked((Decimal)(currentValue + step));

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

        public bool TryGetSpan(out ReadOnlySpan<Decimal> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<Decimal> destination, Index offset)
        {
            return false;
        }
    }
}
