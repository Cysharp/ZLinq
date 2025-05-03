using System.Buffers;
using ZLinq;

namespace Benchmark.ZLinq;

// System.Memory's CopyTo extension methods accept T[] only. It don't accept IEnumerable<T>.
#if !USE_SYSTEM_LINQ

[BenchmarkCategory(Categories.Methods.CopyTo)]
[BenchmarkCategory(Categories.Filters.ZLinqOnly)]
public partial class CopyToBenchmark<T> : EnumerableBenchmarkBase_WithBasicTypes<T>
{
    private T[] buffer = default!;

    protected override void Setup()
    {
        buffer = ArrayPool<T>.Shared.Rent(source.Length);
    }

    protected override void Cleanup()
    {
        ArrayPool<T>.Shared.Return(buffer);
    }


    [Benchmark(Baseline = true)]
    [BenchmarkCategory(Categories.From.Default)]
    public void CopyTo()
    {
        source.Default
              .AsValueEnumerable()
              .CopyTo(buffer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.List)]
    public void CopyTo_FromList()
    {
        source.ListData
              .AsValueEnumerable()
              .CopyTo(buffer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.Enumerable)]
    public void CopyTo_FromEnumerable()
    {
        source.EnumerableData
              .AsValueEnumerable()
              .CopyTo(buffer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.ReadOnlyCollection)]
    public void CopyTo_FromReadOnlyCollection()
    {
        source.ReadOnlyCollectionData
              .AsValueEnumerable()
              .CopyTo(buffer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.ReadOnlyMemory)]
    public void CopyTo_FromReadOnlyMemory()
    {
        source.ReadOnlyMemoryData
              .AsValueEnumerable()
              .CopyTo(buffer);
    }

    [Benchmark]
    [BenchmarkCategory(Categories.From.ReadOnlySequence)]
    public void CopyTo_FromReadOnlySequence()
    {
        source.ReadOnlySequenceData
              .AsValueEnumerable()
              .CopyTo(buffer);
    }

#if NET9_0_OR_GREATER
    [Benchmark]
    [BenchmarkCategory(Categories.From.ReadOnlySpan)]
    [BenchmarkCategory(Categories.Filters.NET9_0_OR_GREATER)]
    public void CopyTo_FromReadOnlySpan()
    {
        source.ReadOnlySpanData
              .AsValueEnumerable()
              .CopyTo(buffer);
    }
#endif
}
#endif
