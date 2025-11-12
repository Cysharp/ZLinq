using Benchmark.ZLinq;using Benchmark.ZLinq;

using ZLinq;using ZLinq;



namespace Benchmark;namespace Benchmark;



[BenchmarkCategory(Categories.Methods.OfType, Categories.Methods.ToList)][BenchmarkCategory(Categories.Methods.OfType, Categories.Methods.ToList)]

public class OfTypeToListBenchmarkpublic class OfTypeToListBenchmark

{{

    private object[] mixedArray = default!;    private object[] mixedArray = default!;

    private List<object> mixedList = default!;    private List<object> mixedList = default!;



    [Params(100, 1000, 10000)]    [Params(100, 1000, 10000)]

    public int Count { get; set; }    public int Count { get; set; }



    [GlobalSetup]    [GlobalSetup]

    public void Setup()    public void Setup()

    {    {

        // Create mixed array with 50% ints and 50% strings        // Create mixed array with 50% ints and 50% strings

        mixedArray = new object[Count];        mixedArray = new object[Count];

        for (int i = 0; i < Count; i++)        for (int i = 0; i < Count; i++)

        {        {

            mixedArray[i] = i % 2 == 0 ? (object)i : i.ToString();            mixedArray[i] = i % 2 == 0 ? (object)i : i.ToString();

        }        }

        mixedList = new List<object>(mixedArray);        mixedList = new List<object>(mixedArray);

    }    }



    [Benchmark]    [Benchmark]

    public List<int> Array_OfType_ToList_SystemLinq()    public List<int> Array_OfType_ToList_SystemLinq()

    {    {

        return mixedArray.OfType<int>().ToList();        return mixedArray.OfType<int>().ToList();

    }    }



    [Benchmark]    [Benchmark]

    public List<int> Array_OfType_ToList_ZLinq()    public List<int> Array_OfType_ToList_ZLinq()

    {    {

        return mixedArray.AsValueEnumerable().OfType<int>().ToList();        return mixedArray.AsValueEnumerable().OfType<int>().ToList();

    }    }



    [Benchmark]    [Benchmark]

    public List<int> List_OfType_ToList_SystemLinq()    public List<int> List_OfType_ToList_SystemLinq()

    {    {

        return mixedList.OfType<int>().ToList();        return mixedList.OfType<int>().ToList();

    }    }



    [Benchmark]    [Benchmark]

    public List<int> List_OfType_ToList_ZLinq()    public List<int> List_OfType_ToList_ZLinq()

    {    {

        return mixedList.AsValueEnumerable().OfType<int>().ToList();        return mixedList.AsValueEnumerable().OfType<int>().ToList();

    }    }

}}

