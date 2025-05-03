using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.ToDictionary)]
public partial class ToDictionaryBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
    where T : notnull
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void ToDictionary()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .Distinct()
                  .ToDictionary(x => x);
    }
}


















