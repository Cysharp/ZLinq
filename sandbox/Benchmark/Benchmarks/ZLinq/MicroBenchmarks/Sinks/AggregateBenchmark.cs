using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Aggregate)]
public partial class AggregateBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Aggregate()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .Aggregate((i, j) => default!);
    }
}


















