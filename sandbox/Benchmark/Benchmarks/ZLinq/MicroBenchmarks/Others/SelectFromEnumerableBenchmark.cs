using ZLinq;

namespace Benchmark.ZLinq;

[BenchmarkCategory(Categories.Methods.Select)]
[GenericTypeArguments(typeof(int))]
[GenericTypeArguments(typeof(string))]
public partial class SelectFromEnumerableBenchmark<T> : EnumerableBenchmarkBase<T>
{
    [Benchmark]
    [BenchmarkCategory(Categories.From.Default)]
    public void Select()
    {
        source.Default
              .AsValueEnumerable()
              .Select(x => x)
              .Consume(consumer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.Array)]
    public void Select_FromArray()
    {
        source.ArrayData
              .AsValueEnumerable()
              .Select(x => x)
              .Consume(consumer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.List)]
    public void Select_FromList()
    {
        source.ListData
              .AsValueEnumerable()
              .Select(x => x)
              .Consume(consumer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.Enumerable)]
    public void Select_FromEnumerable()
    {
        source.EnumerableData
              .AsValueEnumerable()
              .Select(x => x)
              .Consume(consumer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.EnumerableArray)]
    public void Select_FromEnumerable_Array()
    {
        source.EnumerableArrayData
              .AsValueEnumerable()
              .Select(x => x)
              .Consume(consumer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.EnumerableList)]
    public void Select_FromEnumerable_List()
    {
        source.EnumerableListData
              .AsValueEnumerable()
              .Select(x => x)
              .Consume(consumer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.EnumerableIReadOnlyList)]
    public void Select_FromEnumerable_IReadOnlyList()
    {
        source.EnumerableIReadOnlyListData
              .AsValueEnumerable()
              .Select(x => x)
              .Consume(consumer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.EnumerableIList)]
    public void Select_FromEnumerable_IList()
    {
        source.EnumerableIListData
              .AsValueEnumerable()
              .Select(x => x)
              .Consume(consumer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.ReadOnlyCollection)]
    public void Select_FromReadOnlyCollection()
    {
        source.ReadOnlyCollectionData
              .AsValueEnumerable()
              .Select(x => x)
              .Consume(consumer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.ReadOnlyMemory)]
    public void Select_FromReadOnlyMemory()
    {
        source.ReadOnlyMemoryData
              .AsValueEnumerable()
              .Select(x => x)
              .Consume(consumer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.ReadOnlySequence)]
    public void Select_FromReadOnlySequence()
    {
        source.ReadOnlySequenceData
              .AsValueEnumerable()
              .Select(x => x)
              .Consume(consumer);
    }

#if NET8_0_OR_GREATER
    [Benchmark]
    [BenchmarkCategory(Categories.From.ImmutableArray)]
    [BenchmarkCategory(Categories.Filters.NET8_0_OR_GREATER)]
    public void Select_FromImmutableArray()
    {
        source.ImmutableArrayData
              .AsValueEnumerable()
              .Select(x => x)
              .Consume(consumer);
    }
#endif

#if !USE_SYSTEM_LINQ && NET9_0_OR_GREATER
    [Benchmark]
    [BenchmarkCategory(Categories.From.ReadOnlySpan)]
    [BenchmarkCategory(Categories.Filters.ZLinqOnly)]
    [BenchmarkCategory(Categories.Filters.NET9_0_OR_GREATER)]
    public void Select_FromReadOnlySpan()
    {
        source.ReadOnlySpanData
              .AsValueEnumerable()
              .Select(x => x)
              .Consume(consumer);
    }
#endif
}
