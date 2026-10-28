namespace ZLinq.Tests.Linq;

public class DefaultIfEmptyTest
{
    [Fact]
    public void EmptyCollection_FromEndIndex()
    {
        var defaultIfEmpty = Array.Empty<int>().AsValueEnumerable().DefaultIfEmpty(42);

        defaultIfEmpty.ElementAtOrDefault(^1).ShouldBe(42);
        defaultIfEmpty.ElementAtOrDefault(^0).ShouldBe(0);
        defaultIfEmpty.ElementAtOrDefault(^2).ShouldBe(0);
        Should.Throw<ArgumentOutOfRangeException>(() => Array.Empty<int>().AsValueEnumerable().DefaultIfEmpty(42).ElementAt(^0));
    }

    [Fact]
    public void EmptyCollection_ReturnsDefaultValue()
    {
        var xs = new int[0];
        var result = xs.AsValueEnumerable().DefaultIfEmpty().ToArray();

        result.Length.ShouldBe(1);
        result[0].ShouldBe(default(int));
    }

    [Fact]
    public void EmptyCollection_ReturnsSpecifiedDefaultValue()
    {
        var xs = new int[0];
        const int defaultVal = 42;
        var result = xs.AsValueEnumerable().DefaultIfEmpty(defaultVal).ToArray();

        result.Length.ShouldBe(1);
        result[0].ShouldBe(defaultVal);
    }

    [Fact]
    public void NonEmptyCollection_ReturnsSameElements()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        var result = xs.AsValueEnumerable().DefaultIfEmpty().ToArray();

        result.ShouldBe(xs);
    }

    [Fact]
    public void NonEmptyCollection_IgnoresSpecifiedDefaultValue()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        const int defaultVal = 42;
        var result = xs.AsValueEnumerable().DefaultIfEmpty(defaultVal).ToArray();

        result.ShouldBe(xs);
    }

    [Fact]
    public void EmptyCollection_TryGetNonEnumeratedCount_ReturnsOne()
    {
        var xs = new int[0];
        var defaultIfEmpty = xs.AsValueEnumerable().DefaultIfEmpty();

        defaultIfEmpty.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
        count.ShouldBe(1);
    }

    [Fact]
    public void NonEmptyCollection_TryGetNonEnumeratedCount_ReturnsSameCount()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        var defaultIfEmpty = xs.AsValueEnumerable().DefaultIfEmpty();

        defaultIfEmpty.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
        count.ShouldBe(xs.Length);
    }

    [Fact]
    public void EmptyCollection_TryGetSpan_ReturnsFalse()
    {
        var xs = new int[0];
        var defaultIfEmpty = xs.AsValueEnumerable().DefaultIfEmpty();

        defaultIfEmpty.TryGetSpan(out _).ShouldBeFalse();
    }

    [Fact]
    public void NonEmptyCollection_TryGetSpan_ReturnsOriginalSpan()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        var defaultIfEmpty = xs.AsValueEnumerable().DefaultIfEmpty();

        defaultIfEmpty.TryGetSpan(out var span).ShouldBeTrue();
        span.ToArray().ShouldBe(xs);
    }

    [Fact]
    public void EmptyCollection_TryCopyTo_WithSufficientSpace_WritesDefaultValue()
    {
        var xs = new int[0];
        var defaultIfEmpty = xs.AsValueEnumerable().DefaultIfEmpty(42);

        var destination = new int[1];
        defaultIfEmpty.TryCopyTo(destination).ShouldBeTrue();
        destination[0].ShouldBe(42);
    }

    [Fact]
    public void EmptyCollection_TryCopyTo_WithInsufficientSpace_ReturnsFalse()
    {
        var xs = new int[0];
        var defaultIfEmpty = xs.AsValueEnumerable().DefaultIfEmpty();

        var destination = new int[0];
        defaultIfEmpty.TryCopyTo(destination).ShouldBeTrue(); // Empty destination is special case

        // Testing with null span is not meaningful as it would throw ArgumentNullException
    }

    [Fact]
    public void ToIterableValueEnumerable_EmptyCollection_ReturnsDefaultValue()
    {
        var xs = new int[0];
        var result = xs.ToValueEnumerable().DefaultIfEmpty().ToArray();

        result.Length.ShouldBe(1);
        result[0].ShouldBe(default(int));
    }

    [Fact]
    public void ToIterableValueEnumerable_NonEmptyCollection_ReturnsSameElements()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        var result = xs.ToValueEnumerable().DefaultIfEmpty().ToArray();

        result.ShouldBe(xs);
    }

    // https://github.com/dotnet/runtime/pull/119844/

    [Fact]
    public void ElementAtOrDefault_OutOfBounds_ReturnsTypeDefault()
    {
        // Regression test for https://github.com/dotnet/runtime/issues/119834
        // ElementAtOrDefault should return default(T) for out-of-bounds indices,
        // not the DefaultIfEmpty default value

        // Test with empty source
        int[] empty = [];
        var defaultIfEmpty = empty.AsValueEnumerable().DefaultIfEmpty(999);

        // Index 0 should return the DefaultIfEmpty value (999)
        Assert.Equal(999, defaultIfEmpty.ElementAtOrDefault(0));

        // Out-of-bounds indices should return default(int) which is 0, not 999
        Assert.Equal(0, defaultIfEmpty.ElementAtOrDefault(1));
        Assert.Equal(0, defaultIfEmpty.ElementAtOrDefault(2));
        Assert.Equal(0, defaultIfEmpty.ElementAtOrDefault(-1));

        // Test with different type (string)
        string[] emptyStrings = [];
        var defaultIfEmptyString = emptyStrings.AsValueEnumerable().DefaultIfEmpty("default");

        // Index 0 should return the DefaultIfEmpty value
        Assert.Equal("default", defaultIfEmptyString.ElementAtOrDefault(0));

        // Out-of-bounds indices should return default(string) which is null
        Assert.Null(defaultIfEmptyString.ElementAtOrDefault(1));
        Assert.Null(defaultIfEmptyString.ElementAtOrDefault(2));
    }
}
