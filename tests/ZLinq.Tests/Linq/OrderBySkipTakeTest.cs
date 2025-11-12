namespace ZLinq.Tests.Linq;

public class OrderBySkipTakeTest
{
    [Fact]
    public void OrderBy_Skip()
    {
        var source = new[] { 5, 2, 8, 1, 9, 3, 7, 4, 6 };

        // Skip first 3 elements
        var result = source.AsValueEnumerable()
            .OrderBy(x => x)
            .Skip(3)
            .ToArray();

        result.ShouldBe(new[] { 4, 5, 6, 7, 8, 9 });
    }

    [Fact]
    public void OrderBy_Take()
    {
        var source = new[] { 5, 2, 8, 1, 9, 3, 7, 4, 6 };

        // Take first 4 elements
        var result = source.AsValueEnumerable()
            .OrderBy(x => x)
            .Take(4)
            .ToArray();

        result.ShouldBe(new[] { 1, 2, 3, 4 });
    }

    [Fact]
    public void OrderBy_Skip_Then_Take()
    {
        var source = new[] { 5, 2, 8, 1, 9, 3, 7, 4, 6 };

        // Skip 2, then take 4
        var result = source.AsValueEnumerable()
            .OrderBy(x => x)
            .Skip(2)
            .Take(4)
            .ToArray();

        result.ShouldBe(new[] { 3, 4, 5, 6 });
    }

    [Fact]
    public void OrderBy_Take_Then_Skip()
    {
        var source = new[] { 5, 2, 8, 1, 9, 3, 7, 4, 6 };

        // Take 7, then skip 2
        var result = source.AsValueEnumerable()
            .OrderBy(x => x)
            .Take(7)
            .Skip(2)
            .ToArray();

        result.ShouldBe(new[] { 3, 4, 5, 6, 7 });
    }

    [Fact]
    public void OrderBy_Skip_Zero()
    {
        var source = new[] { 5, 2, 8, 1, 9, 3 };

        // Skip 0 should return all elements
        var result = source.AsValueEnumerable()
            .OrderBy(x => x)
            .Skip(0)
            .ToArray();

        result.ShouldBe(new[] { 1, 2, 3, 5, 8, 9 });
    }

    [Fact]
    public void OrderBy_Take_Zero()
    {
        var source = new[] { 5, 2, 8, 1, 9, 3 };

        // Take 0 should return empty
        var result = source.AsValueEnumerable()
            .OrderBy(x => x)
            .Take(0)
            .ToArray();

        result.ShouldBeEmpty();
    }

    [Fact]
    public void Issue227_Order_Skip0_Take0_Should_Return_Empty()
    {
        var data = new int[] { 1, 2, 3 };
        var skip = 0;
        var take = 0;

        // System.Linq behavior (expected)
        var expected = data.AsEnumerable()
                           .Order()
                           .Skip(skip)
                           .Take(take)
                           .ToArray();

        // ZLinq behavior (actual)
        var results = data.AsValueEnumerable()
                          .Order()
                          .Skip(skip)
                          .Take(take)
                          .ToArray();

        results.Length.ShouldBe(expected.Length);
        results.ShouldBe(expected);
    }

    [Fact]
    public void Issue227_Order_Skip1_Take0_Should_Return_Empty()
    {
        var data = new int[] { 1, 2, 3 };
        var skip = 1;
        var take = 0;

        // System.Linq behavior (expected)
        var expected = data.AsEnumerable()
                           .Order()
                           .Skip(skip)
                           .Take(take)
                           .ToArray();

        // ZLinq behavior (actual)
        var results = data.AsValueEnumerable()
                          .Order()
                          .Skip(skip)
                          .Take(take)
                          .ToArray();

        results.Length.ShouldBe(expected.Length);
        results.ShouldBe(expected);
    }

    [Fact]
    public void OrderBy_Skip_TryGetNonEnumeratedCount()
    {
        var source = new[] { 5, 2, 8, 1, 9, 3, 7, 4, 6 };

        var query = source.AsValueEnumerable()
            .OrderBy(x => x)
            .Skip(3);

        query.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
        count.ShouldBe(6); // 9 - 3 = 6
    }

    [Fact]
    public void OrderBy_Take_TryGetNonEnumeratedCount()
    {
        var source = new[] { 5, 2, 8, 1, 9, 3, 7, 4, 6 };

        var query = source.AsValueEnumerable()
            .OrderBy(x => x)
            .Take(4);

        query.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
        count.ShouldBe(4);
    }

    [Fact]
    public void OrderBy_Skip_Take_TryGetNonEnumeratedCount()
    {
        var source = new[] { 5, 2, 8, 1, 9, 3, 7, 4, 6 };

        var query = source.AsValueEnumerable()
            .OrderBy(x => x)
            .Skip(2)
            .Take(3);

        query.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
        count.ShouldBe(3);
    }

    [Fact]
    public void OrderBy_Skip_Beyond_Count()
    {
        var source = new[] { 5, 2, 8, 1, 9 };

        // Skip more elements than in the source
        var result = source.AsValueEnumerable()
            .OrderBy(x => x)
            .Skip(10)
            .ToArray();

        result.ShouldBeEmpty();
    }

    [Fact]
    public void OrderBy_Take_Beyond_Count()
    {
        var source = new[] { 5, 2, 8, 1, 9 };

        // Take more elements than in the source
        var result = source.AsValueEnumerable()
            .OrderBy(x => x)
            .Take(10)
            .ToArray();

        // Should return all elements in order
        result.ShouldBe(new[] { 1, 2, 5, 8, 9 });
    }

    [Fact]
    public void OrderBy_OrderByDescending_Skip_Take()
    {
        var source = new[] { 5, 2, 8, 1, 9, 3, 7, 4, 6 };
        // 9, 8, 7, 6, 5, 4, 3, 2, 1
        // *, *, 7, 6, 5, *

        // Test with OrderByDescending
        var result = source.AsValueEnumerable()
            .OrderByDescending(x => x)
            .Skip(2)
            .Take(3)
            .ToArray();

        result.ShouldBe([7, 6, 5]);
    }

    [Fact]
    public void OrderBy_ThenBy_Skip_Take()
    {
        var people = new[]
        {
            new Person("Alice", "Smith", 25),
            new Person("Bob", "Jones", 30),
            new Person("Charlie", "Smith", 35),
            new Person("David", "Brown", 25),
            new Person("Eve", "Jones", 28)
        };

        var result = people.AsValueEnumerable()
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Skip(1)
            .Take(3)
            .Select(p => p.FirstName)
            .ToArray();

        // Skip Brown, take Jones/Bob, Jones/Eve, Smith/Alice
        result.ShouldBe(new[] { "Bob", "Eve", "Alice" });
    }

#if NET9_0_OR_GREATER

    [Fact]
    public void OrderByTake()
    {
        var rand = new Random();
        for (int i = 100; i < 5000; i++)
        {
            var data = new int[i];
            rand.NextBytes(MemoryMarshal.Cast<int, byte>(data).ToArray());

            var skip = rand.Next(0, i);
            var take = rand.Next(0, i);

            var seq = data.AsValueEnumerable()
                .Order()
                .Skip(skip)
                .Take(take);

            var expected = data.Order().Skip(skip).Take(take).ToArray();

            // TryCopyTo
            seq.ToArray().ShouldBe(expected);

            seq.Where(_ => true).ToArray().ShouldBe(expected);
        }
    }


#endif

    [Fact]
    public void Skip()
    {
        var source = ValueEnumerable.Range(0, 100).Shuffle().ToArray();
        var ordered = source.AsValueEnumerable().OrderBy(i => i);

        Assert.Equal(Enumerable.Range(20, 80), ordered.Skip(20).ToArray());
        Assert.Equal(Enumerable.Range(80, 20), ordered.Skip(50).Skip(30).ToArray());
        Assert.Equal(20, ordered.Skip(20).First());
        Assert.Equal(20, ordered.Skip(20).FirstOrDefault());
        Assert.Equal(Enumerable.Range(0, 100), ordered.Skip(0).ToArray());
        Assert.Equal(Enumerable.Range(0, 100), ordered.Skip(-1).ToArray());
        Assert.Equal(Enumerable.Range(0, 100), ordered.Skip(int.MinValue).ToArray());
        Assert.Equal(new int[] { 99 }, ordered.Skip(99).ToArray());
        Assert.Empty(ordered.Skip(101).ToArray());
        Assert.Empty(ordered.Skip(int.MaxValue).ToArray());
        Assert.Empty(ordered.Skip(100).ToArray());
        Assert.Equal(Enumerable.Range(1, 99), ordered.Skip(1).ToArray());
        Assert.Equal(Enumerable.Range(0, 100), ordered.ToArray());

        Assert.Equal(Enumerable.Range(20, 80), ordered.Skip(20).IterateToArray());
        Assert.Equal(Enumerable.Range(80, 20), ordered.Skip(50).Skip(30).IterateToArray());
        Assert.Equal(20, ordered.Skip(20).First());
        Assert.Equal(20, ordered.Skip(20).FirstOrDefault());
        Assert.Equal(Enumerable.Range(0, 100), ordered.Skip(0).IterateToArray());
        Assert.Equal(Enumerable.Range(0, 100), ordered.Skip(-1).IterateToArray());
        Assert.Equal(Enumerable.Range(0, 100), ordered.Skip(int.MinValue).IterateToArray());
        Assert.Equal(new int[] { 99 }, ordered.Skip(99).IterateToArray());
        Assert.Empty(ordered.Skip(101).IterateToArray());
        Assert.Empty(ordered.Skip(int.MaxValue).IterateToArray());
        Assert.Empty(ordered.Skip(100).IterateToArray());
        Assert.Equal(Enumerable.Range(1, 99), ordered.Skip(1).IterateToArray());
        Assert.Equal(Enumerable.Range(0, 100), ordered.IterateToArray());
    }

    [Fact]
    public void Take()
    {
        var source = ValueEnumerable.Range(0, 100).Shuffle().ToArray();
        var ordered = source.AsValueEnumerable().OrderBy(i => i);

        Assert.Equal(Enumerable.Range(0, 20), ordered.Take(20).ToArray());
        Assert.Equal(Enumerable.Range(0, 30), ordered.Take(50).Take(30).ToArray());
        Assert.Empty(ordered.Take(0).ToArray());
        Assert.Empty(ordered.Take(-1).ToArray());
        Assert.Empty(ordered.Take(int.MinValue).ToArray());
        Assert.Equal(new int[] { 0 }, ordered.Take(1).ToArray());
        Assert.Equal(Enumerable.Range(0, 100), ordered.Take(101).ToArray());
        Assert.Equal(Enumerable.Range(0, 100), ordered.Take(int.MaxValue).ToArray());
        Assert.Equal(Enumerable.Range(0, 100), ordered.Take(100).ToArray());
        Assert.Equal(Enumerable.Range(0, 99), ordered.Take(99).ToArray());
        Assert.Equal(Enumerable.Range(0, 100), ordered.ToArray());

        Assert.Equal(Enumerable.Range(0, 20), ordered.Take(20).IterateToArray());
        Assert.Equal(Enumerable.Range(0, 30), ordered.Take(50).Take(30).IterateToArray());
        Assert.Empty(ordered.Take(0).IterateToArray());
        Assert.Empty(ordered.Take(-1).IterateToArray());
        Assert.Empty(ordered.Take(int.MinValue).IterateToArray());
        Assert.Equal(new int[] { 0 }, ordered.Take(1).IterateToArray());
        Assert.Equal(Enumerable.Range(0, 100), ordered.Take(101).IterateToArray());
        Assert.Equal(Enumerable.Range(0, 100), ordered.Take(int.MaxValue).IterateToArray());
        Assert.Equal(Enumerable.Range(0, 100), ordered.Take(100).IterateToArray());
        Assert.Equal(Enumerable.Range(0, 99), ordered.Take(99).IterateToArray());
        Assert.Equal(Enumerable.Range(0, 100), ordered.IterateToArray());
    }


    [Fact]
    public void TakeThenSkipAll()
    {
        var source = ValueEnumerable.Range(0, 100).Shuffle().ToArray();
        var ordered = source.AsValueEnumerable().OrderBy(i => i);
        var result = ordered.Take(20).Skip(30).ToArray();
        Assert.Empty(result);
        Assert.Empty(ordered.Take(20).Skip(30).IterateToArray());
    }
}
