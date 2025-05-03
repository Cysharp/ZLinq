using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Cast)]
public partial class CastBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Cast()
    {
        source.Default
              .AsValueEnumerable()
              .Cast<T>()
              .Consume(consumer);
    }
}
