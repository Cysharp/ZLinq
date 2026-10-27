#if NET9_0_OR_GREATER

namespace ZLinq.Tests.Linq;

public class StringJoinTest
{
    [Fact]
    public void StringJoin_StringSeparator()
    {
        string.Join(", ", new[] { 1, 2, 3 }.AsValueEnumerable().Select(x => x * 2)).ShouldBe("2, 4, 6");
        string.Join(null, new[] { 1, 2, 3 }.AsValueEnumerable().Select(x => x)).ShouldBe("123");
        string.Join(",", Array.Empty<int>().AsValueEnumerable().Select(x => x)).ShouldBe("");
    }

    [Fact]
    public void StringJoin_CharSeparator()
    {
        string.Join(',', new[] { "a", "b", "c" }.AsValueEnumerable().Select(x => x)).ShouldBe("a,b,c");
    }
}

#endif
