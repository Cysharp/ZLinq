namespace ZLinq.Tests.Linq;

public class PrependTest
{
    [Fact]
    public void PrependToEmpty()
    {
        var xs = Array.Empty<int>();
        var element = 42;

        // Compare with standard LINQ
        var expected = xs.Prepend(element).ToArray();

        // Test with ZLinq
        var actual = xs.AsValueEnumerable().Prepend(element).ToArray();

        actual.ShouldBe(expected);
    }

    [Fact]
    public void PrependToNonEmpty()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };
        var element = 42;

        // Compare with standard LINQ
        var expected = xs.Prepend(element).ToArray();

        // Test with ZLinq
        var actual = xs.AsValueEnumerable().Prepend(element).ToArray();

        actual.ShouldBe(expected);
    }

    [Fact]
    public void TryGetNonEnumeratedCount()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };
        var element = 42;

        var enumerable = xs.AsValueEnumerable().Prepend(element);

        // Should return true and correct count
        enumerable.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
        count.ShouldBe(xs.Length + 1);

        // Test with empty source
        var emptyEnumerable = Array.Empty<int>().AsValueEnumerable().Prepend(element);
        emptyEnumerable.TryGetNonEnumeratedCount(out var emptyCount).ShouldBeTrue();
        emptyCount.ShouldBe(1);
    }

    [Fact]
    public void TryCopyTo()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };
        var element = 42;

        var enumerable = xs.AsValueEnumerable().Prepend(element);

        // Test with sufficient space
        var destination = new int[xs.Length + 1];
        enumerable.TryCopyTo(destination).ShouldBeTrue();

        var expected = xs.Prepend(element).ToArray();
        destination.ShouldBe(expected);
    }

    [Fact]
    public void IterationBehavior()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };
        var element = 42;

        var enumerable = xs.AsValueEnumerable().Prepend(element).Enumerator;

        // Check iteration order
        int index = 0;
        var expected = xs.Prepend(element).ToArray();

        while (enumerable.TryGetNext(out var current))
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

        var enumerable = xs.AsValueEnumerable().Prepend(element).Enumerator;

        enumerable.TryGetNext(out var current).ShouldBeTrue();
        current.ShouldBe(element);

        enumerable.TryGetNext(out _).ShouldBeFalse();
    }

    [Fact]
    public void ProperDisposal()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };
        var element = 42;

        var enumerable = xs.AsValueEnumerable().Prepend(element).Enumerator;

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
        var expected = xs.Prepend(element).ToArray();

        xs.ToValueEnumerable().Prepend(element).ToArray().ShouldBe(expected);
    }

    [Fact]
    public void ElementAtOrDefault_FromEndOutOfRange()
    {
        var xs = new[] { 1, 2, 3 };

        xs.AsValueEnumerable().Prepend(0).ElementAtOrDefault(^4).ShouldBe(0);
        xs.AsValueEnumerable().Prepend(10).ElementAtOrDefault(^4).ShouldBe(10);
        xs.AsValueEnumerable().Prepend(10).ElementAtOrDefault(^5).ShouldBe(0);
        xs.AsValueEnumerable().Prepend(10).ElementAtOrDefault(^10).ShouldBe(0);
        Array.Empty<int>().AsValueEnumerable().Append(100).Prepend(200).ElementAtOrDefault(^3).ShouldBe(0);
        Should.Throw<ArgumentOutOfRangeException>(() => xs.AsValueEnumerable().Prepend(10).ElementAt(^10));
    }

#if NET8_0_OR_GREATER

    [Fact]
    public void WithElementAt()
    {
        int value = default;
        var span = new Span<int>(ref value);

        var emptyPrepend = Array.Empty<int>().AsValueEnumerable().Prepend(10);
        emptyPrepend.TryCopyTo(span, 0).ShouldBeTrue();
        value.ShouldBe(10);

        var xs = new[] { 1, 2, 3, 4, 5 }.AsValueEnumerable().Prepend(10);

        xs.TryCopyTo(span, 0).ShouldBeTrue(); value.ShouldBe(10);
        xs.TryCopyTo(span, 1).ShouldBeTrue(); value.ShouldBe(1);
        xs.TryCopyTo(span, 2).ShouldBeTrue(); value.ShouldBe(2);
        xs.TryCopyTo(span, 3).ShouldBeTrue(); value.ShouldBe(3);
        xs.TryCopyTo(span, 4).ShouldBeTrue(); value.ShouldBe(4);
        xs.TryCopyTo(span, 5).ShouldBeTrue(); value.ShouldBe(5);
        xs.TryCopyTo(span, 6).ShouldBeFalse();

        xs.ToArray().ShouldBe([10, 1, 2, 3, 4, 5]);
    }

    [Fact]
    public void WithTake()
    {
        var dest = new[] { 0, 0, 0 };

        var emptyPrepend = Array.Empty<int>().AsValueEnumerable().Prepend(10);
        emptyPrepend.TryCopyTo(dest, 0).ShouldBeTrue();
        dest.ShouldBe([10, 0, 0]);

        var xs = new[] { 1, 2, 3, 4, 5 }.AsValueEnumerable().Prepend(10);

        xs.TryCopyTo(dest, 0).ShouldBeTrue(); dest.ShouldBe([10, 1, 2]);
        xs.TryCopyTo(dest, 1).ShouldBeTrue(); dest.ShouldBe([1, 2, 3]);
        xs.TryCopyTo(dest, 2).ShouldBeTrue(); dest.ShouldBe([2, 3, 4]);
        Array.Clear(dest);
        xs.TryCopyTo(dest, 3).ShouldBeTrue(); dest.ShouldBe([3, 4, 5]);
        Array.Clear(dest);
        xs.TryCopyTo(dest, 4).ShouldBeTrue(); dest.ShouldBe([4, 5, 0]);
        Array.Clear(dest);
        xs.TryCopyTo(dest, 5).ShouldBeTrue(); dest.ShouldBe([5, 0, 0]);
        xs.TryCopyTo(dest, 6).ShouldBeFalse();
    }

#endif
}
