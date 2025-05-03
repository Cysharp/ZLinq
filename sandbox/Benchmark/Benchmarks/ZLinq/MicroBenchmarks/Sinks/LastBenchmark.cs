using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Last)]
public partial class LastBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Last()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .Last();
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Last_Predicate()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .Last(x => EqualityComparer<T>.Default.Equals(x, midElement));
    }
}


















