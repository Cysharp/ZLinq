using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Any)]
public partial class AnyBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Any()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .Any();
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Any_Predicate()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .Any(x => false); // Use false to enumerabe all element.
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Any_Predicate_ShortCircuit()
    {
        _ = source.Default
                  .AsValueEnumerable()
                  .Any(x => true); // Use false to enumerabe all element.
    }
}


















