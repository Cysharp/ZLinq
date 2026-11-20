using BenchmarkDotNet.Running;

namespace ZLinq.ContextBenchmarks;

internal static class Program
{
    public static void Main(string[] args)
        => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
}
