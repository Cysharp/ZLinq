using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.SingleOrDefault)]
public partial class SingleOrDefaultBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void SingleOrDefault()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .Distinct()
                  .SingleOrDefault(x => EqualityComparer<T>.Default.Equals(x, midElement));
    }
}


















