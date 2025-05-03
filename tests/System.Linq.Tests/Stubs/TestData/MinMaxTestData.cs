// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Tests;

// Original code:
// https://github.com/dotnet/runtime/blob/v10.0.0-preview.2.25163.2/src/libraries/System.Linq/tests/MinTests.cs
// https://github.com/dotnet/runtime/blob/v10.0.0-preview.2.25163.2/src/libraries/System.Linq/tests/MaxTests.cs

namespace System.Linq;

// This class is used to share test data between System.Linq/ZLinq tests.
// Note: It need to use `Shuffler.Shuffle` to show indivisual test cases on Test Explorer.
public class MinMaxTestData
{
    public static IEnumerable<object[]> Min_AllTypes_TestData()
    {
        for (int length = 2; length < 65; length++)
        {
            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (byte)i)), (byte)length };

            // Unit Tests does +T.One so we should generate data up to one value below sbyte.MaxValue, otherwise the type overflows
            if ((length + length) < sbyte.MaxValue)
            {
                yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (sbyte)i)), (sbyte)length };
            }

            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (ushort)i)), (ushort)length };
            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (short)i)), (short)length };
            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (uint)i)), (uint)length };
            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (int)i)), (int)length };
            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (ulong)i)), (ulong)length };
            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (long)i)), (long)length };
            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (float)i)), (float)length };
            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (double)i)), (double)length };
            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (decimal)i)), (decimal)length };
            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (nuint)i)), (nuint)length };
            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (nint)i)), (nint)length };
        }
    }

    public static IEnumerable<object[]> Max_AllTypes_TestData()
    {
        for (int length = 2; length < 65; length++)
        {
            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (byte)i)), (byte)(length + length - 1) };

            // Unit Tests does +T.One so we should generate data up to one value below sbyte.MaxValue
            if ((length + length) < sbyte.MaxValue)
            {
                yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (sbyte)i)), (sbyte)(length + length - 1) };
            }

            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (ushort)i)), (ushort)(length + length - 1) };
            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (short)i)), (short)(length + length - 1) };
            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (char)i)), (char)(length + length - 1) };
            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (uint)i)), (uint)(length + length - 1) };
            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (int)i)), (int)(length + length - 1) };
            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (ulong)i)), (ulong)(length + length - 1) };
            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (long)i)), (long)(length + length - 1) };
            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (float)i)), (float)(length + length - 1) };
            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (double)i)), (double)(length + length - 1) };
            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (decimal)i)), (decimal)(length + length - 1) };
            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (nuint)i)), (nuint)(length + length - 1) };
            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (nint)i)), (nint)(length + length - 1) };
            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (Int128)i)), (Int128)(length + length - 1) };
            yield return new object[] { Shuffler.Shuffle(Enumerable.Range(length, length).Select(i => (UInt128)i)), (UInt128)(length + length - 1) };
        }
    }
}
