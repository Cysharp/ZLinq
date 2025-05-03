using ZLinq;

namespace Benchmark.ZLinq;

#if !USE_SYSTEM_LINQ || NET10_0_OR_GREATER
[BenchmarkCategory(Categories.Methods.Shuffle)]
[BenchmarkCategory(Categories.Filters.NET10_0_OR_GREATER)]
public partial class ShuffleBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Shuffle()
    {
        source.Default
              .AsValueEnumerable()
              .Shuffle()
              .Consume(consumer);
    }
}
#endif
