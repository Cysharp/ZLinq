using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.ToArray)]
public partial class ToArrayBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void ToArray()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .ToArray();
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.Enumerable)]
    public void ToArray_FromEnumerable()
    {
        _ = source.EnumerableData
                  .AsValueEnumerable()
                  .ToArray();
    }
}


















