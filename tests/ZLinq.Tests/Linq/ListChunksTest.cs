namespace ZLinq.Tests.Linq;

// The List<T> specialized operators read the list in chunks on netstandard (tested through net48 and net6.0,
// which consume the netstandard2.0 and netstandard2.1 builds), and through the indexer when the list has 48 elements or fewer.
// These tests cover lengths around the threshold (48) and the chunk size (512) so that both paths and every chunk boundary are exercised.
public class ListChunksTest
{
    public static TheoryData<int> Lengths => new() { 0, 1, 47, 48, 49, 511, 512, 513, 1024, 1500 };

    // Non-monotonic values, so that an element read from a wrong offset is not mistaken for the right one.
    static List<int> CreateList(int length) => Enumerable.Range(0, length).Select(x => x * 7919 % 1009).ToList();

    /// <summary>
    /// Ensures that Count with a predicate on List&lt;T&gt; visits every element exactly once across chunk boundaries.
    /// </summary>
    [Theory]
    [MemberData(nameof(Lengths))]
    public void CountWithPredicate_CountsAllChunks(int length)
    {
        var list = CreateList(length);

        list.AsValueEnumerable().Count(x => x % 3 == 0).ShouldBe(list.Count(x => x % 3 == 0));
    }

    /// <summary>
    /// Ensures that Where().Count() on List&lt;T&gt; visits every element exactly once across chunk boundaries.
    /// </summary>
    [Theory]
    [MemberData(nameof(Lengths))]
    public void WhereCount_CountsAllChunks(int length)
    {
        var list = CreateList(length);

        list.AsValueEnumerable().Where(x => x % 3 == 0).Count().ShouldBe(list.Where(x => x % 3 == 0).Count());
    }

    /// <summary>
    /// Ensures that Select().ToList() on List&lt;T&gt; writes every element to the right position across chunk boundaries.
    /// </summary>
    [Theory]
    [MemberData(nameof(Lengths))]
    public void SelectToList_PreservesOrderAcrossChunks(int length)
    {
        var list = CreateList(length);

        list.AsValueEnumerable().Select(x => x * 2).ToList().ShouldBe(list.Select(x => x * 2).ToList());
    }

    /// <summary>
    /// Ensures that Where().ToArray() on List&lt;T&gt; keeps the matching elements in order across chunk boundaries.
    /// </summary>
    [Theory]
    [MemberData(nameof(Lengths))]
    public void WhereToArray_PreservesOrderAcrossChunks(int length)
    {
        var list = CreateList(length);

        list.AsValueEnumerable().Where(x => x % 3 == 0).ToArray().ShouldBe(list.Where(x => x % 3 == 0).ToArray());
    }

    /// <summary>
    /// Ensures that Where().Select().ToArray() on List&lt;T&gt; keeps the projected elements in order across chunk boundaries.
    /// </summary>
    [Theory]
    [MemberData(nameof(Lengths))]
    public void WhereSelectToArray_PreservesOrderAcrossChunks(int length)
    {
        var list = CreateList(length);

        list.AsValueEnumerable().Where(x => x % 3 == 0).Select(x => x * 2).ToArray()
            .ShouldBe(list.Where(x => x % 3 == 0).Select(x => x * 2).ToArray());
    }

    /// <summary>
    /// Ensures that copying a range of List&lt;T&gt; (TryCopyTo) copies the right elements to the right positions,
    /// both for List&lt;T&gt; itself and for List&lt;T&gt; seen as IEnumerable&lt;T&gt;.
    /// </summary>
    [Theory]
    [MemberData(nameof(Lengths))]
    public void CopyRange_CopiesTheRightElements(int length)
    {
        var list = CreateList(length);
        IEnumerable<int> enumerable = list;

        var skip = length / 3;
        var take = length - skip - 1;

        list.AsValueEnumerable().Skip(skip).Take(take).ToArray().ShouldBe(list.Skip(skip).Take(take).ToArray());
        enumerable.AsValueEnumerable().Skip(skip).Take(take).ToArray().ShouldBe(enumerable.Skip(skip).Take(take).ToArray());
    }

    /// <summary>
    /// Ensures that copying a range of exactly the threshold (48), one more than the threshold, one chunk (512),
    /// one chunk plus one element, and two chunks plus one element copies the right elements,
    /// so that the switch from the indexer to chunks and a last chunk of a single element are exercised.
    /// </summary>
    [Theory]
    [InlineData(48)]
    [InlineData(49)]
    [InlineData(512)]
    [InlineData(513)]
    [InlineData(1025)]
    public void CopyRange_AtThresholdAndChunkBoundaries_CopiesTheRightElements(int count)
    {
        var list = CreateList(count + 2);
        IEnumerable<int> enumerable = list;

        list.AsValueEnumerable().Skip(1).Take(count).ToArray().ShouldBe(list.Skip(1).Take(count).ToArray());
        enumerable.AsValueEnumerable().Skip(1).Take(count).ToArray().ShouldBe(enumerable.Skip(1).Take(count).ToArray());
    }

    /// <summary>
    /// Ensures that the single element reads (First, Last, ElementAt), which copy one element, return the right element,
    /// and that FirstOrDefault returns the default value for an empty list.
    /// </summary>
    [Theory]
    [MemberData(nameof(Lengths))]
    public void SingleElementReads_ReturnTheRightElement(int length)
    {
        var list = CreateList(length);
        if (length == 0)
        {
            list.AsValueEnumerable().FirstOrDefault(-1).ShouldBe(-1);
            return;
        }

        list.AsValueEnumerable().First().ShouldBe(list.First());
        list.AsValueEnumerable().Last().ShouldBe(list.Last());
        list.AsValueEnumerable().ElementAt(length / 2).ShouldBe(list.ElementAt(length / 2));
    }

    /// <summary>
    /// Ensures that the range copies of Select over List&lt;T&gt; (ListSelect.TryCopyTo, used by ToArray, ElementAt, Last,
    /// and Skip().Take()) apply the selector to the right elements and write them to the right positions.
    /// </summary>
    [Theory]
    [MemberData(nameof(Lengths))]
    public void SelectCopyRange_CopiesTheRightElements(int length)
    {
        var list = CreateList(length);

        list.AsValueEnumerable().Select(x => x * 2).ToArray().ShouldBe(list.Select(x => x * 2).ToArray());

        var skip = length / 3;
        var take = length - skip - 1;
        list.AsValueEnumerable().Select(x => x * 2).Skip(skip).Take(take).ToArray()
            .ShouldBe(list.Select(x => x * 2).Skip(skip).Take(take).ToArray());

        if (length > 0)
        {
            list.AsValueEnumerable().Select(x => x * 2).ElementAt(length / 2).ShouldBe(list[length / 2] * 2);
            list.AsValueEnumerable().Select(x => x * 2).Last().ShouldBe(list[length - 1] * 2);
        }
    }

    /// <summary>
    /// Ensures that the chunked read also works for reference type elements, including null.
    /// </summary>
    [Theory]
    [MemberData(nameof(Lengths))]
    public void ReferenceTypeElements_AreReadAcrossChunks(int length)
    {
        var list = Enumerable.Range(0, length).Select(x => x % 5 == 0 ? null : x.ToString()).ToList();

        list.AsValueEnumerable().Count(x => x == null).ShouldBe(list.Count(x => x == null));
        list.AsValueEnumerable().Where(x => x != null).Count().ShouldBe(list.Where(x => x != null).Count());
        list.AsValueEnumerable().Select(x => x?.Length ?? -1).ToList().ShouldBe(list.Select(x => x?.Length ?? -1).ToList());
        list.AsValueEnumerable().Where(x => x != null).ToArray().ShouldBe(list.Where(x => x != null).ToArray());
        list.AsValueEnumerable().Skip(1).ToArray().ShouldBe(list.Skip(1).ToArray());
    }

    /// <summary>
    /// Ensures that the operators read only the elements that existed when the read started, when the predicate or selector
    /// adds elements to the list, on both the indexer path and the chunked path.
    /// A bound that follows the growing count would never terminate, and Select().ToList() would write past the end of its result.
    /// </summary>
    [Theory]
    [MemberData(nameof(Lengths))]
    public void ListGrowsDuringRead_ReadsOnlyTheOriginalElements(int length)
    {
        var expected = CreateList(length);

        var list = CreateList(length);
        list.AsValueEnumerable().Count(x => { list.Add(x); return x % 3 == 0; })
            .ShouldBe(expected.Count(x => x % 3 == 0));

        list = CreateList(length);
        list.AsValueEnumerable().Where(x => { list.Add(x); return x % 3 == 0; }).Count()
            .ShouldBe(expected.Count(x => x % 3 == 0));

        list = CreateList(length);
        list.AsValueEnumerable().Where(x => { list.Add(x); return x % 3 == 0; }).ToArray()
            .ShouldBe(expected.Where(x => x % 3 == 0).ToArray());

        list = CreateList(length);
        list.AsValueEnumerable().Where(x => { list.Add(x); return x % 3 == 0; }).Select(x => x * 2).ToArray()
            .ShouldBe(expected.Where(x => x % 3 == 0).Select(x => x * 2).ToArray());

        list = CreateList(length);
        list.AsValueEnumerable().Select(x => { list.Add(x); return x * 2; }).ToList()
            .ShouldBe(expected.Select(x => x * 2).ToList());

        list = CreateList(length);
        list.AsValueEnumerable().Select(x => { list.Add(x); return x * 2; }).JoinToString(',')
            .ShouldBe(string.Join(",", expected.Select(x => x * 2)));
    }

    /// <summary>
    /// Ensures that Count with a predicate stops at the current count when the predicate removes elements from the list
    /// on netstandard, instead of reading beyond the end of the list.
    /// On .NET 8 or later, the span over the backing array, whose length is fixed when the read starts, is read to the end.
    /// </summary>
    [Fact]
    public void ListShrinksDuringCount_DoesNotReadBeyondCount()
    {
        var list = Enumerable.Range(0, 1500).ToList();
        var visited = 0;

        Should.NotThrow(() => list.AsValueEnumerable().Count(x =>
        {
            visited++;
            if (x == 0)
            {
                list.RemoveRange(1000, 500);
            }
            return true;
        }));

#if NET8_0_OR_GREATER
        visited.ShouldBe(1500);
#else
        visited.ShouldBe(1000);
#endif
    }

    /// <summary>
    /// Ensures that a nested chunked read (a selector that reads another list with Count) uses its own buffer,
    /// so that neither the outer nor the inner read sees the other's elements.
    /// </summary>
    [Fact]
    public void NestedChunkedRead_UsesItsOwnBuffer()
    {
        var outer = Enumerable.Range(0, 1500).ToList();
        var inner = Enumerable.Range(0, 1000).ToList();

        var actual = outer.AsValueEnumerable().Select(x => x + inner.AsValueEnumerable().Count(y => y < 10)).ToList();

        actual.ShouldBe(outer.Select(x => x + 10).ToList());
    }
}
