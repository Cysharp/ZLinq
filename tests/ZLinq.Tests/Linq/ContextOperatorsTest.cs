namespace ZLinq.Tests.Linq;

public class ContextOperatorsTest
{
    [Fact]
    public void SelectAndWhere_UseContext()
    {
        var source = new[] { 1, 2, 3, 4, 5 };
        var multiplier = 4;
        var minimum = 3;
        var increment = 10;

        var selected = source.AsValueEnumerable()
            .Select(static (value, context) => value * context, multiplier)
            .ToArray();
        selected.ShouldBe(new[] { 4, 8, 12, 16, 20 });

        var filteredAndSelected = source.AsValueEnumerable()
            .Where(static (value, context) => value > context, minimum)
            .Select(static (value, context) => value + context, increment)
            .ToArray();
        filteredAndSelected.ShouldBe(new[] { 14, 15 });
    }

    [Fact]
    public void PredicateOperators_UseContext()
    {
        var source = new[] { 1, 2, 3, 4, 5 }.AsValueEnumerable();
        var threshold = 3;

        source.All(static (x, state) => x <= state, threshold).ShouldBeFalse();
        source.Any(static (x, state) => x == state, threshold).ShouldBeTrue();
        source.Count(static (x, state) => x > state, threshold).ShouldBe(2);
        source.LongCount(static (x, state) => x > state, threshold).ShouldBe(2L);
    }

    [Fact]
    public void AggregateOverloads_UseContext()
    {
        var source = new[] { 1, 2, 3 }.AsValueEnumerable();
        var adjustment = 2;

        source.Aggregate(static (acc, x, state) => acc + x + state, adjustment).ShouldBe(10);
        source.Aggregate(10, static (acc, x, state) => acc + x + state, adjustment).ShouldBe(22);
        source.Aggregate(0, static (acc, x, state) => acc + x, static (acc, state) => acc * state, adjustment).ShouldBe(12);
    }

    [Fact]
    public void MinByAndMaxBy_UseContext()
    {
        var source = new[] { "pear", "fig", "watermelon" }.AsValueEnumerable();
        var adjustment = 10;

        source.MinBy(static (x, state) => x.Length + state, adjustment).ShouldBe("fig");
        source.MaxBy(static (x, state) => x.Length + state, adjustment).ShouldBe("watermelon");
    }

    [Fact]
    public void TakeWhile_UsesContext()
    {
        var source = new[] { 1, 2, 3, 4, 5 }.AsValueEnumerable();
        var limit = 4;

        source.TakeWhile(static (x, state) => x < state, limit).ToArray().ShouldBe(new[] { 1, 2, 3 });
    }

    [Fact]
    public void SelectMany_UsesContextForValueEnumerableAndIEnumerableSelectors()
    {
        var source = new[] { 1, 2 }.AsValueEnumerable();
        var multiplier = 10;

        var valueEnumerableResult = source
            .SelectMany(static (x, state) => ValueEnumerable.Range(1, x).Select(y => y * state), multiplier)
            .ToArray();
        valueEnumerableResult.ShouldBe(new[] { 10, 10, 20 });

        var enumerableResult = source
            .SelectMany(static (x, state) => new[] { x + state }, multiplier)
            .ToArray();
        enumerableResult.ShouldBe(new[] { 11, 12 });
    }

    [Fact]
    public void PredicateTerminals_UseContextAndPreserveDefaultAndThrowBehavior()
    {
        var source = new[] { 1, 2, 3, 4, 5 }.AsValueEnumerable();
        var threshold = 3;

        source.First(static (x, state) => x > state, threshold).ShouldBe(4);
        source.FirstOrDefault(static (x, state) => x > 10, threshold).ShouldBe(0);
        source.FirstOrDefault(static (x, state) => x > 10, threshold, -1).ShouldBe(-1);
        source.Last(static (x, state) => x > state, threshold).ShouldBe(5);
        source.LastOrDefault(static (x, state) => x > 10, threshold).ShouldBe(0);
        source.LastOrDefault(static (x, state) => x > 10, threshold, -1).ShouldBe(-1);
        source.Single(static (x, state) => x == state, threshold).ShouldBe(3);
        source.SingleOrDefault(static (x, state) => x > 10, threshold).ShouldBe(0);
        source.SingleOrDefault(static (x, state) => x > 10, threshold, -1).ShouldBe(-1);
        Should.Throw<InvalidOperationException>(() => SingleWithContext(new[] { 1, 2, 3, 4, 5 }, threshold));
    }

    static int SingleWithContext(int[] source, int threshold)
        => source.AsValueEnumerable().Single(static (x, state) => x > state, threshold);
}
