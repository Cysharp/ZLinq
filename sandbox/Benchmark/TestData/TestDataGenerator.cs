namespace Benchmark;

/// <summary>
/// Helper class to generate test data for benchmark.
/// </summary>
public static class TestDataGenerator
{
    private static readonly DefaultDataGenerator DataGenerator = DefaultDataGenerator.Instance;

    public static T[] GetArray<T>(int length)
    {
        var array = GC.AllocateUninitializedArray<T>(length);
        foreach (ref var item in array.AsSpan())
        {
            item = GenerateRandomValue<T>();
        }
        return array;
    }

    public static T GetValue<T>(int length)
        where T : notnull
    {
        return GenerateRandomValue<T>()!;
    }

    private static T? GenerateRandomValue<T>()
    {
        var context = new DataGeneratorContext
        {
            DataGenerator = DataGenerator,
            Random = Random.Shared,
        };

        var type = typeof(T);
        return (T?)DataGenerator.Generate(type, context);
    }
}
