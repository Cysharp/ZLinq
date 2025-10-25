#if NET8_0_OR_GREATER

using System;
using System.Numerics;

namespace ZLinq.Simd;

partial struct Vectorizable<T>
{
    public ZipVectorizable<T, TResult> Zip<TResult>(ReadOnlySpan<T> second, Func<Vector<T>, Vector<T>, Vector<TResult>> vectorSelector, Func<T, T, TResult> selector)
    {
        return new(source, second, vectorSelector, selector);
    }

    public Zip3Vectorizable<T, TResult> Zip<TResult>(ReadOnlySpan<T> second, ReadOnlySpan<T> third, Func<Vector<T>, Vector<T>, Vector<T>, Vector<TResult>> vectorSelector, Func<T, T, T, TResult> selector)
    {
        return new(source, second, third, vectorSelector, selector);
    }
}

public readonly ref struct ZipVectorizable<T, TResult>(ReadOnlySpan<T> first, ReadOnlySpan<T> second, Func<Vector<T>, Vector<T>, Vector<TResult>> vectorSelector, Func<T, T, TResult> selector)
    where T : struct, INumber<T>
{
    readonly ReadOnlySpan<T> first = first;
    readonly ReadOnlySpan<T> second = second;

    public TResult[] ToArray()
    {
        var result = GC.AllocateUninitializedArray<TResult>(Math.Min(first.Length, second.Length));
        CopyTo(result);
        return result;
    }

    public void CopyTo(Span<TResult> destination)
    {
        var firstSrc = first;
        var secondSrc = second;

        var smallerLength = Math.Min(firstSrc.Length, secondSrc.Length);
        if ((uint)destination.Length < (uint)smallerLength)
        {
            Throws.Argument(nameof(destination), $"Destination span is too small. Required at least {smallerLength} elements but has {destination.Length} elements.");
        }

        if (Vector.IsHardwareAccelerated && Vector<T>.IsSupported && smallerLength >= Vector<T>.Count)
        {
            var firstVector = MemoryMarshal.Cast<T, Vector<T>>(firstSrc);
            var secondVector = MemoryMarshal.Cast<T, Vector<T>>(secondSrc);

            var smallerVectorLength = Math.Min(firstVector.Length, secondVector.Length);
            for (int i = 0; (uint)i < (uint)smallerVectorLength; i++)
            {
                var v = vectorSelector(firstVector[i], secondVector[i]);
                v.CopyTo(destination);
                destination = destination.Slice(Vector<T>.Count);
            }

            firstSrc = firstSrc.Slice(smallerVectorLength * Vector<T>.Count);
            secondSrc = secondSrc.Slice(smallerVectorLength * Vector<T>.Count);
        }

        var min = Math.Min(firstSrc.Length, secondSrc.Length);
        for (int i = 0; (uint)i < (uint)min; i++)
        {
            destination[i] = selector(firstSrc[i], secondSrc[i]);
        }
    }
}

public readonly ref struct Zip3Vectorizable<T, TResult>(ReadOnlySpan<T> first, ReadOnlySpan<T> second, ReadOnlySpan<T> third, Func<Vector<T>, Vector<T>, Vector<T>, Vector<TResult>> vectorSelector, Func<T, T, T, TResult> selector)
    where T : struct, INumber<T>
{
    readonly ReadOnlySpan<T> first = first;
    readonly ReadOnlySpan<T> second = second;
    readonly ReadOnlySpan<T> third = third;

    public TResult[] ToArray()
    {
        var result = GC.AllocateUninitializedArray<TResult>(Math.Min(Math.Min(first.Length, second.Length), third.Length));
        CopyTo(result);
        return result;
    }

    public void CopyTo(Span<TResult> destination)
    {
        var firstSrc = first;
        var secondSrc = second;
        var thirdSrc = third;

        var smallerLength = Math.Min(Math.Min(firstSrc.Length, secondSrc.Length), third.Length);
        if ((uint)destination.Length < (uint)smallerLength)
        {
            Throws.Argument(nameof(destination), $"Destination span is too small. Required at least {smallerLength} elements but has {destination.Length} elements.");
        }

        if (Vector.IsHardwareAccelerated && Vector<T>.IsSupported && smallerLength >= Vector<T>.Count)
        {
            var firstVector = MemoryMarshal.Cast<T, Vector<T>>(firstSrc);
            var secondVector = MemoryMarshal.Cast<T, Vector<T>>(secondSrc);
            var thirdVector = MemoryMarshal.Cast<T, Vector<T>>(thirdSrc);

            var smallerVectorLength = Math.Min(Math.Min(firstVector.Length, secondVector.Length), thirdVector.Length);
            for (int i = 0; (uint)i < (uint)smallerVectorLength; i++)
            {
                var v = vectorSelector(firstVector[i], secondVector[i], thirdVector[i]);
                v.CopyTo(destination);
                destination = destination.Slice(Vector<T>.Count);
            }

            firstSrc = firstSrc.Slice(smallerVectorLength * Vector<T>.Count);
            secondSrc = secondSrc.Slice(smallerVectorLength * Vector<T>.Count);
            thirdSrc = thirdSrc.Slice(smallerVectorLength * Vector<T>.Count);
        }

        var min = Math.Min(Math.Min(firstSrc.Length, secondSrc.Length), thirdSrc.Length);
        for (int i = 0; (uint)i < (uint)min; i++)
        {
            destination[i] = selector(firstSrc[i], secondSrc[i], thirdSrc[i]);
        }
    }
}

#endif
