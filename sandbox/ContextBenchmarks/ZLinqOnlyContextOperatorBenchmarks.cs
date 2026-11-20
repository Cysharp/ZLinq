using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using ZLinq;

namespace ZLinq.ContextBenchmarks;

[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class ZLinqOnlyContextOperatorBenchmarks
{
    [Params(100, 10_000)]
    public int N { get; set; }

    int[] source = default!;
    int threshold;
    int adjustment;

    [GlobalSetup]
    public void Setup()
    {
        source = System.Linq.Enumerable.Range(0, N).ToArray();
        threshold = N / 2;
        adjustment = 3;
    }

    [Benchmark(Baseline = true), BenchmarkCategory("LongCount")]
    public long Linq_LongCount()
    {
        var state = threshold;
        return source.LongCount(x => x >= state);
    }


    [Benchmark, BenchmarkCategory("LongCount")]
    public long ZLinq_LongCount()
    {
        var state = threshold;
        return source.AsValueEnumerable().LongCount(x => x >= state);
    }


    [Benchmark, BenchmarkCategory("LongCount")]
    public long ZLinqWithContext_LongCount()
        => source.AsValueEnumerable().LongCount(static (x, state) => x >= state, threshold);


    [Benchmark(Baseline = true), BenchmarkCategory("AggregateResultSelector")]
    public int Linq_AggregateResultSelector()
    {
        var state = adjustment;
        return source.Aggregate(1, (acc, x) => acc + x + state, acc => acc * state);
    }


    [Benchmark, BenchmarkCategory("AggregateResultSelector")]
    public int ZLinq_AggregateResultSelector()
    {
        var state = adjustment;
        return source.AsValueEnumerable().Aggregate(1, (acc, x) => acc + x + state, acc => acc * state);
    }


    [Benchmark, BenchmarkCategory("AggregateResultSelector")]
    public int ZLinqWithContext_AggregateResultSelector()
        => source.AsValueEnumerable().Aggregate(1, static (acc, x, state) => acc + x + state, static (acc, state) => acc * state, adjustment);


    [Benchmark(Baseline = true), BenchmarkCategory("SelectManyIEnumerable")]
    public int Linq_SelectManyIEnumerable()
    {
        var state = adjustment;
        return source.SelectMany(x => System.Linq.Enumerable.Range(x + state, 2)).Sum();
    }


    [Benchmark, BenchmarkCategory("SelectManyIEnumerable")]
    public int ZLinq_SelectManyIEnumerable()
    {
        var state = adjustment;
        return source.AsValueEnumerable().SelectMany(x => System.Linq.Enumerable.Range(x + state, 2)).Sum();
    }


    [Benchmark, BenchmarkCategory("SelectManyIEnumerable")]
    public int ZLinqWithContext_SelectManyIEnumerable()
        => source.AsValueEnumerable().SelectMany(static (x, state) => System.Linq.Enumerable.Range(x + state, 2), adjustment).Sum();


    [Benchmark(Baseline = true), BenchmarkCategory("First")]
    public int Linq_First()
    {
        var state = threshold;
        return source.First(x => x >= state);
    }


    [Benchmark, BenchmarkCategory("First")]
    public int ZLinq_First()
    {
        var state = threshold;
        return source.AsValueEnumerable().First(x => x >= state);
    }


    [Benchmark, BenchmarkCategory("First")]
    public int ZLinqWithContext_First()
        => source.AsValueEnumerable().First(static (x, state) => x >= state, threshold);


    [Benchmark(Baseline = true), BenchmarkCategory("FirstOrDefault")]
    public int Linq_FirstOrDefault()
    {
        var state = N;
        return source.FirstOrDefault(x => x > state);
    }


    [Benchmark, BenchmarkCategory("FirstOrDefault")]
    public int ZLinq_FirstOrDefault()
    {
        var state = N;
        return source.AsValueEnumerable().FirstOrDefault(x => x > state);
    }


    [Benchmark, BenchmarkCategory("FirstOrDefault")]
    public int ZLinqWithContext_FirstOrDefault()
        => source.AsValueEnumerable().FirstOrDefault(static (x, state) => x > state, N);


    [Benchmark(Baseline = true), BenchmarkCategory("FirstOrDefaultWithDefault")]
    public int Linq_FirstOrDefaultWithDefault()
    {
        var state = N;
        return source.FirstOrDefault(x => x > state, -1);
    }


    [Benchmark, BenchmarkCategory("FirstOrDefaultWithDefault")]
    public int ZLinq_FirstOrDefaultWithDefault()
    {
        var state = N;
        return source.AsValueEnumerable().FirstOrDefault(x => x > state, -1);
    }


    [Benchmark, BenchmarkCategory("FirstOrDefaultWithDefault")]
    public int ZLinqWithContext_FirstOrDefaultWithDefault()
        => source.AsValueEnumerable().FirstOrDefault(static (x, state) => x > state, N, -1);


    [Benchmark(Baseline = true), BenchmarkCategory("Last")]
    public int Linq_Last()
    {
        var state = threshold;
        return source.Last(x => x >= state);
    }


    [Benchmark, BenchmarkCategory("Last")]
    public int ZLinq_Last()
    {
        var state = threshold;
        return source.AsValueEnumerable().Last(x => x >= state);
    }


    [Benchmark, BenchmarkCategory("Last")]
    public int ZLinqWithContext_Last()
        => source.AsValueEnumerable().Last(static (x, state) => x >= state, threshold);


    [Benchmark(Baseline = true), BenchmarkCategory("LastOrDefault")]
    public int Linq_LastOrDefault()
    {
        var state = N;
        return source.LastOrDefault(x => x > state);
    }


    [Benchmark, BenchmarkCategory("LastOrDefault")]
    public int ZLinq_LastOrDefault()
    {
        var state = N;
        return source.AsValueEnumerable().LastOrDefault(x => x > state);
    }


    [Benchmark, BenchmarkCategory("LastOrDefault")]
    public int ZLinqWithContext_LastOrDefault()
        => source.AsValueEnumerable().LastOrDefault(static (x, state) => x > state, N);


    [Benchmark(Baseline = true), BenchmarkCategory("LastOrDefaultWithDefault")]
    public int Linq_LastOrDefaultWithDefault()
    {
        var state = N;
        return source.LastOrDefault(x => x > state, -1);
    }


    [Benchmark, BenchmarkCategory("LastOrDefaultWithDefault")]
    public int ZLinq_LastOrDefaultWithDefault()
    {
        var state = N;
        return source.AsValueEnumerable().LastOrDefault(x => x > state, -1);
    }


    [Benchmark, BenchmarkCategory("LastOrDefaultWithDefault")]
    public int ZLinqWithContext_LastOrDefaultWithDefault()
        => source.AsValueEnumerable().LastOrDefault(static (x, state) => x > state, N, -1);


    [Benchmark(Baseline = true), BenchmarkCategory("Single")]
    public int Linq_Single()
    {
        var state = threshold;
        return source.Single(x => x == state);
    }


    [Benchmark, BenchmarkCategory("Single")]
    public int ZLinq_Single()
    {
        var state = threshold;
        return source.AsValueEnumerable().Single(x => x == state);
    }


    [Benchmark, BenchmarkCategory("Single")]
    public int ZLinqWithContext_Single()
        => source.AsValueEnumerable().Single(static (x, state) => x == state, threshold);


    [Benchmark(Baseline = true), BenchmarkCategory("SingleOrDefault")]
    public int Linq_SingleOrDefault()
    {
        var state = N;
        return source.SingleOrDefault(x => x > state);
    }


    [Benchmark, BenchmarkCategory("SingleOrDefault")]
    public int ZLinq_SingleOrDefault()
    {
        var state = N;
        return source.AsValueEnumerable().SingleOrDefault(x => x > state);
    }


    [Benchmark, BenchmarkCategory("SingleOrDefault")]
    public int ZLinqWithContext_SingleOrDefault()
        => source.AsValueEnumerable().SingleOrDefault(static (x, state) => x > state, N);


    [Benchmark(Baseline = true), BenchmarkCategory("SingleOrDefaultWithDefault")]
    public int Linq_SingleOrDefaultWithDefault()
    {
        var state = N;
        return source.SingleOrDefault(x => x > state, -1);
    }


    [Benchmark, BenchmarkCategory("SingleOrDefaultWithDefault")]
    public int ZLinq_SingleOrDefaultWithDefault()
    {
        var state = N;
        return source.AsValueEnumerable().SingleOrDefault(x => x > state, -1);
    }


    [Benchmark, BenchmarkCategory("SingleOrDefaultWithDefault")]
    public int ZLinqWithContext_SingleOrDefaultWithDefault()
        => source.AsValueEnumerable().SingleOrDefault(static (x, state) => x > state, N, -1);
}
