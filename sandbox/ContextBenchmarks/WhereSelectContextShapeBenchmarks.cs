using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using HonkPerf.NET.RefLinq;
using ZLinq;

namespace ZLinq.ContextBenchmarks;

[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class WhereSelectContextShapeBenchmarks
{
    [Params(100, 1_000, 10_000)]
    public int N { get; set; }

    int[] source = default!;
    ObjectContext objectContext = default!;
    (long LongValue, double DoubleValue, string StringValue) tupleContext;

    [GlobalSetup]
    public void Setup()
    {
        source = System.Linq.Enumerable.Range(0, N).ToArray();
        var longValue = N / 2;
        objectContext = new ObjectContext(longValue, 0.25, "ctx");
        tupleContext = (longValue, 0.25, "ctx");
    }

    [Benchmark(Baseline = true), BenchmarkCategory("WhereSelect_ObjectContext")]
    public double Linq_ObjectContext()
    {
        var state = objectContext;
        return source
            .Where(x => x > state.LongValue + state.DoubleValue + state.StringValue.Length)
            .Select(x => x + state.LongValue + state.DoubleValue + state.StringValue.Length)
            .Sum();
    }

    [Benchmark, BenchmarkCategory("WhereSelect_ObjectContext")]
    public double ZLinq_ObjectContext()
    {
        var state = objectContext;
        return source.AsValueEnumerable()
            .Where(x => x > state.LongValue + state.DoubleValue + state.StringValue.Length)
            .Select(x => x + state.LongValue + state.DoubleValue + state.StringValue.Length)
            .Sum();
    }

    [Benchmark, BenchmarkCategory("WhereSelect_ObjectContext")]
    public double ZLinqWithContext_ObjectContext()
        => source.AsValueEnumerable()
            .Where(static (x, state) => x > state.LongValue + state.DoubleValue + state.StringValue.Length, objectContext)
            .Select(static (x, state) => x + state.LongValue + state.DoubleValue + state.StringValue.Length, objectContext)
            .Sum();

    [Benchmark, BenchmarkCategory("WhereSelect_ObjectContext")]
    public double HonkPerf_ObjectContext()
        => source.ToRefLinq()
            .Where(static (x, state) => x > state.LongValue + state.DoubleValue + state.StringValue.Length, objectContext)
            .Select(static (x, state) => x + state.LongValue + state.DoubleValue + state.StringValue.Length, objectContext)
            .Sum();

    [Benchmark(Baseline = true), BenchmarkCategory("WhereSelect_ValueTupleContext")]
    public double Linq_ValueTupleContext()
    {
        var state = tupleContext;
        return source
            .Where(x => x > state.LongValue + state.DoubleValue + state.StringValue.Length)
            .Select(x => x + state.LongValue + state.DoubleValue + state.StringValue.Length)
            .Sum();
    }

    [Benchmark, BenchmarkCategory("WhereSelect_ValueTupleContext")]
    public double ZLinq_ValueTupleContext()
    {
        var state = tupleContext;
        return source.AsValueEnumerable()
            .Where(x => x > state.LongValue + state.DoubleValue + state.StringValue.Length)
            .Select(x => x + state.LongValue + state.DoubleValue + state.StringValue.Length)
            .Sum();
    }

    [Benchmark, BenchmarkCategory("WhereSelect_ValueTupleContext")]
    public double ZLinqWithContext_ValueTupleContext()
        => source.AsValueEnumerable()
            .Where(static (x, state) => x > state.LongValue + state.DoubleValue + state.StringValue.Length, tupleContext)
            .Select(static (x, state) => x + state.LongValue + state.DoubleValue + state.StringValue.Length, tupleContext)
            .Sum();

    [Benchmark, BenchmarkCategory("WhereSelect_ValueTupleContext")]
    public double HonkPerf_ValueTupleContext()
        => source.ToRefLinq()
            .Where(static (x, state) => x > state.LongValue + state.DoubleValue + state.StringValue.Length, tupleContext)
            .Select(static (x, state) => x + state.LongValue + state.DoubleValue + state.StringValue.Length, tupleContext)
            .Sum();

    sealed class ObjectContext(long longValue, double doubleValue, string stringValue)
    {
        public readonly long LongValue = longValue;
        public readonly double DoubleValue = doubleValue;
        public readonly string StringValue = stringValue;
    }
}
