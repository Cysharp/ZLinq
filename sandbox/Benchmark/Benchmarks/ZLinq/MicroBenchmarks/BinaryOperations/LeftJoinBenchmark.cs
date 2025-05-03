using System.Runtime.CompilerServices;
using ZLinq;

namespace Benchmark.ZLinq;

#if !USE_SYSTEM_LINQ || NET10_0_OR_GREATER
[BenchmarkCategory(Categories.Methods.LeftJoin)]
[BenchmarkCategory(Categories.Filters.SystemLinq_NET10_0_OR_GREATER)]
[GenericTypeArguments(typeof(int))]
public partial class LeftJoinBenchmark<T> : EnumerableBenchmarkBase<T>
{
    protected override void Setup()
    {
        if (source.Length > 10_000 && Unsafe.SizeOf<T>() <= 2)
            throw new NotSupportedException("LeftJoin generate large data unexpectedly when collection have a lot of duplicated keys.");
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void LeftJoin()
    {
        source.Default
              .AsValueEnumerable()
              .LeftJoin(source.ArrayData, x => x, x => x, (x, y) => (x, y))
              .Consume(consumer);
    }
}
#endif
