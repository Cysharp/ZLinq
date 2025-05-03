using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Union)]
public partial class UnionBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Union()
    {
        source.Default
              .AsValueEnumerable()
              .Union(source.ArrayData)
              .Consume(consumer); ;
    }
}
