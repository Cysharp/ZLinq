namespace ZLinq.Tests.Linq;

public class RangeTest
{
    [Fact]
    public void EndsAtInt32MaxValue()
    {
        ValueEnumerable.Range(int.MaxValue, 1).IterateToArray().ShouldBe([int.MaxValue]);
        ValueEnumerable.Range(int.MaxValue, 1).ToArray().ShouldBe([int.MaxValue]);
        ValueEnumerable.Range(int.MaxValue, 1).Where(_ => true).ToArray().ShouldBe([int.MaxValue]);
        ValueEnumerable.Range(int.MaxValue, 1).Contains(int.MaxValue).ShouldBeTrue();
        ValueEnumerable.Range(int.MaxValue, 1).Max().ShouldBe(int.MaxValue);
        ValueEnumerable.Range(int.MaxValue - 2, 3).IterateToArray().ShouldBe([int.MaxValue - 2, int.MaxValue - 1, int.MaxValue]);
        ValueEnumerable.Range(int.MaxValue - 2, 3).Select(x => (long)x).IterateToArray().ShouldBe([int.MaxValue - 2L, int.MaxValue - 1L, int.MaxValue]);
        ValueEnumerable.Range(int.MaxValue - 2, 3).Select(x => (long)x).Where(_ => true).ToArray().ShouldBe([int.MaxValue - 2L, int.MaxValue - 1L, int.MaxValue]);
        ValueEnumerable.Range(int.MaxValue, 0).IterateToArray().ShouldBeEmpty();
        ValueEnumerable.Range(int.MinValue, 2).IterateToArray().ShouldBe([int.MinValue, int.MinValue + 1]);
    }

    [Fact]
    public void Vectorize()
    {
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                ValueEnumerable.Range(i, j).ToArray().ShouldBe(Enumerable.Range(i, j).ToArray());
            }
        }

        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {

                ValueEnumerable.Range(i, j).ToList().ShouldBe(Enumerable.Range(i, j).ToList());
            }
        }
    }

    [Fact]
    public void Standard()
    {
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                ValueEnumerable.Range(i, j).Select(x => x).ToArray().ShouldBe(Enumerable.Range(i, j).ToArray());
            }
        }
    }

    [Fact]
    public void EmptyCount()
    {
        ValueEnumerable.Range(-100, 0).ToArray().ShouldBe([]);
        ValueEnumerable.Range(-100, 0).Select(x => x).ToArray().ShouldBe([]);
    }

    [Fact]
    public void Validation()
    {
        TestUtil.Throws<ArgumentOutOfRangeException>(
            () => Enumerable.Range(0, -1),
            () => ValueEnumerable.Range(0, -1));

        TestUtil.Throws<ArgumentOutOfRangeException>(
            () => Enumerable.Range(0, -10),
            () => ValueEnumerable.Range(0, -10));

        TestUtil.Throws<ArgumentOutOfRangeException>(
            () => Enumerable.Range(100, int.MaxValue - 50),
            () => ValueEnumerable.Range(100, int.MaxValue - 50));

        var xs = new int[] { 1, 2, 3, 4, 5 };
        foreach (var item in xs.AsValueEnumerable())
        {
            Console.WriteLine(item);
        }
    }

    [Fact]
    public void ForEach()
    {
        var e = Enumerable.Range(1, 100).GetEnumerator();
        foreach (var item in ValueEnumerable.Range(1, 100))
        {
            e.MoveNext();
            item.ShouldBe(e.Current);
        }
    }

    [Fact]
    public void BoundaryValues()
    {
        // Test with start at int.MinValue
        var minStart = ValueEnumerable.Range(int.MinValue, 10).ToArray();
        var minStartExpected = Enumerable.Range(int.MinValue, 10).ToArray();
        minStart.ShouldBe(minStartExpected);

        // Test with start near int.MaxValue
        var maxStart = ValueEnumerable.Range(int.MaxValue - 5, 5).ToArray();
        maxStart.ShouldBe(Enumerable.Range(int.MaxValue - 5, 5).ToArray());

        // Test with maximum valid count from 0
        ValueEnumerable.Range(0, int.MaxValue).Take(100).ToArray()
            .ShouldBe(Enumerable.Range(0, int.MaxValue).Take(100).ToArray());
    }

    [Fact]
    public void WithElementAt()
    {
        ValueEnumerable.Range(100, 10).ElementAt(5).ShouldBe(105);
    }

    [Fact]
    public void NegativeStart()
    {
        // Test with negative start values
        var range1 = ValueEnumerable.Range(-10, 20).ToArray();
        range1.ShouldBe(Enumerable.Range(-10, 20).ToArray());

        range1.Length.ShouldBe(20);
        range1[0].ShouldBe(-10);
        range1[19].ShouldBe(9);
    }

    [Fact]
    public void EnumerationAfterDispose()
    {
        // Ensure proper behavior after disposal
        var range = ValueEnumerable.Range(1, 5);
        var enumerator = range.GetEnumerator();

        // Use and dispose
        var results = new List<int>();
        while (enumerator.MoveNext())
        {
            results.Add(enumerator.Current);
        }
        results.ShouldBe(new[] { 1, 2, 3, 4, 5 });

        enumerator.Dispose();

        // After disposal, MoveNext should return false
        enumerator.MoveNext().ShouldBeFalse();
    }
}
