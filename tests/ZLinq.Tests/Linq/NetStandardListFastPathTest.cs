namespace ZLinq.Tests.Linq;

// Tests for the List<T> specialized paths of netstandard (tested through net48 and net6.0,
// which consume the netstandard2.0 and netstandard2.1 builds):
// - JoinToString over List<string> copies the elements into a pooled array to use the fast path for strings.
// - ToArray/ToList/CopyTo use the bulk copies of the public List<T> API for List<T> and array sources.
// - The other operators that fill a List<T> write into a temporary buffer (FillCollection) that is cached per thread.
// On .NET 8 or later, the same tests pin down that both builds behave the same.
public class NetStandardListFastPathTest
{
    public static TheoryData<int> Lengths => new() { 0, 1, 2, 100, 1024 };

    public static TheoryData<int> CopyLengths => new() { 0, 1, 47, 48, 49, 1000 };

    /// <summary>
    /// Ensures that JoinToString over List&lt;string&gt; produces the same result as string.Join,
    /// including null elements and every separator length (empty, single char, multiple chars).
    /// </summary>
    [Theory]
    [MemberData(nameof(Lengths))]
    public void JoinToString_ListOfString_MatchesStringJoin(int length)
    {
        var list = Enumerable.Range(0, length).Select(x => x % 7 == 3 ? null : x.ToString()).ToList();

        list.AsValueEnumerable().JoinToString("").ShouldBe(string.Join("", list));
        list.AsValueEnumerable().JoinToString(',').ShouldBe(string.Join(",", list));
        list.AsValueEnumerable().JoinToString(", ").ShouldBe(string.Join(", ", list));
    }

    /// <summary>
    /// Ensures that CopyTo(List&lt;T&gt;) from a List&lt;T&gt; source or an array source replaces the whole content of
    /// a destination that already has elements, with the source elements in order.
    /// </summary>
    [Theory]
    [MemberData(nameof(CopyLengths))]
    public void CopyToList_FromListOrArraySource_ReplacesTheDestination(int length)
    {
        var source = Enumerable.Range(0, length).Select(x => x * 7919 % 1009).ToList();
        var array = source.ToArray();

        var destination = new List<int> { -1, -2, -3, -4, -5 };
        source.AsValueEnumerable().CopyTo(destination);
        destination.ShouldBe(source);

        destination = new List<int> { -1, -2, -3, -4, -5 };
        array.AsValueEnumerable().CopyTo(destination);
        destination.ShouldBe(source);
    }

    /// <summary>
    /// Ensures that CopyTo(List&lt;T&gt;) with the source list itself as the destination leaves the list empty on every build,
    /// because the destination is cleared before the source is read.
    /// </summary>
    [Theory]
    [MemberData(nameof(CopyLengths))]
    public void CopyToList_ToTheSourceItself_LeavesTheListEmpty(int length)
    {
        var list = Enumerable.Range(0, length).ToList();

        list.AsValueEnumerable().CopyTo(list);

        list.ShouldBeEmpty();
    }

    /// <summary>
    /// Ensures that ToList and ToArray from a List&lt;T&gt; source, and ToList from an array source,
    /// return a new instance with the same elements that does not share storage with the source.
    /// </summary>
    [Theory]
    [MemberData(nameof(CopyLengths))]
    public void ToListAndToArray_FromListOrArraySource_ReturnIndependentCopies(int length)
    {
        var list = Enumerable.Range(0, length).ToList();
        var array = list.ToArray();

        var listFromList = list.AsValueEnumerable().ToList();
        listFromList.ShouldNotBeSameAs(list);
        listFromList.ShouldBe(list);

        var arrayFromList = list.AsValueEnumerable().ToArray();
        arrayFromList.ShouldBe(array);

        var listFromArray = array.AsValueEnumerable().ToList();
        listFromArray.ShouldBe(array);

        if (length > 0)
        {
            listFromList[0] = -1;
            arrayFromList[0] = -1;
            listFromArray[0] = -1;

            list[0].ShouldBe(0);
            array[0].ShouldBe(0);
        }
    }

    public static TheoryData<int, int> NestedLengths => new()
    {
        { 100, 100 },
        { 100, 1000 },
        { 1000, 100 },
        { 1000, 1000 },
    };

    /// <summary>
    /// Ensures that a ToList nested in the selector of another ToList with the same element type does not share
    /// the temporary buffer of the outer ToList, for both the per-thread retained buffer (256 elements or fewer)
    /// and the buffer rented from ArrayPool.
    /// </summary>
    [Theory]
    [MemberData(nameof(NestedLengths))]
    public void NestedToList_DoesNotShareTheFillBuffer(int outerLength, int innerLength)
    {
        var outer = Enumerable.Range(0, outerLength).ToList();
        var inner = Enumerable.Range(0, innerLength).ToList();

        var actual = outer.AsValueEnumerable()
            .Select(x => x + inner.AsValueEnumerable().Select(y => -y).ToList()[x % innerLength])
            .ToList();

        actual.ShouldBe(outer.Select(x => x - x % innerLength).ToList());
    }

    /// <summary>
    /// Ensures that after a selector throws in the middle of ToList, the exception reaches the caller
    /// and the next ToList on the same thread still returns the right elements.
    /// </summary>
    [Theory]
    [InlineData(100)]
    [InlineData(1000)]
    public void ToList_AfterSelectorThrows_NextToListIsCorrect(int length)
    {
        var list = Enumerable.Range(0, length).ToList();

        Should.Throw<InvalidOperationException>(() =>
            list.AsValueEnumerable().Select(x => x == length / 2 ? throw new InvalidOperationException() : x).ToList());

        list.AsValueEnumerable().Select(x => x * 2).ToList().ShouldBe(list.Select(x => x * 2).ToList());
    }

    const int Marker = int.MinValue + 12345;

    // Leaves Marker in the temporary buffer that the next fill of `length` elements on this thread reuses:
    // the per-thread retained buffer for 256 elements or fewer, and an ArrayPool buffer for more.
    static void LeaveMarkerInFillBuffer(int length)
    {
        new int[length].AsValueEnumerable().Select(_ => Marker).ToList();
    }

    /// <summary>
    /// Ensures that when an enumerator yields fewer elements than its non-enumerated count, ToList and CopyTo(List&lt;T&gt;)
    /// fill the rest with default instead of exposing data left in the reused or pooled temporary buffer.
    /// </summary>
    [Theory]
    [InlineData(100)]
    [InlineData(1000)]
    public void Fill_WithFewerElementsThanCounted_DoesNotExposeStaleBufferData(int length)
    {
        var actual = length * 3 / 5;
        var expected = Enumerable.Range(1, actual).Concat(Enumerable.Repeat(0, length - actual)).ToList();

        LeaveMarkerInFillBuffer(length);
        new ValueEnumerable<OverCountedEnumerator, int>(new(length, actual)).ToList().ShouldBe(expected);

        LeaveMarkerInFillBuffer(length);
        var destination = new List<int>();
        new ValueEnumerable<OverCountedEnumerator, int>(new(length, actual)).CopyTo(destination);
        destination.ShouldBe(expected);
    }

    /// <summary>
    /// Ensures that when the selector of Select().ToList() shrinks the source list during a chunked read,
    /// the elements read before the shrink are kept and no data left in the pooled temporary buffer reaches the result.
    /// </summary>
    [Fact]
    public void SelectToList_WhenSelectorShrinksTheList_DoesNotExposeStaleBufferData()
    {
        const int length = 1000;
        const int remaining = 600;
        var list = Enumerable.Range(0, length).ToList();

        LeaveMarkerInFillBuffer(length);
        var result = list.AsValueEnumerable().Select(x =>
        {
            if (x == 0)
            {
                list.RemoveRange(remaining, length - remaining);
            }
            return x * 2;
        }).ToList();

        result.Count.ShouldBe(length);
        result.Take(remaining).ShouldBe(Enumerable.Range(0, remaining).Select(x => x * 2));
        result.ShouldNotContain(Marker);
    }

    // Reports `reportedCount` as its non-enumerated count but yields only 1, 2, ..., `yieldedCount`, violating the contract,
    // to make the fill paths write fewer elements than they reserved.
    struct OverCountedEnumerator(int reportedCount, int yieldedCount) : IValueEnumerator<int>
    {
        int index;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = reportedCount;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<int> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<int> destination, Index offset) => false;

        public bool TryGetNext(out int current)
        {
            if (index < yieldedCount)
            {
                current = ++index;
                return true;
            }

            current = default;
            return false;
        }

        public void Dispose()
        {
        }
    }

    /// <summary>
    /// Ensures that consecutive ToList calls whose sizes switch between the per-thread retained buffer (256 elements or fewer)
    /// and the buffer rented from ArrayPool, and that grow the retained buffer, return the right elements every time,
    /// with no element left over from a previous call.
    /// </summary>
    [Fact]
    public void ToList_WithAlternatingSizes_ReturnsTheRightElements()
    {
        foreach (var length in new[] { 300, 10, 256, 257, 16, 100, 0, 1 })
        {
            var list = Enumerable.Range(0, length).Select(x => x % 5 == 0 ? null : x.ToString()).ToList();

            list.AsValueEnumerable().Select(x => x + "!").ToList().ShouldBe(list.Select(x => x + "!").ToList());
        }
    }
}
