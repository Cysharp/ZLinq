using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.SkipLast)]
public partial class SkipLastBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void SkipLast()
    {
        source.Default
              .AsValueEnumerable()
              .SkipLast(source.Length / 10)
              .Consume(consumer);
    }
}
