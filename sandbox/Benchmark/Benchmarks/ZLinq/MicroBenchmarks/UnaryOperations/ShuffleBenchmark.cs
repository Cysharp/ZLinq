using ZLinq;

namespace Benchmark.ZLinq;

#if !USE_SYSTEM_LINQ || NET10_0_OR_GREATER
[BenchmarkCategory(Categories.Methods.Shuffle)]
[BenchmarkCategory(Categories.Filters.SystemLinq_NET10_0_OR_GREATER)]
public partial class ShuffleBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    private readonly int TakeCount = 100;

    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Shuffle()
    {
        source.Default
              .AsValueEnumerable()
              .Shuffle()
              .Consume(consumer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void ShuffleTake()
    {
        source.Default
              .AsValueEnumerable()
              .Shuffle()
              .Take(TakeCount)
              .Consume(consumer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void ShuffleTakeLast()
    {
        source.Default
              .AsValueEnumerable()
              .Shuffle()
              .Take(TakeCount)
              .Consume(consumer);
    }
}
#endif
