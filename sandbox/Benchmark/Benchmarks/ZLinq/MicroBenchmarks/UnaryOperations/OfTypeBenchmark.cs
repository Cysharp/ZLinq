using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.OfType)]
public partial class OfTypeBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void OfType()
    {
        source.Default
              .AsValueEnumerable()
              .OfType<T>()
              .Consume(consumer);
    }
}
