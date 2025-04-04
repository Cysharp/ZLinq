﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using ZLinq;

namespace Benchmark;

// Based on: https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-9/
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class Net90OptimizedBenchmark
{
    int[] source = default!;

    [GlobalSetup]
    public void Setup()
    {
        source = Enumerable.Range(1, 1000)
                           .Select(_ => Random.Shared.Next(1, 1000))
                           .ToArray();
    }

    [Benchmark]
    [BenchmarkCategory(Categories.LINQ)]
    public int DistinctFirst_SystemLinq()
    {
        return source.Distinct()
                     .First();
    }

    [Benchmark]
    [BenchmarkCategory(Categories.LINQ)]
    public int AppendSelectLast_SystemLinq()
    {
        return source.Append(42)
                     .Select(x => x * 2)
                     .Last();
    }

    [Benchmark]
    [BenchmarkCategory(Categories.LINQ)]
    public int RangeReverseCount_SystemLinq()
    {
        return source.Reverse()
                     .Count();
    }

    [Benchmark]
    [BenchmarkCategory(Categories.LINQ)]
    public int DefaultIfEmptySelectElementAt_SystemLinq()
    {
        return source.DefaultIfEmpty()
                     .Select(i => i * 2)
                     .ElementAt(999);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.LINQ)]
    public int ListSkipTakeElementAt_SystemLinq()
    {
        return source.Skip(500)
                     .Take(100)
                     .ElementAt(99);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.LINQ)]
    public int RangeUnionFirst_SystemLinq()
    {
        return source.Union(Enumerable.Range(500, 1000))
                     .First();
    }

    [Benchmark]
    [BenchmarkCategory(Categories.LINQ)]
    public int SelectWhereSelectSum_SystemLinq()
    {
        return source.Select(i => i * 2)
                     .Where(i => i % 2 == 0)
                     .Select(i => i * 2)
                     .Sum();
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    public int ZDistinctFirst_ZLinq()
    {
        return source.AsValueEnumerable()
                     .Distinct()
                     .First();
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    public int ZAppendSelectLast_ZLinq()
    {
        return source.AsValueEnumerable()
                     .Append(42)
                     .Select(x => x * 2)
                     .Last();
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    public int ZRangeReverseCount_ZLinq()
    {
        return source.AsValueEnumerable()
                     .Reverse()
                     .Count();
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    public int ZDefaultIfEmptySelectElementAt_ZLinq()
    {
        return source.AsValueEnumerable()
                     .DefaultIfEmpty()
                     .Select(i => i * 2)
                     .ElementAt(999);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    public int ZListSkipTakeElementAt_ZLinq()
    {
        return source.AsValueEnumerable()
                     .Skip(500)
                     .Take(100)
                     .ElementAt(99);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    public int ZRangeUnionFirst_ZLinq()
    {
        return source.AsValueEnumerable()
                     .Union(Enumerable.Range(500, 1000))
                     .First();
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    public int ZRangeUnionFirst_ZLinqOptimized()
    {
        return source.AsValueEnumerable()
                     .Union(ValueEnumerable.Range(500, 1000))
                     .First();
    }

    [Benchmark]
    [BenchmarkCategory(Categories.ZLinq)]
    public int ZSelectWhereSelectSum_ZLinq()
    {
        return source.AsValueEnumerable()
                     .Select(i => i * 2)
                     .Where(i => i % 2 == 0)
                     .Select(i => i * 2).Sum();
    }
}
