using System;
using System.Linq;

namespace ZLinq.Tests.Linq;

public class ReverseTest
{
    [Fact]
    public void Empty_Sequence()
    {
        // Arrange
        var xs = Array.Empty<int>();

        // Act
        var reversed = xs.AsValueEnumerable().Reverse();

        // Assert
        // Should be able to get count without enumeration
        reversed.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
        count.ShouldBe(0);

        // Should be able to get span since the underlying buffer is materialized
        reversed.TryGetSpan(out var span).ShouldBeTrue();
        span.IsEmpty.ShouldBeTrue();

        // Enumeration should complete immediately
        using var e = reversed.Enumerator;
        e.TryGetNext(out var next).ShouldBeFalse();

        // Dispose properly
        reversed.Dispose();
    }

    [Fact]
    public void NonEmpty_Sequence()
    {
        // Arrange
        var xs = new[] { 1, 2, 3, 4, 5 };
        var expected = xs.Reverse().ToArray();

        // Act
        var reversed = xs.AsValueEnumerable().Reverse();

        // Assert
        // Should be able to get count without enumeration
        reversed.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
        count.ShouldBe(xs.Length);

        // Should be able to get span since the underlying buffer is materialized
        reversed.TryGetSpan(out var span).ShouldBeTrue();
        span.Length.ShouldBe(xs.Length);
        span.ToArray().ShouldBe(expected);

        // Enumeration should work and return reversed elements
        var result = reversed.ToArray();
        result.ShouldBe(expected);

        // Dispose properly
        reversed.Dispose();
    }

    [Fact]
    public void Enumeration_ReturnsElementsInReverseOrder()
    {
        // Arrange
        var xs = new[] { 1, 2, 3, 4, 5 };
        var expected = xs.Reverse().ToArray();

        // Act
        var reversed = xs.AsValueEnumerable().Reverse();

        // Assert
        var index = 0;
        using var e = reversed.Enumerator;
        while (e.TryGetNext(out var item))
        {
            item.ShouldBe(expected[index]);
            index++;
        }
        index.ShouldBe(expected.Length);

        // Dispose properly
        reversed.Dispose();
    }

    [Fact]
    public void TryCopyTo_CopiesReversedElements()
    {
        // Arrange
        var xs = new[] { 1, 2, 3, 4, 5 };
        var expected = xs.Reverse().ToArray();
        var destination = new int[xs.Length];

        // Act
        var reversed = xs.AsValueEnumerable().Reverse();
        var result = reversed.TryCopyTo(destination, 0);

        // Assert
        result.ShouldBeTrue();
        destination.ShouldBe(expected);

        // Dispose properly
        reversed.Dispose();
    }

    [Fact]
    public void TryCopyTo_WithOffset()
    {
        // Arrange
        var xs = new[] { 1, 2, 3, 4, 5 };
        var expected = xs.Reverse().Skip(2).ToArray(); // 5,4,3 -> 3,4,5
        var destination = new int[3];

        // Act
        var reversed = xs.AsValueEnumerable().Reverse();
        var result = reversed.TryCopyTo(destination, 2); // Starting from index 2 in the reversed sequence

        // Assert
        result.ShouldBeTrue();
        destination.ShouldBe(expected);

        // Dispose properly
        reversed.Dispose();
    }

    [Fact]
    public void TryCopyTo_DestinationTooSmall()
    {
        // Arrange
        var xs = new[] { 1, 2, 3, 4, 5 };
        var destination = new int[3]; // Smaller than source

        // Act
        var reversed = xs.AsValueEnumerable().Reverse();
        var result = reversed.TryCopyTo(destination, 0);

        // Assert - TryCopyTo should still work with smaller destination
        result.ShouldBeTrue();
        destination.ShouldBe(new[] { 5, 4, 3 });

        // Dispose properly
        reversed.Dispose();
    }

    [Fact]
    public void MultipleEnumeration_ReturnsCorrectResults()
    {
        // Arrange
        var xs = new[] { 1, 2, 3, 4, 5 };
        var expected = xs.Reverse().ToArray();

        // Act
        var reversed = xs.AsValueEnumerable().Reverse();

        // Assert - First enumeration
        var result1 = reversed.ToArray();
        result1.ShouldBe(expected);

        // Second enumeration should also work
        var result2 = reversed.ToArray();
        result2.ShouldBe(expected);

        // Dispose properly
        reversed.Dispose();
    }

    [Fact]
    public void InitBuffer_CalledOnce()
    {
        // Arrange - This tests that buffer initialization happens correctly
        var xs = new[] { 1, 2, 3, 4, 5 };

        // Act
        var reversed = xs.AsValueEnumerable().Reverse();

        // Get span (triggers buffer initialization)
        using var e = reversed.Enumerator;
        e.TryGetSpan(out var span1).ShouldBeTrue();

        var array = reversed.ToArray();

        span1.ToArray().ShouldBe(array);

        // Perform enumeration (should use the same buffer)

        //// Get span again (should use the same buffer)
        e.TryGetSpan(out var span2);

        //// Assert
        span2.ToArray().ShouldBe(array);
    }

    [Fact]
    public void ComplexObjects_ReversedCorrectly()
    {
        // Arrange
        var objects = new[]
        {
            new { Id = 1, Name = "First" },
            new { Id = 2, Name = "Second" },
            new { Id = 3, Name = "Third" },
        };
        var expected = objects.Reverse().ToArray();

        // Act
        var reversed = objects.AsValueEnumerable().Reverse();

        // Assert
        var result = reversed.ToArray();
        result.Length.ShouldBe(expected.Length);

        for (int i = 0; i < result.Length; i++)
        {
            result[i].Id.ShouldBe(expected[i].Id);
            result[i].Name.ShouldBe(expected[i].Name);
        }

        // Dispose properly
        reversed.Dispose();
    }
}

#if !NET10_0_OR_GREATER
// Workaround code for breaking changes that introduced .NET 10 with LangVersion:14.
// See: https://learn.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/10.0/csharp-overload-resolution
file static class ExtensionMethods
{
    public static IEnumerable<T> Reverse<T>(this T[] source)
        => Enumerable.Reverse(source);
}
#endif
