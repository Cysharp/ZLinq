namespace ZLinq.Tests.Linq;

public class GroupingContainsTest
{
    [Fact]
    public void GroupByContainsOnlyPresentElements()
    {
        var source = new[] { 1, 2, 3 };
        var group = source.AsValueEnumerable().GroupBy(x => 0).Single();

        group.Contains(0).ShouldBeFalse();
        group.Contains(2).ShouldBeTrue();
        group.AsValueEnumerable().Contains(0).ShouldBeFalse();
    }

    [Fact]
    public void LookupContainsOnlyPresentElements()
    {
        var source = new[] { "a", "b", "c" };
        var group = source.AsValueEnumerable().ToLookup(x => 0)[0];

        group.Contains(null!).ShouldBeFalse();
        group.Contains("b").ShouldBeTrue();
    }

    [Fact]
    public void GroupContainingDefaultStillFindsIt()
    {
        var group = new[] { 0, 1, 2 }.AsValueEnumerable().GroupBy(x => 0).Single();

        group.Contains(0).ShouldBeTrue();
    }
}
