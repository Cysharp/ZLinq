using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using StructLinq;
using System.Numerics;
using System.Runtime.InteropServices;
using ZLinq;
using ZLinq.Simd;

namespace Benchmark;

[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class SimdAggregate
{
    int[] numbers;

    public SimdAggregate()
    {
        numbers = new int[100000];
        var rand = new Random();
        rand.NextBytes(MemoryMarshal.Cast<int, byte>(numbers).ToArray());
    }

    [Benchmark]
    public int ForMin()
    {
        var min = numbers[0];
        foreach (var value in numbers.AsSpan(1))
        {
            if (value < min) min = value;
        }
        return min;
    }

    [Benchmark]
    public int SimdAggregateMin()
    {
        return numbers.AsVectorizable().Aggregate((x, y) => Vector.Min(x, y), (x, y) => Math.Min(x, y));
    }
}
