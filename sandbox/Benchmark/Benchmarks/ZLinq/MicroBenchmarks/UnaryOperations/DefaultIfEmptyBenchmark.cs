using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.DefaultIfEmpty)]
public partial class DefaultIfEmptyBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void DefaultIfEmpty()
    {
        source.Default
              .AsValueEnumerable()
              .DefaultIfEmpty()
              .Consume(consumer);
    }
}
