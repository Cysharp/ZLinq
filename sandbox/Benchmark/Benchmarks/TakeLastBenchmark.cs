using ZLinq;

namespace Benchmark;

[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class TakeLastBenchmark : EnumerableBenchmarkBase<int>
{
    [Benchmark]
    [BenchmarkCategory(Categories.LINQ)]
    [BenchmarkCategory(Categories.From.Array)]
    public void FromArray_Linq()
    {
        foreach (var _ in source.ArrayData.TakeLast(100)) ;
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    [BenchmarkCategory(Categories.From.Array)]
    public void FromArray_ZLinq()
    {
        foreach (var _ in source.ArrayData.AsValueEnumerable().TakeLast(100)) ;
    }

    [Benchmark]
    [BenchmarkCategory(Categories.LINQ)]
    [BenchmarkCategory(Categories.From.List)]
    public void FromList_Linq()
    {
        foreach (var _ in source.ListData.TakeLast(100)) ;
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    [BenchmarkCategory(Categories.From.List)]
    public void FromList_ZLinq()
    {
        foreach (var _ in source.ListData.AsValueEnumerable().TakeLast(100)) ;
    }

    [Benchmark]
    [BenchmarkCategory(Categories.LINQ)]
    [BenchmarkCategory(Categories.From.Enumerable)]
    public void FromEnumerable_Linq()
    {
        foreach (var _ in source.EnumerableData.TakeLast(100)) ;
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    [BenchmarkCategory(Categories.From.Enumerable)]
    public void FromEnumerable_ZLinq()
    {
        foreach (var _ in source.EnumerableData.AsValueEnumerable().TakeLast(100)) ;
    }
}
