using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.ThenByDescending)]
public partial class ThenByDescendingBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void ThenByDescending()
    {
        source.Default
              .AsValueEnumerable()
              .Order()
              .ThenByDescending(x => x)
              .Consume(consumer);
    }
}
