using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.SelectMany)]
public partial class SelectManyBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void SelectMany()
    {
        source.Default
              .AsValueEnumerable()
              .SelectMany(x => Enumerable.Repeat(x, 1))
              .Consume(consumer);
    }
}
