using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Zip)]
public partial class ZipBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Zip()
    {
        source.Default
              .AsValueEnumerable()
              .Zip(source.ArrayData, (x, y) => (x, y))
              .Consume(consumer);
    }
}
