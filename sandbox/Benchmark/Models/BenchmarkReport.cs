using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Benchmark;

internal class BenchmarkResultsReport
{
    public required string Title { get; init; }

    public int TotalBenchmarkCount { get; init; }

    public required string Arguments { get; init; }

    public required string EnvironmentInfo { get; init; }

    public required BenchmarkInfo[] Items { get; init; }

    public static BenchmarkResultsReport FromSummaries(Summary[] summaries)
    {
        var args = Environment.GetCommandLineArgs();
        var extraArgs = args.SkipWhile(x => x != "--").Skip(1).ToArray();
        var configKey = extraArgs.FirstOrDefault() ?? "Default";

        return new BenchmarkResultsReport
        {
            Title = $"BenchmarkReport({configKey})",
            Arguments = string.Join(' ', args.Skip(1)),
            TotalBenchmarkCount = summaries.Sum(x => x.GetNumberOfExecutedBenchmarks()),
            EnvironmentInfo = string.Join(Environment.NewLine, HostEnvironmentInfo.GetCurrent().ToFormattedString()),
            Items = summaries.SelectMany(collectionSelector: x => x.BenchmarksCases, resultSelector: (summary, benchmarkCase) => (summary, benchmarkCase))
                             .Select(x => BenchmarkInfo.FromBenchmarkCase(x.benchmarkCase, x.summary))
                             .ToArray(),
        };
    }

    internal class BenchmarkInfo
    {
        public required string BenchmarkTypeName { get; init; }
        public required string GenericsArgumentTypeName { get; init; }
        public required string Namespace { get; init; }
        public required string FilterName { get; init; }
        public required string[] Categories { get; init; }

        public required string Markdown { get; init; }

        public static BenchmarkInfo FromBenchmarkCase(BenchmarkCase benchmarkCase, Summary summary)
        {
            var descriptor = benchmarkCase.Descriptor;
            var type = descriptor.Type;

            return new BenchmarkInfo
            {
                BenchmarkTypeName = type.GetDisplayName(),
                GenericsArgumentTypeName = type.GenericTypeArguments.FirstOrDefault()?.GetDisplayName() ?? "",
                Namespace = type.Namespace ?? "",
                FilterName = descriptor.GetFilterName(),
                Categories = descriptor.Categories,
                Markdown = summary.Table.GetSummaryTableMarkdown(),
            };
        }
    }
}
