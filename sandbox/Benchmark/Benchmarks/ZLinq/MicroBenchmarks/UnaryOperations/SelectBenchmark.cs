using BenchmarkDotNet.Attributes;
using System.Runtime.CompilerServices;
using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Select)]
public partial class SelectBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Select_Linq()
    {
        source.Default
              .AsValueEnumerable()
              .Select(x => x)
              .Consume(consumer);
    }
}
