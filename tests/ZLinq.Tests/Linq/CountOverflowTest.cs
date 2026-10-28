using System.Collections;

namespace ZLinq.Tests.Linq;

// https://github.com/Cysharp/ZLinq/issues/242
public class CountOverflowTest
{
    // reports int.MaxValue as Count without allocating, enumerates nothing.
    sealed class MaxCountCollection : ICollection<int>
    {
        public int Count => int.MaxValue;
        public bool IsReadOnly => true;
        public void Add(int item) => throw new NotSupportedException();
        public void Clear() => throw new NotSupportedException();
        public bool Contains(int item) => false;
        public void CopyTo(int[] array, int arrayIndex) { }
        public bool Remove(int item) => throw new NotSupportedException();
        public IEnumerator<int> GetEnumerator() => Enumerable.Empty<int>().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Fact]
    public void CountThrowsOverflow()
    {
        var source = new MaxCountCollection();

        Should.Throw<OverflowException>(() => source.AsValueEnumerable().Append(1).Count());
        Should.Throw<OverflowException>(() => source.AsValueEnumerable().Prepend(1).Count());
        Should.Throw<OverflowException>(() => source.AsValueEnumerable().Append(1).Prepend(2).Count());
    }

    [Fact]
    public void LongCountFallsBackToEnumeration()
    {
        var source = new MaxCountCollection();

        // the count exceeds int.MaxValue, so LongCount enumerates the source (which yields nothing) like System.Linq.
        source.AsValueEnumerable().Append(1).LongCount().ShouldBe(1);
        source.AsValueEnumerable().Prepend(1).LongCount().ShouldBe(1);
        source.AsValueEnumerable().Concat(new[] { 1 }).LongCount().ShouldBe(1);
    }
}
