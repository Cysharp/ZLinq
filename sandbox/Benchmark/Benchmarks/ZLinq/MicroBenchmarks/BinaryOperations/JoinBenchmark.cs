using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Join)]
public partial class JoinBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Join()
    {
        var ass = source.Default
              .AsValueEnumerable()
              .Join(source.ArrayData, x => x, x => x, (x, y) => (x, y));
    }
}
