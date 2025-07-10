using System.Runtime.InteropServices;
using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.Emit;

namespace Benchmark;

/// <summary>
/// Benchmark config that use InProcessEmitToolchain and RunStrategy.Monitoring.
/// It's run fastest. Because it skip overhead evaluating steps.
/// To obtain more accurate benchwork results. It need to adjust `--iterationCount` argument.
/// If benchmark time is less than 100ms. Warning messages are displayed)
/// </summary>
public class InProcessMonitoringBenchmarkConfig : BaseBenchmarkConfig
{
    public InProcessMonitoringBenchmarkConfig() : base()
    {
        // Configure base job config
        var baseJobConfig = GetBaseJobConfig().Freeze();

        // Add benchmark job.
        AddJob(baseJobConfig.WithToolchain(InProcessEmitToolchain.Instance)
                            .WithStrategy(RunStrategy.Monitoring)
                            .WithId($"InProcessMonitoring({RuntimeInformation.FrameworkDescription})"));

        // Configure additional settings.
        AddConfigurations();
    }

    protected override void AddAnalyzers()
    {
        AddAnalyser(DefaultConfig.Instance
                                 .GetAnalysers()
                                 .Where(x => x is not MinIterationTimeAnalyser) // Remove MinIterationTimeAnalyser
                                 .ToArray());
    }
}
