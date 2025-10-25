#if NET8_0_OR_GREATER
using System;
using System.Numerics;
using static ZLinq.ValueEnumerableExtensions; // use some SIMD methods from ValueEnumerableExtensions

namespace ZLinq.Simd;

/*
 * vectorized loop template.
        if (Vector.IsHardwareAccelerated && Vector<T>.IsSupported && source.Length >= Vector<T>.Count)
        {
            var vectors = MemoryMarshal.Cast<T, Vector<T>>(source);
            foreach (var item in vectors)
            {
            }

            source = source.Slice(vectors.Length * Vector<T>.Count);
        }

        foreach (T item in source)
        {
        }
*/

public static class VectorizableExtensions
{
    public static Vectorizable<T> AsVectorizable<T>(this T[] source) where T : struct, INumber<T> => new(source);
    public static Vectorizable<T> AsVectorizable<T>(this Span<T> source) where T : struct, INumber<T> => new(source);
    public static Vectorizable<T> AsVectorizable<T>(this ReadOnlySpan<T> source) where T : struct, INumber<T> => new(source);

    public static void VectorizedFillRange(this int[] source, int start)
    {
        FromRange.FillIncremental(source.AsSpan(), start);
    }

    public static void VectorizedFillRange(this Span<int> source, int start)
    {
        FromRange.FillIncremental(source, start);
    }

    public static void VectorizedUpdate<T>(this T[] source, Func<Vector<T>, Vector<T>> vectorFunc, Func<T, T> func)
        where T : struct, INumber<T> => Update(source, vectorFunc, func);

    public static void VectorizedUpdate<T>(this Span<T> source, Func<Vector<T>, Vector<T>> vectorFunc, Func<T, T> func)
        where T : struct, INumber<T> => Update(source, vectorFunc, func);

    static void Update<T>(Span<T> source, Func<Vector<T>, Vector<T>> vectorFunc, Func<T, T> func)
        where T : struct, INumber<T>
    {
        if (Vector.IsHardwareAccelerated && Vector<T>.IsSupported && source.Length >= Vector<T>.Count)
        {
            var vectors = MemoryMarshal.Cast<T, Vector<T>>(source);
            for (int i = 0; (uint)i < (uint)vectors.Length; i++)
            {
                vectors[i] = vectorFunc(vectors[i]);
            }

            source = source.Slice(vectors.Length * Vector<T>.Count);
        }

        for (int i = 0; (uint)i < (uint)source.Length; i++)
        {
            source[i] = func(source[i]);
        }
    }
}

public readonly ref partial struct Vectorizable<T>(ReadOnlySpan<T> source)
    where T : struct, INumber<T>
{
    readonly ReadOnlySpan<T> source = source;

    // LINQ Methods

    public T Sum()
    {
        return SumSpan(source);
    }

    public T SumUnchecked()
    {
        return SimdSumNumberUnchecked(source);
    }

    public double Average()
    {
        if (typeof(T) == typeof(int))
        {
            return AverageIntSimd(UnsafeSpanBitCast<T, int>(source));
        }
        else
        {
            var sum = SumSpan(source);
            return double.CreateChecked(sum) / (double)source.Length;
        }
    }

    public T Max()
    {
        return MaxSpan(source, Comparer<T>.Default);
    }

    public T Min()
    {
        return MinSpan(source, Comparer<T>.Default);
    }

    public bool Contains(T value)
    {
#if NET10_0_OR_GREATER
        return source.Contains(value);
#else
        return InvokeSpanContains(source, value);
#endif
    }

    public bool SequenceEqual(ReadOnlySpan<T> second)
    {
        return source.SequenceEqual(second);
    }
}

#endif
