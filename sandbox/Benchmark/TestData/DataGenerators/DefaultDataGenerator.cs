using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Benchmark;

public class DefaultDataGenerator : IDataGenerator
{
    // ASCII chars that exclude non-printable chars.
    private static readonly char[] AsciiChars = @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~".ToCharArray();
    private static readonly ConcurrentDictionary<Type, IDataItemGenerator> GeneratorMappings = new();

    public static readonly DefaultDataGenerator Instance = new();

    static DefaultDataGenerator()
    {
        // Register known DataGenerators
        Register(SampleClassGenerator.Instance);
        Register(SampleStructGenerator.Instance);
        Register(SampleRecordClassGenerator.Instance);
        Register(SampleRecordStructGenerator.Instance);
        Register(SampleReadOnlyRecordStructGenerator.Instance);
    }

    private DefaultDataGenerator() { }

    internal static void Register<T>(IDataItemGenerator<T> generator)
        where T : notnull
    {
        if (GeneratorMappings.TryAdd(typeof(T), generator))
            return;

        throw new InvalidOperationException($"DataGenerator for {typeof(T).GetDisplayName()} is already registered.");
    }

    public T Generate<T>(DataGeneratorContext context)
        where T : notnull
    {
        return (T)Generate(typeof(T), context)!;
    }

    public object? Generate(Type type, DataGeneratorContext context)
    {
        if (GeneratorMappings.TryGetValue(type, out var generator))
            return generator.Generate(context);

        var rand = context.Random;
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.SByte:
                return (sbyte)rand.Next(sbyte.MinValue, sbyte.MaxValue + 1);
            case TypeCode.Byte:
                return (byte)rand.Next(byte.MinValue, byte.MaxValue + 1);
            case TypeCode.Int16:
                return (short)rand.Next(short.MinValue, short.MaxValue + 1);
            case TypeCode.UInt16:
                return (ushort)rand.Next(ushort.MinValue, ushort.MaxValue + 1);
            case TypeCode.Int32:
                return (int)rand.NextInt64(int.MinValue, (long)int.MaxValue + 1);
            case TypeCode.UInt32:
                return (uint)rand.NextInt64(uint.MinValue, (long)uint.MaxValue + 1);
            case TypeCode.Int64:
                {
                    Span<byte> bytes = stackalloc byte[sizeof(long)];
                    rand.NextBytes(bytes);
                    return MemoryMarshal.Read<long>(bytes);
                }
            case TypeCode.UInt64:
                {
                    Span<byte> bytes = stackalloc byte[sizeof(ulong)];
                    rand.NextBytes(bytes);
                    return MemoryMarshal.Read<ulong>(bytes);
                }
            case TypeCode.Decimal:
                {
                    Span<byte> buffer = stackalloc byte[sizeof(decimal)];
                    rand.NextBytes(buffer);
                    return MemoryMarshal.Read<decimal>(buffer);
                }
            case TypeCode.Single:
                return rand.NextSingle();
            case TypeCode.Double:
                return rand.NextDouble();
            case TypeCode.Boolean:
                return rand.Next(0, 2) == 1;
            case TypeCode.Char:
                return AsciiChars[rand.Next(AsciiChars.Length)];
            case TypeCode.DateTime:
                {
                    Span<byte> bytes = stackalloc byte[Unsafe.SizeOf<DateTime>()];
                    rand.NextBytes(bytes);
                    var secsOffset = BitConverter.ToUInt32(bytes);
                    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    return epoch.AddSeconds(secsOffset);
                }
            case TypeCode.DBNull:
                return DBNull.Value;
            case TypeCode.String:
                var chars = rand.GetItems(AsciiChars, rand.Next(context.MaxStringLength));
                return new string(chars);
            case TypeCode.Object:
                if (type.IsEnum)
                {
                    Array allEnumValues = Enum.GetValues(type);
                    return allEnumValues.GetValue(rand.Next(allEnumValues.Length))!;
                }

                // IntPtr
                if (type == typeof(nint))
                {
                    var size = Unsafe.SizeOf<nint>();
                    Span<byte> bytes = stackalloc byte[size];
                    rand.NextBytes(bytes);
                    return MemoryMarshal.Read<nint>(bytes);
                }

                // UIntPtr
                if (type == typeof(nuint))
                {
                    var size = Unsafe.SizeOf<nuint>();
                    Span<byte> bytes = stackalloc byte[size];
                    rand.NextBytes(bytes);
                    return MemoryMarshal.Read<nuint>(bytes);
                }

#if NET9_0_OR_GREATER
                if (type == typeof(Int128))
                {
                    var size = Unsafe.SizeOf<Int128>();
                    Span<byte> bytes = stackalloc byte[size];
                    rand.NextBytes(bytes);
                    return MemoryMarshal.Read<Int128>(bytes);
                }

                if (type == typeof(UInt128))
                {
                    var size = Unsafe.SizeOf<UInt128>();
                    Span<byte> bytes = stackalloc byte[size];
                    rand.NextBytes(bytes);
                    return MemoryMarshal.Read<UInt128>(bytes);
                }
#endif

                // Half
                if (type == typeof(Half))
                {
                    var size = Unsafe.SizeOf<Half>();
                    Span<byte> bytes = stackalloc byte[size];
                    rand.NextBytes(bytes);
                    return MemoryMarshal.Read<Half>(bytes);
                }

                // Nullable<T>
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var elementType = type.GetGenericArguments().First();

                    const double NullValueRate = 0.1d;
                    return rand.NextSingle() < NullValueRate
                        ? null
                        : Generate(elementType, context)!;
                }

                if (type.IsArray)
                {
                    var elementType = type.GetElementType()!;
                    var array = Array.CreateInstance(elementType, rand.Next(0, context.MaxEnumerableLength + 1));
                    for (int i = 0; i < array.Length; ++i)
                    {
                        var item = context.DataGenerator.Generate(elementType, context);
                        array.SetValue(item, i);
                    }
                    return array;
                }

                // TODO: Add other types by default. (e.g. List/Dictionary based collections, or Tuple/ValueTuple)

                // Other class/struct should be supported by register custom IDataItemGenerator.
                goto default;

            case TypeCode.Empty:
            default:
                throw new NotSupportedException($"Unknown type: {type.GetDisplayName()}");
        }
    }
}
