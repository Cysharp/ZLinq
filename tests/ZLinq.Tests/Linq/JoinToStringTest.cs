namespace ZLinq.Tests.Linq;

public class JoinToStringTest
{
    [Fact]
    public void JoinToString_SingleNullElement_ReturnsEmptyString()
    {
        var array = new object?[] { null };
        var expected = string.Join(",", array);

        array.AsValueEnumerable().JoinToString(",").ShouldBe(expected);
        array.AsValueEnumerable().JoinToString(',').ShouldBe(expected);
        array.ToList().AsValueEnumerable().JoinToString(",").ShouldBe(expected);
        array.ToList().AsValueEnumerable().JoinToString(',').ShouldBe(expected);
        array.ToValueEnumerable().JoinToString(",").ShouldBe(expected);
        new Uri?[] { null }.AsValueEnumerable().JoinToString(",").ShouldBe(expected);
    }

    [Fact]
    public void JoinToString_EmptySequence_ReturnsEmptyString()
    {
        // Arrange
        var empty = Array.Empty<int>();

        // Act
        var result = empty.AsValueEnumerable().JoinToString(",");

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public void JoinToString_SingleItem_ReturnsItemAsString()
    {
        // Arrange
        var singleItem = new[] { 42 };

        // Act
        var result = singleItem.AsValueEnumerable().JoinToString(",");

        // Assert
        result.ShouldBe("42");
    }

    [Fact]
    public void JoinToString_EmptySequence_EmptySeparator()
    {
        // Arrange
        var empty = Array.Empty<int>();

        // Act
        var result = empty.AsValueEnumerable().JoinToString("");

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public void JoinToString_SingleItem_EmptySeparator()
    {
        // Arrange
        var singleItem = new[] { 42 };

        // Act
        var result = singleItem.AsValueEnumerable().JoinToString("");

        // Assert
        result.ShouldBe("42");
    }

    [Fact]
    public void JoinToString_MultipleItems_WithNonEmptySeparator()
    {
        // Arrange
        var items = new[] { 1, 2, 3, 4, 5 };
        var separator = ", ";

        // Act - ZLinq
        var actual = items.AsValueEnumerable().JoinToString(separator);

        // Act - Standard approach for comparison
        var expected = string.Join(separator, items);

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void JoinToString_MultipleItems_WithEmptySeparator()
    {
        // Arrange
        var items = new[] { 1, 2, 3, 4, 5 };
        var separator = "";

        // Act - ZLinq
        var actual = items.AsValueEnumerable().JoinToString(separator);

        // Act - Standard approach for comparison
        var expected = string.Join(separator, items);

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void JoinToString_WithNullValues_HandlesNullsCorrectly()
    {
        // Arrange
        var items = new string?[] { "a", null, "c", null, "e" };
        var separator = "-";

        // Act - ZLinq
        var actual = items.AsValueEnumerable().JoinToString(separator);

        // Act - Standard approach for comparison
        var expected = string.Join(separator, items);

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void JoinToString_WithComplexObjects_UsesToStringMethod()
    {
        // Arrange
        var items = new[]
        {
            new Person { Name = "Alice", Age = 30 },
            new Person { Name = "Bob", Age = 25 },
            new Person { Name = "Charlie", Age = 35 }
        };
        var separator = "; ";

        // Act - ZLinq
        var actual = items.AsValueEnumerable().JoinToString(separator);

        // Act - Standard approach for comparison
        var expected = string.Join(separator, items.AsEnumerable());

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void JoinToString_WithSpanOptimization_ReturnsCorrectResult()
    {
        // This test specifically targets the TryGetSpan optimization path
        // Arrange
        var items = new[] { 10, 20, 30, 40, 50 };
        var separator = ",";

        // Act - ZLinq
        var actual = items.AsValueEnumerable().JoinToString(separator);

        // Act - Standard approach for comparison
        var expected = string.Join(separator, items);

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void JoinToString_EnsuresDisposal()
    {
        // Arrange
        var disposed = false;

        IEnumerable<int> GetSequence()
        {
            try
            {
                yield return 1;
                yield return 2;
                yield return 3;
            }
            finally
            {
                disposed = true;
            }
        }

        // Act
        var result = GetSequence().AsValueEnumerable().JoinToString(",");

        // Assert
        disposed.ShouldBeTrue();
        result.ShouldBe("1,2,3");
    }

    [Fact]
    public void JoinToString_LargerThanStackallocLimit_WorksCorrectly()
    {
        // This test verifies that strings larger than the stackalloc buffer limit work correctly
        // Arrange
        var items = Enumerable.Range(1, 1000).ToArray(); // Much larger than the StackallocCharBufferSizeLimit
        var separator = ",";

        // Act - ZLinq
        var actual = items.AsValueEnumerable().JoinToString(separator);

        // Act - Standard approach for comparison
        var expected = string.Join(separator, items);

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void JoinToString_Enumerable_MultipleItem()
    {
        // Arrange
        var valueEnumerable = GetSource().AsValueEnumerable();

        // Char separator
        {
            // Act
            var actual = valueEnumerable.JoinToString(',');

            // Assert
            var expected = string.Join(",", GetSource());
            actual.ShouldBe(expected);
        }

        // Empty separator
        {
            // Act
            var actual = valueEnumerable.JoinToString("");

            // Assert
            var expected = string.Join("", GetSource());
            actual.ShouldBe(expected);
        }

        static IEnumerable<int> GetSource()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }
    }

    [Fact]
    public void JoinToString_Enumerable_SingleItem()
    {
        var valueEnumerable = GetSource().AsValueEnumerable();

        // Char separator
        {
            // Act
            var actual = valueEnumerable.JoinToString(',');

            // Assert
            var expected = string.Join(",", GetSource());
            actual.ShouldBe("1");
        }
        // Empty Separator
        {
            // Act
            var actual = valueEnumerable.JoinToString("");

            // Assert
            var expected = string.Join("", GetSource());
            actual.ShouldBe("1");
        }

        static IEnumerable<int> GetSource()
        {
            yield return 1;
        }
    }

    [Fact]
    public void JoinToString_Enumerable_EmptySource()
    {
        // Arrange
        var valueEnumerable = GetEmptySource().AsValueEnumerable();

        // Act
        var actual = valueEnumerable.JoinToString(',');

        // Assert
        actual.ShouldBe("");

        static IEnumerable<int> GetEmptySource()
        {
            yield break;
        }
    }

    private class Person
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }

        public override string ToString() => $"{Name} ({Age})";
    }


    [Fact]
    public void JoinToString_ArraySelect_WithVariousSeparators()
    {
        // Arrange
        var items = new[] { 1, 2, 3, 4, 5 };
        var selector = (int x) => x * 10;

        // Act - With string separator
        var actualString = items.AsValueEnumerable().Select(selector).JoinToString(", ");

        // Act - With char separator
        var actualChar = items.AsValueEnumerable().Select(selector).JoinToString(',');

        // Act - With empty separator
        var actualEmpty = items.AsValueEnumerable().Select(selector).JoinToString("");

        // Assert
        var expectedString = string.Join(", ", items.Select(selector));
        var expectedChar = string.Join(",", items.Select(selector));
        var expectedEmpty = string.Join("", items.Select(selector));

        actualString.ShouldBe(expectedString);
        actualChar.ShouldBe(expectedChar);
        actualEmpty.ShouldBe(expectedEmpty);
    }

    [Fact]
    public void JoinToString_ArrayWhere_WithVariousSeparators()
    {
        // Arrange
        var items = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var predicate = (int x) => x % 2 == 0;

        // Act - With string separator
        var actualString = items.AsValueEnumerable().Where(predicate).JoinToString(", ");

        // Act - With char separator
        var actualChar = items.AsValueEnumerable().Where(predicate).JoinToString(',');

        // Act - With empty separator
        var actualEmpty = items.AsValueEnumerable().Where(predicate).JoinToString("");

        // Assert
        var expectedString = string.Join(", ", items.Where(predicate));
        var expectedChar = string.Join(",", items.Where(predicate));
        var expectedEmpty = string.Join("", items.Where(predicate));

        actualString.ShouldBe(expectedString);
        actualChar.ShouldBe(expectedChar);
        actualEmpty.ShouldBe(expectedEmpty);
    }

    [Fact]
    public void JoinToString_ArrayWhereSelect_WithVariousSeparators()
    {
        // Arrange
        var items = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var predicate = (int x) => x % 2 == 0;
        var selector = (int x) => $"Item-{x}";

        // Act - With string separator
        var actualString = items.AsValueEnumerable().Where(predicate).Select(selector).JoinToString(" | ");

        // Act - With char separator
        var actualChar = items.AsValueEnumerable().Where(predicate).Select(selector).JoinToString('|');

        // Act - With empty separator
        var actualEmpty = items.AsValueEnumerable().Where(predicate).Select(selector).JoinToString("");

        // Assert
        var expectedString = string.Join(" | ", items.Where(predicate).Select(selector));
        var expectedChar = string.Join("|", items.Where(predicate).Select(selector));
        var expectedEmpty = string.Join("", items.Where(predicate).Select(selector));

        actualString.ShouldBe(expectedString);
        actualChar.ShouldBe(expectedChar);
        actualEmpty.ShouldBe(expectedEmpty);
    }

    [Fact]
    public void JoinToString_ListSelect_WithVariousSeparators()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3, 4, 5 };
        var selector = (int x) => x * 10;

        // Act - With string separator
        var actualString = items.AsValueEnumerable().Select(selector).JoinToString(", ");

        // Act - With char separator
        var actualChar = items.AsValueEnumerable().Select(selector).JoinToString(',');

        // Act - With empty separator
        var actualEmpty = items.AsValueEnumerable().Select(selector).JoinToString("");

        // Assert
        var expectedString = string.Join(", ", items.Select(selector));
        var expectedChar = string.Join(",", items.Select(selector));
        var expectedEmpty = string.Join("", items.Select(selector));

        actualString.ShouldBe(expectedString);
        actualChar.ShouldBe(expectedChar);
        actualEmpty.ShouldBe(expectedEmpty);
    }

    [Fact]
    public void JoinToString_ListWhere_WithVariousSeparators()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var predicate = (int x) => x % 2 == 0;

        // Act - With string separator
        var actualString = items.AsValueEnumerable().Where(predicate).JoinToString(", ");

        // Act - With char separator
        var actualChar = items.AsValueEnumerable().Where(predicate).JoinToString(',');

        // Act - With empty separator
        var actualEmpty = items.AsValueEnumerable().Where(predicate).JoinToString("");

        // Assert
        var expectedString = string.Join(", ", items.Where(predicate));
        var expectedChar = string.Join(",", items.Where(predicate));
        var expectedEmpty = string.Join("", items.Where(predicate));

        actualString.ShouldBe(expectedString);
        actualChar.ShouldBe(expectedChar);
        actualEmpty.ShouldBe(expectedEmpty);
    }

    [Fact]
    public void JoinToString_ListWhereSelect_WithVariousSeparators()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var predicate = (int x) => x % 2 == 0;
        var selector = (int x) => $"Item-{x}";

        // Act - With string separator
        var actualString = items.AsValueEnumerable().Where(predicate).Select(selector).JoinToString(" | ");

        // Act - With char separator
        var actualChar = items.AsValueEnumerable().Where(predicate).Select(selector).JoinToString('|');

        // Act - With empty separator
        var actualEmpty = items.AsValueEnumerable().Where(predicate).Select(selector).JoinToString("");

        // Assert
        var expectedString = string.Join(" | ", items.Where(predicate).Select(selector));
        var expectedChar = string.Join("|", items.Where(predicate).Select(selector));
        var expectedEmpty = string.Join("", items.Where(predicate).Select(selector));

        actualString.ShouldBe(expectedString);
        actualChar.ShouldBe(expectedChar);
        actualEmpty.ShouldBe(expectedEmpty);
    }

    [Fact]
    public void JoinToString_StringArrayOptimization()
    {
        // Arrange
        var stringArray = new[] { "apple", "banana", "cherry", "date", "elderberry" };

        // Act - With string separator
        var actualString = stringArray.AsValueEnumerable().JoinToString(", ");

        // Act - With char separator
        var actualChar = stringArray.AsValueEnumerable().JoinToString(',');

        // Act - With empty separator
        var actualEmpty = stringArray.AsValueEnumerable().JoinToString("");

        // Assert
        var expectedString = string.Join(", ", stringArray);
        var expectedChar = string.Join(",", stringArray);
        var expectedEmpty = string.Join("", stringArray);

        actualString.ShouldBe(expectedString);
        actualChar.ShouldBe(expectedChar);
        actualEmpty.ShouldBe(expectedEmpty);
    }

    [Fact]
    public void JoinToString_StringList_Optimization()
    {
        // Arrange
        var stringList = new List<string> { "apple", "banana", "cherry", "date", "elderberry" };

        // Act - With string separator
        var actualString = stringList.AsValueEnumerable().JoinToString(", ");

        // Act - With char separator
        var actualChar = stringList.AsValueEnumerable().JoinToString(',');

        // Act - With empty separator
        var actualEmpty = stringList.AsValueEnumerable().JoinToString("");

        // Assert
        var expectedString = string.Join(", ", stringList);
        var expectedChar = string.Join(",", stringList);
        var expectedEmpty = string.Join("", stringList);

        actualString.ShouldBe(expectedString);
        actualChar.ShouldBe(expectedChar);
        actualEmpty.ShouldBe(expectedEmpty);
    }

    [Fact]
    public void JoinToString_EdgeCases_WithOptimizedPaths()
    {
        // Empty array with select
        var emptyArraySelect = Array.Empty<int>().AsValueEnumerable().Select(x => x * 2).JoinToString(",");
        emptyArraySelect.ShouldBe("");

        // Single item array with select
        var singleArraySelect = new[] { 42 }.AsValueEnumerable().Select(x => x * 2).JoinToString(",");
        singleArraySelect.ShouldBe("84");

        // Empty array with where
        var emptyArrayWhere = Array.Empty<int>().AsValueEnumerable().Where(x => x > 0).JoinToString(",");
        emptyArrayWhere.ShouldBe("");

        // Array with where that filters all items
        var allFilteredArrayWhere = new[] { 1, 2, 3 }.AsValueEnumerable().Where(x => x > 10).JoinToString(",");
        allFilteredArrayWhere.ShouldBe("");

        // Empty list with select
        var emptyListSelect = new List<int>().AsValueEnumerable().Select(x => x * 2).JoinToString(",");
        emptyListSelect.ShouldBe("");

        // Single item list with select
        var singleListSelect = new List<int> { 42 }.AsValueEnumerable().Select(x => x * 2).JoinToString(",");
        singleListSelect.ShouldBe("84");

        // Empty list with where
        var emptyListWhere = new List<int>().AsValueEnumerable().Where(x => x > 0).JoinToString(",");
        emptyListWhere.ShouldBe("");

        // List with where that filters all items
        var allFilteredListWhere = new List<int> { 1, 2, 3 }.AsValueEnumerable().Where(x => x > 10).JoinToString(",");
        allFilteredListWhere.ShouldBe("");
    }

    [Fact]
    public void JoinToString_WithNullableReferences_InOptimizedPaths()
    {
        // Arrange
        var arrayWithNulls = new string?[] { "a", null, "c", null, "e" };
        var listWithNulls = new List<string?> { "a", null, "c", null, "e" };

        // Act - Array paths
        var arrayResult = arrayWithNulls.AsValueEnumerable().JoinToString(",");
        var arraySelectResult = arrayWithNulls.AsValueEnumerable().Select(x => x).JoinToString(",");
        var arrayWhereResult = arrayWithNulls.AsValueEnumerable().Where(x => true).JoinToString(",");
        var arrayWhereSelectResult = arrayWithNulls.AsValueEnumerable().Where(x => true).Select(x => x).JoinToString(",");

        // Act - List paths
        var listResult = listWithNulls.AsValueEnumerable().JoinToString(",");
        var listSelectResult = listWithNulls.AsValueEnumerable().Select(x => x).JoinToString(",");
        var listWhereResult = listWithNulls.AsValueEnumerable().Where(x => true).JoinToString(",");
        var listWhereSelectResult = listWithNulls.AsValueEnumerable().Where(x => true).Select(x => x).JoinToString(",");

        // Expected
        var expected = string.Join(",", arrayWithNulls);

        // Assert
        arrayResult.ShouldBe(expected);
        arraySelectResult.ShouldBe(expected);
        arrayWhereResult.ShouldBe(expected);
        arrayWhereSelectResult.ShouldBe(expected);

        listResult.ShouldBe(expected);
        listSelectResult.ShouldBe(expected);
        listWhereResult.ShouldBe(expected);
        listWhereSelectResult.ShouldBe(expected);
    }
}
