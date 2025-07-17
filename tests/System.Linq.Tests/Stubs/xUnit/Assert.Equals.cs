using System.Numerics;
using ZLinq.Linq;

namespace ZLinq.Tests;

public static partial class Assert
{
    internal static void Equal<TEnumerator, T>(
        ValueEnumerable<TEnumerator, T> expected,
        ValueEnumerable<TEnumerator, T> actual)
        where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<TEnumerator, T>(
        IEnumerable<T> expected,
        ValueEnumerable<TEnumerator, T> actual)
        where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        => Xunit.Assert.Equal(expected, actual.ToArray());

    internal static void Equals<T>(
        T[] expected,
        ValueEnumerable<Join<FromArray<T>, FromEnumerable<T>, T, T, T, T>, T> actual)
    {
        Xunit.Assert.Equal(expected, actual.ToArray());
    }

    internal static void Equal<TEnumerator, T>(
       IEnumerable<T> expected,
       ValueEnumerable<Union<TEnumerator, TEnumerator, T>, T> actual,
       IEqualityComparer<T> comparer)
       where TEnumerator : struct, IValueEnumerator<T>
    {
        Xunit.Assert.Equal(expected, actual.ToArray(), comparer);
    }

    internal static void Equal<T>(
        T[] expected,
        ValueEnumerable<Union<FromArray<T>, FromEnumerable<T>, T>, T> actual,
        IEqualityComparer<T> comparer)
    {
        Xunit.Assert.Equal(expected, actual.ToArray(), comparer);
    }

    internal static void Equal<T>(
        ValueEnumerable<Reverse<FromEnumerable<T>, T>, T> expected,
        IEnumerable<T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual);
    }

    internal static void Equal<T>(
        ValueEnumerable<Reverse<FromEnumerable<T>, T>, T> expected,
        ValueEnumerable<OrderBy<Select<Reverse<FromEnumerable<T>, T>, T, T>, T, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<SkipLast<FromEnumerable<T>, T>, T> expected,
        IEnumerable<T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<TakeLast<FromEnumerable<T>, T>, T> expected,
        IEnumerable<T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray()); ;
    }

    internal static void Equal<T>(
        ValueEnumerable<Take<OrderBy<FromArray<T>, T, T>, T>, T> expected,
        ValueEnumerable<Take<Take<OrderBy<FromArray<T>, T, T>, T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Skip<FromArray<T>, T>, T> expected,
        ValueEnumerable<Skip<FromEnumerable<T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Take<FromArray<T>, T>, T> expected,
        ValueEnumerable<Take<FromEnumerable<T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Take<FromArray<T>, T>, T> expected,
        ValueEnumerable<ArrayWhere<T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Skip<FromArray<T>, T>, T> expected,
        ValueEnumerable<ArrayWhere<T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Skip<FromArray<T>, T>, T> expected,
        ValueEnumerable<Skip<Reverse<FromArray<T>, T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Take<FromArray<T>, T>, T> expected,
        ValueEnumerable<Take<Reverse<FromArray<T>, T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Skip<FromArray<T>, T>, T> expected,
        ValueEnumerable<Skip<Reverse<FromEnumerable<T>, T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Take<FromArray<T>, T>, T> expected,
        ValueEnumerable<Take<Reverse<FromEnumerable<T>, T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal(
        ValueEnumerable<Select<FromArray<int>, int, int>, int> expected,
        IEnumerable<int> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal(
        ValueEnumerable<Select<FromEnumerable<IGrouping<string, int>>, IGrouping<string, int>, IGrouping<string, int>>, IGrouping<string, int>> expected,
        IGrouping<string, int>[] actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Take<FromArray<T>, T>, T> expected,
        ValueEnumerable<Where<FromArray<T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }
    internal static void Equal<T>(
        ValueEnumerable<Skip<FromArray<T>, T>, T> expected,
        ValueEnumerable<Where<FromArray<T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Take<FromArray<T>, T>, T> expected,
        ValueEnumerable<Where2<FromArray<T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Skip<FromArray<T>, T>, T> expected,
        ValueEnumerable<Where2<FromArray<T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<ArraySelect<T, T>, T> expected,
        IEnumerable<T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual);
    }

    #region Append/Prepend

    internal static void Equal<T>(
        ValueEnumerable<Append<FromEnumerable<T>, T>, T> expected,
        ValueEnumerable<Append<FromEnumerable<T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<TEnumerator, T>(
        ValueEnumerable<Append<Select<TEnumerator, T, T>, T>, T> expected,
        ValueEnumerable<Append<Select<TEnumerator, T, T>, T>, T> actual)
        where TEnumerator : struct, IValueEnumerator<T>
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<TEnumerator, T>(
        ValueEnumerable<Append<Select<TEnumerator, T, T>, T>, T> expected,
        ValueEnumerable<Concat<Select<TEnumerator, T, T>, FromEnumerable<T>, T>, T> actual)
                where TEnumerator : struct, IValueEnumerator<T>
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Append<ArraySelect<T, T>, T>, T> expected,
        ValueEnumerable<Concat<ArraySelect<T, T>, FromEnumerable<T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<TEnumerator, T>(
        ValueEnumerable<Append<TEnumerator, T>, T> expected,
        ValueEnumerable<Append<Append<TEnumerator, T>, T>, T> actual)
        where TEnumerator : struct, IValueEnumerator<T>
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    #endregion

    #region Prepend

    internal static void Equal<TEnumerator, T>(
        ValueEnumerable<Prepend<Select<TEnumerator, T, T>, T>, T> expected,
        ValueEnumerable<Prepend<Select<TEnumerator, T, T>, T>, T> actual)
        where TEnumerator : struct, IValueEnumerator<T>
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<TEnumerator, T>(
        ValueEnumerable<Prepend<Select<TEnumerator, T, T>, T>, T> expected,
        ValueEnumerable<Concat<TEnumerator, Select<TEnumerator, T, T>, T>, T> actual)
            where TEnumerator : struct, IValueEnumerator<T>
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    // ValueEnumerable<Concat<ArraySelect<T, T>, FromEnumerable<T>, T>, T> actual)
    internal static void Equal<T>(
        ValueEnumerable<Prepend<ArraySelect<T, T>, T>, T> expected,
        ValueEnumerable<Concat<FromArray<T>, ArraySelect<T, T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<TEnumerator, T>(
        ValueEnumerable<Prepend<Select<TEnumerator, T, T>, T>, T> expected,
        ValueEnumerable<Prepend<TEnumerator, T>, T> actual)
            where TEnumerator : struct, IValueEnumerator<T>
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<TEnumerator, T>(
        ValueEnumerable<Prepend<ArraySelect<T, T>, T>, T> expected,
        ValueEnumerable<Prepend<TEnumerator, T>, T> actual)
        where TEnumerator : struct, IValueEnumerator<T>
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<TEnumerator, T>(
        ValueEnumerable<Prepend<ListSelect<T, T>, T>, T> expected,
        ValueEnumerable<Prepend<TEnumerator, T>, T> actual)
        where TEnumerator : struct, IValueEnumerator<T>
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    #endregion

    internal static void Equal<T>(
        ValueEnumerable<Chunk<FromEnumerable<T>, T>, T[]> expected,
        ValueEnumerable<Chunk<FromEnumerable<T>, T>, T[]> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Intersect<Select<FromArray<T>, T, T>, Select<FromArray<T>, T, T>, T>, T> expected,
        ValueEnumerable<Intersect<Select<FromArray<T>, T, T>, Select<FromArray<T>, T, T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        IEnumerable<T> expected,
        ValueEnumerable<IntersectBy<FromEnumerable<T>, FromEnumerable<T>, T, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<TSource, TKey>(
        IEnumerable<TSource> expected,
        ValueEnumerable<IntersectBy<FromEnumerable<TSource>, FromEnumerable<TKey>, TSource, TKey>, TSource> actual)
    {
        Xunit.Assert.Equal(expected, actual.ToArray());
    }

    internal static void Equal<T>(
        IEnumerable<T> expected,
        ValueEnumerable<Intersect<FromEnumerable<T>, FromEnumerable<T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected, actual.ToArray());
    }

    internal static void Equal<TEnumerator, T>(
        IEnumerable<T> expected,
        ValueEnumerable<Distinct<TEnumerator, T>, T> actual,
        IEqualityComparer<T> comparer)
        where TEnumerator : struct, IValueEnumerator<T>
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray(), comparer);
    }

    internal static void Equal<T>(
        ValueEnumerable<GroupBy3<FromArray<T>, T, string, IEnumerable<T>>, IEnumerable<T>> expected,
        IEnumerable<IEnumerable<T>> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual);
    }

    internal static void Equal<T>(
        ValueEnumerable<GroupBy4<FromArray<T>, T, string, T, IEnumerable<T>>, IEnumerable<T>> expected,
        IEnumerable<IEnumerable<T>> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual);
    }

    internal static void Equal<T>(
        ValueEnumerable<GroupBy2<FromArray<T>, T, string, int>, IGrouping<string, int>> expected,
        List<IGrouping<string, int>> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<GroupBy<FromArray<T>, T, string>, IGrouping<string, T>> expected,
        List<IGrouping<string, T>> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<GroupBy2<FromArray<T>, T, string, int>, IGrouping<string, int>> expected,
        IGrouping<string, int>[] actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<GroupBy<FromArray<T>, T, string>, IGrouping<string, T>> expected,
        IGrouping<string, T>[] actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        IEnumerable<T> expected,
        ValueEnumerable<Take<FromInfiniteSequence<T>, T>, T> actual)
            where T : IAdditionOperators<T, T, T>
    {
        Xunit.Assert.Equal(expected, actual.ToArray());
    }

    internal static void Equal(
        ValueEnumerable<FromRange, int> expected,
        ValueEnumerable<Take<FromRange, int>, int> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }
}
