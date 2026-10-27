namespace ZLinq.Tests.Linq;

public class LeftJoinTest
{
    [Fact]
    public void LeftJoin_BasicFunctionality()
    {
        // Arrange
        var outer = new[] { 1, 2, 3, 4 };
        var inner = new[] { 2, 3, 4, 5 };

        // Act - Standard LINQ as reference (using GroupJoin + SelectMany to emulate left join)
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => new { o, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, i) => $"{x.o}:{i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .LeftJoin(inner, o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void LeftJoin_WithComplexObjects()
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
            .GroupJoin(orders, c => c.Id, o => o.CustomerId, (c, g) => new { c, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, o) => new { Customer = x.c.Name, OrderId = o?.OrderId })
            .ToArray();

        // Act - ZLinq
        var actual = customers
            .AsValueEnumerable()
            .LeftJoin(orders.AsValueEnumerable(), c => c.Id, o => o.CustomerId, (c, o) => new { Customer = c.Name, OrderId = o?.OrderId })
            .ToArray();

        // Assert
        actual.Length.ShouldBe(expected.Length);
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i].Customer.ShouldBe(expected[i].Customer);
            actual[i].OrderId.ShouldBe(expected[i].OrderId);
        }
    }

    [Fact]
    public void LeftJoin_WithCustomComparer()
    {
        // Arrange
        var outer = new[] { "A", "B", "C", "D" };
        var inner = new[] { "a", "b", "c", "e" };
        var comparer = StringComparer.OrdinalIgnoreCase;

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => new { o, g }, comparer)
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, i) => $"{x.o}:{i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .LeftJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => $"{o}:{i}", comparer)
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void LeftJoin_WithEmptyOuter()
    {
        // Arrange
        var outer = Array.Empty<int>();
        var inner = new[] { 1, 2, 3, 4 };

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => new { o, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, i) => $"{x.o}:{i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .LeftJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Assert
        actual.ShouldBeEmpty();
        actual.ShouldBe(expected);
    }

    [Fact]
    public void LeftJoin_WithEmptyInner()
    {
        // Arrange
        var outer = new[] { 1, 2, 3, 4 };
        var inner = Array.Empty<int>();

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => new { o, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, i) => $"{x.o}:{i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .LeftJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Assert
        actual.Length.ShouldBe(expected.Length);
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i].ShouldBe(expected[i]);
        }
    }

    [Fact]
    public void LeftJoin_WithNullKeys()
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
            .GroupJoin(inner, o => o.Key, i => i.Key, (o, g) => new { o, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, i) => new { OuterId = x.o.Id, InnerId = i?.Id })
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .LeftJoin(inner.AsValueEnumerable(), o => o.Key, i => i.Key, (o, i) => new { OuterId = o.Id, InnerId = i?.Id })
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
    public void LeftJoin_WithNoMatches()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 4, 5, 6 };

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => new { o, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, i) => new { Outer = x.o, Inner = i })
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .LeftJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => new { Outer = o, Inner = i })
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
    public void LeftJoin_WithMultipleMatches()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 1, 1, 2, 2, 2, 3 };

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => new { o, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, i) => new { Outer = x.o, Inner = i })
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .LeftJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => new { Outer = o, Inner = i })
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
    public void LeftJoin_WithIEnumerableInner()
    {
        // Arrange
        var outer = new[] { 1, 2, 3, 4 };
        var inner = Enumerable.Range(2, 4); // 2, 3, 4, 5

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => new { o, g })
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, i) => $"{x.o}:{i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .LeftJoin(inner, o => o, i => i, (o, i) => $"{o}:{i}")
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void LeftJoin_WithIEnumerableInnerAndComparer()
    {
        // Arrange
        var outer = new[] { "A", "B", "C", "D" };
        var inner = new List<string> { "a", "b", "c", "e" };
        var comparer = StringComparer.OrdinalIgnoreCase;

        // Act - Standard LINQ as reference
        var expected = outer
            .GroupJoin(inner, o => o, i => i, (o, g) => new { o, g }, comparer)
            .SelectMany(x => x.g.DefaultIfEmpty(), (x, i) => $"{x.o}:{i}")
            .ToArray();

        // Act - ZLinq
        var actual = outer
            .AsValueEnumerable()
            .LeftJoin(inner, o => o, i => i, (o, i) => $"{o}:{i}", comparer)
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void LeftJoin_EnsuresDisposal()
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
            .LeftJoin(GetInnerSequence().AsValueEnumerable(), o => o, i => i, (o, i) => new { o, i })
            .ToArray();

        // Assert
        outerDisposed.ShouldBeTrue();
        innerDisposed.ShouldBeTrue();
    }

    [Fact]
    public void LeftJoin_DisposesInnerEvenWithIncompleteOuterIteration()
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
            .LeftJoin(GetInnerSequence().AsValueEnumerable(), o => o, i => i, (o, i) => new { o, i });

        // Only take the first result and then dispose
        var firstResult = joinQuery.First();

        TestUtil.Dispose(joinQuery);

        // Assert
        innerDisposed.ShouldBeTrue();
    }

    [Fact]
    public void LeftJoin_TryGetNonEnumeratedCount_ReturnsFalse()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 2, 3, 4 };

        var leftJoinQuery = outer
            .AsValueEnumerable()
            .LeftJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => new { o, i });

        // Act
        var result = leftJoinQuery.TryGetNonEnumeratedCount(out var count);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void LeftJoin_TryGetSpan_ReturnsFalse()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 2, 3, 4 };

        var leftJoinQuery = outer
            .AsValueEnumerable()
            .LeftJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => new { o, i });

        // Act
        var result = leftJoinQuery.TryGetSpan(out var span);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void LeftJoin_TryCopyTo_ReturnsFalse()
    {
        // Arrange
        var outer = new[] { 1, 2, 3 };
        var inner = new[] { 2, 3, 4 };
        var result = new (int Outer, int? Inner)[4];

        var leftJoinQuery = outer
            .AsValueEnumerable()
            .LeftJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => (Outer: o, Inner: (int?)i));

        // Act
        var copyResult = leftJoinQuery.TryCopyTo(result.AsSpan(), 0);

        // Assert
        copyResult.ShouldBeFalse();
    }

    //[Fact]
    //public void LeftJoin_NullKeyHandling()
    //{
    //    // Arrange
    //    var outer = new[]
    //    {
    //        new { Id = 1, Key = (string)null! },
    //        new { Id = 2, Key = "B" },
    //    };

    //    var inner = new[]
    //    {
    //        new { Id = 101, Key = (string)null! },
    //        new { Id = 102, Key = "C" },
    //    };

    //    // Act
    //    var actual = outer
    //        .AsValueEnumerable()
    //        .LeftJoin(inner.AsValueEnumerable(), o => o.Key, i => i.Key, (o, i) => new { OuterId = o.Id, InnerId = i?.Id })
    //        .ToArray();

    //    // Assert
    //    actual.Length.ShouldBe(2);

    //    // First element (null key) should have matched with the inner element with null key
    //    actual[0].OuterId.ShouldBe(1);
    //    actual[0].InnerId.ShouldBe(101);

    //    // Second element (key "B") should have no match, so inner is null
    //    actual[1].OuterId.ShouldBe(2);
    //    actual[1].InnerId.ShouldBeNull();
    //}

    //[Fact]
    //public void LeftJoin_AllElementsWithNoMatches()
    //{
    //    // Arrange
    //    var outer = new[] { 1, 2, 3 };
    //    var inner = new[] { 4, 5, 6 };

    //    // Act
    //    var actual = outer
    //        .AsValueEnumerable()
    //        .LeftJoin(inner.AsValueEnumerable(), o => o, i => i, (o, i) => $"{o}:{(i.HasValue ? i.ToString() : "null")}")
    //        .ToArray();

    //    // Assert
    //    actual.Length.ShouldBe(3);
    //    actual[0].ShouldBe("1:null");
    //    actual[1].ShouldBe("2:null");
    //    actual[2].ShouldBe("3:null");
    //}

    // key is the substring after the first character, e.g. "a1" -> "1"
    static string KeyOf(string s) => s.Substring(1);

    // key is null if the second character is '-'
    static string? NullableKeyOf(string s) => s[1] == '-' ? null : s.Substring(1);

    /// <summary>
    /// Verifies that LeftJoin without resultSelector yields matched pairs and unmatched outer elements with default inner,
    /// in outer order, for both IEnumerable and ValueEnumerable inner sources.
    /// </summary>
    [Fact]
    public void LeftJoin_Tuple_ReturnsMatchedAndUnmatchedOuter()
    {
        var outer = new[] { "a1", "b2", "c3" };
        var inner = new[] { "x2", "y1", "z2", "w4" };

        var expected = new (string, string?)[] { ("a1", "y1"), ("b2", "x2"), ("b2", "z2"), ("c3", null) };

        outer.AsValueEnumerable().LeftJoin(inner, KeyOf, KeyOf).ToArray().ShouldBe(expected);
        outer.AsValueEnumerable().LeftJoin(inner.AsValueEnumerable(), KeyOf, KeyOf).ToArray().ShouldBe(expected);
    }

    /// <summary>
    /// Verifies that the tuple elements of LeftJoin without resultSelector are accessible by the names Outer and Inner.
    /// </summary>
    [Fact]
    public void LeftJoin_Tuple_HasNamedElements()
    {
        var outer = new[] { "a1", "b2" };
        var inner = new[] { "x1" };

        var actual = outer.AsValueEnumerable().LeftJoin(inner, KeyOf, KeyOf).ToArray();

        actual.Length.ShouldBe(2);
        actual[0].Outer.ShouldBe("a1");
        actual[0].Inner.ShouldBe("x1");
        actual[1].Outer.ShouldBe("b2");
        actual[1].Inner.ShouldBeNull();
    }

    /// <summary>
    /// Verifies that LeftJoin without resultSelector uses the specified comparer to match keys.
    /// </summary>
    [Fact]
    public void LeftJoin_Tuple_WithComparer()
    {
        var outer = new[] { "A", "B", "C" };
        var inner = new[] { "a", "b", "b", "d" };

        var actual = outer.AsValueEnumerable().LeftJoin(inner, o => o, i => i, StringComparer.OrdinalIgnoreCase).ToArray();

        actual.ShouldBe(new (string, string?)[] { ("A", "a"), ("B", "b"), ("B", "b"), ("C", null) });
    }

    /// <summary>
    /// Verifies that an outer element with a null key never matches and is yielded with default inner.
    /// </summary>
    [Fact]
    public void LeftJoin_Tuple_NullOuterKey_YieldsDefaultInner()
    {
        var outer = new[] { "o1", "o-" };
        var inner = new[] { "i-", "i1" };

        var actual = outer.AsValueEnumerable().LeftJoin(inner, NullableKeyOf, NullableKeyOf).ToArray();

        actual.ShouldBe(new (string, string?)[] { ("o1", "i1"), ("o-", null) });
    }

    /// <summary>
    /// Verifies that LeftJoin without resultSelector throws ArgumentNullException for null inner and key selectors.
    /// </summary>
    [Fact]
    public void LeftJoin_Tuple_NullArguments_Throw()
    {
        var outer = new[] { "a1" };
        var inner = new[] { "x1" };

        Should.Throw<ArgumentNullException>(() => outer.AsValueEnumerable().LeftJoin((IEnumerable<string>)null!, KeyOf, KeyOf));
        Should.Throw<ArgumentNullException>(() => outer.AsValueEnumerable().LeftJoin(inner, null!, KeyOf));
        Should.Throw<ArgumentNullException>(() => outer.AsValueEnumerable().LeftJoin(inner, KeyOf, null!));
    }

    /// <summary>
    /// Verifies that LeftJoin without resultSelector disposes both outer and inner sources,
    /// both after full enumeration and after early termination while outer is still being enumerated.
    /// </summary>
    [Fact]
    public void LeftJoin_Tuple_DisposesBothSources()
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
                yield return "y3";
            }
            finally
            {
                innerDisposed = true;
            }
        }

        GetOuterSequence().AsValueEnumerable().LeftJoin(GetInnerSequence(), KeyOf, KeyOf).ToArray();
        outerDisposed.ShouldBeTrue();
        innerDisposed.ShouldBeTrue();

        outerDisposed = false;
        innerDisposed = false;
        using (var e = GetOuterSequence().AsValueEnumerable().LeftJoin(GetInnerSequence(), KeyOf, KeyOf).Enumerator)
        {
            e.TryGetNext(out _).ShouldBeTrue();
        }
        outerDisposed.ShouldBeTrue();
        innerDisposed.ShouldBeTrue();
    }

    /// <summary>
    /// Verifies that LeftJoin without resultSelector does not report a non-enumerated count, span, or copy capability,
    /// because the result count cannot be known without joining.
    /// </summary>
    [Fact]
    public void LeftJoin_Tuple_OptimizationMethods_ReturnFalse()
    {
        var outer = new[] { "a1" };
        var inner = new[] { "x1" };

        var query = outer.AsValueEnumerable().LeftJoin(inner, KeyOf, KeyOf);
        query.TryGetNonEnumeratedCount(out _).ShouldBeFalse();
        query.TryGetSpan(out _).ShouldBeFalse();
        query.TryCopyTo(new (string, string?)[1].AsSpan(), 0).ShouldBeFalse();
    }
}
