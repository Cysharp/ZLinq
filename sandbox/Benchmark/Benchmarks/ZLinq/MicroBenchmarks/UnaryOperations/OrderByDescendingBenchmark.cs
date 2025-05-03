using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.OrderByDescending)]
public partial class OrderByDescendingBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void OrderByDescending()
    {
        source.Default
              .AsValueEnumerable()
              .OrderByDescending(x => x)
              .Consume(consumer);
    }
}
