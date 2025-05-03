using System.Numerics;
using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Average)]
[GenericTypeArguments(typeof(double))]
public class AverageBenchmark<T> : EnumerableBenchmarkBase<T>
    where T : struct
#if NET8_0_OR_GREATER
    , INumber<T>
#endif
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Average()
    {
#if !USE_SYSTEM_LINQ
        _ = source.Default
                  .AsValueEnumerable()
                  .Average(x => x / T.CreateChecked(source.Length));
#else
        _ = source.Default
                  .Cast<double>() // Cast<double> is required for System.Linq. It don't have Average<T>() overload.
                  .Average();
#endif
    }
}
