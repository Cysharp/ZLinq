using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using ZLinq;

namespace Benchmark;

[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[GenericTypeArguments(typeof(string))]
public class SelectFromSourceTypes<T> where T : notnull
{
    readonly Consumer consumer = new();

    readonly T[] source = Enumerable.Range(1, 10_000_000).Select(x => default(T)!).ToArray();

    [Benchmark]
    [BenchmarkCategory(Categories.LINQ)]
    public void LinqSelect()
    {
        source.Select(x => x)
              .Consume(consumer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.LINQ)]
    public void LinqSelect_WithoutConsume()
    {
        foreach (var _ in source.Select(x => x))
        {
        }
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    public void ZLinqSelect()
    {
        source.AsValueEnumerable()
              .Select(x => x)
              .Consume(consumer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    public void ZLinqSelect_WithoutConsume()
    {
        var valueEnumerable = source.AsValueEnumerable().Select(x => x);
        using var e = valueEnumerable.Enumerator;
        while (e.TryGetNext(out _))
        {
        }
    }

    // ValueEnumerable.Consume performance should be same as following code.
    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    public void ZLinq_ConsumeDirect()
    {
        var valueEnumerable = source.AsValueEnumerable().Select(x => x);
        using var e = valueEnumerable.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(item);
    }
}
