namespace Benchmark;

public class SampleStructGenerator : IDataItemGenerator<SampleStruct>
{
    public static readonly SampleStructGenerator Instance = new();

    public SampleStruct Generate(DataGeneratorContext context)
    {
        var generator = context.DataGenerator;
        return new SampleStruct
        {
            IntData = generator.Generate<int>(context),
            StringData = generator.Generate<string>(context),
        };
    }
}
