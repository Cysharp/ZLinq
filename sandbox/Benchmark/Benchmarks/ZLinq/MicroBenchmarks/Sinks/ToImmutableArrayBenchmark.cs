using System.Buffers;
using ZLinq;

namespace Benchmark.ZLinq;

#if !USE_ZLINQ_NUGET_PACKAGE || ZLINQ_1_2_0_OR_GREATER
#if NET8_0_OR_GREATER
[BenchmarkCategory(Categories.Methods.ToImmutableArray)]
public partial class ToImmutableArrayBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    [BenchmarkCategory(Categories.Filters.NET8_0_OR_GREATER)]
    [BenchmarkCategory(Categories.Filters.ZLINQ_1_2_0_OR_GREATER)]
    public void ToImmutableArray()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .ToImmutableArray();
    }
}
#endif
#endif
