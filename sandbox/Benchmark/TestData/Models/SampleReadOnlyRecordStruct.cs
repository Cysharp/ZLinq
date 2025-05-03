namespace Benchmark;

public readonly record struct SampleReadOnlyRecordStruct
{
    public int IntData { get; init; }

    public required string StringData { get; init; }
}
