namespace Benchmark;

public class SampleClassGenerator : IDataItemGenerator<SampleClass>
{
    public static readonly SampleClassGenerator Instance = new();

    public SampleClass Generate(DataGeneratorContext context)
    {
        var generator = context.DataGenerator;
        return new SampleClass
        {
            IntData = generator.Generate<int>(context),
            StringData = generator.Generate<string>(context),
        };
    }
}

