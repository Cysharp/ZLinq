using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using Kokuban;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Benchmark;

public static partial class SummariesExtensions
{
    /// <summary>
    /// Render benchmark summaries to console.
    /// </summary>
    public static void RenderToConsole(this Summary[] summaries)
    {
        var logger = ConsoleLogger.Default;
        RenderMarkdownTables(logger, summaries);
        RenderSummariesResult(logger, summaries);
        logger.Flush();
    }

    /// <summary>
    /// Render benchmark summaries to file.
    /// </summary>
    public static void ExportToFiles(this Summary[] summaries)
    {
        var resultsDirectoryPath = summaries[0].ResultsDirectoryPath;

        // export benchmark reports markdown file
        const string ReportFileName = "reports.md";
        using var streamLogger = new StreamLogger(Path.Combine(resultsDirectoryPath, ReportFileName));
        RenderMarkdownTables(streamLogger, summaries);

        // export benchmark metadat json file
        const string MetadataFileName = "reports.json";
        ExportMetadataToFile(summaries, Path.Combine(resultsDirectoryPath, MetadataFileName));
    }

    internal static string GetSummaryTableMarkdown(this SummaryTable table)
    {
        // Temporary capture logs to filter logs.
        var logCapture = new LogCapture();
        table.RenderToConsole(logCapture);
        var lines = logCapture.CapturedOutput
                              .Where(x => x.Kind == LogKind.Statistic || x.Kind == LogKind.Header || x.Text == Environment.NewLine)
                              .Select(x => x.Text)
                              .ToArray();

        return string.Join("", lines).TrimEnd();
    }

    private static void RenderMarkdownTables(ILogger logger, Summary[] summaries)
    {
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
            logger.WriteLineStatistic($"## {benchmarkTypeName}");
            logger.WriteLine();

            var tableMarkdown = GetSummaryTableMarkdown(summary.Table);
            logger.WriteLineStatistic(tableMarkdown);
            logger.WriteLine();
        }
    }

    private static void ExportMetadataToFile(Summary[] summaries, string path)
    {
        var report = BenchmarkResultsReport.FromSummaries(summaries);

        // Serialize to json
        var json = JsonSerializer.Serialize(report, new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        });

        File.WriteAllText(path, json);
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
    private static void RenderSummariesResult(ILogger logger, Summary[] summaries)
    {
        // Print executed benchmarks count.
        var benchmarkCount = summaries.Sum(x => x.GetNumberOfExecutedBenchmarks());
        var benchmarkTotalTime = summaries.Select(x => x.TotalTime).Aggregate(TimeSpan.Zero, (x, y) => x + y);

        logger.WriteLine();
        logger.WriteLine($"{benchmarkCount} benchmarks are executed. Elapsed: {benchmarkTotalTime.ToString(@"hh\:mm\:ss\.fff")}");
        logger.WriteLine();

        // LogFilePath/ResultsDirectoryPath is shared between summaries. So use first element.
        var firstSummary = summaries[0];

        // Print log file path.
        var logFilePath = firstSummary.LogFilePath;
        logger.WriteLine("Benchmark LogFile:");
        WriteLineAsClickableLink(logger, logFilePath);

        // Print results directory path.
        var resultsDirectoryPath = firstSummary.ResultsDirectoryPath;
        logger.WriteLine();
        logger.WriteLine("Benchmark Results Directory:");
        WriteLineAsClickableLink(logger, resultsDirectoryPath);

        // Print exported file links.
        logger.WriteLine();
        logger.WriteLine("Exported Files:");
        foreach (var summary in summaries)
        {
            var exportedFiles = ExtractExportedFiles(summary);
            foreach (var path in exportedFiles)
            {
                var relativePath = Path.GetRelativePath(resultsDirectoryPath, path);
                WriteLineAsClickableLink(logger, path, caption: relativePath);
            }
        }
        logger.WriteLine();

        // If benchmark failed. Print detailed log file path as error.
        var benchmarksWithTroubles = summaries.SelectMany(x => x.Reports).Where(r => !r.GetResultRuns().Any()).Select(r => r.BenchmarkCase).ToArray();
        if (benchmarksWithTroubles.Any())
        {
            logger.WriteLine(Chalk.Red[$"Error: {benchmarksWithTroubles.Length} benchmarks faled to run."]);
            logger.WriteLine();

            // Print clickable log file link.
            logger.WriteLine(Chalk.Red[$"For detailed error messages. Confirm the output log file."]);
            logger.Write(Chalk.Red["  " + ToClickableLink(logFilePath)]);
            logger.Write(Chalk.Red["."]);
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

    private static void WriteLineAsClickableLink(ILogger logger, string link, string? caption = null, string prefixIndent = "  ")
    {
        logger.Write($"{prefixIndent}");
        logger.WriteLine(ToClickableLink(link, caption));
    }

    private const string ESC = "\u001B"; // \e
    private static string ToClickableLink(string url, string? caption = null)
    {
        caption ??= url;
        return $"{ESC}]8;;{url}{ESC}\\{caption}{ESC}]8;;{ESC}\\";
    }

    // Currently this API is not publicky exposed.
    // See: https://github.com/dotnet/BenchmarkDotNet/issues/2619
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = nameof(GetArtifactFullName))]
    private static extern string GetArtifactFullName(ExporterBase target, Summary summary);
}
