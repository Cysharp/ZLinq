using System.Numerics;
using ZLinq.Linq;

namespace ZLinq.Tests;

/// <summary>
/// Assert.Same/NotSame relating test is not supported.
/// </summary>
public static partial class Assert
{
    internal static void NotSame<TEnumerator, T>(
        ValueEnumerable<TEnumerator, T> expected,
        ValueEnumerable<TEnumerator, T> actual)
        where TEnumerator : struct, IValueEnumerator<T>
    {
        throw new NotSupportedException("ZLinq use struct-based enumerable. Don't compare instances by reference.");
    }

    internal static void NotSame<TEnumerator, T>(
        IEnumerable<T> expected,
        ValueEnumerable<TEnumerator, T> actual)
        where TEnumerator : struct, IValueEnumerator<T>
    {
        throw new NotSupportedException("ZLinq use struct-based enumerable. Don't compare instances by reference.");
    }

    internal static void NotSame<T>(
        ValueEnumerable<Select<FromEnumerable<T>, T, T>, T> expected,
        ValueEnumerable<OfType<Select<FromEnumerable<T>, T, T>, T, T>, T> actual)
    {
        throw new NotSupportedException("ZLinq use struct-based enumerable. Don't compare instances by reference.");
    }

    internal static void NotSame<TEnumerator, TRange, TSelect, TOfType>(
        ValueEnumerable<Select<TEnumerator, TRange, TSelect>, TSelect> expected,
        ValueEnumerable<OfType<Select<TEnumerator, TRange, TSelect>, TSelect, TOfType>, TOfType> actual)
            where TEnumerator : struct, IValueEnumerator<TRange>
    {
        throw new NotSupportedException("ZLinq use struct-based enumerable. Don't compare instances by reference.");
    }

    internal static void NotSame(
        ValueEnumerable<FromRange, int> expected,
        ValueEnumerator<FromRange, int> actual)
    {
        throw new NotSupportedException("ZLinq use struct-based enumerable. Don't compare instances by reference.");
    }

    internal static void NotSame(
        ValueEnumerator<FromRange, int> expected,
        ValueEnumerator<FromRange, int> actual)
    {
        throw new NotSupportedException("ZLinq use struct-based enumerable. Don't compare instances by reference.");
    }

    internal static void NotSame(
        ValueEnumerator<FromInt32Sequence, int> expected,
        ValueEnumerator<FromInt32Sequence, int> actual)
    {
        throw new NotSupportedException("ZLinq use struct-based enumerable. Don't compare instances by reference.");
    }

    internal static void NotSame(
       ValueEnumerator<FromInt32InfiniteSequence, int> expected,
       ValueEnumerator<FromInt32InfiniteSequence, int> actual)
    {
        throw new NotSupportedException("ZLinq use struct-based enumerable. Don't compare instances by reference.");
    }

    internal static void NotSame<T>(
       ValueEnumerator<FromInfiniteSequence<T>, T> expected,
       ValueEnumerator<FromInfiniteSequence<T>, T> actual)
           where T : IAdditionOperators<T, T, T>
    {
        throw new NotSupportedException("ZLinq use struct-based enumerable. Don't compare instances by reference.");
    }
}
