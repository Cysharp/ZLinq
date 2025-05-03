using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Filters;
using BenchmarkDotNet.Running;
using System.Text.RegularExpressions;

namespace Benchmark;

/// <summary>
/// Custom filter that filter benchmarks with `Benchmark.ZLinq.*` namespace and `ZLinqOnly` category.
/// </summary>
public partial class ZLinqBenchmarkFilter : IFilter
{
    [GeneratedRegex(@"Benchmark.ZLinq.*", RegexOptions.CultureInvariant)]
    public static partial Regex ZLinqNamespaceRegex();

    public virtual bool Predicate(BenchmarkCase benchmarkCase)
    {
        // Filter by namespace
        var fullBenchmarkName = FullNameProvider.GetBenchmarkName(benchmarkCase);
        var nameWithoutArgs = benchmarkCase.Descriptor.GetFilterName();

        bool isMatched = ZLinqNamespaceRegex().IsMatch(fullBenchmarkName)
                      || ZLinqNamespaceRegex().IsMatch(nameWithoutArgs);

        if (!isMatched)
            return false;

        switch (benchmarkCase.Job.Id)
        {
            // Filter by `ZLinqOnly` category for SystemLinq benchmarks.
            case SystemLinqBenchmarkConfig.SystemLinqJobId:
                var categories = benchmarkCase.Descriptor.Categories;
                return !categories.Contains(Categories.Filters.ZLinqOnly);
            default:
                return true;
        }
    }
}
