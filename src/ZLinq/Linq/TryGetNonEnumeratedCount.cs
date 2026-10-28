namespace ZLinq;

partial class ValueEnumerableExtensions
{
    /// <summary>Attempts to determine the number of elements in a sequence without forcing an enumeration.</summary>
    /// <param name="source">The source sequence.</param>
    /// <param name="count">When this method returns, contains the number of elements in the source sequence, or 0 if the count couldn't be determined without enumeration.</param>
    /// <returns><see langword="true"/> if the count of the source can be determined without enumeration; otherwise, <see langword="false"/>.</returns>
    public static bool TryGetNonEnumeratedCount<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, out int count)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        using (var enumerator = source.Enumerator)
        {
            return enumerator.TryGetNonEnumeratedCount(out count);
        }
    }
}
