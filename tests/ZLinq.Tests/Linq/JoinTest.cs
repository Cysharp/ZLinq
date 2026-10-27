namespace ZLinq.Tests.Linq;

public class JoinTest
{
    [Fact]
    public void Join_BasicFunctionality()
    {
        // Arrange
        var outer = new[] { 1, 2, 3, 4 };
        var inner = new[] { 2, 3, 4, 5 };

        // Act - Standard LINQ as reference
        var expected = outer
            .Join(inner, o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .Join(inner.AsValueEnumerable(), o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void Join_WithComplexObjects()
    {
        // Arrange
        var customers = new[]
        {
            new { Id = 1, Name = "Alice" },
            new { Id = 2, Name = "Bob" },
            new { Id = 3, Name = "Charlie" }
        };

        var orders = new[]
        {
            new { CustomerId = 1, OrderId = 101 },
            new { CustomerId = 1, OrderId = 102 },
            new { CustomerId = 2, OrderId = 201 },
            new { CustomerId = 4, OrderId = 401 }
        };

        // Act - Standard LINQ as reference
        var expected = customers
            .Join(orders, c => c.Id, o => o.CustomerId, (c, o) => new { c.Name, o.OrderId })
            .ToArray();

        // Act - ZLinq
        var actual = customers
            .AsValueEnumerable()
            .Join(orders.AsValueEnumerable(), c => c.Id, o => o.CustomerId, (c, o) => new { c.Name, o.OrderId })
            .ToArray();

        // Assert
        actual.Length.ShouldBe(expected.Length);
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i].Name.ShouldBe(expected[i].Name);
            actual[i].OrderId.ShouldBe(expected[i].OrderId);
        }
    }

    [Fact]
    public void Join_WithCustomComparer()
    {
        // Arrange
        var outer = new[] { "A", "B", "C", "D" };
        var inner = new[] { "a", "b", "c", "e" };
        var comparer = StringComparer.OrdinalIgnoreCase;

        // Act - Standard LINQ as reference
        var expected = outer
            .Join(inner, o => o, i => i, (o, i) => $"{o}:{i}", comparer)
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .Join(inner.AsValueEnumerable(), o => o, i => i, (o, i) => $"{o}:{i}", comparer)
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void Join_WithEmptyOuter()
    {
        // Arrange
        var outer = Array.Empty<int>();
        var inner = new[] { 1, 2, 3, 4 };

        // Act - Standard LINQ as reference
        var expected = outer
            .Join(inner, o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .Join(inner.AsValueEnumerable(), o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Assert
        actual.ShouldBeEmpty();
        actual.ShouldBe(expected);
    }

    [Fact]
    public void Join_WithEmptyInner()
    {
        // Arrange
        var outer = new[] { 1, 2, 3, 4 };
        var inner = Array.Empty<int>();

        // Act - Standard LINQ as reference
        var expected = outer
            .Join(inner, o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .Join(inner.AsValueEnumerable(), o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Assert
        actual.ShouldBeEmpty();
        actual.ShouldBe(expected);
    }

    [Fact]
    public void Join_WithNullKeys()
    {
        // Arrange
        var outer = new[]
        {
            new { Id = 1, Key = "A" },
            new { Id = 2, Key = (string)null! },
            new { Id = 3, Key = "C" },
        };

        var inner = new[]
        {
            new { Id = 101, Key = "A" },
            new { Id = 102, Key = (string)null! },
            new { Id = 103, Key = "C" },
        };

        // Act - Standard LINQ as reference
        var expected = outer
            .Join(inner, o => o.Key, i => i.Key, (o, i) => new { OuterId = o.Id, InnerId = i.Id })
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .Join(inner.AsValueEnumerable(), o => o.Key, i => i.Key, (o, i) => new { OuterId = o.Id, InnerId = i.Id })
            .ToArray();

        // Assert
        actual.Length.ShouldBe(expected.Length);
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i].OuterId.ShouldBe(expected[i].OuterId);
            actual[i].InnerId.ShouldBe(expected[i].InnerId);
        }
    }

    [Fact]
    public void Join_WithMultipleMatches()
    {
        // Arrange
        var outer = new[] { 1, 2, 2, 3 };
        var inner = new[] { 1, 2, 2, 4 };

        // Act - Standard LINQ as reference
        var expected = outer
            .Join(inner, o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .Join(inner.AsValueEnumerable(), o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void Join_WithIEnumerableInner()
    {
        // Arrange
        var outer = new[] { 1, 2, 3, 4 };
        var inner = Enumerable.Range(2, 4); // 2, 3, 4, 5

        // Act - Standard LINQ as reference
        var expected = outer
            .Join(inner, o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .Join(inner, o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void Join_WithIEnumerableInnerAndComparer()
    {
        // Arrange
        var outer = new[] { "A", "B", "C", "D" };
        var inner = new List<string> { "a", "b", "c", "e" };
        var comparer = StringComparer.OrdinalIgnoreCase;

        // Act - Standard LINQ as reference
        var expected = outer
            .Join(inner, o => o, i => i, (o, i) => $"{o}:{i}", comparer)
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .Join(inner, o => o, i => i, (o, i) => $"{o}:{i}", comparer)
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void Join_EnsuresDisposal()
    {
        // Arrange
        var outerDisposed = false;
        var innerDisposed = false;

        IEnumerable<int> GetOuterSequence()
        {
            try
            {
                yield return 1;
                yield return 2;
                yield return 3;
            }
            finally
            {
                outerDisposed = true;
            }
        }

        IEnumerable<int> GetInnerSequence()
        {
            try
            {
                yield return 2;
                yield return 3;
                yield return 4;
            }
            finally
            {
                innerDisposed = true;
            }
        }

        // Act
        var result = GetOuterSequence()
            .AsValueEnumerable()
            .Join(GetInnerSequence().AsValueEnumerable(), o => o, i => i, (o, i) => o + i)
            .ToArray();

        // Assert
        outerDisposed.ShouldBeTrue();
        innerDisposed.ShouldBeTrue();
        result.ShouldBe(new[] { 4, 6 });
    }

    [Fact]
    public void Join_DisposesInnerEvenWithIncompleteOuterIteration()
    {
        // Arrange
        var innerDisposed = false;

        IEnumerable<int> GetInnerSequence()
        {
            try
            {
                yield return 2;
                yield return 3;
                yield return 4;
            }
            finally
            {
                innerDisposed = true;
            }
        }

        var outer = new[] { 1, 2, 3, 4 };

        // Act
        var joinQuery = outer
            .AsValueEnumerable()
            .Join(GetInnerSequence().AsValueEnumerable(), o => o, i => i, (o, i) => o + i);

        // Only take the first result and then dispose
        var firstResult = joinQuery.First();

        TestUtil.Dispose(joinQuery);

        // Assert
        innerDisposed.ShouldBeTrue();
        firstResult.ShouldBe(4);
    }

    [Fact]
    public void Join_TryGetNonEnumeratedCount_ReturnsFalse()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 2, 3, 4 };

        var joinQuery = outer
            .AsValueEnumerable()
            .Join(inner.AsValueEnumerable(), o => o, i => i, (o, i) => o + i);

        // Act
        var result = joinQuery.TryGetNonEnumeratedCount(out var count);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void Join_TryGetSpan_ReturnsFalse()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 2, 3, 4 };

        var joinQuery = outer
            .AsValueEnumerable()
            .Join(inner.AsValueEnumerable(), o => o, i => i, (o, i) => o + i);

        // Act
        var result = joinQuery.TryGetSpan(out var span);

        // Assert
        result.ShouldBeFalse();
    }

    // key is the substring after the first character, e.g. "a1" -> "1"
    static string KeyOf(string s) => s.Substring(1);

    // key is null if the second character is '-'
    static string? NullableKeyOf(string s) => s[1] == '-' ? null : s.Substring(1);

    /// <summary>
    /// Verifies that Join without resultSelector yields only matched (Outer, Inner) pairs in outer order,
    /// for both IEnumerable and ValueEnumerable inner sources.
    /// </summary>
    [Fact]
    public void Join_Tuple_ReturnsMatchedPairs()
    {
        var outer = new[] { "a1", "b2", "c3" };
        var inner = new[] { "x2", "y1", "z2", "w4" };

        var expected = new[] { ("a1", "y1"), ("b2", "x2"), ("b2", "z2") };

        outer.AsValueEnumerable().Join(inner, KeyOf, KeyOf).ToArray().ShouldBe(expected);
        outer.AsValueEnumerable().Join(inner.AsValueEnumerable(), KeyOf, KeyOf).ToArray().ShouldBe(expected);
    }

    /// <summary>
    /// Verifies that the tuple elements of Join without resultSelector are accessible by the names Outer and Inner.
    /// </summary>
    [Fact]
    public void Join_Tuple_HasNamedElements()
    {
        var outer = new[] { "a1" };
        var inner = new[] { "x1" };

        var actual = outer.AsValueEnumerable().Join(inner, KeyOf, KeyOf).ToArray();

        actual.Length.ShouldBe(1);
        actual[0].Outer.ShouldBe("a1");
        actual[0].Inner.ShouldBe("x1");
    }

    /// <summary>
    /// Verifies that Join without resultSelector uses the specified comparer to match keys.
    /// </summary>
    [Fact]
    public void Join_Tuple_WithComparer()
    {
        var outer = new[] { "A", "B", "C" };
        var inner = new[] { "a", "b", "b", "d" };

        var actual = outer.AsValueEnumerable().Join(inner, o => o, i => i, StringComparer.OrdinalIgnoreCase).ToArray();

        actual.ShouldBe(new[] { ("A", "a"), ("B", "b"), ("B", "b") });
    }

    /// <summary>
    /// Verifies that Join without resultSelector never matches null keys, which is the behavior of System.Linq's Join.
    /// </summary>
    [Fact]
    public void Join_Tuple_IgnoresNullKeys()
    {
        var outer = new[] { "o1", "o-" };
        var inner = new[] { "i-", "i1" };

        var actual = outer.AsValueEnumerable().Join(inner, NullableKeyOf, NullableKeyOf).ToArray();

        actual.ShouldBe(new[] { ("o1", "i1") });
    }

    /// <summary>
    /// Verifies that Join without resultSelector throws ArgumentNullException for null inner and key selectors.
    /// </summary>
    [Fact]
    public void Join_Tuple_NullArguments_Throw()
    {
        var outer = new[] { "a1" };
        var inner = new[] { "x1" };

        Should.Throw<ArgumentNullException>(() => outer.AsValueEnumerable().Join((IEnumerable<string>)null!, KeyOf, KeyOf));
        Should.Throw<ArgumentNullException>(() => outer.AsValueEnumerable().Join(inner, null!, KeyOf));
        Should.Throw<ArgumentNullException>(() => outer.AsValueEnumerable().Join(inner, KeyOf, null!));
    }

    /// <summary>
    /// Verifies that Join without resultSelector disposes both outer and inner sources,
    /// both after full enumeration and after early termination while outer is still being enumerated.
    /// </summary>
    [Fact]
    public void Join_Tuple_DisposesBothSources()
    {
        var outerDisposed = false;
        var innerDisposed = false;

        IEnumerable<string> GetOuterSequence()
        {
            try
            {
                yield return "a1";
                yield return "b2";
            }
            finally
            {
                outerDisposed = true;
            }
        }

        IEnumerable<string> GetInnerSequence()
        {
            try
            {
                yield return "x1";
                yield return "y2";
            }
            finally
            {
                innerDisposed = true;
            }
        }

        GetOuterSequence().AsValueEnumerable().Join(GetInnerSequence(), KeyOf, KeyOf).ToArray();
        outerDisposed.ShouldBeTrue();
        innerDisposed.ShouldBeTrue();

        outerDisposed = false;
        innerDisposed = false;
        using (var e = GetOuterSequence().AsValueEnumerable().Join(GetInnerSequence(), KeyOf, KeyOf).Enumerator)
        {
            e.TryGetNext(out _).ShouldBeTrue();
        }
        outerDisposed.ShouldBeTrue();
        innerDisposed.ShouldBeTrue();
    }

    /// <summary>
    /// Verifies that Join without resultSelector does not report a non-enumerated count, span, or copy capability,
    /// because the result count cannot be known without joining.
    /// </summary>
    [Fact]
    public void Join_Tuple_OptimizationMethods_ReturnFalse()
    {
        var outer = new[] { "a1" };
        var inner = new[] { "x1" };

        var query = outer.AsValueEnumerable().Join(inner, KeyOf, KeyOf);
        query.TryGetNonEnumeratedCount(out _).ShouldBeFalse();
        query.TryGetSpan(out _).ShouldBeFalse();
        query.TryCopyTo(new (string, string)[1].AsSpan(), 0).ShouldBeFalse();
    }
}
