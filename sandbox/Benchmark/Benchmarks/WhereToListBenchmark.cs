using Benchmark.ZLinq;
using ZLinq;

namespace Benchmark;

[BenchmarkCategory(Categories.Methods.Where, Categories.Methods.ToList)]
public class WhereToListBenchmark
{
    private int[] array = default!;
    private List<int> list = default!;

    [Params(100, 1000, 10000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        array = Enumerable.Range(1, Count).ToArray();
        list = Enumerable.Range(1, Count).ToList();
    }

    [Benchmark]
    public List<int> Array_Where_ToList_SystemLinq()
    {
        return array.Where(x => x % 2 == 0).ToList();
    }

    [Benchmark]
    public List<int> Array_Where_ToList_ZLinq()
    {
        return array.AsValueEnumerable().Where(x => x % 2 == 0).ToList();
    }

    [Benchmark]
    public List<int> List_Where_ToList_SystemLinq()
    {
        return list.Where(x => x % 2 == 0).ToList();
    }

    [Benchmark]
    public List<int> List_Where_ToList_ZLinq()
    {
        return list.AsValueEnumerable().Where(x => x % 2 == 0).ToList();
    }

    [Benchmark]
    public List<string> Array_WhereSelect_ToList_SystemLinq()
    {
        return array.Where(x => x % 2 == 0).Select(x => x.ToString()).ToList();
    }

    [Benchmark]
    public List<string> Array_WhereSelect_ToList_ZLinq()
    {
        return array.AsValueEnumerable().Where(x => x % 2 == 0).Select(x => x.ToString()).ToList();
    }

    [Benchmark]
    public List<string> List_WhereSelect_ToList_SystemLinq()
    {
        return list.Where(x => x % 2 == 0).Select(x => x.ToString()).ToList();
    }

    [Benchmark]
    public List<string> List_WhereSelect_ToList_ZLinq()
    {
        return list.AsValueEnumerable().Where(x => x % 2 == 0).Select(x => x.ToString()).ToList();
    }
}
