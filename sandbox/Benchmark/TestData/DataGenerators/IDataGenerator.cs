namespace Benchmark;

public interface IDataGenerator
{
    object? Generate(Type type, DataGeneratorContext context);

    T Generate<T>(DataGeneratorContext context)
        where T : notnull
    {
        return (T)Generate(typeof(T), context)!;
    }
}

