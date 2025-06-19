using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;

namespace Benchmark;

/// <summary>
/// BenchmarkConfig that run benchmarks with System.Linq
/// </summary>
public class SystemLinqBenchmarkConfig : BaseBenchmarkConfig
{
    internal const string SystemLinqJobId = "SystemLinq";
    internal const string ZLinqJobId = "ZLinq";

    public SystemLinqBenchmarkConfig() : base()
    {
        var baseJobConfig = GetBaseJobConfig().Freeze();

        // Add job for System.Linq benchmarks.
        AddJob(baseJobConfig.WithToolchain(Constants.DefaultToolchain)
                            .WithCustomBuildConfiguration(Constants.CustomBuildConfigurations.SystemLinq)
                            .WithId(SystemLinqJobId)
                            .AsBaseline());

        // Add job for ZLinq benchmarks.
        AddJob(baseJobConfig.WithToolchain(Constants.DefaultToolchain)
                            .WithId(ZLinqJobId));

        // Show summary with declared order (Default: execution order)
        WithOrderer(new DefaultOrderer(summaryOrderPolicy: SummaryOrderPolicy.Declared, jobOrderPolicy: JobOrderPolicy.Numeric));

        // Configure additional settings.
        AddConfigurations();
    }

    protected override void AddAnalyzers()
    {
        // Remove BaselineCustomAnalyzer to suppress warning.
        // Because baseline benchmark might be excluded by filter.
        var analyzers = DefaultConfig.Instance
                                     .GetAnalysers()
                                     .Where(x => x is not BaselineCustomAnalyzer)
                                     .ToArray();

        AddAnalyser(analyzers);
    }

    protected override void AddColumnHidingRules()
    {
        // HideColumns(Column.Toolchain);
        HideColumns(Column.Arguments);
        HideColumns(Column.BuildConfiguration);
    }

    protected override void AddFilters()
    {
        base.AddFilters();
        AddFilter(new ZLinqBenchmarkFilter());
    }

    protected override void AddLogicalGroupRules()
    {
        AddLogicalGroupRules(
        [
            BenchmarkLogicalGroupRule.ByCategory,
            BenchmarkLogicalGroupRule.ByMethod,
        ]);
    }
}
