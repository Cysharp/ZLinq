namespace ZLinq.Tests.Linq;

public class AppendTest
{
    [Fact]
    public void AppendToEmpty()
    {
        var xs = Array.Empty<int>();
        var element = 42;

        // Compare with standard LINQ
        var expected = xs.Append(element).ToArray();

        // Test with ZLinq
        var actual = xs.AsValueEnumerable().Append(element).ToArray();

        actual.ShouldBe(expected);
    }

    [Fact]
    public void AppendToNonEmpty()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };
        var element = 42;

        // Compare with standard LINQ
        var expected = xs.Append(element).ToArray();

        // Test with ZLinq
        var actual = xs.AsValueEnumerable().Append(element).ToArray();

        actual.ShouldBe(expected);
    }

    [Fact]
    public void TryGetNonEnumeratedCount()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };
        var element = 42;

        var enumerable = xs.AsValueEnumerable().Append(element);

        // Should return true and correct count
        enumerable.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
        count.ShouldBe(xs.Length + 1);

        // Test with empty source
        var emptyEnumerable = Array.Empty<int>().AsValueEnumerable().Append(element);
        emptyEnumerable.TryGetNonEnumeratedCount(out var emptyCount).ShouldBeTrue();
        emptyCount.ShouldBe(1);
    }

    [Fact]
    public void TryCopyTo()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };
        var element = 42;

        var enumerable = xs.AsValueEnumerable().Append(element);

        // Test with sufficient space
        var destination = new int[xs.Length + 1];
        enumerable.TryCopyTo(destination).ShouldBeTrue();

        var expected = xs.Append(element).ToArray();
        destination.ShouldBe(expected);

        var smallDestination = new int[xs.Length];
        enumerable.TryCopyTo(smallDestination).ShouldBeTrue();
        smallDestination.ShouldBe([1, 2, 3, 4, 5]);
    }

    [Fact]
    public void IterationBehavior()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };
        var element = 42;

        var enumerable = xs.AsValueEnumerable().Append(element);

        // Check iteration order
        int index = 0;
        var expected = xs.Append(element).ToArray();
        var e = enumerable.Enumerator;
        while (e.TryGetNext(out var current))
        {
            current.ShouldBe(expected[index++]);
        }

        index.ShouldBe(expected.Length);
    }

    [Fact]
    public void IterationEmptySource()
    {
        var xs = Array.Empty<int>();
        var element = 42;

        var enumerable = xs.AsValueEnumerable().Append(element).Enumerator;

        enumerable.TryGetNext(out var current).ShouldBeTrue();
        current.ShouldBe(element);

        enumerable.TryGetNext(out _).ShouldBeFalse();
    }

    [Fact]
    public void ProperDisposal()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };
        var element = 42;

        var enumerable = xs.AsValueEnumerable().Append(element).Enumerator;

        // After disposal, it should return false
        enumerable.TryGetNext(out _);  // Consume one element
        enumerable.Dispose();
        enumerable.TryGetNext(out _).ShouldBeFalse();
    }

    [Fact]
    public void SourceWithToIterableValueEnumerable()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };
        var element = 42;

        // Use ToIterableValueEnumerable to avoid Span optimization
        var expected = xs.Append(element).ToArray();

        xs.ToValueEnumerable().Append(element).ToArray().ShouldBe(expected);
    }

    [Fact]
    public void ElementAtOrDefault_FromEndOutOfRange()
    {
        var xs = new[] { 1, 2, 3 };

        xs.AsValueEnumerable().Append(9).ElementAtOrDefault(^4).ShouldBe(1);
        xs.AsValueEnumerable().Append(9).ElementAtOrDefault(^5).ShouldBe(0);
        xs.AsValueEnumerable().Append(9).ElementAtOrDefault(^10).ShouldBe(0);
        xs.AsValueEnumerable().Take(2).Append(9).ElementAtOrDefault(^4).ShouldBe(0);
        Array.Empty<int>().AsValueEnumerable().Append(9).ElementAtOrDefault(^2).ShouldBe(0);
        Should.Throw<ArgumentOutOfRangeException>(() => xs.AsValueEnumerable().Append(9).ElementAt(^10));
    }

#if NET8_0_OR_GREATER

    [Fact]
    public void WithElementAt()
    {
        int value = default;
        var span = new Span<int>(ref value);

        var emptyAppend = Array.Empty<int>().AsValueEnumerable().Append(10);
        emptyAppend.TryCopyTo(span, 0).ShouldBeTrue();
        value.ShouldBe(10);

        var xs = new[] { 1, 2, 3, 4, 5 }.AsValueEnumerable().Append(10);

        xs.TryCopyTo(span, 0).ShouldBeTrue(); value.ShouldBe(1);
        xs.TryCopyTo(span, 1).ShouldBeTrue(); value.ShouldBe(2);
        xs.TryCopyTo(span, 2).ShouldBeTrue(); value.ShouldBe(3);
        xs.TryCopyTo(span, 3).ShouldBeTrue(); value.ShouldBe(4);
        xs.TryCopyTo(span, 4).ShouldBeTrue(); value.ShouldBe(5);
        xs.TryCopyTo(span, 5).ShouldBeTrue(); value.ShouldBe(10);
        xs.TryCopyTo(span, 6).ShouldBeFalse();

        xs.ToArray().ShouldBe([1, 2, 3, 4, 5, 10]);
    }

    [Fact]
    public void WithTake()
    {
        var dest = new[] { 0, 0, 0 };

        var emptyAppend = Array.Empty<int>().AsValueEnumerable().Append(10);
        emptyAppend.TryCopyTo(dest, 0).ShouldBeTrue();
        dest.ShouldBe([10, 0, 0]);

        var xs = new[] { 1, 2, 3, 4, 5 }.AsValueEnumerable().Append(10);

        xs.TryCopyTo(dest, 0).ShouldBeTrue(); dest.ShouldBe([1, 2, 3]);
        xs.TryCopyTo(dest, 1).ShouldBeTrue(); dest.ShouldBe([2, 3, 4]);
        xs.TryCopyTo(dest, 2).ShouldBeTrue(); dest.ShouldBe([3, 4, 5]);
        Array.Clear(dest);
        xs.TryCopyTo(dest, 3).ShouldBeTrue(); dest.ShouldBe([4, 5, 10]);
        Array.Clear(dest);
        xs.TryCopyTo(dest, 4).ShouldBeTrue(); dest.ShouldBe([5, 10, 0]);
        Array.Clear(dest);
        xs.TryCopyTo(dest, 5).ShouldBeTrue(); dest.ShouldBe([10, 0, 0]);
        xs.TryCopyTo(dest, 6).ShouldBeFalse();
    }

#endif
}
