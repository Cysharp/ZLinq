using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.GroupJoin)]
public partial class GroupJoinBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void GroupJoin()
    {
        source.Default
              .AsValueEnumerable()
              .GroupJoin(source.ArrayData, x => x, x => x, (x, y) => (x, y))
              .Consume(consumer);
    }
}
