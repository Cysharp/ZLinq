namespace ZLinq.Tests.Linq;

public class ChunkLargeSizeTest
{
    [Theory]
    [InlineData(int.MaxValue)]
    [InlineData(0X7FFFFFC7)]
    [InlineData(100)]
    [InlineData(101)]
    [InlineData(33)]
    public void LargeChunkSizePreservesElements(int size)
    {
        var source = Enumerable.Range(1, 100).ToArray();
        var actual = source.AsValueEnumerable().Chunk(size).ToArray();

        actual.Length.ShouldBe((int)((source.Length + (long)size - 1) / size));
        for (var i = 0; i < actual.Length; i++)
        {
            actual[i].ShouldBe(source.Skip(i * size).Take(size).ToArray());
        }
    }
}
