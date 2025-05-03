using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.ElementAtOrDefault)]
public partial class ElementAtOrDefaultBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void ElementAtOrDefault()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .ElementAtOrDefault(source.Length / 2);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void ElementAtOrDefault_Last()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .ElementAtOrDefault(^1);
    }
}


















