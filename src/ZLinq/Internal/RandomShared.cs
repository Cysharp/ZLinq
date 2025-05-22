using System.Security.Cryptography;

namespace ZLinq.Internal;

internal static class RandomShared
{
    public static void Shuffle<T>(Span<T> span)
    {
#if NET8_0_OR_GREATER
        Random.Shared.Shuffle(span);
#else
        Shared.Value.Shuffle(span);
#endif
    }

    public static void PartialShuffle<T>(Span<T> span, int count)
    {
#if NET8_0_OR_GREATER
        Random.Shared.PartialShuffle(span, count);
#else
        Shared.Value.PartialShuffle(span, count);
#endif
    }

#if !NET8_0_OR_GREATER

    private static ThreadLocal<Random> Shared = new ThreadLocal<Random>(() =>
    {
#if NETSTANDARD2_0
        using (var rng = new RNGCryptoServiceProvider())
        {
            var buffer = new byte[sizeof(int)];
            rng.GetBytes(buffer);
            var seed = BitConverter.ToInt32(buffer, 0);
            return new Random(seed);
        }
#else
        var seed = RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
        return new Random(seed);
#endif
    });

    static void Shuffle<T>(this Random random, Span<T> values)
    {
        int n = values.Length;

        for (int i = 0; i < n - 1; i++)
        {
            int j = random.Next(i, n);

            if (j != i)
            {
                T temp = values[i];
                values[i] = values[j];
                values[j] = temp;
            }
        }
    }

#endif

    static void PartialShuffle<T>(this Random random, Span<T> values, int count)
    {
        if (count <= 0) return;

        int n = Math.Min(values.Length, count);

        if (n == values.Length)
        {
            n--;  // exclude last item, otherwise includes n
        }

        for (int i = 0; i < n; i++) // < n
        {
            int j = random.Next(i, values.Length); // shuffle based length

            if (j != i)
            {
                T temp = values[i];
                values[i] = values[j];
                values[j] = temp;
            }
        }
    }
}
