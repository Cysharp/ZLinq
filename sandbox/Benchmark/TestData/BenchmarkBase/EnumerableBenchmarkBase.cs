using ZLinq;

namespace Benchmark;

/// <summary>
/// Base class for benchmarks that use various enumerable types.
/// Benchmark targe types can be specified with GenericTypeArguments attribute on derived class.
/// </summary>
public abstract class EnumerableBenchmarkBase<T>
{
    // TODO: Dynamically adjust elements count based on benchmark type.
    //[Params([1_000, 10_000, 100_000, 1_000_000])]
    [Params([1_000_000])]
    public int N { get; set; }

    protected readonly Consumer consumer = new();

    protected TestDataSource<T> source = default!;

    // Elements that used by tests
    protected T? firstElement = default!;
    protected T? midElement = default!;
    protected T? lastElement = default!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        source = new TestDataSource<T>(length: N);

        firstElement = source.ArrayData.First();
        midElement = source.ArrayData.ElementAt(source.Length / 2);
        lastElement = source.ArrayData.Last();

        // Additional setup that defined by derived class
        Setup();
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        Cleanup();
    }

    /// <summary>
    /// Override this method to add logics to GlobalSetup.
    /// </summary>
    protected virtual void Setup() { }

    /// <summary>
    /// Override this method to add logics to GlobalCleanup.
    /// </summary>
    protected virtual void Cleanup() { }
}
