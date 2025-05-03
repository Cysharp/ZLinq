namespace Benchmark;

public interface IDataItemGenerator
{
    object Generate(DataGeneratorContext context);
}

public interface IDataItemGenerator<T> : IDataItemGenerator
    where T : notnull
{
    public abstract new T Generate(DataGeneratorContext context);

    object IDataItemGenerator.Generate(DataGeneratorContext context)
        => Generate(context);
}
