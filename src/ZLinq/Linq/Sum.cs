using System.Numerics;

namespace ZLinq;

partial class ValueEnumerableExtensions
{
    // Enumerable.Sum is for int/long/float/double/decimal and nullable
    // ZLinq supports more types(using INumber<T> constraint)
    // And, added the SumUnchecked method.

    /// <summary>Computes the sum of the sequence of values that are obtained by invoking a transform function on each element of the input sequence.</summary>
    /// <param name="source">The source sequence.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <returns>The sum of the projected values.</returns>
    public static TResult Sum<TEnumerator, TSource, TResult>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TResult> selector)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        where TResult : struct
#if NET8_0_OR_GREATER
        , INumber<TResult>
#endif
    {
        ArgumentNullException.ThrowIfNull(selector);

#if NET8_0_OR_GREATER
        using (var enumerator = source.Enumerator)
        {
            var sum = TResult.Zero;
            while (enumerator.TryGetNext(out var item))
            {
                var value = selector(item);
                checked { sum += TResult.CreateChecked(value); }
            }
            return sum;
        }
#else
        // While performance improvements can be expected by inlining, within ZLinq's zero-allocation iteration, the value of doing so cannot be justified.
        return source.Select(selector).Sum();
#endif
    }

    /// <summary>Computes the sum of the sequence of values that are obtained by invoking a transform function on each element of the input sequence.</summary>
    /// <param name="source">The source sequence.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <returns>The sum of the projected values.</returns>
    public static Nullable<TResult> Sum<TEnumerator, TSource, TResult>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Nullable<TResult>> selector)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        where TResult : struct
#if NET8_0_OR_GREATER
        , INumber<TResult>
#endif
    {
        ArgumentNullException.ThrowIfNull(selector);

#if NET8_0_OR_GREATER
        using (var enumerator = source.Enumerator)
        {
            var sum = TResult.Zero;
            while (enumerator.TryGetNext(out var item))
            {
                var value = selector(item);
                if (value is not null)
                {
                    checked { sum += TResult.CreateChecked(value.GetValueOrDefault()); }
                }
            }
            return sum;
        }
#else
        // While performance improvements can be expected by inlining, within ZLinq's zero-allocation iteration, the value of doing so cannot be justified.
        return source.Select(selector).Sum();
#endif
    }

    /// <summary>Computes the sum of a sequence of numeric values.</summary>
    /// <param name="source">The source sequence.</param>
    /// <returns>The sum of the values in the sequence.</returns>
    public static Nullable<TSource> Sum<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, Nullable<TSource>> source)
        where TEnumerator : struct, IValueEnumerator<Nullable<TSource>>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        where TSource : struct
#if NET8_0_OR_GREATER
        , INumber<TSource>
#endif
    {
#if NET8_0_OR_GREATER
        using (var enumerator = source.Enumerator)
        {
            var sum = TSource.Zero;
            while (enumerator.TryGetNext(out var value))
            {
                if (value is not null)
                {
                    checked { sum += TSource.CreateChecked(value.GetValueOrDefault()); }
                }
            }
            return sum;
        }
#else
        return source.Where(static x => x.HasValue).Select(static x => x.GetValueOrDefault()).Sum();
#endif
    }

    /// <summary>Computes the sum of a sequence of numeric values.</summary>
    /// <param name="source">The source sequence.</param>
    /// <returns>The sum of the values in the sequence.</returns>
    public static TSource Sum<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        where TSource : struct
#if NET8_0_OR_GREATER
        , INumber<TSource>
#endif
    {
        if (typeof(TSource) == typeof(float))
        {
            using (var enumerator = source.Enumerator)
            {
                double sum = default; // calculate as double
                while (enumerator.TryGetNext(out var item))
                {
                    sum += (double)Unsafe.As<TSource, float>(ref item);
                }
                var result = (float)sum;
                return Unsafe.As<float, TSource>(ref result);
            }
        }
#if NET8_0_OR_GREATER
        else
        {
            using (var enumerator = source.Enumerator)
            {
                if (enumerator.TryGetSpan(out var span))
                {
                    return SumSpan(span);
                }

                var sum = TSource.Zero;
                while (enumerator.TryGetNext(out var item))
                {
                    checked { sum += TSource.CreateChecked(item); }
                }
                return sum;
            }
        }
#else
        #region generate from FileGen.Commands.Sum
        else if (typeof(TSource) == typeof(byte))
        {
            using (var enumerator = source.Enumerator)
            {
                byte sum = default;
                while (enumerator.TryGetNext(out var item))
                {
                    checked { sum += Unsafe.As<TSource, byte>(ref item); }
                }
                return Unsafe.As<byte, TSource>(ref sum);
            }
        }
        else if (typeof(TSource) == typeof(sbyte))
        {
            using (var enumerator = source.Enumerator)
            {
                sbyte sum = default;
                while (enumerator.TryGetNext(out var item))
                {
                    checked { sum += Unsafe.As<TSource, sbyte>(ref item); }
                }
                return Unsafe.As<sbyte, TSource>(ref sum);
            }
        }
        else if (typeof(TSource) == typeof(short))
        {
            using (var enumerator = source.Enumerator)
            {
                short sum = default;
                while (enumerator.TryGetNext(out var item))
                {
                    checked { sum += Unsafe.As<TSource, short>(ref item); }
                }
                return Unsafe.As<short, TSource>(ref sum);
            }
        }
        else if (typeof(TSource) == typeof(ushort))
        {
            using (var enumerator = source.Enumerator)
            {
                ushort sum = default;
                while (enumerator.TryGetNext(out var item))
                {
                    checked { sum += Unsafe.As<TSource, ushort>(ref item); }
                }
                return Unsafe.As<ushort, TSource>(ref sum);
            }
        }
        else if (typeof(TSource) == typeof(int))
        {
            using (var enumerator = source.Enumerator)
            {
                int sum = default;
                while (enumerator.TryGetNext(out var item))
                {
                    checked { sum += Unsafe.As<TSource, int>(ref item); }
                }
                return Unsafe.As<int, TSource>(ref sum);
            }
        }
        else if (typeof(TSource) == typeof(uint))
        {
            using (var enumerator = source.Enumerator)
            {
                uint sum = default;
                while (enumerator.TryGetNext(out var item))
                {
                    checked { sum += Unsafe.As<TSource, uint>(ref item); }
                }
                return Unsafe.As<uint, TSource>(ref sum);
            }
        }
        else if (typeof(TSource) == typeof(long))
        {
            using (var enumerator = source.Enumerator)
            {
                long sum = default;
                while (enumerator.TryGetNext(out var item))
                {
                    checked { sum += Unsafe.As<TSource, long>(ref item); }
                }
                return Unsafe.As<long, TSource>(ref sum);
            }
        }
        else if (typeof(TSource) == typeof(ulong))
        {
            using (var enumerator = source.Enumerator)
            {
                ulong sum = default;
                while (enumerator.TryGetNext(out var item))
                {
                    checked { sum += Unsafe.As<TSource, ulong>(ref item); }
                }
                return Unsafe.As<ulong, TSource>(ref sum);
            }
        }
        else if (typeof(TSource) == typeof(double))
        {
            using (var enumerator = source.Enumerator)
            {
                double sum = default;
                while (enumerator.TryGetNext(out var item))
                {
                    checked { sum += Unsafe.As<TSource, double>(ref item); }
                }
                return Unsafe.As<double, TSource>(ref sum);
            }
        }
        else if (typeof(TSource) == typeof(decimal))
        {
            using (var enumerator = source.Enumerator)
            {
                decimal sum = default;
                while (enumerator.TryGetNext(out var item))
                {
                    checked { sum += Unsafe.As<TSource, decimal>(ref item); }
                }
                return Unsafe.As<decimal, TSource>(ref sum);
            }
        }
        else if (typeof(TSource) == typeof(nint))
        {
            using (var enumerator = source.Enumerator)
            {
                nint sum = default;
                while (enumerator.TryGetNext(out var item))
                {
                    checked { sum += Unsafe.As<TSource, nint>(ref item); }
                }
                return Unsafe.As<nint, TSource>(ref sum);
            }
        }
        else if (typeof(TSource) == typeof(nuint))
        {
            using (var enumerator = source.Enumerator)
            {
                nuint sum = default;
                while (enumerator.TryGetNext(out var item))
                {
                    checked { sum += Unsafe.As<TSource, nuint>(ref item); }
                }
                return Unsafe.As<nuint, TSource>(ref sum);
            }
        }
        #endregion
        else
        {
            Throws.NotSupportedType(typeof(TSource));
            return default!;
        }
#endif
    }

#if NET8_0_OR_GREATER

    /// <summary>Computes the sum of a sequence of numeric values without overflow checking.</summary>
    /// <param name="source">The source sequence.</param>
    /// <returns>The sum of the values in the sequence.</returns>
    /// <remarks>Unlike Sum, an arithmetic overflow is not detected, which allows faster SIMD computation.</remarks>
    public static TSource SumUnchecked<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        where TSource : struct, INumber<TSource>
    {
        using var enumerator = source.Enumerator;
        if (enumerator.TryGetSpan(out var span))
        {
            return SimdSumNumberUnchecked(span);
        }

        var sum = TSource.Zero;
        while (enumerator.TryGetNext(out var item))
        {
            unchecked { sum += item; }
        }
        return sum;
    }
#endif

#if NET8_0_OR_GREATER

    internal static TSource SumSpan<TSource>(ReadOnlySpan<TSource> span)
        where TSource : struct, INumber<TSource>
    {
        // SIMD Support: sbyte, short, int, long, byte, ushort, uint, ulong, double

        // Signed
        if (typeof(TSource) == typeof(int))
        {
            var sum = SimdSumSignedNumberChecked(UnsafeSpanBitCast<TSource, int>(span));
            return Unsafe.As<int, TSource>(ref sum);
        }
        else if (typeof(TSource) == typeof(long))
        {
            var sum = SimdSumSignedNumberChecked(UnsafeSpanBitCast<TSource, long>(span));
            return Unsafe.As<long, TSource>(ref sum);
        }
        else if (typeof(TSource) == typeof(sbyte))
        {
            var sum = SimdSumSignedNumberChecked(UnsafeSpanBitCast<TSource, sbyte>(span));
            return Unsafe.As<sbyte, TSource>(ref sum);
        }
        else if (typeof(TSource) == typeof(short))
        {
            var sum = SimdSumSignedNumberChecked(UnsafeSpanBitCast<TSource, short>(span));
            return Unsafe.As<short, TSource>(ref sum);
        }
        // Unsigned
        else if (typeof(TSource) == typeof(byte))
        {
            var sum = SimdSumUnsignedNumberChecked(UnsafeSpanBitCast<TSource, byte>(span));
            return Unsafe.As<byte, TSource>(ref sum);
        }
        else if (typeof(TSource) == typeof(ushort))
        {
            var sum = SimdSumUnsignedNumberChecked(UnsafeSpanBitCast<TSource, ushort>(span));
            return Unsafe.As<ushort, TSource>(ref sum);
        }
        else if (typeof(TSource) == typeof(uint))
        {
            var sum = SimdSumUnsignedNumberChecked(UnsafeSpanBitCast<TSource, uint>(span));
            return Unsafe.As<uint, TSource>(ref sum);
        }
        else if (typeof(TSource) == typeof(ulong))
        {
            var sum = SimdSumUnsignedNumberChecked(UnsafeSpanBitCast<TSource, ulong>(span));
            return Unsafe.As<ulong, TSource>(ref sum);
        }
        // double
        else if (typeof(TSource) == typeof(double))
        {
            // double uses unchecked operation
            return SimdSumNumberUnchecked(span);
        }
        else
        {
            var sum = TSource.Zero;
            foreach (var item in span)
            {
                checked { sum += TSource.CreateChecked(item); }
            }
            return sum;
        }
    }

    // TFrom == TTo
    internal static ReadOnlySpan<TTo> UnsafeSpanBitCast<TFrom, TTo>(ReadOnlySpan<TFrom> span)
    {
#if NET9_0_OR_GREATER
        return Unsafe.BitCast<ReadOnlySpan<TFrom>, ReadOnlySpan<TTo>>(span);
#else
        ref var from = ref MemoryMarshal.GetReference(span);
        ref var to = ref Unsafe.As<TFrom, TTo>(ref from);
        return MemoryMarshal.CreateReadOnlySpan(ref to, span.Length);
#endif
    }

    static T SimdSumSignedNumberChecked<T>(ReadOnlySpan<T> span)
       where T : struct, ISignedNumber<T>, IMinMaxValue<T>, IBinaryInteger<T>
    {
        ref var current = ref MemoryMarshal.GetReference(span);
        ref var end = ref Unsafe.Add(ref current, span.Length);

        var result = T.Zero;

        if (Vector.IsHardwareAccelerated && Vector<T>.IsSupported && span.Length >= Vector<T>.Count)
        {
            ref var to = ref Unsafe.Subtract(ref end, Vector<T>.Count);

            var vectorSum = Vector<T>.Zero;
            var overflowTest = new Vector<T>(T.MinValue);
            do
            {
                var data = Vector.LoadUnsafe(ref current);
                var sum = vectorSum + data;

                // Check for overflow for each value.
                // This technique uses the same checks as described in Hacker's Delight.
                // sum = a + b;
                // overflow = (sum ^ a) & (sum ^ b);
                var overflow = (sum ^ vectorSum) & (sum ^ data);
                if ((overflow & overflowTest) != Vector<T>.Zero)
                {
                    Throws.Overflow();
                }

                vectorSum = sum;
                current = ref Unsafe.Add(ref current, Vector<T>.Count);
            } while (!Unsafe.IsAddressGreaterThan(ref current, ref to)); // (current <= to) -> !(current > to) 

            // Perform the final addition using checked. 
            // Do not use Vector.Sum as it does not check for overflow.
            for (int i = 0; i < Vector<T>.Count; i++)
            {
                checked { result += vectorSum[i]; }
            }
        }

        // fill rest
        while (Unsafe.IsAddressLessThan(ref current, ref end))
        {
            checked { result += current; }
            current = ref Unsafe.Add(ref current, 1);
        }

        return result;
    }

    static T SimdSumUnsignedNumberChecked<T>(ReadOnlySpan<T> span)
        where T : struct, IUnsignedNumber<T>, IMinMaxValue<T>, IBinaryInteger<T>
    {
        ref var current = ref MemoryMarshal.GetReference(span);
        ref var end = ref Unsafe.Add(ref current, span.Length);

        var result = T.Zero;

        if (Vector.IsHardwareAccelerated && Vector<T>.IsSupported && span.Length >= Vector<T>.Count)
        {
            ref var to = ref Unsafe.Subtract(ref end, Vector<T>.Count);
            var vectorSum = Vector<T>.Zero;
            do
            {
                var data = Vector.LoadUnsafe(ref current);
                var sum = vectorSum + data;

                // overflow check: sum < vectorSum
                var overflow = Vector.LessThan(sum, vectorSum);
                if (Vector.GreaterThanAny(overflow, Vector<T>.Zero))
                {
                    Throws.Overflow();
                }

                vectorSum = sum;
                current = ref Unsafe.Add(ref current, Vector<T>.Count);
            } while (!Unsafe.IsAddressGreaterThan(ref current, ref to));

            // Perform the final addition using checked. 
            // Do not use Vector.Sum as it does not check for overflow.
            for (int i = 0; i < Vector<T>.Count; i++)
            {
                checked { result += vectorSum[i]; }
            }
        }

        // fill rest
        while (Unsafe.IsAddressLessThan(ref current, ref end))
        {
            checked { result += current; }
            current = ref Unsafe.Add(ref current, 1);
        }

        return result;
    }

    internal static T SimdSumNumberUnchecked<T>(ReadOnlySpan<T> span)
       where T : struct, INumberBase<T>
    {
        ref var current = ref MemoryMarshal.GetReference(span);
        ref var end = ref Unsafe.Add(ref current, span.Length);

        var result = T.Zero;

        if (Vector.IsHardwareAccelerated && Vector<T>.IsSupported && span.Length >= Vector<T>.Count)
        {
            ref var to = ref Unsafe.Subtract(ref end, Vector<T>.Count);
            var vectorSum = Vector<T>.Zero;
            do
            {
                var data = Vector.LoadUnsafe(ref current);
                vectorSum += data;
                current = ref Unsafe.Add(ref current, Vector<T>.Count);
            } while (!Unsafe.IsAddressGreaterThan(ref current, ref to)); // (current <= to) -> !(current > to) 

            result = Vector.Sum(vectorSum);
        }

        // fill rest
        while (Unsafe.IsAddressLessThan(ref current, ref end))
        {
            unchecked { result += current; } // use unchecked
            current = ref Unsafe.Add(ref current, 1);
        }

        return result;
    }

#endif
}
