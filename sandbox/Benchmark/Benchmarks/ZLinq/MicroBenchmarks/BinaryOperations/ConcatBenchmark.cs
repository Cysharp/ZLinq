using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Concat)]
public partial class ConcatBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Concat()
    {
        source.Default
              .AsValueEnumerable()
              .Concat(source.ArrayData)
              .Consume(consumer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Concat_Enumerable()
    {
        source.Default
              .AsValueEnumerable()
              .Concat(source.EnumerableData)
              .Consume(consumer);
    }
}
