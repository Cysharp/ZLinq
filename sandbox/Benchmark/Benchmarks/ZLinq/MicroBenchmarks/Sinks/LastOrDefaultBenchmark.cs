using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.LastOrDefault)]
public partial class LastOrDefaultBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void LastOrDefault()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .LastOrDefault();
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void LastOrDefault_Predicate()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .LastOrDefault(x => EqualityComparer<T>.Default.Equals(x, midElement));
    }
}


















