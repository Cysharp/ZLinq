using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Contains)]
public partial class ContainsBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Contains()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .Contains(midElement!);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Contains_Last()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .Contains(lastElement!);
    }
}


















