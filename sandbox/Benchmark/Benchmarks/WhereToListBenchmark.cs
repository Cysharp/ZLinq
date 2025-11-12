using Benchmark.ZLinq;using Benchmark.ZLinq;

using ZLinq;using ZLinq;



namespace Benchmark;namespace Benchmark;



[BenchmarkCategory(Categories.Methods.Where, Categories.Methods.ToList)][BenchmarkCategory(Categories.Methods.Where, Categories.Methods.ToList)]

public class WhereToListBenchmarkpublic class WhereToListBenchmark

{{

    private int[] array = default!;    private int[] array = default!;

    private List<int> list = default!;    private List<int> list = default!;



    [Params(100, 1000, 10000)]    [Params(100, 1000, 10000)]

    public int Count { get; set; }    public int Count { get; set; }



    [GlobalSetup]    [GlobalSetup]

    public void Setup()    public void Setup()

    {    {

        array = Enumerable.Range(1, Count).ToArray();        array = Enumerable.Range(1, Count).ToArray();

        list = Enumerable.Range(1, Count).ToList();        list = Enumerable.Range(1, Count).ToList();

    }    }



    [Benchmark]    [Benchmark]

    public List<int> Array_Where_ToList_SystemLinq()    public List<int> Array_Where_ToList_SystemLinq()

    {    {

        return array.Where(x => x % 2 == 0).ToList();        return array.Where(x => x % 2 == 0).ToList();

    }    }



    [Benchmark]    [Benchmark]

    public List<int> Array_Where_ToList_ZLinq()    public List<int> Array_Where_ToList_ZLinq()

    {    {

        return array.AsValueEnumerable().Where(x => x % 2 == 0).ToList();        return array.AsValueEnumerable().Where(x => x % 2 == 0).ToList();

    }    }



    [Benchmark]    [Benchmark]

    public List<int> List_Where_ToList_SystemLinq()    public List<int> List_Where_ToList_SystemLinq()

    {    {

        return list.Where(x => x % 2 == 0).ToList();        return list.Where(x => x % 2 == 0).ToList();

    }    }



    [Benchmark]    [Benchmark]

    public List<int> List_Where_ToList_ZLinq()    public List<int> List_Where_ToList_ZLinq()

    {    {

        return list.AsValueEnumerable().Where(x => x % 2 == 0).ToList();        return list.AsValueEnumerable().Where(x => x % 2 == 0).ToList();

    }    }



    [Benchmark]    [Benchmark]

    public List<string> Array_WhereSelect_ToList_SystemLinq()    public List<string> Array_WhereSelect_ToList_SystemLinq()

    {    {

        return array.Where(x => x % 2 == 0).Select(x => x.ToString()).ToList();        return array.Where(x => x % 2 == 0).Select(x => x.ToString()).ToList();

    }    }



    [Benchmark]    [Benchmark]

    public List<string> Array_WhereSelect_ToList_ZLinq()    public List<string> Array_WhereSelect_ToList_ZLinq()

    {    {

        return array.AsValueEnumerable().Where(x => x % 2 == 0).Select(x => x.ToString()).ToList();        return array.AsValueEnumerable().Where(x => x % 2 == 0).Select(x => x.ToString()).ToList();

    }    }



    [Benchmark]    [Benchmark]

    public List<string> List_WhereSelect_ToList_SystemLinq()    public List<string> List_WhereSelect_ToList_SystemLinq()

    {    {

        return list.Where(x => x % 2 == 0).Select(x => x.ToString()).ToList();        return list.Where(x => x % 2 == 0).Select(x => x.ToString()).ToList();

    }    }



    [Benchmark]    [Benchmark]

    public List<string> List_WhereSelect_ToList_ZLinq()    public List<string> List_WhereSelect_ToList_ZLinq()

    {    {

        return list.AsValueEnumerable().Where(x => x % 2 == 0).Select(x => x.ToString()).ToList();        return list.AsValueEnumerable().Where(x => x % 2 == 0).Select(x => x.ToString()).ToList();

    }    }

}}

