using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.ThenBy)]
public partial class ThenByBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void ThenBy()
    {
        source.Default
              .AsValueEnumerable()
              .OrderDescending()
              .ThenBy(x => x)
              .Consume(consumer);
    }
}
