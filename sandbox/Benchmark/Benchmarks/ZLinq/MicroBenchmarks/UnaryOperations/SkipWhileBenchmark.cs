using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.SkipWhile)]
public partial class SkipWhileBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void SkipWhile()
    {
        source.Default
              .AsValueEnumerable()
              .SkipWhile(x => true)
              .Consume(consumer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void SkipWhile_ShortCircuit()
    {
        source.Default
              .AsValueEnumerable()
              .SkipWhile(x => false)
              .Consume(consumer);
    }
}
