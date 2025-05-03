using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Append)]
public partial class AppendBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Append()
    {
        source.Default
              .AsValueEnumerable()
              .Append(midElement!)
              .Consume(consumer);
    }
}
