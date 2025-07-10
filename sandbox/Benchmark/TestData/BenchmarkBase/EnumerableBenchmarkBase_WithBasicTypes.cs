namespace Benchmark;

////[GenericTypeArguments(typeof(bool))]
////[GenericTypeArguments(typeof(sbyte))]
////[GenericTypeArguments(typeof(byte))]
////[GenericTypeArguments(typeof(short))]
////[GenericTypeArguments(typeof(ushort))]
////[GenericTypeArguments(typeof(int))]
////[GenericTypeArguments(typeof(uint))]
////[GenericTypeArguments(typeof(long))]
////[GenericTypeArguments(typeof(ulong))]
////[GenericTypeArguments(typeof(decimal))]
////[GenericTypeArguments(typeof(Half))]
////[GenericTypeArguments(typeof(float))]
////[GenericTypeArguments(typeof(double))]
////[GenericTypeArguments(typeof(nint))]
////[GenericTypeArguments(typeof(nuint))]
////#if NET9_0_OR_GREATER
////[GenericTypeArguments(typeof(Int128))]
////[GenericTypeArguments(typeof(UInt128))]
////#endif
////[GenericTypeArguments(typeof(string))]
////[GenericTypeArguments(typeof(SampleClass))]
////[GenericTypeArguments(typeof(SampleStruct))]
////[GenericTypeArguments(typeof(SampleReadonlyStruct))]
////[GenericTypeArguments(typeof(SampleRecordClass))]
////[GenericTypeArguments(typeof(SampleRecordStruct))]
////[GenericTypeArguments(typeof(SampleReadOnlyRecordStruct))]

[GenericTypeArguments(typeof(int))]    // ValueType
[GenericTypeArguments(typeof(string))] // ReferenceType
public abstract class EnumerableBenchmarkBase_WithBasicTypes<T> : EnumerableBenchmarkBase<T>
{
}
