using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.OrderBy)]
public partial class OrderByBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void OrderBy()
    {
        source.Default
              .AsValueEnumerable()
              .OrderBy(x => x)
              .Consume(consumer);
    }
}
