namespace ZLinq;

#if NET9_0_OR_GREATER

// ValueEnumerable is a ref struct on .NET 9+, so it never binds to the string.Join overloads.
// These extension members let existing string.Join(separator, values) call sites compile unchanged (C# 14+).
public static class StringJoinExtensions
{
    extension(string)
    {
        /// <summary>Concatenates the elements of a sequence, using the specified separator between each element. It forwards to JoinToString.</summary>
        /// <param name="separator">The separator to insert between elements, or <see langword="null"/> for no separator.</param>
        /// <param name="values">The sequence to concatenate.</param>
        /// <returns>A string that consists of the elements of the sequence delimited by the separator, or <see cref="string.Empty"/> if the sequence has no elements.</returns>
        public static string Join<TEnumerator, TSource>(string? separator, ValueEnumerable<TEnumerator, TSource> values)
            where TEnumerator : struct, IValueEnumerator<TSource>, allows ref struct
            => values.JoinToString(separator.AsSpan());

        /// <summary>Concatenates the elements of a sequence, using the specified separator between each element. It forwards to JoinToString.</summary>
        /// <param name="separator">The separator to insert between elements.</param>
        /// <param name="values">The sequence to concatenate.</param>
        /// <returns>A string that consists of the elements of the sequence delimited by the separator, or <see cref="string.Empty"/> if the sequence has no elements.</returns>
        public static string Join<TEnumerator, TSource>(char separator, ValueEnumerable<TEnumerator, TSource> values)
            where TEnumerator : struct, IValueEnumerator<TSource>, allows ref struct
            => values.JoinToString(separator);
    }
}

#endif
