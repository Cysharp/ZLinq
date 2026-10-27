namespace ZLinq;

partial class ValueEnumerableExtensions
{
    /// <summary>
    /// Unlike the semantics of normal CopyTo, this allows the destination to be smaller than the source.
    /// Returns the number of elements copied.
    /// </summary>
    public static int CopyTo<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Span<TSource> destination)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
    {
        using var enumerator = source.Enumerator;

        if (destination.IsEmpty)
        {
            return 0;
        }

        if (enumerator.TryGetNonEnumeratedCount(out var count))
        {
            if (enumerator.TryCopyTo(destination, 0))
            {
                return Math.Min(count, destination.Length);
            }
        }

        var i = 0;
        while (enumerator.TryGetNext(out var current))
        {
            destination[i] = current;
            i++;
            if (i == destination.Length)
            {
                return i;
            }
        }

        return i;
    }

    /// <summary>
    /// List is cleared and then filled with the elements of the source. Destination size is list.Count.
    /// </summary>
    public static void CopyTo<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, List<TSource> list)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
    {
        ArgumentNullException.ThrowIfNull(list);

        list.Clear(); // clear before fill.

        using var enumerator = source.Enumerator;

        if (enumerator.TryGetNonEnumeratedCount(out var count))
        {
#if NET8_0_OR_GREATER
            CollectionsMarshal.SetCount(list, count); // expand internal T[] buffer
#else
            if (list.Capacity < count)
            {
                list.Capacity = count; // Grow only buffer is smaller.
            }
            CollectionsMarshal.UnsafeSetCount(list, count); // only set count
#endif

            var span = CollectionsMarshal.AsSpan(list);
            if (enumerator.TryCopyTo(span, 0))
            {
                return;
            }

            var i = 0;
            while (enumerator.TryGetNext(out var current))
            {
                span[i] = current;
                i++;
            }
        }
        else
        {
            while (enumerator.TryGetNext(out var item))
            {
                list.Add(item);
            }
        }
    }
}
