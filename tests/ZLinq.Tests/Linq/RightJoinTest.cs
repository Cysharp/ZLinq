namespace ZLinq.Tests.Linq;

public class RightJoinTest
{
    [Fact]
    public void RightJoin_BasicFunctionality()
    {
        // Arrange
        var outer = new[] { 1, 2, 3, 4 };
        var inner = new[] { 2, 3, 4, 5 };

        // Act - Standard LINQ as reference (using GroupJoin + SelectMany to emulate right join)
        var expected = inner
            .GroupJoin(outer, i => i, o => o, (i, g) => new { i, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, o) => $"{o}:{x.i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .RightJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void RightJoin_WithComplexObjects()
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

        // Act - Standard LINQ as reference (using GroupJoin + SelectMany to emulate right join)
        var expected = orders
            .GroupJoin(customers, o => o.CustomerId, c => c.Id, (o, g) => new { o, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, c) => new { CustomerName = c?.Name, x.o.OrderId })
            .ToArray();

        // Act - ZLinq
        var actual = customers
            .AsValueEnumerable()
            .RightJoin(orders.AsValueEnumerable(), c => c.Id, o => o.CustomerId, (c, o) => new { CustomerName = c?.Name, o.OrderId })
            .ToArray();

        // Assert
        actual.Length.ShouldBe(expected.Length);
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i].CustomerName.ShouldBe(expected[i].CustomerName);
            actual[i].OrderId.ShouldBe(expected[i].OrderId);
        }
    }

    [Fact]
    public void RightJoin_WithCustomComparer()
    {
        // Arrange
        var outer = new[] { "A", "B", "C", "D" };
        var inner = new[] { "a", "b", "c", "e" };
        var comparer = StringComparer.OrdinalIgnoreCase;

        // Act - Standard LINQ as reference (using GroupJoin + SelectMany to emulate right join)
        var expected = inner
            .GroupJoin(outer, i => i, o => o, (i, g) => new { i, g }, comparer)
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, o) => $"{o}:{x.i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .RightJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => $"{o}:{i}", comparer)
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void RightJoin_WithEmptyOuter()
    {
        // Arrange
        var outer = Array.Empty<int>();
        var inner = new[] { 1, 2, 3, 4 };

        // Act - Standard LINQ as reference (using GroupJoin + SelectMany to emulate right join)
        var expected = inner
            .GroupJoin(outer, i => i, o => o, (i, g) => new { i, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, o) => $"{o}:{x.i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .RightJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Assert
        actual.Length.ShouldBe(expected.Length);
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i].ShouldBe(expected[i]);
        }
    }

    [Fact]
    public void RightJoin_WithEmptyInner()
    {
        // Arrange
        var outer = new[] { 1, 2, 3, 4 };
        var inner = Array.Empty<int>();

        // Act - Standard LINQ as reference (using GroupJoin + SelectMany to emulate right join)
        var expected = inner
            .GroupJoin(outer, i => i, o => o, (i, g) => new { i, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, o) => $"{o}:{x.i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .RightJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Assert
        actual.ShouldBeEmpty();
        actual.ShouldBe(expected);
    }

    [Fact]
    public void RightJoin_WithNullKeys()
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
            new { Id = 103, Key = "D" },
        };

        // Act - Standard LINQ as reference (using GroupJoin + SelectMany to emulate right join)
        var expected = inner
            .GroupJoin(outer, i => i.Key, o => o.Key, (i, g) => new { i, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, o) => new { OuterId = o?.Id, InnerId = x.i.Id })
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .RightJoin(inner.AsValueEnumerable(), o => o.Key, i => i.Key, (o, i) => new { OuterId = o?.Id, InnerId = i.Id })
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
    public void RightJoin_WithNoMatches()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 4, 5, 6 };

        // Act - Standard LINQ as reference (using GroupJoin + SelectMany to emulate right join)
        var expected = inner
            .GroupJoin(outer, i => i, o => o, (i, g) => new { i, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, o) => new { Outer = o, Inner = x.i })
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .RightJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => new { Outer = o, Inner = i })
            .ToArray();

        // Assert
        actual.Length.ShouldBe(expected.Length);
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i].Outer.ShouldBe(expected[i].Outer);
            actual[i].Inner.ShouldBe(expected[i].Inner);
        }
    }

    [Fact]
    public void RightJoin_WithMultipleMatches()
    {
        // Arrange
        var outer = new[] { 1, 1, 2, 2, 2, 3 };
        var inner = new[] { 1, 2, 3 };

        // Act - Standard LINQ as reference (using GroupJoin + SelectMany to emulate right join)
        var expected = inner
            .GroupJoin(outer, i => i, o => o, (i, g) => new { i, g })
            .SelectMany(x => x.g, (x, o) => new { Outer = o, Inner = x.i })
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .RightJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => new { Outer = o, Inner = i })
            .ToArray();

        // Assert
        actual.Length.ShouldBe(expected.Length);
        // The order might be different, so we need to check that the results contain the same elements
        var expectedGroups = expected.GroupBy(x => x.Inner).ToDictionary(g => g.Key, g => g.Select(x => x.Outer).ToList());
        var actualGroups = actual.GroupBy(x => x.Inner).ToDictionary(g => g.Key, g => g.Select(x => x.Outer).ToList());

        expectedGroups.Keys.ShouldBe(actualGroups.Keys);
        foreach (var key in expectedGroups.Keys)
        {
            actualGroups[key].ShouldBe(expectedGroups[key], ignoreOrder: true);
        }
    }

    [Fact]
    public void RightJoin_WithIEnumerableInner()
    {
        // Arrange
        var outer = new[] { 1, 2, 3, 4 };
        var inner = Enumerable.Range(2, 4); // 2, 3, 4, 5

        // Act - Standard LINQ as reference (using GroupJoin + SelectMany to emulate right join)
        var expected = inner
            .GroupJoin(outer, i => i, o => o, (i, g) => new { i, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, o) => $"{o}:{x.i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .RightJoin(inner, o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void RightJoin_WithIEnumerableInnerAndComparer()
    {
        // Arrange
        var outer = new[] { "A", "B", "C", "D" };
        var inner = new List<string> { "a", "b", "c", "e" };
        var comparer = StringComparer.OrdinalIgnoreCase;

        // Act - Standard LINQ as reference (using GroupJoin + SelectMany to emulate right join)
        var expected = inner
            .GroupJoin(outer, i => i, o => o, (i, g) => new { i, g }, comparer)
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, o) => $"{o}:{x.i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .RightJoin(inner, o => o, i => i, (o, i) => $"{o}:{i}", comparer)
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void RightJoin_EnsuresDisposal()
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
            .RightJoin(GetInnerSequence().AsValueEnumerable(), o => o, i => i, (o, i) => new { o, i })
            .ToArray();

        // Assert
        outerDisposed.ShouldBeTrue();
        innerDisposed.ShouldBeTrue();
    }

    [Fact]
    public void RightJoin_DisposesOuterEvenWithIncompleteInnerIteration()
    {
        // Arrange
        var outerDisposed = false;

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

        var inner = new[] { 2, 3, 4, 5 };

        // Act
        var joinQuery = GetOuterSequence()
            .AsValueEnumerable()
            .RightJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => new { o, i });

        // Only take the first result and then dispose
        var firstResult = joinQuery.First();

        TestUtil.Dispose(joinQuery);

        // Assert
        outerDisposed.ShouldBeTrue();
    }

    [Fact]
    public void RightJoin_TryGetNonEnumeratedCount_ReturnsFalse()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 2, 3, 4 };

        var rightJoinQuery = outer
            .AsValueEnumerable()
            .RightJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => new { o, i });

        // Act
        var result = rightJoinQuery.TryGetNonEnumeratedCount(out var count);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void RightJoin_TryGetSpan_ReturnsFalse()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 2, 3, 4 };

        var rightJoinQuery = outer
            .AsValueEnumerable()
            .RightJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => new { o, i });

        // Act
        var result = rightJoinQuery.TryGetSpan(out var span);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void RightJoin_TryCopyTo_ReturnsFalse()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 2, 3, 4 };
        var result = new (int? Outer, int Inner)[4];

        var rightJoinQuery = outer
            .AsValueEnumerable()
            .RightJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => (Outer: (int?)o, Inner: i));

        // Act
        var copyResult = rightJoinQuery.TryCopyTo(result.AsSpan(), 0);

        // Assert
        copyResult.ShouldBeFalse();
    }

    [Fact]
    public void RightJoin_AllInnerElementsAreReturned()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 3, 4, 5, 6 };

        // Act
        var actual = outer
            .AsValueEnumerable()
            .RightJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => new { Outer = o, Inner = i })
            .ToArray();

        // Assert
        actual.Length.ShouldBe(4); // All elements from inner should be present

        // Only inner[0] (3) should have a match
        actual[0].Inner.ShouldBe(3);
        actual[0].Outer.ShouldBe(3);

        // Rest should have null for outer
        actual[1].Inner.ShouldBe(4);
        actual[1].Outer.ShouldBe(0);

        actual[2].Inner.ShouldBe(5);
        actual[2].Outer.ShouldBe(0);

        actual[3].Inner.ShouldBe(6);
        actual[3].Outer.ShouldBe(0);
    }

    // key is the substring after the first character, e.g. "a1" -> "1"
    static string KeyOf(string s) => s.Substring(1);

    // key is null if the second character is '-'
    static string? NullableKeyOf(string s) => s[1] == '-' ? null : s.Substring(1);

    /// <summary>
    /// Verifies that RightJoin without resultSelector yields matched pairs and unmatched inner elements with default outer,
    /// in inner order, for both IEnumerable and ValueEnumerable inner sources.
    /// </summary>
    [Fact]
    public void RightJoin_Tuple_ReturnsMatchedAndUnmatchedInner()
    {
        var outer = new[] { "a1", "b2", "c3" };
        var inner = new[] { "x2", "y1", "z2", "w4" };

        var expected = new (string?, string)[] { ("b2", "x2"), ("a1", "y1"), ("b2", "z2"), (null, "w4") };

        outer.AsValueEnumerable().RightJoin(inner, KeyOf, KeyOf).ToArray().ShouldBe(expected);
        outer.AsValueEnumerable().RightJoin(inner.AsValueEnumerable(), KeyOf, KeyOf).ToArray().ShouldBe(expected);
    }

    /// <summary>
    /// Verifies that the tuple elements of RightJoin without resultSelector are accessible by the names Outer and Inner.
    /// </summary>
    [Fact]
    public void RightJoin_Tuple_HasNamedElements()
    {
        var outer = new[] { "a1" };
        var inner = new[] { "x1", "y2" };

        var actual = outer.AsValueEnumerable().RightJoin(inner, KeyOf, KeyOf).ToArray();

        actual.Length.ShouldBe(2);
        actual[0].Outer.ShouldBe("a1");
        actual[0].Inner.ShouldBe("x1");
        actual[1].Outer.ShouldBeNull();
        actual[1].Inner.ShouldBe("y2");
    }

    /// <summary>
    /// Verifies that RightJoin without resultSelector uses the specified comparer to match keys.
    /// </summary>
    [Fact]
    public void RightJoin_Tuple_WithComparer()
    {
        var outer = new[] { "A", "B", "C" };
        var inner = new[] { "a", "b", "b", "d" };

        var actual = outer.AsValueEnumerable().RightJoin(inner, o => o, i => i, StringComparer.OrdinalIgnoreCase).ToArray();

        actual.ShouldBe(new (string?, string)[] { ("A", "a"), ("B", "b"), ("B", "b"), (null, "d") });
    }

    /// <summary>
    /// Verifies that an inner element with a null key never matches and is yielded with default outer.
    /// </summary>
    [Fact]
    public void RightJoin_Tuple_NullInnerKey_YieldsDefaultOuter()
    {
        var outer = new[] { "o1", "o-" };
        var inner = new[] { "i-", "i1" };

        var actual = outer.AsValueEnumerable().RightJoin(inner, NullableKeyOf, NullableKeyOf).ToArray();

        actual.ShouldBe(new (string?, string)[] { (null, "i-"), ("o1", "i1") });
    }

    /// <summary>
    /// Verifies that RightJoin without resultSelector throws ArgumentNullException for null inner and key selectors.
    /// </summary>
    [Fact]
    public void RightJoin_Tuple_NullArguments_Throw()
    {
        var outer = new[] { "a1" };
        var inner = new[] { "x1" };

        Should.Throw<ArgumentNullException>(() => outer.AsValueEnumerable().RightJoin((IEnumerable<string>)null!, KeyOf, KeyOf));
        Should.Throw<ArgumentNullException>(() => outer.AsValueEnumerable().RightJoin(inner, null!, KeyOf));
        Should.Throw<ArgumentNullException>(() => outer.AsValueEnumerable().RightJoin(inner, KeyOf, null!));
    }

    /// <summary>
    /// Verifies that RightJoin without resultSelector disposes both outer and inner sources,
    /// both after full enumeration and after early termination while inner is still being enumerated.
    /// RightJoin builds the lookup from outer and streams inner, so the roles of the sources are the reverse of Join.
    /// </summary>
    [Fact]
    public void RightJoin_Tuple_DisposesBothSources()
    {
        var outerDisposed = false;
        var innerDisposed = false;

        IEnumerable<string> GetOuterSequence()
        {
            try
            {
                yield return "a1";
                yield return "b3";
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

        GetOuterSequence().AsValueEnumerable().RightJoin(GetInnerSequence(), KeyOf, KeyOf).ToArray();
        outerDisposed.ShouldBeTrue();
        innerDisposed.ShouldBeTrue();

        outerDisposed = false;
        innerDisposed = false;
        using (var e = GetOuterSequence().AsValueEnumerable().RightJoin(GetInnerSequence(), KeyOf, KeyOf).Enumerator)
        {
            e.TryGetNext(out _).ShouldBeTrue();
        }
        outerDisposed.ShouldBeTrue();
        innerDisposed.ShouldBeTrue();
    }

    /// <summary>
    /// Verifies that RightJoin without resultSelector does not report a non-enumerated count, span, or copy capability,
    /// because the result count cannot be known without joining.
    /// </summary>
    [Fact]
    public void RightJoin_Tuple_OptimizationMethods_ReturnFalse()
    {
        var outer = new[] { "a1" };
        var inner = new[] { "x1" };

        var query = outer.AsValueEnumerable().RightJoin(inner, KeyOf, KeyOf);
        query.TryGetNonEnumeratedCount(out _).ShouldBeFalse();
        query.TryGetSpan(out _).ShouldBeFalse();
        query.TryCopyTo(new (string?, string)[1].AsSpan(), 0).ShouldBeFalse();
    }
}
