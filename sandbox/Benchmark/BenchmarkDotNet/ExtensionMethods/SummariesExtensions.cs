using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using Kokuban;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Benchmark;

public static partial class SummariesExtensions
{
    private static readonly ILogger Logger = ConsoleLogger.Default;

    /// <summary>
    /// Render benchmark summaries to console.
    /// </summary>
    public static void RenderToConsole(this Summary[] summaries)
    {
        RenderMarkdownTables(summaries);

        RenderSummariesResult(summaries);
    }

    private static void RenderMarkdownTables(Summary[] summaries)
    {
        // Temporary capture logs.
        var logger = new LogCapture();

        foreach (var grouping in summaries.GroupBy(x => x.BenchmarksCases[0].Descriptor.Type.Name))
        {
            var items = grouping.ToArray();

            Summary summary = items[0];
            var benchmarkTypeName = summary.BenchmarksCases[0].Descriptor.Type.GetDisplayName();

            // Join benchmarks summaries that using generic type.
            if (items.Length > 1)
            {
                benchmarkTypeName = grouping.Key.Replace("`1", "<T>");
                summary = Join(items, TimeSpan.FromTicks(items.Sum(x => x.TotalTime.Ticks)));
            }

            logger.WriteLine();
            logger.WriteLineStatistic($"## Benchmark Results: {benchmarkTypeName}");
            logger.WriteLine();

            summary.Table.RenderToConsole(logger);

            logger.WriteLine();
        }

        // Render accumulated logs to console
        var lines = logger.CapturedOutput
                          .Where(x => x.Kind == LogKind.Statistic || x.Kind == LogKind.Header || x.Text == Environment.NewLine)
                          .Select(x => x.Text)
                          .ToArray();

        var text = string.Join("", lines).TrimEnd();
        Logger.WriteLineStatistic(text);
    }

    // Helper methods to create joined summary.
    private static Summary Join(Summary[] summaries, TimeSpan totalTime)
            => new Summary(
                $"BenchmarkRun-joined-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}",
                summaries.SelectMany(summary => summary.Reports).ToImmutableArray(),
                HostEnvironmentInfo.GetCurrent(),
                summaries.First().ResultsDirectoryPath,
                summaries.First().LogFilePath,
                totalTime,
                summaries.First().GetCultureInfo(),
                summaries.SelectMany(summary => summary.ValidationErrors).ToImmutableArray(),
                summaries.SelectMany(summary => summary.ColumnHidingRules).ToImmutableArray()
    );

    // Render benchmark result summaries with clickable links.
    private static void RenderSummariesResult(Summary[] summaries)
    {
        // Print executed benchmarks count.
        var benchmarkCount = summaries.Sum(x => x.GetNumberOfExecutedBenchmarks());

        Logger.WriteLine();
        Logger.WriteLine($"{benchmarkCount} benchmarks are executed.");
        Logger.WriteLine();

        // LogFilePath/ResultsDirectoryPath is shared between summaries. So use first element.
        var summary = summaries[0];

        // Print log file path.
        var logFilePath = summary.LogFilePath;
        Logger.WriteLine("Benchmark LogFile:");
        WriteLineAsClickableLink(logFilePath);

        // Print results directory path.
        var resultsDirectoryPath = summary.ResultsDirectoryPath;
        Logger.WriteLine();
        Logger.WriteLine("Benchmark Results Directory:");
        WriteLineAsClickableLink(resultsDirectoryPath);

        // Print exported file links.
        Logger.WriteLine();
        Logger.WriteLine("Exported Files:");
        var exportedFiles = ExtractExportedFiles(summary);
        foreach (var path in exportedFiles)
        {
            var exportedFilePath = Path.Combine(Directory.GetCurrentDirectory(), path);
            WriteLineAsClickableLink(exportedFilePath);
        }
        Logger.WriteLine();

        // If benchmark failed. Print detailed log file path as error.
        var benchmarksWithTroubles = summaries.SelectMany(x => x.Reports).Where(r => !r.GetResultRuns().Any()).Select(r => r.BenchmarkCase).ToArray();
        if (benchmarksWithTroubles.Any())
        {
            Logger.WriteLine(Chalk.Red[$"Error: {benchmarksWithTroubles.Length} benchmarks faled to run."]);
            Logger.WriteLine();

            // Print clickable log file link.
            Logger.WriteLine(Chalk.Red[$"For detailed error messages. Confirm the output log file."]);
            Logger.Write(Chalk.Red["  " + ToClickableLink(logFilePath)]);
            Logger.Write(Chalk.Red["."]);
        }
    }

    private static string[] ExtractExportedFiles(Summary summary)
    {
        var config = summary.BenchmarksCases[0].Config;
        var exporters = config.GetExporters().OfType<ExporterBase>();

        return config.GetExporters()
                     .OfType<ExporterBase>()
                     .Select(x => GetArtifactFullName(x, summary))
                     .ToArray();
    }

    private static void WriteLineAsClickableLink(string link, string prefixIndent = "  ")
    {
        Logger.Write($"{prefixIndent}");
        Logger.WriteLine(ToClickableLink(link));
    }

    private const string ESC = "\u001B"; // \e
    private static string ToClickableLink(string url, string? caption = null)
    {
        caption ??= url;
        return $"{ESC}]8;;{url}{ESC}\\{url}{ESC}]8;;{ESC}\\";
    }

    // Currently this API is not publicky exposed.
    // See: https://github.com/dotnet/BenchmarkDotNet/issues/2619
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = nameof(GetArtifactFullName))]
    private static extern string GetArtifactFullName(ExporterBase target, Summary summary);
}
