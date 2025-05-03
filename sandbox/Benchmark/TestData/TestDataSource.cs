using System.Buffers;
using System.Collections.ObjectModel;
using System.Collections.Immutable;

namespace Benchmark;

/// <summary>
/// Helper class to provide collection test data.
/// </summary>
public class TestDataSource<T>
{
    readonly T[] ArrayItems;

    readonly Lazy<T[]> DefaultItems;
    readonly Lazy<List<T>> ListItems;

    readonly Lazy<IEnumerable<T>> EnumerableIReadOnlyListItems;
    readonly Lazy<IEnumerable<T>> EnumerableIListItems;

    readonly Lazy<ReadOnlyCollection<T>> ReadOnlyCollectionItems;
    readonly Lazy<ReadOnlySequence<T>> ReadOnlySequenceItems;
    readonly Lazy<ReadOnlyMemory<T>> ReadOnlyMemoryItems;

#if NET8_0_OR_GREATER
    readonly Lazy<ImmutableArray<T>> ImmutableArrayItems;
#endif

    public T[] Empty => Array.Empty<T>();

    // Use T[] as default collection.
    // Because it's run faster than IEnumerable<T> when using ZLinq.
    public T[] Default => DefaultItems.Value;

#pragma warning disable format
    public T[]                   ArrayData                     => ArrayItems;                            // FromArray
    public List<T>               ListData                      => ListItems.Value;                       // FromList
    public ReadOnlyCollection<T> ReadOnlyCollectionData        => ReadOnlyCollectionItems.Value;         // FromEnumerable

    public IEnumerable<T>        EnumerableData                => ForceNotCollection(ArrayItems);        // FromEnumerable
    public IEnumerable<T>        EnumerableArrayData           => ArrayItems;                            // FromEnumerable
    public IEnumerable<T>        EnumerableListData            => ListItems.Value;                       // FromEnumerable
    public IEnumerable<T>        EnumerableIReadOnlyListData   => EnumerableIReadOnlyListItems.Value;    // FromEnumerable
    public IEnumerable<T>        EnumerableIListData           => EnumerableIListItems.Value;            // FromEnumerable

    public ReadOnlyMemory<T>     ReadOnlyMemoryData            => ReadOnlyMemoryItems.Value;             // FromReadOnlySequence
    public ReadOnlySequence<T>   ReadOnlySequenceData          => ReadOnlySequenceItems.Value;           // FromMemory

#if NET8_0_OR_GREATER
    public ImmutableArray<T>     ImmutableArrayData            => ImmutableArrayItems.Value;             // FromImmutableArray
#endif

#if NET9_0_OR_GREATER
    public ReadOnlySpan<T> ReadOnlySpanData                    => ArrayItems;                            // FromSpan
#endif

#pragma warning restore format
    public int Length => ArrayItems.Length;

    public TestDataSource(T[] items)
    {
        ArrayItems = items;

        // Other collection is lazy initialized.
        DefaultItems = new(() => items);
        ListItems = new(() => items.ToList());
        ReadOnlyCollectionItems = new(() => new ReadOnlyCollection<T>(items));
        ReadOnlySequenceItems = new(() => new ReadOnlySequence<T>(items));
        ReadOnlyMemoryItems = new(() => new ReadOnlyMemory<T>(items));

        EnumerableIReadOnlyListItems = new(() => new Collection<T>(items));   // Use non List<T> derived class. to avoid List<T> optimization path.
        EnumerableIListItems = new(() => new TestCollection_IList<T>(items));

#if NET8_0_OR_GREATER
        ImmutableArrayItems = new(() => items.ToImmutableArray());
#endif
    }

    public TestDataSource(int length)
        : this(TestDataGenerator.GetArray<T>(length))
    {
    }

    static IEnumerable<TItem> ForceNotCollection<TItem>(IEnumerable<TItem> source)
    {
        foreach (var item in source)
            yield return item;
    }
}
