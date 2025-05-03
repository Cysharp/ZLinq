namespace Benchmark;

public class SampleRecordClassGenerator : IDataItemGenerator<SampleRecordClass>
{
    public static readonly SampleRecordClassGenerator Instance = new();

    public SampleRecordClass Generate(DataGeneratorContext context)
    {
        var generator = context.DataGenerator;
        return new SampleRecordClass
        {
            IntData = generator.Generate<int>(context),
            StringData = generator.Generate<string>(context),
        };
    }
}

