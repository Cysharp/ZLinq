using System.Runtime.CompilerServices;
using ZLinq;

namespace Benchmark.ZLinq;

#if !USE_SYSTEM_LINQ || NET10_0_OR_GREATER
[BenchmarkCategory(Categories.Methods.RightJoin)]
[BenchmarkCategory(Categories.Filters.SystemLinq_NET10_0_OR_GREATER)]
[GenericTypeArguments(typeof(int))]
public partial class RightJoinBenchmark<T> : EnumerableBenchmarkBase<T>
{
    protected override void Setup()
    {
        if (source.Length > 10_000 && Unsafe.SizeOf<T>() <= 2)
            throw new NotSupportedException("RightJoin generate large data unexpectedly when collection have a lot of duplicated keys.");
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void RightJoin()
    {
        source.Default
              .AsValueEnumerable()
              .RightJoin(source.ArrayData, x => x, x => x, (x, y) => (x, y))
              .Consume(consumer);
    }
}
#endif
