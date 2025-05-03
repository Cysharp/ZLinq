using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Count)]
public partial class CountBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Count()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .Count();
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Count_Predicate()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .Count(x => EqualityComparer<T>.Default.Equals(x, x));
    }
}


















