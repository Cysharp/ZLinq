using System.Collections;
using ZLinq;

namespace Benchmark;

/// <summary>
/// Test collection that implements <see cref="IList{T}"/>.;
/// </summary>
public class TestCollection_IList<T> : IList<T>
{
    readonly List<T> source;

    public TestCollection_IList(IEnumerable<T> source)
    {
        this.source = source.ToList();
    }

    public T this[int index]
    {
        get => source[index];
        set => source[index] = value;
    }

    public int Count => source.Count;

    public bool IsReadOnly => false;

    public void Add(T item) => source.Add(item);

    public void Clear() => source.Clear();

    public bool Contains(T item) => source.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => source.CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator() => source.GetEnumerator();

    public int IndexOf(T item) => source.IndexOf(item);

    public void Insert(int index, T item) => source.Insert(index, item);

    public bool Remove(T item) => source.Remove(item);

    public void RemoveAt(int index) => source.RemoveAt(index);

    IEnumerator IEnumerable.GetEnumerator() => source.GetEnumerator();
}


















