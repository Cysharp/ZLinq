using Benchmark.ZLinq;
using ZLinq;

namespace Benchmark;

[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class TakeRangeBench : EnumerableBenchmarkBase<int>
{
    [Benchmark]
    [BenchmarkCategory(Categories.LINQ)]
    [BenchmarkCategory(Categories.From.Array)]
    public void FromArray_Linq()
    {
        foreach (var _ in source.ArrayData.Take(range: 100..^100)) ;
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    [BenchmarkCategory(Categories.From.Array)]
    public void FromArray_ZLinq()
    {
        foreach (var _ in source.ArrayData.AsValueEnumerable().Take(range: 100..^100)) ;
    }

    [Benchmark]
    [BenchmarkCategory(Categories.LINQ)]
    [BenchmarkCategory(Categories.From.Enumerable)]
    public void FromEnumerable_WithTake_Linq()
    {
        foreach (var _ in source.EnumerableData.Take(range: 100..^100).Take(1000)) ;
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    [BenchmarkCategory(Categories.From.Enumerable)]
    public void FromEnumerable_WithTake_ZLinq()
    {
        foreach (var _ in source.EnumerableData.AsValueEnumerable().Take(range: 100..^100).Take(1000)) ;
    }

    [Benchmark]
    [BenchmarkCategory(Categories.LINQ)]
    [BenchmarkCategory(Categories.From.Enumerable)]
    public void FromEnumerable_Linq()
    {
        foreach (var _ in source.EnumerableData.Take(range: 100..^100)) ;
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    [BenchmarkCategory(Categories.From.Enumerable)]
    public void FromEnumerable_ZLinq()
    {
        foreach (var _ in source.EnumerableData.AsValueEnumerable().Take(range: 100..^100)) ;
    }

    [Benchmark]
    [BenchmarkCategory(Categories.LINQ)]
    [BenchmarkCategory(Categories.From.Enumerable)]
    public void FromEnumerable_FromEnd_FromStart_Linq()
    {
        foreach (var _ in source.EnumerableData.Take(range: ^(N - 100)..(N - 100))) ;
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    [BenchmarkCategory(Categories.From.Enumerable)]
    public void FromEnumerable_FromEnd_FromStart_ZLinq()
    {
        foreach (var _ in source.EnumerableData.AsValueEnumerable().Take(range: ^(N - 100)..(N - 100))) ;
    }
}
