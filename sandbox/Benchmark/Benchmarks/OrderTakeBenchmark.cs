using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using System.Runtime.InteropServices;
using ZLinq;

namespace Benchmark;

[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class OrderTakeBenchmark
{
    [Params(/*10, 100, 1000, 10000,*/ 100000)]
    public int N;

    [Params(10, 100, 1000, 10000, 100000)]
    public int M;

    int[] array = default!;

    [GlobalSetup]
    public void Setup()
    {
        var rand = new Random(42);
        array = new int[N];
        rand.NextBytes(MemoryMarshal.Cast<int, byte>(array).ToArray());
    }

    [Benchmark]
    [BenchmarkCategory(Categories.LINQ)]
    public void OrderTakeLinq()
    {
        var src = array.Order().Take(M);
        using var e = src.GetEnumerator();
        while (e.MoveNext())
        {
            _ = e.Current;
        }
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    public void OrderTakeZLinq()
    {
        var src = array.AsValueEnumerable().Order().Take(M);

        using var e = src.Enumerator;
        while (e.TryGetNext(out var item))
        {
        }
    }
}
