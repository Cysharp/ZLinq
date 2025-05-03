using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.FirstOrDefault)]
public partial class FirstOrDefaultBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void FirstOrDefault()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .FirstOrDefault();
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void FirstOrDefault_Predicate()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .FirstOrDefault(x => EqualityComparer<T>.Default.Equals(x, midElement));
    }
}


















