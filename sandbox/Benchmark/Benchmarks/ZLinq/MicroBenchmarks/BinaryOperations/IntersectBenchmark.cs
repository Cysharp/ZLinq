using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Intersect)]
public partial class IntersectBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Intersect()
    {
        source.Default
              .AsValueEnumerable()
              .Intersect(source.ArrayData)
              .Consume(consumer);
    }
}
