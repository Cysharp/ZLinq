namespace ZLinq.Tests.Linq;

public class FullJoinTest
{
    // key is the second character, e.g. "a1" -> "1"
    static string KeyOf(string s) => s.Substring(1);

    // key is null if the second character is '-'
    static string? NullableKeyOf(string s) => s[1] == '-' ? null : s.Substring(1);

    /// <summary>
    /// Verifies that FullJoin without resultSelector yields matched pairs and unmatched outer elements in outer order,
    /// followed by unmatched inner elements, for both IEnumerable and ValueEnumerable inner sources.
    /// </summary>
    [Fact]
    public void FullJoin_Tuple_ReturnsMatchedAndUnmatchedElements()
    {
        var outer = new[] { "a1", "b2", "c3" };
        var inner = new[] { "x2", "y1", "z2", "w4" };

        var expected = new (string?, string?)[] { ("a1", "y1"), ("b2", "x2"), ("b2", "z2"), ("c3", null), (null, "w4") };

        outer.AsValueEnumerable().FullJoin(inner, KeyOf, KeyOf).ToArray().ShouldBe(expected);
        outer.AsValueEnumerable().FullJoin(inner.AsValueEnumerable(), KeyOf, KeyOf).ToArray().ShouldBe(expected);
    }

    /// <summary>
    /// Verifies that the tuple elements of FullJoin without resultSelector are accessible by the names Outer and Inner.
    /// </summary>
    [Fact]
    public void FullJoin_Tuple_HasNamedElements()
    {
        var outer = new[] { "a1" };
        var inner = new[] { "x1" };

        var actual = outer.AsValueEnumerable().FullJoin(inner, KeyOf, KeyOf).ToArray();

        actual.Length.ShouldBe(1);
        actual[0].Outer.ShouldBe("a1");
        actual[0].Inner.ShouldBe("x1");
    }

    /// <summary>
    /// Verifies that FullJoin with resultSelector passes default for the missing side,
    /// and yields the same sequence as the tuple overload.
    /// </summary>
    [Fact]
    public void FullJoin_ResultSelector_ReturnsMatchedAndUnmatchedElements()
    {
        var outer = new[] { "a1", "b2", "c3" };
        var inner = new[] { "x2", "y1", "z2", "w4" };

        var expected = new[] { "a1:y1", "b2:x2", "b2:z2", "c3:", ":w4" };

        outer.AsValueEnumerable().FullJoin(inner, KeyOf, KeyOf, (o, i) => $"{o}:{i}").ToArray().ShouldBe(expected);
        outer.AsValueEnumerable().FullJoin(inner.AsValueEnumerable(), KeyOf, KeyOf, (o, i) => $"{o}:{i}").ToArray().ShouldBe(expected);
    }

    /// <summary>
    /// Verifies that FullJoin passes default (not null) for the missing side when the element type is a value type.
    /// </summary>
    [Fact]
    public void FullJoin_ValueType_UsesDefaultForMissingSide()
    {
        var outer = new[] { 1, 2 };
        var inner = new[] { 2, 3 };

        var actual = outer.AsValueEnumerable().FullJoin(inner, o => o, i => i).ToArray();

        actual.ShouldBe(new[] { (1, 0), (2, 2), (0, 3) });
    }

    /// <summary>
    /// Verifies that FullJoin uses the specified comparer to match keys, in both overloads.
    /// </summary>
    [Fact]
    public void FullJoin_WithComparer()
    {
        var outer = new[] { "A", "B", "C" };
        var inner = new[] { "a", "b", "b", "d" };
        var comparer = StringComparer.OrdinalIgnoreCase;

        outer.AsValueEnumerable().FullJoin(inner, o => o, i => i, comparer).ToArray()
            .ShouldBe(new (string?, string?)[] { ("A", "a"), ("B", "b"), ("B", "b"), ("C", null), (null, "d") });

        outer.AsValueEnumerable().FullJoin(inner, o => o, i => i, (o, i) => $"{o}:{i}", comparer).ToArray()
            .ShouldBe(new[] { "A:a", "B:b", "B:b", "C:", ":d" });
    }

    /// <summary>
    /// Verifies that null keys never match, and that elements with null keys on both sides are still yielded as unmatched,
    /// which is the behavior of System.Linq's FullJoin.
    /// </summary>
    [Fact]
    public void FullJoin_NullKeys_AreYieldedAsUnmatched()
    {
        var outer = new[] { "o1", "o-" };
        var inner = new[] { "i-", "i1" };

        var actual = outer.AsValueEnumerable().FullJoin(inner, NullableKeyOf, NullableKeyOf).ToArray();

        actual.ShouldBe(new (string?, string?)[] { ("o1", "i1"), ("o-", null), (null, "i-") });
    }

    /// <summary>
    /// Verifies that unmatched inner elements are yielded grouped by key in the order the keys first appear in inner,
    /// which is the same order as System.Linq's FullJoin.
    /// </summary>
    [Fact]
    public void FullJoin_UnmatchedInner_AreGroupedByKey()
    {
        var outer = new[] { "a1" };
        var inner = new[] { "p9", "q8", "r9" };

        var actual = outer.AsValueEnumerable().FullJoin(inner, KeyOf, KeyOf).ToArray();

        actual.ShouldBe(new (string?, string?)[] { ("a1", null), (null, "p9"), (null, "r9"), (null, "q8") });
    }

    /// <summary>
    /// Verifies that FullJoin yields all inner elements when outer is empty.
    /// </summary>
    [Fact]
    public void FullJoin_EmptyOuter_ReturnsAllInner()
    {
        var outer = new List<string>();
        var inner = new[] { "x1", "y2" };

        var actual = outer.AsValueEnumerable().FullJoin(inner, KeyOf, KeyOf).ToArray();

        actual.ShouldBe(new (string?, string?)[] { (null, "x1"), (null, "y2") });
    }

    /// <summary>
    /// Verifies that FullJoin yields inner elements in their source order (not grouped by key) when outer is an empty array,
    /// which is the same order as System.Linq's FullJoin, for both overloads and for both IEnumerable and ValueEnumerable inner sources.
    /// </summary>
    [Fact]
    public void FullJoin_EmptyOuterArray_YieldsInnerInSourceOrder()
    {
        var outer = Array.Empty<string>();
        var inner = new[] { "p9", "q8", "r9" };

        var expected = new (string?, string?)[] { (null, "p9"), (null, "q8"), (null, "r9") };

        outer.AsValueEnumerable().FullJoin(inner, KeyOf, KeyOf).ToArray().ShouldBe(expected);
        outer.AsValueEnumerable().FullJoin(inner.AsValueEnumerable(), KeyOf, KeyOf).ToArray().ShouldBe(expected);
        outer.AsValueEnumerable().FullJoin(inner, KeyOf, KeyOf, (o, i) => $"{o}:{i}").ToArray().ShouldBe(new[] { ":p9", ":q8", ":r9" });
    }

    /// <summary>
    /// Verifies that FullJoin also yields inner elements in their source order when outer is an empty array
    /// typed as IEnumerable, because System.Linq's FullJoin checks the runtime type of outer.
    /// </summary>
    [Fact]
    public void FullJoin_EmptyOuterArrayAsEnumerable_YieldsInnerInSourceOrder()
    {
        IEnumerable<string> outer = Array.Empty<string>();
        var inner = new[] { "p9", "q8", "r9" };

        outer.AsValueEnumerable().FullJoin(inner, KeyOf, KeyOf).ToArray()
            .ShouldBe(new (string?, string?)[] { (null, "p9"), (null, "q8"), (null, "r9") });
        outer.AsValueEnumerable().FullJoin(inner, KeyOf, KeyOf, (o, i) => $"{o}:{i}").ToArray()
            .ShouldBe(new[] { ":p9", ":q8", ":r9" });
    }

    /// <summary>
    /// Verifies that FullJoin yields inner elements grouped by key when outer is empty but not an array,
    /// which is the same order as System.Linq's FullJoin (it only special-cases empty arrays).
    /// </summary>
    [Fact]
    public void FullJoin_EmptyOuterNonArray_YieldsInnerGroupedByKey()
    {
        var outer = new List<string>();
        var inner = new[] { "p9", "q8", "r9" };

        outer.AsValueEnumerable().FullJoin(inner, KeyOf, KeyOf).ToArray()
            .ShouldBe(new (string?, string?)[] { (null, "p9"), (null, "r9"), (null, "q8") });
        outer.AsValueEnumerable().FullJoin(inner, KeyOf, KeyOf, (o, i) => $"{o}:{i}").ToArray()
            .ShouldBe(new[] { ":p9", ":r9", ":q8" });
    }

    /// <summary>
    /// Verifies that FullJoin does not invoke the key selectors when outer is an empty array,
    /// which is the same behavior as System.Linq's FullJoin.
    /// </summary>
    [Fact]
    public void FullJoin_EmptyOuterArray_DoesNotInvokeKeySelectors()
    {
        var outer = Array.Empty<string>();
        var inner = new[] { "p9", "q8" };
        var invoked = 0;

        string CountingKeyOf(string s)
        {
            invoked++;
            return KeyOf(s);
        }

        outer.AsValueEnumerable().FullJoin(inner, CountingKeyOf, CountingKeyOf).ToArray();
        outer.AsValueEnumerable().FullJoin(inner, CountingKeyOf, CountingKeyOf, (o, i) => $"{o}:{i}").ToArray();

        invoked.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that FullJoin disposes both sources when outer is an empty array and inner is streamed without building a lookup,
    /// both after full enumeration and after early termination, for both overloads.
    /// </summary>
    [Fact]
    public void FullJoin_EmptyOuterArray_DisposesInner()
    {
        var innerDisposed = false;

        IEnumerable<string> GetInnerSequence()
        {
            try
            {
                yield return "p9";
                yield return "q8";
            }
            finally
            {
                innerDisposed = true;
            }
        }

        var outer = Array.Empty<string>();

        outer.AsValueEnumerable().FullJoin(GetInnerSequence(), KeyOf, KeyOf).ToArray();
        innerDisposed.ShouldBeTrue();

        innerDisposed = false;
        outer.AsValueEnumerable().FullJoin(GetInnerSequence(), KeyOf, KeyOf, (o, i) => $"{o}:{i}").ToArray();
        innerDisposed.ShouldBeTrue();

        innerDisposed = false;
        var tupleQuery = outer.AsValueEnumerable().FullJoin(GetInnerSequence(), KeyOf, KeyOf);
        tupleQuery.First().ShouldBe(((string?)null, "p9"));
        TestUtil.Dispose(tupleQuery);
        innerDisposed.ShouldBeTrue();

        innerDisposed = false;
        var selectorQuery = outer.AsValueEnumerable().FullJoin(GetInnerSequence(), KeyOf, KeyOf, (o, i) => $"{o}:{i}");
        selectorQuery.First().ShouldBe(":p9");
        TestUtil.Dispose(selectorQuery);
        innerDisposed.ShouldBeTrue();
    }

    /// <summary>
    /// Verifies that FullJoin yields all outer elements when inner is empty.
    /// </summary>
    [Fact]
    public void FullJoin_EmptyInner_ReturnsAllOuter()
    {
        var outer = new[] { "a1", "b2" };
        var inner = Array.Empty<string>();

        var actual = outer.AsValueEnumerable().FullJoin(inner, KeyOf, KeyOf).ToArray();

        actual.ShouldBe(new (string?, string?)[] { ("a1", null), ("b2", null) });
    }

    /// <summary>
    /// Verifies that FullJoin yields nothing when both sources are empty.
    /// </summary>
    [Fact]
    public void FullJoin_BothEmpty_ReturnsEmpty()
    {
        var outer = Array.Empty<string>();
        var inner = Array.Empty<string>();

        outer.AsValueEnumerable().FullJoin(inner, KeyOf, KeyOf).ToArray().ShouldBeEmpty();
        outer.AsValueEnumerable().FullJoin(inner, KeyOf, KeyOf, (o, i) => $"{o}:{i}").ToArray().ShouldBeEmpty();
    }

    /// <summary>
    /// Verifies that TryGetNext keeps returning false after the enumeration has completed.
    /// </summary>
    [Fact]
    public void FullJoin_TryGetNext_ReturnsFalseAfterCompleted()
    {
        var outer = new[] { "a1" };
        var inner = new[] { "x2" };

        using var e = outer.AsValueEnumerable().FullJoin(inner, KeyOf, KeyOf).Enumerator;

        e.TryGetNext(out var first).ShouldBeTrue();
        first.ShouldBe(("a1", (string?)null));
        e.TryGetNext(out var second).ShouldBeTrue();
        second.ShouldBe(((string?)null, "x2"));
        e.TryGetNext(out _).ShouldBeFalse();
        e.TryGetNext(out _).ShouldBeFalse();
    }

    /// <summary>
    /// Verifies that FullJoin does not report a non-enumerated count, span, or copy capability,
    /// because the result count cannot be known without joining.
    /// </summary>
    [Fact]
    public void FullJoin_OptimizationMethods_ReturnFalse()
    {
        var outer = new[] { "a1" };
        var inner = new[] { "x1" };

        var tupleQuery = outer.AsValueEnumerable().FullJoin(inner, KeyOf, KeyOf);
        tupleQuery.TryGetNonEnumeratedCount(out _).ShouldBeFalse();
        tupleQuery.TryGetSpan(out _).ShouldBeFalse();
        tupleQuery.TryCopyTo(new (string?, string?)[1].AsSpan(), 0).ShouldBeFalse();

        var selectorQuery = outer.AsValueEnumerable().FullJoin(inner, KeyOf, KeyOf, (o, i) => o + i);
        selectorQuery.TryGetNonEnumeratedCount(out _).ShouldBeFalse();
        selectorQuery.TryGetSpan(out _).ShouldBeFalse();
        selectorQuery.TryCopyTo(new string[1].AsSpan(), 0).ShouldBeFalse();
    }

    /// <summary>
    /// Verifies that FullJoin throws ArgumentNullException for null inner, key selectors, and result selector.
    /// </summary>
    [Fact]
    public void FullJoin_NullArguments_Throw()
    {
        var outer = new[] { "a1" };
        var inner = new[] { "x1" };

        Should.Throw<ArgumentNullException>(() => outer.AsValueEnumerable().FullJoin((IEnumerable<string>)null!, KeyOf, KeyOf));
        Should.Throw<ArgumentNullException>(() => outer.AsValueEnumerable().FullJoin(inner, null!, KeyOf));
        Should.Throw<ArgumentNullException>(() => outer.AsValueEnumerable().FullJoin(inner, KeyOf, null!));
        Should.Throw<ArgumentNullException>(() => outer.AsValueEnumerable().FullJoin(inner, KeyOf, KeyOf, (Func<string?, string?, string>)null!));
    }
}
