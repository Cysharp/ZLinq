using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Single)]
public partial class SingleBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Single()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .Distinct()
                  .Single(x => EqualityComparer<T>.Default.Equals(x, midElement));
    }
}


















