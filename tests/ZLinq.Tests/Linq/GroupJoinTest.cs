namespace ZLinq.Tests.Linq;

public class GroupJoinTest
{
    [Fact]
    public void GroupJoin_BasicFunctionality()
    {
        // Arrange
        var outer = new[] { 1, 2, 3, 4 };
        var inner = new[] { 2, 3, 4, 5 };

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => $"{o}:{string.Join(",", g)}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .GroupJoin(inner, o => o, i => i, (o, g) => $"{o}:{string.Join(",", g)}")
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void GroupJoin_WithComplexObjects()
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
            .GroupJoin(orders, c => c.Id, o => o.CustomerId, (c, orders) => new { Customer = c.Name, OrderCount = orders.Count() })
            .ToArray();

        // Act - ZLinq
        var actual = customers
            .AsValueEnumerable()
            .GroupJoin(orders.AsValueEnumerable(), c => c.Id, o => o.CustomerId, (c, orders) => new { Customer = c.Name, OrderCount = orders.Count() })
            .ToArray();

        // Assert
        actual.Length.ShouldBe(expected.Length);
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i].Customer.ShouldBe(expected[i].Customer);
            actual[i].OrderCount.ShouldBe(expected[i].OrderCount);
        }
    }

    [Fact]
    public void GroupJoin_WithCustomComparer()
    {
        // Arrange
        var outer = new[] { "A", "B", "C", "D" };
        var inner = new[] { "a", "b", "c", "e" };
        var comparer = StringComparer.OrdinalIgnoreCase;

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => $"{o}:[{string.Join(",", g)}]", comparer)
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .GroupJoin(inner.AsValueEnumerable(), o => o, i => i, (o, g) => $"{o}:[{string.Join(",", g)}]", comparer)
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void GroupJoin_WithEmptyOuter()
    {
        // Arrange
        var outer = Array.Empty<int>();
        var inner = new[] { 1, 2, 3, 4 };

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => $"{o}:{string.Join(",", g)}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .GroupJoin(inner.AsValueEnumerable(), o => o, i => i, (o, g) => $"{o}:{string.Join(",", g)}")
            .ToArray();

        // Assert
        actual.ShouldBeEmpty();
        actual.ShouldBe(expected);
    }

    [Fact]
    public void GroupJoin_WithEmptyInner()
    {
        // Arrange
        var outer = new[] { 1, 2, 3, 4 };
        var inner = Array.Empty<int>();

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => $"{o}:{string.Join(",", g)}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .GroupJoin(inner.AsValueEnumerable(), o => o, i => i, (o, g) => $"{o}:{string.Join(",", g)}")
            .ToArray();

        // Assert
        actual.Length.ShouldBe(expected.Length);
        actual.ShouldBe(expected);
    }

    [Fact]
    public void GroupJoin_WithNullKeys()
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
            .GroupJoin(inner, o => o.Key, i => i.Key, (o, g) => new { OuterId = o.Id, GroupCount = g.Count() })
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .GroupJoin(inner.AsValueEnumerable(), o => o.Key, i => i.Key, (o, g) => new { OuterId = o.Id, GroupCount = g.Count() })
            .ToArray();

        // Assert
        actual.Length.ShouldBe(expected.Length);
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i].OuterId.ShouldBe(expected[i].OuterId);
            actual[i].GroupCount.ShouldBe(expected[i].GroupCount);
        }
    }

    [Fact]
    public void GroupJoin_WithMultipleMatches()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 1, 2, 2, 3, 3, 3 };

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => new { Key = o, Count = g.Count() })
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .GroupJoin(inner.AsValueEnumerable(), o => o, i => i, (o, g) => new { Key = o, Count = g.Count() })
            .ToArray();

        // Assert
        actual.Length.ShouldBe(expected.Length);
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i].Key.ShouldBe(expected[i].Key);
            actual[i].Count.ShouldBe(expected[i].Count);
        }
    }

    [Fact]
    public void GroupJoin_WithIEnumerableInner()
    {
        // Arrange
        var outer = new[] { 1, 2, 3, 4 };
        var inner = Enumerable.Range(1, 5); // 1, 2, 3, 4, 5

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => $"{o}:{string.Join(",", g)}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .GroupJoin(inner, o => o, i => i, (o, g) => $"{o}:{string.Join(",", g)}")
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void GroupJoin_WithIEnumerableInnerAndComparer()
    {
        // Arrange
        var outer = new[] { "A", "B", "C", "D" };
        var inner = new List<string> { "a", "b", "c", "e" };
        var comparer = StringComparer.OrdinalIgnoreCase;

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => $"{o}:{string.Join(",", g)}", comparer)
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .GroupJoin(inner, o => o, i => i, (o, g) => $"{o}:{string.Join(",", g)}", comparer)
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void GroupJoin_EnsuresDisposal()
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
            .GroupJoin(GetInnerSequence().AsValueEnumerable(), o => o, i => i, (o, g) => $"{o}:{g.Count()}")
            .ToArray();

        // Assert
        outerDisposed.ShouldBeTrue();
        innerDisposed.ShouldBeTrue();
        result.Length.ShouldBe(3);
    }

    [Fact]
    public void GroupJoin_DisposesInnerEvenWithIncompleteOuterIteration()
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
            .GroupJoin(GetInnerSequence().AsValueEnumerable(), o => o, i => i, (o, g) => new { Key = o, Group = g });

        // Only take the first result and then dispose
        var firstResult = joinQuery.First();

        TestUtil.Dispose(joinQuery);

        // Assert
        innerDisposed.ShouldBeTrue();
    }

    [Fact]
    public void GroupJoin_TryGetNonEnumeratedCount_ReturnsFalse()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 2, 3, 4 };

        var groupJoinQuery = outer
            .AsValueEnumerable()
            .GroupJoin(inner.AsValueEnumerable(), o => o, i => i, (o, g) => new { Key = o, Group = g });

        // Act
        var result = groupJoinQuery.TryGetNonEnumeratedCount(out var count);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void GroupJoin_TryGetSpan_ReturnsFalse()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 2, 3, 4 };

        var groupJoinQuery = outer
            .AsValueEnumerable()
            .GroupJoin(inner.AsValueEnumerable(), o => o, i => i, (o, g) => new { Key = o, Group = g });

        // Act
        var result = groupJoinQuery.TryGetSpan(out var span);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void GroupJoin_TryCopyTo_ReturnsFalse()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 2, 3, 4 };
        var result = new (int, IEnumerable<int>)[3];

        var groupJoinQuery = outer
            .AsValueEnumerable()
            .GroupJoin(inner.AsValueEnumerable(), o => o, i => i, (o, g) => (Key: o, Group: g));

        // Act
        var copyResult = groupJoinQuery.TryCopyTo(result.AsSpan(), 0);

        // Assert
        copyResult.ShouldBeFalse();
    }

    // key is the substring after the first character, e.g. "a1" -> "1"
    static string KeyOf(string s) => s.Substring(1);

    // key is null if the second character is '-'
    static string? NullableKeyOf(string s) => s[1] == '-' ? null : s.Substring(1);

    /// <summary>
    /// Verifies that GroupJoin without resultSelector yields one grouping per outer element in outer order,
    /// whose key is the outer element and whose elements are the matching inner elements in inner order,
    /// for both IEnumerable and ValueEnumerable inner sources.
    /// </summary>
    [Fact]
    public void GroupJoin_Grouping_KeyIsOuterAndElementsAreMatchedInner()
    {
        var outer = new[] { "a1", "b2", "c3" };
        var inner = new[] { "x2", "y1", "z2", "w4" };

        var actual1 = outer.AsValueEnumerable().GroupJoin(inner, KeyOf, KeyOf).ToArray();
        var actual2 = outer.AsValueEnumerable().GroupJoin(inner.AsValueEnumerable(), KeyOf, KeyOf).ToArray();

        foreach (var actual in new[] { actual1, actual2 })
        {
            actual.Select(x => x.Key).ShouldBe(new[] { "a1", "b2", "c3" });
            actual[0].ShouldBe(new[] { "y1" });
            actual[1].ShouldBe(new[] { "x2", "z2" });
            actual[2].ShouldBeEmpty();
        }
    }

    /// <summary>
    /// Verifies that GroupJoin without resultSelector uses the specified comparer to match keys.
    /// </summary>
    [Fact]
    public void GroupJoin_Grouping_WithComparer()
    {
        var outer = new[] { "A", "B", "C" };
        var inner = new[] { "a", "b", "b", "d" };

        var actual = outer.AsValueEnumerable().GroupJoin(inner, o => o, i => i, StringComparer.OrdinalIgnoreCase).ToArray();

        actual.Select(x => x.Key).ShouldBe(new[] { "A", "B", "C" });
        actual[0].ShouldBe(new[] { "a" });
        actual[1].ShouldBe(new[] { "b", "b" });
        actual[2].ShouldBeEmpty();
    }

    /// <summary>
    /// Verifies that an outer element with a null key never matches and is yielded as an empty grouping.
    /// </summary>
    [Fact]
    public void GroupJoin_Grouping_NullOuterKey_YieldsEmptyGrouping()
    {
        var outer = new[] { "o1", "o-" };
        var inner = new[] { "i-", "i1" };

        var actual = outer.AsValueEnumerable().GroupJoin(inner, NullableKeyOf, NullableKeyOf).ToArray();

        actual.Select(x => x.Key).ShouldBe(new[] { "o1", "o-" });
        actual[0].ShouldBe(new[] { "i1" });
        actual[1].ShouldBeEmpty();
    }

    /// <summary>
    /// Verifies that the yielded groupings can be enumerated multiple times with the same result.
    /// </summary>
    [Fact]
    public void GroupJoin_Grouping_CanBeEnumeratedMultipleTimes()
    {
        var outer = new[] { "a1" };
        var inner = new[] { "x1", "y1" };

        var grouping = outer.AsValueEnumerable().GroupJoin(inner, KeyOf, KeyOf).First();

        grouping.ShouldBe(new[] { "x1", "y1" });
        grouping.ShouldBe(new[] { "x1", "y1" });
    }

    /// <summary>
    /// Verifies that GroupJoin without resultSelector throws ArgumentNullException for null inner and key selectors.
    /// </summary>
    [Fact]
    public void GroupJoin_Grouping_NullArguments_Throw()
    {
        var outer = new[] { "a1" };
        var inner = new[] { "x1" };

        Should.Throw<ArgumentNullException>(() => outer.AsValueEnumerable().GroupJoin((IEnumerable<string>)null!, KeyOf, KeyOf));
        Should.Throw<ArgumentNullException>(() => outer.AsValueEnumerable().GroupJoin(inner, null!, KeyOf));
        Should.Throw<ArgumentNullException>(() => outer.AsValueEnumerable().GroupJoin(inner, KeyOf, null!));
    }

    /// <summary>
    /// Verifies that GroupJoin without resultSelector disposes both outer and inner sources,
    /// both after full enumeration and after early termination while outer is still being enumerated.
    /// </summary>
    [Fact]
    public void GroupJoin_Grouping_DisposesBothSources()
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

        GetOuterSequence().AsValueEnumerable().GroupJoin(GetInnerSequence(), KeyOf, KeyOf).ToArray();
        outerDisposed.ShouldBeTrue();
        innerDisposed.ShouldBeTrue();

        outerDisposed = false;
        innerDisposed = false;
        using (var e = GetOuterSequence().AsValueEnumerable().GroupJoin(GetInnerSequence(), KeyOf, KeyOf).Enumerator)
        {
            e.TryGetNext(out _).ShouldBeTrue();
        }
        outerDisposed.ShouldBeTrue();
        innerDisposed.ShouldBeTrue();
    }

    /// <summary>
    /// Verifies that GroupJoin without resultSelector does not report a non-enumerated count, span, or copy capability,
    /// because the result count cannot be known without joining.
    /// </summary>
    [Fact]
    public void GroupJoin_Grouping_OptimizationMethods_ReturnFalse()
    {
        var outer = new[] { "a1" };
        var inner = new[] { "x1" };

        var query = outer.AsValueEnumerable().GroupJoin(inner, KeyOf, KeyOf);
        query.TryGetNonEnumeratedCount(out _).ShouldBeFalse();
        query.TryGetSpan(out _).ShouldBeFalse();
        query.TryCopyTo(new IGrouping<string, string>[1].AsSpan(), 0).ShouldBeFalse();
    }

    /// <summary>
    /// Verifies that the groupings yielded by GroupJoin without resultSelector are exposed as read-only IList and IReadOnlyList,
    /// so that consumers can use the count and indexer without enumerating, for both matched and empty groupings.
    /// Also verifies that Contains does not report the default value from the unused capacity of the underlying buffer.
    /// </summary>
    [Fact]
    public void GroupJoin_Grouping_IsReadOnlyList()
    {
        var outer = new[] { "a1", "b2" };
        var inner = new[] { "x1", "y1", "z1" }; // 3 elements, so the underlying buffer has unused capacity

        var actual = outer.AsValueEnumerable().GroupJoin(inner, KeyOf, KeyOf).ToArray();

        var matched = actual[0].ShouldBeAssignableTo<IList<string>>()!;
        matched.ShouldBeAssignableTo<IReadOnlyList<string>>();
        matched.Count.ShouldBe(3);
        matched[1].ShouldBe("y1");
        matched.IndexOf("z1").ShouldBe(2);
        matched.Contains("x1").ShouldBeTrue();
        matched.Contains(null!).ShouldBeFalse();
        var copied = new string[4];
        matched.CopyTo(copied, 1);
        copied.ShouldBe(new[] { null!, "x1", "y1", "z1" });
        matched.IsReadOnly.ShouldBeTrue();
        Should.Throw<NotSupportedException>(() => matched.Add("w1"));
        Should.Throw<NotSupportedException>(() => matched[0] = "w1");

        var empty = actual[1].ShouldBeAssignableTo<IList<string>>()!;
        empty.Count.ShouldBe(0);
        empty.Contains(null!).ShouldBeFalse();
    }
}
