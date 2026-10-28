#if NET8_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ZLinq;

partial class ValueEnumerableExtensions
{
    // ImmutableArray
    // ImmutableList
    // ImmutableDictionary
    // ImmutableSortedDictionary
    // ImmutableHashSet
    // ImmutableSortedSet

    /// <summary>Creates an ImmutableArray from a sequence.</summary>
    /// <param name="source">The source sequence.</param>
    /// <returns>An immutable array that contains the elements from the sequence.</returns>
    public static ImmutableArray<TSource> ToImmutableArray<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        using var e = source.Enumerator;

        if (e.TryGetSpan(out var span))
        {
            return ImmutableArray.Create(span);
        }
        else
        {
            if (e.TryGetNonEnumeratedCount(out var count))
            {
                var array = GC.AllocateUninitializedArray<TSource>(count);

                if (e.TryCopyTo(array, offset: 0))
                {
                    return ImmutableCollectionsMarshal.AsImmutableArray(array);
                }
                else
                {
                    var i = 0;
                    while (e.TryGetNext(out var current))
                    {
                        array[i] = current;
                        i++;
                    }
                    return ImmutableCollectionsMarshal.AsImmutableArray(array);
                }
            }
            else
            {
                var builder = ImmutableArray.CreateBuilder<TSource>();
                while (e.TryGetNext(out var current))
                {
                    builder.Add(current);
                }
                return builder.ToImmutable();
            }
        }
    }

    /// <summary>Creates an ImmutableList from a sequence.</summary>
    /// <param name="source">The source sequence.</param>
    /// <returns>An immutable list that contains the elements from the sequence.</returns>
    public static ImmutableList<TSource> ToImmutableList<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        using var e = source.Enumerator;

        if (e.TryGetSpan(out var span))
        {
            return ImmutableList.Create(span);
        }
        else
        {
            var builder = ImmutableList.CreateBuilder<TSource>();
            while (e.TryGetNext(out var current))
            {
                builder.Add(current);
            }
            return builder.ToImmutable();
        }
    }

    /// <summary>Creates an ImmutableDictionary from a sequence according to a specified key selector function.</summary>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">A function to extract the key from each element.</param>
    /// <param name="comparer">An equality comparer to compare keys, or <see langword="null"/> to use the default equality comparer.</param>
    /// <returns>An immutable dictionary that contains keys and values.</returns>
    public static ImmutableDictionary<TKey, TSource> ToImmutableDictionary<TEnumerator, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer = null)
       where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
       where TKey : notnull
    {
        using var e = source.Enumerator;

        var builder = ImmutableDictionary.CreateBuilder<TKey, TSource>(comparer);

        if (e.TryGetSpan(out var span))
        {
            foreach (var current in span)
            {
                builder.Add(keySelector(current), current);
            }
        }
        else
        {
            while (e.TryGetNext(out var current))
            {
                builder.Add(keySelector(current), current);
            }
        }

        return builder.ToImmutable();
    }

    /// <summary>Creates an ImmutableDictionary from a sequence according to a specified key selector function.</summary>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">A function to extract the key from each element.</param>
    /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
    /// <param name="keyComparer">An equality comparer to compare keys, or <see langword="null"/> to use the default equality comparer.</param>
    /// <param name="valueComparer">An equality comparer to compare values, or <see langword="null"/> to use the default equality comparer.</param>
    /// <returns>An immutable dictionary that contains keys and values.</returns>
    public static ImmutableDictionary<TKey, TValue> ToImmutableDictionary<TEnumerator, TSource, TKey, TValue>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> elementSelector, IEqualityComparer<TKey>? keyComparer = null, IEqualityComparer<TValue>? valueComparer = null)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        where TKey : notnull
    {
        using var e = source.Enumerator;

        var builder = ImmutableDictionary.CreateBuilder<TKey, TValue>(keyComparer, valueComparer);

        if (e.TryGetSpan(out var span))
        {
            foreach (var current in span)
            {
                builder.Add(keySelector(current), elementSelector(current));
            }
        }
        else
        {
            while (e.TryGetNext(out var current))
            {
                builder.Add(keySelector(current), elementSelector(current));
            }
        }

        return builder.ToImmutable();
    }

    /// <summary>Creates an ImmutableDictionary from a sequence of key/value pairs.</summary>
    /// <param name="source">The source sequence.</param>
    /// <param name="keyComparer">An equality comparer to compare keys, or <see langword="null"/> to use the default equality comparer.</param>
    /// <param name="valueComparer">An equality comparer to compare values, or <see langword="null"/> to use the default equality comparer.</param>
    /// <returns>An immutable dictionary that contains the keys and values from the sequence.</returns>
    public static ImmutableDictionary<TKey, TValue> ToImmutableDictionary<TEnumerator, TKey, TValue>(this ValueEnumerable<TEnumerator, KeyValuePair<TKey, TValue>> source, IEqualityComparer<TKey>? keyComparer = null, IEqualityComparer<TValue>? valueComparer = null)
        where TEnumerator : struct, IValueEnumerator<KeyValuePair<TKey, TValue>>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        where TKey : notnull
    {
        using var e = source.Enumerator;

        var builder = ImmutableDictionary.CreateBuilder<TKey, TValue>(keyComparer, valueComparer);

        if (e.TryGetSpan(out var span))
        {
            foreach (var current in span)
            {
                builder.Add(current.Key, current.Value);
            }
        }
        else
        {
            while (e.TryGetNext(out var current))
            {
                builder.Add(current.Key, current.Value);
            }
        }

        return builder.ToImmutable();
    }

    /// <summary>Creates an ImmutableSortedDictionary from a sequence according to a specified key selector function.</summary>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">A function to extract the key from each element.</param>
    /// <param name="comparer">A comparer to compare keys, or <see langword="null"/> to use the default comparer.</param>
    /// <returns>An immutable sorted dictionary that contains keys and values.</returns>
    public static ImmutableSortedDictionary<TKey, TSource> ToImmutableSortedDictionary<TEnumerator, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer = null)
       where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
       where TKey : notnull
    {
        using var e = source.Enumerator;

        var builder = ImmutableSortedDictionary.CreateBuilder<TKey, TSource>(comparer);

        if (e.TryGetSpan(out var span))
        {
            foreach (var current in span)
            {
                builder.Add(keySelector(current), current);
            }
        }
        else
        {
            while (e.TryGetNext(out var current))
            {
                builder.Add(keySelector(current), current);
            }
        }

        return builder.ToImmutable();
    }

    /// <summary>Creates an ImmutableSortedDictionary from a sequence according to a specified key selector function.</summary>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">A function to extract the key from each element.</param>
    /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
    /// <param name="keyComparer">A comparer to compare keys, or <see langword="null"/> to use the default comparer.</param>
    /// <param name="valueComparer">An equality comparer to compare values, or <see langword="null"/> to use the default equality comparer.</param>
    /// <returns>An immutable sorted dictionary that contains keys and values.</returns>
    public static ImmutableSortedDictionary<TKey, TValue> ToImmutableSortedDictionary<TEnumerator, TSource, TKey, TValue>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> elementSelector, IComparer<TKey>? keyComparer = null, IEqualityComparer<TValue>? valueComparer = null)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        where TKey : notnull
    {
        using var e = source.Enumerator;

        var builder = ImmutableSortedDictionary.CreateBuilder<TKey, TValue>(keyComparer, valueComparer);

        if (e.TryGetSpan(out var span))
        {
            foreach (var current in span)
            {
                builder.Add(keySelector(current), elementSelector(current));
            }
        }
        else
        {
            while (e.TryGetNext(out var current))
            {
                builder.Add(keySelector(current), elementSelector(current));
            }
        }

        return builder.ToImmutable();
    }

    /// <summary>Creates an ImmutableSortedDictionary from a sequence of key/value pairs.</summary>
    /// <param name="source">The source sequence.</param>
    /// <param name="keyComparer">A comparer to compare keys, or <see langword="null"/> to use the default comparer.</param>
    /// <param name="valueComparer">An equality comparer to compare values, or <see langword="null"/> to use the default equality comparer.</param>
    /// <returns>An immutable sorted dictionary that contains the keys and values from the sequence.</returns>
    public static ImmutableSortedDictionary<TKey, TValue> ToImmutableSortedDictionary<TEnumerator, TKey, TValue>(this ValueEnumerable<TEnumerator, KeyValuePair<TKey, TValue>> source, IComparer<TKey>? keyComparer = null, IEqualityComparer<TValue>? valueComparer = null)
        where TEnumerator : struct, IValueEnumerator<KeyValuePair<TKey, TValue>>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        where TKey : notnull
    {
        using var e = source.Enumerator;

        var builder = ImmutableSortedDictionary.CreateBuilder<TKey, TValue>(keyComparer, valueComparer);

        if (e.TryGetSpan(out var span))
        {
            foreach (var current in span)
            {
                builder.Add(current.Key, current.Value);
            }
        }
        else
        {
            while (e.TryGetNext(out var current))
            {
                builder.Add(current.Key, current.Value);
            }
        }

        return builder.ToImmutable();
    }

    /// <summary>Creates an ImmutableHashSet from a sequence.</summary>
    /// <param name="source">The source sequence.</param>
    /// <param name="equalityComparer">An equality comparer to compare values, or <see langword="null"/> to use the default equality comparer.</param>
    /// <returns>An immutable hash set that contains distinct values from the sequence.</returns>
    public static ImmutableHashSet<TSource> ToImmutableHashSet<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, IEqualityComparer<TSource>? equalityComparer = null)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        using var e = source.Enumerator;

        if (e.TryGetSpan(out var span))
        {
            return ImmutableHashSet.Create(equalityComparer, span);
        }
        else
        {
            var builder = ImmutableHashSet.CreateBuilder<TSource>(equalityComparer);

            while (e.TryGetNext(out var current))
            {
                builder.Add(current);
            }

            return builder.ToImmutable();
        }
    }

    /// <summary>Creates an ImmutableSortedSet from a sequence.</summary>
    /// <param name="source">The source sequence.</param>
    /// <param name="comparer">A comparer to compare elements, or <see langword="null"/> to use the default comparer.</param>
    /// <returns>An immutable sorted set that contains distinct values from the sequence.</returns>
    public static ImmutableSortedSet<TSource> ToImmutableSortedSet<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, IComparer<TSource>? comparer = null)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        using var e = source.Enumerator;

        if (e.TryGetSpan(out var span))
        {
            return ImmutableSortedSet.Create(comparer, span);
        }
        else
        {
            var builder = ImmutableSortedSet.CreateBuilder<TSource>(comparer);

            while (e.TryGetNext(out var current))
            {
                builder.Add(current);
            }

            return builder.ToImmutable();
        }
    }
}

#endif
