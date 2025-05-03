using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.LongCount)]
public partial class LongCountBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void LongCount()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .LongCount();
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void LongCount_Predicate()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .LongCount(x => EqualityComparer<T>.Default.Equals(x, x));
    }
}


















