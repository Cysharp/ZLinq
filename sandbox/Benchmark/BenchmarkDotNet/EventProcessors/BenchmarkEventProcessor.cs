using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.EventProcessors;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains;
using BenchmarkDotNet.Toolchains.Results;
using BenchmarkDotNet.Validators;
using System.Diagnostics;
using System.Text;

namespace Benchmark;

/// <summary>
/// Custom EventProcessor to report benchmark progress.
/// </summary>
public class BenchmarkEventProcessor : EventProcessor
{
    private static readonly ILogger Logger = ConsoleLogger.Default;

    private const string Indent = "  ";
    private int totalBenchmarkCount = 0;
    private int completedBenchmarkCount = 0;

    private bool isAnyOfBenchmarkBuildFailed = false;
    private int maxBenchmarkMethodNameLength = 0;
    private int maxBenchmarkParameterInfoLength = 0;
    private bool containsMultipleJobIds = false;

    public static readonly BenchmarkEventProcessor Instance = new();

    private BenchmarkEventProcessor() { }

    public override void OnValidationError(ValidationError validationError)
    {
        var benchmarkCase = validationError.BenchmarkCase;
        if (benchmarkCase == null)
        {
            // ValidationError that is not linked to specific benchmark
            Logger.WriteLineError($"ValidationError: {validationError.Message}");
            return;
        }
        else
        {
            var name = GetBenchmarkCaseDisplayName(benchmarkCase);
            Logger.WriteLineError($"ValidationError: {name}: {validationError.Message}");
        }
    }

    public override void OnStartBuildStage(IReadOnlyList<BuildPartition> partitions)
    {
        Logger.WriteLine($"Building {partitions.Count} benchmark projects...");
        totalBenchmarkCount = partitions.Sum(x => x.Benchmarks.Length);
        completedBenchmarkCount = 0;
    }

    public override void OnBuildComplete(BuildPartition partition, BuildResult buildResult)
    {
        var benchmarkCase = partition.RepresentativeBenchmarkCase;
        var jobId = benchmarkCase.Job.ResolvedId;

        if (!buildResult.IsBuildSuccess)
        {
            isAnyOfBenchmarkBuildFailed = true;

            Logger.WriteLineError($"{Indent}Failed to build benchmark project ({jobId}):");
            Logger.WriteLineError($"{buildResult.ErrorMessage}");

            if (buildResult.GenerateException != null)
                Logger.WriteLineError($"{buildResult.GenerateException.ToString()}");
            
            return;
        }

        if (benchmarkCase.Job.Infrastructure.TryGetToolchain(out var toolchain))
        {
            Logger.WriteLine($"{Indent}Build Completed: {toolchain.Name} ({jobId})");
        }
        else
        {
            // If toolchain is not available. Use JobId instead. Because `ToolchainExtensions.GetToolchain` method is marked as internal.
            Logger.WriteLine($"{Indent}Build Completed: {jobId}");
        }
    }

    public override void OnEndBuildStage()
    {
        if (isAnyOfBenchmarkBuildFailed)
            throw new OperationCanceledException("Benchmark stopped unexpectedly. Because an error occurred during the build phase of benchmark projects.");
    }

    public override void OnStartRunStage()
    {
        Logger.WriteLine($"Start run benchmarks...");
    }

    public override void OnStartRunBenchmarksInType(Type type, IReadOnlyList<BenchmarkCase> benchmarks)
    {
        Logger.WriteLine($"Start run benchmarks in type: {type.GetDisplayName()}...");

        // Gets max length of benchmark method name.
        maxBenchmarkMethodNameLength = benchmarks.Count > 0
            ? benchmarks.Max(x => x.Descriptor.WorkloadMethod.Name.Length)
            : 0;

        // Gets max length of benchmark parameters info.
        maxBenchmarkParameterInfoLength = benchmarks.Count > 0
            ? benchmarks.Max(x => x.HasParameters ? x.Parameters.PrintInfo.Length : 0)
            : 0;

        // Gets benchmarks contains multiple JobIds or not.
        containsMultipleJobIds = benchmarks.DistinctBy(x => x.Job.ResolvedId).Count() > 1;
    }

    public override void OnStartRunBenchmark(BenchmarkCase benchmarkCase)
    {
        var benchmarkCaseName = GetBenchmarkCaseDisplayName(benchmarkCase);

        var totalDigits = totalBenchmarkCount.ToString().Length;
        var index = (completedBenchmarkCount + 1).ToString().PadLeft(totalDigits);
        var prefixText = $"[{index}/{totalBenchmarkCount}]";

        Logger.WriteLine($"{Indent}{prefixText} Running benchmark: {benchmarkCaseName}...");
    }

    public override void OnEndRunBenchmark(BenchmarkCase benchmarkCase, BenchmarkReport report)
    {
        if (!report.Success)
        {
            var lines = report.ExecuteResults.SelectMany(x => x.Results)
                                             .Where(x => !string.IsNullOrEmpty(x));
            var text = string.Join(Environment.NewLine, lines).Trim();

            if (!string.IsNullOrEmpty(text))
                Logger.WriteLineError(text);
        }

        ++completedBenchmarkCount;
    }

    public override void OnEndRunBenchmarksInType(Type type, Summary summary)
    {
        PrintBenchmarkProblems(summary);

        // var totalTimeText = summary.TotalTime.TotalSeconds.ToString("f1");
        // Logger.WriteLine($"End run benchmarks in type: {type.GetDisplayName()}. Total Time: {totalTimeText} seconds");
    }

    public override void OnEndRunStage()
    {
        Logger.WriteLine($"End run benchmarks...");
    }

    private static void PrintBenchmarkProblems(Summary summary)
    {
        // Run analyzers and print warnings/errors
        {
            var conclusions = summary.BenchmarksCases
                                     .SelectMany(benchmark => benchmark.Config.GetCompositeAnalyser().Analyse(summary))
                                     .Distinct()
                                     .ToArray();

            PrintConclutions(conclusions, ConclusionKind.Warning);
            PrintConclutions(conclusions, ConclusionKind.Error);
        }

        // Print ConfigAnalysisConclusion warnings/errors.
        {
            var conclusions = summary.BenchmarksCases
                                     .SelectMany(benchmark => benchmark.Config.ConfigAnalysisConclusion)
                                     .Distinct()
                                     .ToArray();

            PrintConclutions(conclusions, ConclusionKind.Warning);
            PrintConclutions(conclusions, ConclusionKind.Error);
        }
    }

    private static void PrintConclutions(Conclusion[] conclusions, ConclusionKind kind)
    {
        var filterdItems = conclusions.Where(x => x.Kind == kind)
                                      .GroupBy(x => x.AnalyserId)
                                      .SelectMany(x => x)
                                      .ToArray();

        if (filterdItems.Length == 0)
            return;

        var logKind = kind switch
        {
            ConclusionKind.Warning => LogKind.Warning,
            ConclusionKind.Error => LogKind.Error,
            ConclusionKind.Hint => LogKind.Hint,
            _ => LogKind.Default,
        };

        Logger.WriteLine(logKind, $"{Indent}{kind}:");
        foreach (var conclusion in filterdItems)
        {
            var category = conclusion.AnalyserId;
            var lines = conclusion.Message.Split(Environment.NewLine);

            if (lines.Length == 1)
            {
                Logger.WriteLine(logKind, $"{Indent}{Indent}{kind}:{category}: {conclusion.Message}");
            }
            else
            {
                Logger.WriteLine(logKind, $"{Indent}{Indent}{kind}:{category}:");
                foreach (var line in lines)
                {
                    Logger.WriteLine(logKind, $"{Indent}{Indent}{line}");
                }
            }
        }
    }

    private string GetBenchmarkCaseDisplayName(BenchmarkCase benchmarkCase)
    {
        var sb = new StringBuilder();
        var methodName = benchmarkCase.Descriptor.WorkloadMethod.Name.PadRight(maxBenchmarkMethodNameLength);

        sb.Append(methodName);

        if (benchmarkCase.HasParameters)
        {
            var parametersInfo = benchmarkCase.Parameters.PrintInfo;
            var parametersInfoWithParentheses = $"({parametersInfo})".PadRight(maxBenchmarkParameterInfoLength + 2);
            sb.Append($" {parametersInfoWithParentheses}");
        }

        if (containsMultipleJobIds)
            sb.Append($" JobId({benchmarkCase.Job.ResolvedId})");

        return sb.ToString();
    }
}
