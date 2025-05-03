using BenchmarkDotNet.Filters;
using BenchmarkDotNet.Running;

namespace Benchmark;

/// <summary>
/// Custom BenchmarkDotNet filter to exclude benchmarks based on target framework specified as category.
/// This filter is required because BenchmarkDotNet don't support conditional benchmarks with `#if` directive.
/// </summary>
public partial class TargetFrameworkFilter : IFilter
{
    public static readonly TargetFrameworkFilter Instance = new();

    public virtual bool Predicate(BenchmarkCase benchmarkCase)
    {
        var toolchain = benchmarkCase.Job.Infrastructure.Toolchain;
        var versionText = toolchain.Name.Substring(".NET ".Length); // Expected `.NET 8.0` format.

        if (!Version.TryParse(versionText, out var targetVersion))
            return true; // Return true if failed to resolve target version.

        var categories = benchmarkCase.Descriptor.Categories;
        foreach (var category in categories)
        {
            switch (category)
            {
                case Categories.Filters.NET8_0_OR_GREATER:
                    return targetVersion.Major >= 8;
                case Categories.Filters.NET9_0_OR_GREATER:
                    return targetVersion.Major >= 9;
                case Categories.Filters.NET10_0_OR_GREATER:
                    return targetVersion.Major >= 10;

                case Categories.Filters.SystemLinq_NET10_0_OR_GREATER:
                    if (benchmarkCase.Job.Id == SystemLinqBenchmarkConfig.SystemLinqJobId)
                        goto case Categories.Filters.NET10_0_OR_GREATER;
                    continue;

                default:
                    continue;
            }
        }

        return true;
    }
}
