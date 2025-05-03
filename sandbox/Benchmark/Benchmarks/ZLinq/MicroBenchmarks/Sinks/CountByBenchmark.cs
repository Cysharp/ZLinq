using ZLinq;

namespace Benchmark.ZLinq;

#if !USE_SYSTEM_LINQ || NET9_0_OR_GREATER
[BenchmarkCategory(Categories.Methods.CountBy)]
public partial class CountBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
#if USE_SYSTEM_LINQ
    where T : notnull
#endif
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void CountBy()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .CountBy(x => x);
    }
}
#endif


















