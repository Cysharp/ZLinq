using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Skip)]
public partial class SkipBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Skip()
    {
        source.Default
              .AsValueEnumerable()
              .Skip(source.Length / 10)
              .Consume(consumer);
    }
}
