#if NET8_0_OR_GREATER

using System;
using System.Numerics;

namespace ZLinq.Simd;

partial struct Vectorizable<T>
{
    public SelectVectorizable<T, TResult> Select<TResult>(Func<Vector<T>, Vector<TResult>> vectorSelector, Func<T, TResult> selector)
    {
        return new(source, vectorSelector, selector);
    }
}

public readonly ref struct SelectVectorizable<T, TResult>(ReadOnlySpan<T> source, Func<Vector<T>, Vector<TResult>> vectorSelector, Func<T, TResult> selector)
    where T : struct, INumber<T>
{
    readonly ReadOnlySpan<T> source = source;

    public TResult[] ToArray()
    {
        var result = GC.AllocateUninitializedArray<TResult>(source.Length);
        CopyTo(result);
        return result;
    }

    public void CopyTo(Span<TResult> destination)
    {
        if ((uint)destination.Length < (uint)source.Length)
        {
            Throws.Argument(nameof(destination), "Destination span is too small to contain the result of the transformation.");
        }

        var src = source;

        if (Vector.IsHardwareAccelerated && Vector<T>.IsSupported && src.Length >= Vector<T>.Count)
        {
            var vectors = MemoryMarshal.Cast<T, Vector<T>>(src);
            foreach (var item in vectors)
            {
                vectorSelector(item).CopyTo(destination);
                destination = destination.Slice(Vector<T>.Count);
            }

            src = src.Slice(vectors.Length * Vector<T>.Count);
        }

        for (int i = 0; (uint)i < (uint)src.Length; i++)
        {
            destination[i] = selector(src[i]);
        }
    }
}

#endif
