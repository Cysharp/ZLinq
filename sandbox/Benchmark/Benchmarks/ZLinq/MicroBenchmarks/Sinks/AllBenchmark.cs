using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.All)]
public partial class AllBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void All()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .All(x => true);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void All_ShortCircuit()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .All(x => false);
    }
}


















