﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileGen;

public partial class Commands
{
    readonly static string[] PrimitiveTypes = ["byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "float", "double", "bool", "char", "decimal", "nint", "nuint"];
    readonly static string[] PrimitiveTypesPlusString = [.. PrimitiveTypes, "string"];
    readonly static string[] PrimitiveNumbers = ["byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "float", "double", "decimal", "nint", "nuint"];
    readonly static string[] PrimitiveNumbersWithoutFloat = ["byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "double", "decimal", "nint", "nuint"];
    readonly static string[] PrimitivesForMinMax = ["byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "nint", "nuint", "Int128", "UInt128"];

    public void TypeOfContains()
    {
        var sb = new StringBuilder();
        foreach (var type in PrimitiveTypesPlusString)
        {
            var code = $$"""
            else if (typeof(T) == typeof({{type}}))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, {{type}}> (ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, {{type}}>(ref value));
            }
""";
            sb.AppendLine(code);
        }

        Console.WriteLine(sb.ToString());
    }

    public void Sum()
    {
        var sb = new StringBuilder();
        sb.AppendLine("#region generate from FileGen.Commands.Sum");
        foreach (var type in PrimitiveNumbersWithoutFloat)
        {
            var code = $$"""
        else if (typeof(TSource) == typeof({{type}}))
        {
            using (var enumerator = source.Enumerator)
            {
                {{type}} sum = default;
                while (enumerator.TryGetNext(out var item))
                {
                    checked { sum += Unsafe.As<TSource, {{type}}>(ref item); }
                }
                return Unsafe.As<{{type}}, TSource>(ref sum);
            }
        }
""";
            sb.AppendLine(code);
        }
        sb.AppendLine("#endregion");

        Console.WriteLine(sb.ToString());
    }

    public void Average()
    {
        var sb = new StringBuilder();
        sb.AppendLine("#region generate from FileGen.Commands.Average");
        foreach (var type in PrimitiveNumbersWithoutFloat)
        {
            var code = $$"""
        else if (typeof(TSource) == typeof({{type}}))
        {
            using (var enumerator = source.Enumerator)
            {
                if (!enumerator.TryGetNext(out var current))
                {
                    Throws.NoElements();
                }

                {{type}} sum = Unsafe.As<TSource, {{type}}>(ref current);
                long count = 1;
                while (enumerator.TryGetNext(out current))
                {
                    checked { sum += Unsafe.As<TSource, {{type}}>(ref current); }
                    count++;
                }

                return (double)sum / (double)count;
            }
        }
""";
            sb.AppendLine(code);
        }
        sb.AppendLine("#endregion");

        Console.WriteLine(sb.ToString());
    }

    public void AverageNullable()
    {
        var sb = new StringBuilder();
        sb.AppendLine("#region generate from FileGen.Commands.AverageNullable");
        foreach (var type in PrimitiveNumbersWithoutFloat)
        {
            var code = $$"""
        else if (typeof(TSource) == typeof({{type}}))
        {
            using (var enumerator = source.Enumerator)
            {
                while (enumerator.TryGetNext(out var first))
                {
                    if (first.HasValue)
                    {
                        var firstValue = first.GetValueOrDefault();
                        var sum = Unsafe.As<TSource, {{type}}>(ref firstValue);
                        long count = 1;

                        while (enumerator.TryGetNext(out var current))
                        {
                            if (current.HasValue)
                            {
                                var currentValue = current.GetValueOrDefault();
                                checked { sum += Unsafe.As<TSource, {{type}}>(ref currentValue); }
                                count++;
                            }
                        }

                        return (double)sum / (double)count;
                    }
                }
                return null;
            }
        }
""";
            sb.AppendLine(code);
        }
        sb.AppendLine("#endregion");

        Console.WriteLine(sb.ToString());
    }

    public void Min()
    {
        var sb = new StringBuilder();
        sb.AppendLine("#region generate from FileGen.Commands.Min");
        foreach (var type in PrimitivesForMinMax)
        {
            var code = $$"""
        else if (typeof(TSource) == typeof({{type}}))
        {
            if (comparer != Comparer<TSource>.Default) return MinSpanComparer(span, comparer);
            var result = SimdMinBinaryInteger(UnsafeSpanBitCast<TSource, {{type}}>(span));
            return Unsafe.As<{{type}}, TSource>(ref result);
        }
""";
            sb.AppendLine(code);
        }
        sb.AppendLine("#endregion");

        Console.WriteLine(sb.ToString());
    }

    public void InterpolatedStringHandlerAppendFormatted()
    {
        var sb = new StringBuilder();
        foreach (var type in PrimitiveTypes)
        {
            var code = $$"""
        else if (typeof(T) == typeof({{type}}))
        {
            int charsWritten;
            while (!(Unsafe.As<T, {{type}}>(ref value)).TryFormat(_chars.Slice(_pos), out charsWritten, default, _provider))
            {
                Grow();
            }

            _pos += charsWritten;
            return;
        }
""";
            sb.AppendLine(code);
        }

        Console.WriteLine(sb.ToString());
    }
}
