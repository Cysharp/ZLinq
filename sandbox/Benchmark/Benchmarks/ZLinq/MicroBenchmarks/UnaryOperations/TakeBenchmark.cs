using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Take)]
public partial class TakeBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Take()
    {
        source.Default
              .AsValueEnumerable()
              .Take(source.Length / 10)
              .Consume(consumer);
    }
}
