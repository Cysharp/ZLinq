using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using HonkPerf.NET.RefLinq;
using HonkEnumerable = HonkPerf.NET.RefLinq.Enumerable;
using ZLinq;

namespace ZLinq.ContextBenchmarks;

[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class FourImplementationContextOperatorBenchmarks
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

    [Benchmark(Baseline = true), BenchmarkCategory("Select")]
    public int Linq_Select()
    {
        var state = adjustment;
        return source.Select(x => x + state).Sum();
    }


    [Benchmark, BenchmarkCategory("Select")]
    public int ZLinq_Select()
    {
        var state = adjustment;
        return source.AsValueEnumerable().Select(x => x + state).Sum();
    }


    [Benchmark, BenchmarkCategory("Select")]
    public int ZLinqWithContext_Select()
        => source.AsValueEnumerable().Select(static (x, state) => x + state, adjustment).Sum();


    [Benchmark, BenchmarkCategory("Select")]
    public int HonkPerf_Select()
        => source.ToRefLinq().Select(static (x, state) => x + state, adjustment).Sum();


    [Benchmark(Baseline = true), BenchmarkCategory("Where")]
    public int Linq_Where()
    {
        var state = threshold;
        return source.Where(x => x >= state).Sum();
    }


    [Benchmark, BenchmarkCategory("Where")]
    public int ZLinq_Where()
    {
        var state = threshold;
        return source.AsValueEnumerable().Where(x => x >= state).Sum();
    }


    [Benchmark, BenchmarkCategory("Where")]
    public int ZLinqWithContext_Where()
        => source.AsValueEnumerable().Where(static (x, state) => x >= state, threshold).Sum();


    [Benchmark, BenchmarkCategory("Where")]
    public int HonkPerf_Where()
        => source.ToRefLinq().Where(static (x, state) => x >= state, threshold).Sum();


    [Benchmark(Baseline = true), BenchmarkCategory("All")]
    public bool Linq_All()
    {
        var state = N;
        return source.All(x => x < state);
    }


    [Benchmark, BenchmarkCategory("All")]
    public bool ZLinq_All()
    {
        var state = N;
        return source.AsValueEnumerable().All(x => x < state);
    }


    [Benchmark, BenchmarkCategory("All")]
    public bool ZLinqWithContext_All()
        => source.AsValueEnumerable().All(static (x, state) => x < state, N);


    [Benchmark, BenchmarkCategory("All")]
    public bool HonkPerf_All()
        => source.ToRefLinq().All(static (x, state) => x < state, N);


    [Benchmark(Baseline = true), BenchmarkCategory("Any")]
    public bool Linq_Any()
    {
        var state = threshold;
        return source.Any(x => x >= state);
    }


    [Benchmark, BenchmarkCategory("Any")]
    public bool ZLinq_Any()
    {
        var state = threshold;
        return source.AsValueEnumerable().Any(x => x >= state);
    }


    [Benchmark, BenchmarkCategory("Any")]
    public bool ZLinqWithContext_Any()
        => source.AsValueEnumerable().Any(static (x, state) => x >= state, threshold);


    [Benchmark, BenchmarkCategory("Any")]
    public bool HonkPerf_Any()
        => source.ToRefLinq().Any(static (x, state) => x >= state, threshold);


    [Benchmark(Baseline = true), BenchmarkCategory("Count")]
    public int Linq_Count()
    {
        var state = threshold;
        return source.Count(x => x >= state);
    }


    [Benchmark, BenchmarkCategory("Count")]
    public int ZLinq_Count()
    {
        var state = threshold;
        return source.AsValueEnumerable().Count(x => x >= state);
    }


    [Benchmark, BenchmarkCategory("Count")]
    public int ZLinqWithContext_Count()
        => source.AsValueEnumerable().Count(static (x, state) => x >= state, threshold);


    [Benchmark, BenchmarkCategory("Count")]
    public int HonkPerf_Count()
        => source.ToRefLinq().Count(static (x, state) => x >= state, threshold);


    //[Benchmark(Baseline = true), BenchmarkCategory("AggregateNoSeed")]
    //public int Linq_AggregateNoSeed()
    //{
    //    var state = adjustment;
    //    return source.Aggregate((acc, x) => acc + x + state);
    //}


    //[Benchmark, BenchmarkCategory("AggregateNoSeed")]
    //public int ZLinq_AggregateNoSeed()
    //{
    //    var state = adjustment;
    //    return source.AsValueEnumerable().Aggregate((acc, x) => acc + x + state);
    //}


    //[Benchmark, BenchmarkCategory("AggregateNoSeed")]
    //public int ZLinqWithContext_AggregateNoSeed()
    //    => source.AsValueEnumerable().Aggregate(static (acc, x, state) => acc + x + state, adjustment);


    //[Benchmark, BenchmarkCategory("AggregateNoSeed")]
    //public int HonkPerf_AggregateNoSeed()
    //    => source.ToRefLinq().Aggregate(static (acc, x, state) => acc + x + state, adjustment);


    //[Benchmark(Baseline = true), BenchmarkCategory("AggregateSeed")]
    //public int Linq_AggregateSeed()
    //{
    //    var state = adjustment;
    //    return source.Aggregate(1, (acc, x) => acc + x + state);
    //}


    //[Benchmark, BenchmarkCategory("AggregateSeed")]
    //public int ZLinq_AggregateSeed()
    //{
    //    var state = adjustment;
    //    return source.AsValueEnumerable().Aggregate(1, (acc, x) => acc + x + state);
    //}


    //[Benchmark, BenchmarkCategory("AggregateSeed")]
    //public int ZLinqWithContext_AggregateSeed()
    //    => source.AsValueEnumerable().Aggregate(1, static (acc, x, state) => acc + x + state, adjustment);


    //[Benchmark, BenchmarkCategory("AggregateSeed")]
    //public int HonkPerf_AggregateSeed()
    //    => source.ToRefLinq().Aggregate(1, static (acc, x, state) => acc + x + state, adjustment);


    //[Benchmark(Baseline = true), BenchmarkCategory("MinBy")]
    //public int Linq_MinBy()
    //{
    //    var state = adjustment;
    //    return source.MinBy(x => x + state);
    //}


    //[Benchmark, BenchmarkCategory("MinBy")]
    //public int ZLinq_MinBy()
    //{
    //    var state = adjustment;
    //    return source.AsValueEnumerable().MinBy(x => x + state);
    //}


    //[Benchmark, BenchmarkCategory("MinBy")]
    //public int ZLinqWithContext_MinBy()
    //    => source.AsValueEnumerable().MinBy(static (x, state) => x + state, adjustment);


    //[Benchmark, BenchmarkCategory("MinBy")]
    //public int HonkPerf_MinBy()
    //    => source.ToRefLinq().MinBy(static (x, state) => x + state, adjustment);


    //[Benchmark(Baseline = true), BenchmarkCategory("MaxBy")]
    //public int Linq_MaxBy()
    //{
    //    var state = adjustment;
    //    return source.MaxBy(x => x + state);
    //}


    //[Benchmark, BenchmarkCategory("MaxBy")]
    //public int ZLinq_MaxBy()
    //{
    //    var state = adjustment;
    //    return source.AsValueEnumerable().MaxBy(x => x + state);
    //}


    //[Benchmark, BenchmarkCategory("MaxBy")]
    //public int ZLinqWithContext_MaxBy()
    //    => source.AsValueEnumerable().MaxBy(static (x, state) => x + state, adjustment);


    //[Benchmark, BenchmarkCategory("MaxBy")]
    //public int HonkPerf_MaxBy()
    //    => source.ToRefLinq().MaxBy(static (x, state) => x + state, adjustment);


    //[Benchmark(Baseline = true), BenchmarkCategory("SelectManyValueEnumerable")]
    //public int Linq_SelectManyValueEnumerable()
    //{
    //    var state = adjustment;
    //    return source.SelectMany(x => System.Linq.Enumerable.Range(x + state, 2)).Sum();
    //}


    //[Benchmark, BenchmarkCategory("SelectManyValueEnumerable")]
    //public int ZLinq_SelectManyValueEnumerable()
    //{
    //    var state = adjustment;
    //    return source.AsValueEnumerable().SelectMany(x => ValueEnumerable.Range(x + state, 2)).Sum();
    //}


    //[Benchmark, BenchmarkCategory("SelectManyValueEnumerable")]
    //public int ZLinqWithContext_SelectManyValueEnumerable()
    //    => source.AsValueEnumerable().SelectMany(static (x, state) => ValueEnumerable.Range(x + state, 2), adjustment).Sum();


    //[Benchmark, BenchmarkCategory("SelectManyValueEnumerable")]
    //public int HonkPerf_SelectMany()
    //    => source.ToRefLinq().SelectMany(static (x, state) => HonkEnumerable.Range(x + state, 2), adjustment).Sum();


    //[Benchmark(Baseline = true), BenchmarkCategory("TakeWhile")]
    //public int Linq_TakeWhile()
    //{
    //    var state = threshold;
    //    return source.TakeWhile(x => x < state).Sum();
    //}


    //[Benchmark, BenchmarkCategory("TakeWhile")]
    //public int ZLinq_TakeWhile()
    //{
    //    var state = threshold;
    //    return source.AsValueEnumerable().TakeWhile(x => x < state).Sum();
    //}


    //[Benchmark, BenchmarkCategory("TakeWhile")]
    //public int ZLinqWithContext_TakeWhile()
    //    => source.AsValueEnumerable().TakeWhile(static (x, state) => x < state, threshold).Sum();


    //[Benchmark, BenchmarkCategory("TakeWhile")]
    //public int HonkPerf_TakeWhile()
    //    => source.ToRefLinq().TakeWhile(static (x, state) => x < state, threshold).Sum();
}
