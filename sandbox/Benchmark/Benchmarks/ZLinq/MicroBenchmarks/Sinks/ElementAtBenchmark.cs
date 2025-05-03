using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.ElementAt)]
public partial class ElementAtBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void ElementAt()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .ElementAt(source.Length / 2);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void ElementAt_Last()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .ElementAt(^1);
    }
}


















