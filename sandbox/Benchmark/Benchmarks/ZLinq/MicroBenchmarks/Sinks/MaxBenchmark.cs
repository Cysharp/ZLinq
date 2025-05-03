using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Max)]
public partial class MaxBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Max()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .Max();
    }
}
