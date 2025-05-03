using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.ToLookup)]
public partial class ToLookupBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void ToLookup()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .ToLookup(x => x);
    }
}


















