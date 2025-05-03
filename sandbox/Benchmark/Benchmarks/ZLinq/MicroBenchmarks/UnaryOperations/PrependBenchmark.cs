using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Prepend)]
public partial class PrependBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Prepend()
    {
        source.Default
              .AsValueEnumerable()
              .Prepend(midElement!)
              .Consume(consumer);
    }
}
