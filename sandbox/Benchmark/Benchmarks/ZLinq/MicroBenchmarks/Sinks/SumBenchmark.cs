using System.Numerics;
using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Sum)]
[GenericTypeArguments(typeof(double))]
public partial class SumBenchmark<T> : EnumerableBenchmarkBase<T>
    where T : struct, INumber<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Sum()
    {
#if !USE_SYSTEM_LINQ
        _ = source.Default
                  .AsValueEnumerable()
                  .Sum(x => x / T.CreateChecked(source.Length));
#else
        _ = source.Default
                  .AsValueEnumerable()
                  .Cast<double>() // Cast<double> is required for System.Linq. It don't have Sum<T>() overload.
                  .Sum(x => x / source.Length);
#endif
    }

    // TODO: BenchmarkDotNet don't support conditional benchmark with `#if` condition.
    // ZLinq specific benchmark need to be defined to separate namespaces.

#if !USE_SYSTEM_LINQ
    [Benchmark]
    [BenchmarkCategory(Categories.Filters.ZLinqOnly)]
    [BenchmarkCategory(Categories.From.Default)]
    public void SumUnchecked()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .SumUnchecked();
    }
#endif
}

















