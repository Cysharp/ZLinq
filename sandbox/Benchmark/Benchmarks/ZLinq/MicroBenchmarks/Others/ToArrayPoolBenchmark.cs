using System.Buffers;
using ZLinq;

namespace Benchmark.ZLinq;

// System.Memory's CopyTo extension methods accept T[] only. It don't accept IEnumerable<T>.
#if !USE_SYSTEM_LINQ

[BenchmarkCategory(Categories.Methods.ToArrayPool)]
[BenchmarkCategory(Categories.Filters.ZLinqOnly)]
public partial class ToArrayPoolBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void CopyTo()
    {
#if ZLINQ_1_4_0_OR_GREATER
        var ret = source.Default
                        .AsValueEnumerable()
                        .ToArrayPool();
#else
        var ret = source.Default
                .AsValueEnumerable()
                .ToArrayPool();
        ArrayPool<T>.Shared.Return(ret.Array);
#endif
    }
}
#endif
