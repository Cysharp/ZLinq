using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.ToHashSet)]
public partial class ToHashSetBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void ToHashSet()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .ToHashSet();
    }
}


















