namespace ZLinq.Tests.Linq;

public class CopyToTest
{
    #region CopyTo_Span

    [Fact]
    public void CopyTo_Span_EmptySource()
    {
        // Arrange
        var source = Array.Empty<int>().AsValueEnumerable();
        var dest = new int[5];
        Array.Fill(dest, -1);

        // Act
        var elementsCopied = source.CopyTo(dest.AsSpan());

        // Assert
        elementsCopied.ShouldBe(0);
        dest.ShouldBe(new[] { -1, -1, -1, -1, -1 });
    }

    [Fact]
    public void CopyTo_Span_SmallDestination()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 }.AsValueEnumerable();
        var dest = new int[3];

        // Act
        var elementsCopied = source.CopyTo(dest.AsSpan());

        // Assert
        elementsCopied.ShouldBe(3);
        dest.ShouldBe(new[] { 1, 2, 3 });
    }

    [Fact]
    public void CopyTo_Span_ExactSizeDestination()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 }.AsValueEnumerable();
        var dest = new int[5];

        // Act
        var elementsCopied = source.CopyTo(dest.AsSpan());

        // Assert
        elementsCopied.ShouldBe(5);
        dest.ShouldBe(new[] { 1, 2, 3, 4, 5 });
    }

    [Fact]
    public void CopyTo_Span_LargeDestination()
    {
        // Arrange
        var source = new[] { 1, 2, 3 }.AsValueEnumerable();
        var dest = new int[5];
        Array.Fill(dest, -1);

        // Act
        var elementsCopied = source.CopyTo(dest.AsSpan());

        // Assert
        elementsCopied.ShouldBe(3);
        dest.ShouldBe(new[] { 1, 2, 3, -1, -1 });
    }

    [Fact]
    public void CopyTo_Span_EmptyDestination()
    {
        // Arrange
        var source = new[] { 1, 2, 3 }.AsValueEnumerable();
        var dest = Span<int>.Empty;

        // Act
        var elementsCopied = source.CopyTo(dest);

        // Assert
        elementsCopied.ShouldBe(0);
    }

    [Fact]
    public void CopyTo_Span_EmptyDestination_FilteredSource()
    {
        var source = new[] { 1, 2, 3 }.AsValueEnumerable().Where(x => x > 0);

        source.CopyTo(Span<int>.Empty).ShouldBe(0);
    }

    [Fact]
    public void CopyTo_Span_EmptyDestination_QueueSource()
    {
        var source = new Queue<int>(new[] { 1, 2, 3 }).AsValueEnumerable();

        source.CopyTo(Span<int>.Empty).ShouldBe(0);
    }

    [Fact]
    public void CopyTo_Span_WithTryGetNonEnumeratedCount()
    {
        // Arrange - use a source that can report its count
        var source = ValueEnumerable.Range(1, 5);
        var dest = new int[3];

        // Act
        var elementsCopied = source.CopyTo(dest.AsSpan());

        // Assert
        elementsCopied.ShouldBe(3);
        dest.ShouldBe(new[] { 1, 2, 3 });
    }

    [Fact]
    public void CopyTo_Span_WithTryCopyTo()
    {
        // Arrange - use a source that can directly copy to span
        var source = new[] { 1, 2, 3, 4, 5 }.AsValueEnumerable();
        var dest = new int[3];

        // Act
        var elementsCopied = source.CopyTo(dest.AsSpan());

        // Assert
        elementsCopied.ShouldBe(3);
        dest.ShouldBe(new[] { 1, 2, 3 });
    }

    [Fact]
    public void CopyTo_Span_FallbackPath()
    {
        // Arrange - use a source that can't report count or directly copy
        var source = TestUtil.ToValueEnumerable(new[] { 1, 2, 3, 4, 5 });
        var dest = new int[3];

        // Act
        var elementsCopied = source.CopyTo(dest.AsSpan());

        // Assert
        elementsCopied.ShouldBe(3);
        dest.ShouldBe(new[] { 1, 2, 3 });
    }

    #endregion

    #region CopyTo_List

    [Fact]
    public void CopyTo_List_EmptySource()
    {
        // Arrange
        var source = Array.Empty<int>().AsValueEnumerable();
        var list = new List<int> { -1, -2, -3 };

        // Act
        source.CopyTo(list);

        // Assert
        list.ShouldBeEmpty();
    }

    [Fact]
    public void CopyTo_List_NonEmptySource()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 }.AsValueEnumerable();
        var list = new List<int> { -1, -2, -3 };

        // Act
        source.CopyTo(list);

        // Assert
        list.ShouldBe(new[] { 1, 2, 3, 4, 5 });
    }

    [Fact]
    public void CopyTo_List_WithTryGetNonEnumeratedCount()
    {
        // Arrange - use a source that can report its count
        var source = ValueEnumerable.Range(1, 5);
        var list = new List<int> { -1, -2, -3 };

        // Act
        source.CopyTo(list);

        // Assert
        list.ShouldBe(new[] { 1, 2, 3, 4, 5 });
    }

    [Fact]
    public void CopyTo_List_WithTryCopyTo()
    {
        // Arrange - use a source that can directly copy to span
        var source = new[] { 1, 2, 3, 4, 5 }.AsValueEnumerable();
        var list = new List<int> { -1, -2, -3 };

        // Act
        source.CopyTo(list);

        // Assert
        list.ShouldBe(new[] { 1, 2, 3, 4, 5 });
    }

    [Fact]
    public void CopyTo_List_FallbackPath()
    {
        // Arrange - use a source that can't report count or directly copy
        var source = TestUtil.ToValueEnumerable(new[] { 1, 2, 3, 4, 5 });
        var list = new List<int> { -1, -2, -3 };

        // Act
        source.CopyTo(list);

        // Assert
        list.ShouldBe(new[] { 1, 2, 3, 4, 5 });
    }

    [Fact]
    public void CopyTo_List_NullList_ThrowsArgumentNullException()
    {
        // Arrange
        List<int>? nullList = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new[] { 1, 2, 3 }.AsValueEnumerable().CopyTo(nullList!));
    }

    [Fact]
    public void CopyTo_List_GrowsCapacityIfNeeded()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }.AsValueEnumerable();
        var list = new List<int>(2); // Initial capacity too small

        // Act
        source.CopyTo(list);

        // Assert
        list.ShouldBe(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        list.Capacity.ShouldBeGreaterThanOrEqualTo(10);
    }

    [Fact]
    public void CopyTo_List_PreservesCapacityIfLargeEnough()
    {
        // Arrange
        var source = new[] { 1, 2, 3 }.AsValueEnumerable();
        var list = new List<int>(10); // Capacity larger than needed
        var originalCapacity = list.Capacity;

        // Act
        source.CopyTo(list);

        // Assert
        list.ShouldBe(new[] { 1, 2, 3 });
        list.Capacity.ShouldBe(originalCapacity); // Capacity should not change
    }

    #endregion
}
