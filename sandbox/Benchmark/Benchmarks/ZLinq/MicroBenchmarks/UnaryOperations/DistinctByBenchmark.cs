using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.DistinctBy)]
public partial class DistinctByBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void DistinctBy()
    {
        source.Default
              .AsValueEnumerable()
              .DistinctBy(x => x)
              .Consume(consumer);
    }
}
