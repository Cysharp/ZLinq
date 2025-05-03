using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Distinct)]
public partial class DistinctBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Distinct()
    {
        source.Default
              .AsValueEnumerable()
              .Distinct()
              .Consume(consumer);
    }
}
