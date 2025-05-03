using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Where)]
public partial class WhereBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Where()
    {
        source.Default
              .AsValueEnumerable()
              .Where(x => true)
              .Consume(consumer);
    }
}
