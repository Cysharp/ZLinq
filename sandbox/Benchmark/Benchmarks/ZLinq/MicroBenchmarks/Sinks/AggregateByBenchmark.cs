using ZLinq;

namespace Benchmark.ZLinq;

#if !USE_SYSTEM_LINQ || NET9_0_OR_GREATER
[BenchmarkCategory(Categories.Methods.AggregateBy)]
public partial class AggregateByBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
#if USE_SYSTEM_LINQ
    where T : notnull
#endif
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void AggregateBy()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .AggregateBy(x => x, seed: 0, (i, j) => default!);
    }
}
#endif

















