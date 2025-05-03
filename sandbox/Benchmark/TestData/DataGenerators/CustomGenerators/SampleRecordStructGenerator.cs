namespace Benchmark;

public class SampleRecordStructGenerator : IDataItemGenerator<SampleRecordStruct>
{
    public static readonly SampleRecordStructGenerator Instance = new();

    public SampleRecordStruct Generate(DataGeneratorContext context)
    {
        var generator = context.DataGenerator;
        return new SampleRecordStruct
        {
            IntData = generator.Generate<int>(context),
            StringData = generator.Generate<string>(context),
        };
    }
}

