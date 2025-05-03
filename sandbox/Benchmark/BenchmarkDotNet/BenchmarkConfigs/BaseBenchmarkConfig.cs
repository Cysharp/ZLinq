using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;

namespace Benchmark;

/// <summary>
/// Base benchmark config.
/// </summary>
public abstract class BaseBenchmarkConfig : ManualConfig
{
    public BaseBenchmarkConfig()
    {
        WithSummaryStyle(SummaryStyle.Default
                                     .WithMaxParameterColumnWidth(40)); // Default: 20 chars

        // Set default options that based on DefaultConfig.
        WithUnionRule(DefaultConfig.Instance.UnionRule);
        WithArtifactsPath(DefaultConfig.Instance.ArtifactsPath);

#if DEBUG
        // Allow benchmarks for debug build.
        WithOptions(ConfigOptions.DisableOptimizationsValidator);
#endif

        // Additional settings used for debugging
        // WithOptions(ConfigOptions.StopOnFirstError);
        // WithOptions(ConfigOptions.KeepBenchmarkFiles);
        // WithOptions(ConfigOptions.GenerateMSBuildBinLog);
    }

    // Use ShortRun as base Job.(LaunchCount=1  TargetCount=3 WarmupCount = 3)
    protected virtual Job GetBaseJobConfig() =>
        Job.ShortRun
           .WithStrategy(RunStrategy.Throughput) // Use default RunStrategy
           .DontEnforcePowerPlan();

    /// <summary>
    /// Add configurations.
    /// </summary>
    protected void AddConfigurations()
    {
        AddLoggers();
        AddColumnProviders();
        AddValidators();
        AddFilters();
        AddDiagnosers();
        AddAnalyzers();
        AddEventProcessors();
        AddExporters();
        AddHardwareCounters();
        AddColumnHidingRules();
        AddLogicalGroupRules();
    }

    protected virtual void AddExporters()
    {
        AddExporter(MarkdownExporter.Console); // Use ConsoleMarkdownExporter to disable group higligting with `**`.
    }

    protected virtual void AddLoggers()
    {
        // Set NullLogger to suppress verbose benchmark progress logs. Benchmark progress is reported by custom EventProcessor instead.
        AddLogger(NullLogger.Instance);
    }

    protected virtual void AddAnalyzers()
    {
        AddAnalyser(DefaultConfig.Instance.GetAnalysers().ToArray());
    }

    protected virtual void AddColumnProviders()
    {
        AddColumnProvider(DefaultColumnProviders.Instance); // Descriptor, Job, Statistics, Params, Metrics
    }

    protected virtual void AddFilters()
    {
        AddFilter(TargetFrameworkFilter.Instance);
    }

    protected virtual void AddDiagnosers()
    {
        AddDiagnoser(MemoryDiagnoser.Default);

        // TODO: Enable following diagnoser (Temporary disabled because extra columns are shown it has no data)
        //AddDiagnoser(ExceptionDiagnoser.Default);
        //AddDiagnoser(ThreadingDiagnoser.Default);
    }

    protected virtual void AddValidators()
    {
        AddValidator(DefaultConfig.Instance.GetValidators().ToArray());
    }

    protected virtual void AddEventProcessors()
    {
        // Use custom event processor to report benchmark progress.
        AddEventProcessor(BenchmarkEventProcessor.Instance);
    }

    protected virtual void AddHardwareCounters()
    {
    }

    protected virtual void AddColumnHidingRules()
    {
    }

    protected virtual void AddLogicalGroupRules()
    {
    }
}
