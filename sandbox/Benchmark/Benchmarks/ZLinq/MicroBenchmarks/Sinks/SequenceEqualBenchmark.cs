using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.SequenceEqual)]
public partial class SequenceEqualBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void SequenceEqual()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .SequenceEqual(source.Default);
    }
}


















