using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.TakeWhile)]
public partial class TakeWhileBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void TakeWhile()
    {
        source.Default
              .AsValueEnumerable()
              .TakeWhile(x => true)
              .Consume(consumer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void TakeWhile_ShortCircuit()
    {
        source.Default
              .AsValueEnumerable()
              .TakeWhile(x => false)
              .Consume(consumer);
    }
}
