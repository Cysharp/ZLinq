using System;
using System.Diagnostics.CodeAnalysis;

namespace Benchmark;

public struct SampleStruct : IEquatable<SampleStruct>
{
    public int IntData { get; init; }

    public required string StringData { get; init; }

    // struct needs to implement IEquatable<T> to suppress boxing when using `EqualityComparer<T>.Default`
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is SampleStruct equatable && Equals(equatable);

    public bool Equals(SampleStruct other) =>
               this.IntData == other.IntData
            && this.StringData == other.StringData;

    public static bool operator ==(SampleStruct left, SampleStruct right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SampleStruct left, SampleStruct right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
        => HashCode.Combine(IntData, StringData);
}
