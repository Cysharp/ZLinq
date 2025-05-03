using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Min)]
public partial class MinBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Min()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .Min();
    }
}
