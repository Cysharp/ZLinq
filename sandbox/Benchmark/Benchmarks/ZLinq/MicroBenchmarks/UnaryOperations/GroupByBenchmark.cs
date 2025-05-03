using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.GroupBy)]
public partial class GroupByBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void GroupBy()
    {
        source.Default
              .AsValueEnumerable()
              .GroupBy(x => x)
              .Consume(consumer);
    }
}
