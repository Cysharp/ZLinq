namespace ZLinq;

#if NET9_0_OR_GREATER

// ValueEnumerable is a ref struct on .NET 9+, so it never binds to the string.Join overloads.
// These extension members let existing string.Join(separator, values) call sites compile unchanged (C# 14+).
public static class StringJoinExtensions
{
    extension(string)
    {
        public static string Join<TEnumerator, TSource>(string? separator, ValueEnumerable<TEnumerator, TSource> values)
            where TEnumerator : struct, IValueEnumerator<TSource>, allows ref struct
            => values.JoinToString(separator.AsSpan());

        public static string Join<TEnumerator, TSource>(char separator, ValueEnumerable<TEnumerator, TSource> values)
            where TEnumerator : struct, IValueEnumerator<TSource>, allows ref struct
            => values.JoinToString(separator);
    }
}

#endif
