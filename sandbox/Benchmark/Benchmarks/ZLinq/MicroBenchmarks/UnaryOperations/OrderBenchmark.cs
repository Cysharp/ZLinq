using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Order)]
public partial class OrderBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Order()
    {
        source.Default
              .AsValueEnumerable()
              .Order()
              .Consume(consumer);
    }
}
