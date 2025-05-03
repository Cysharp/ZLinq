using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.TakeLast)]
public partial class TakeLastBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void TakeLast()
    {
        source.Default
              .AsValueEnumerable()
              .TakeLast(source.Length / 10)
              .Consume(consumer);
    }
}
