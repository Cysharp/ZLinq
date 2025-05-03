using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Except)]
public partial class ExceptBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Except()
    {
        source.Default
              .AsValueEnumerable()
              .Except(source.ArrayData)
              .Consume(consumer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Except_Enumerable()
    {
        source.Default
              .AsValueEnumerable()
              .Except(source.EnumerableData)
              .Consume(consumer);
    }
}
