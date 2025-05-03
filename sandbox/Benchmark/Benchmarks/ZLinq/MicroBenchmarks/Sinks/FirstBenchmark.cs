using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.First)]
public partial class FirstBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void First()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .First();
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void FirstOrDefault()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .First(x => EqualityComparer<T>.Default.Equals(x, midElement));
    }
}


















