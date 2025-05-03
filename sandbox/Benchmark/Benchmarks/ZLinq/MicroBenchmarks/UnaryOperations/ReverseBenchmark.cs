using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Reverse)]
public partial class ReverseBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Reverse()
    {
        source.Default
              .AsValueEnumerable()
              .Reverse()
              .Consume(consumer);
    }
}
