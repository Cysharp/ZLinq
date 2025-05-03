using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.ToList)]
public partial class ToListBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void ToList()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .ToList();
    }
}


















