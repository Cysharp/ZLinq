namespace Benchmark;

public class SampleReadOnlyRecordStructGenerator : IDataItemGenerator<SampleReadOnlyRecordStruct>
{
    public static readonly SampleReadOnlyRecordStructGenerator Instance = new();

    public SampleReadOnlyRecordStruct Generate(DataGeneratorContext context)
    {
        var generator = context.DataGenerator;
        return new SampleReadOnlyRecordStruct
        {
            IntData = generator.Generate<int>(context),
            StringData = generator.Generate<string>(context),
        };
    }
}

