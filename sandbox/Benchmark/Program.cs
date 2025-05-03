#pragma warning disable

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Cathei.LinqGen;
using Kokuban;
using Microsoft.DiagnosticsHub;
using Perfolizer.Horology;
using SpanLinq;
using System.Diagnostics;
using ZLinq;
using ZLinq.Internal;

namespace Benchmark;


internal static class Program
{
    public static int Main(string[] args)
    {
#if DEBUG
        var bench = new Benchmark.AnyPredicate();
        bench.N = 1000;
        bench.Setup();

        bench.ArrayExists();
        bench.Linq();
        bench.ZLinq();


        return 0;
#endif

        if (args.Length != 0)
        {
            Console.WriteLine($"Start ZLinq benchmarks with args: {string.Join(' ', args)}");
            Console.WriteLine();
        }

        try
        {
            // Gets custom benchmark config
            var globalConfig = GetCustomBenchmakConfig(args ?? []);

            // Run benchmark
            var switcher = BenchmarkSwitcher.FromAssemblies([typeof(Program).Assembly]);
            var summaries = switcher.Run(args, globalConfig).ToArray();

            if (summaries.Length == 0)
            {
                if (IsShowInfoMode(args))
                    return 0;

                // Benchmark is not found by specified filter. or failed on benchmark validation/build phase.
                Console.WriteLine();
                Console.WriteLine(Chalk.Red["Benchmark is not executed. Verify benchmark log file."]);
                return 1;
            }

            // Render benchmark results to console
            summaries.RenderToConsole();

            // Export benchmark results report/metadata to files
            summaries.ExportToFiles();

            return 0;
        }
        catch (OperationCanceledException ex)
        {
            Console.WriteLine();
            Console.WriteLine(Chalk.Red[ex.Message]);
            return 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine(Chalk.Red[ex.ToString()]);
            return 1;
        }
    }

    /// <summary>
    /// Gets custom benchmark config from extra arguments.
    /// </summary>
    private static IConfig GetCustomBenchmakConfig(string[] args)
    {
        // Gets BenchmarkDotNet arguments.
        var benchmarkArgs = args.TakeWhile(x => x != "--").ToArray();

        // Gets extra arguments.
        var extraArgs = args.SkipWhile(x => x != "--").Skip(1).ToArray();

        var key = extraArgs.FirstOrDefault() ?? "Default";
        ManualConfig config = key switch
        {
            "Default" => new DefaultBenchmarkConfig(),
            "InProcess" => new InProcessBenchmarkConfig(),
            "InProcessMonitoring" or "Test" => new InProcessMonitoringBenchmarkConfig(),
            "NuGetVersions" => new NuGetVersionsBenchmarkConfig(),
            "TargetFrameworks" => new TargetFrameworksBenchmarkConfig(),
            "ColdStart" => new ColdStartBenchmarkConfig(),
            "SystemLinq" => new SystemLinqBenchmarkConfig(),
            _ => throw new ArgumentException($"Specified benchmark config key is not supported: {key}"),
        };

        if (IsBenchmarkSelectMode(benchmarkArgs) || IsShowInfoMode(benchmarkArgs) || extraArgs.Contains("--verbose"))
            config.AddLogger(ConsoleLogger.Default);

        Console.WriteLine($"Run Benchmarks with config: {config.GetType().Name}");
        return config;
    }

    private static bool IsBenchmarkSelectMode(string[] args)
    {
        // If filter parameter is not specified.
        // Select benchmark by using BenchmarkSwitcher
        return !args.AsSpan().Contains("--filter");
    }

    private static bool IsShowInfoMode(string[] args)
    {
        // Check parameter that show information only.
        if (args.AsSpan().ContainsAny(["--help", "--list", "--info", "--version"]))
            return true;

        return false;
    }
}
