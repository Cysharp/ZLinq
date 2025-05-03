namespace Benchmark;

public class DataGeneratorContext
{
    public required IDataGenerator DataGenerator { get; init; }

    public int Depth { get; set; } = 0;

    #region Properties for random data generator
    public required Random Random { get; init; }

    /// <summary>
    /// Max length of generated string length.
    /// </summary>
    public int MaxStringLength { get; set; } = 50;

    /// <summary>
    /// Max length of generated enumerable length.
    /// </summary>
    public int MaxEnumerableLength { get; set; } = 50;
    #endregion
}
