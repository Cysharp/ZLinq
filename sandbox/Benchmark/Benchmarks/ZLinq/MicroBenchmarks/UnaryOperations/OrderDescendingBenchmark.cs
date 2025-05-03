using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.OrderDescending)]
public partial class OrderDescendingBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void OrderDescending()
    {
        source.Default
              .AsValueEnumerable()
              .OrderDescending()
              .Consume(consumer);
    }
}
