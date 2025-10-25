namespace ZLinq;

partial class ValueEnumerableExtensions
{
    const int StackallocCharBufferSizeLimit = 256;

#if NET8_0_OR_GREATER
    static string FastAllocateString(int length) => string.Create(length, (object?)null, static (_, _) => { });
#else
    static string FastAllocateString(int length) => new string('\0', length);
#endif

    public static string JoinToString<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, string separator)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        return JoinToString(source, separator.AsSpan());
    }

    public static string JoinToString<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, char separator)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
    {
#if NET8_0_OR_GREATER
        var span = new ReadOnlySpan<char>(ref separator);
#else
        ReadOnlySpan<char> span = stackalloc char[] { separator };
#endif
        return JoinToString(source, span);
    }

    public static string JoinToString<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, ReadOnlySpan<char> separator)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        using var e = source.Enumerator;

        if (e.TryGetSpan(out var span))
        {
#if !NETSTANDARD2_0
            if (typeof(TSource) == typeof(string))
            {
                var stringSpan = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<TSource, string>(ref MemoryMarshal.GetReference(span)), span.Length);
                return JoinToString(stringSpan, separator);
            }
#endif

            if (span.Length == 0) return "";
            if (span.Length == 1) return span[0]!.ToString() ?? "";

            var result = new ValueStringBuilder(stackalloc char[StackallocCharBufferSizeLimit]);

            if (separator.Length == 0)
            {
                for (int i = 0; (uint)i < (uint)span.Length; i++)
                {
                    result.Append(span[i]);
                }
            }
            else if (separator.Length == 1)
            {
                var separatorChar = separator[0];

                result.Append(span[0]);
                for (int i = 1; (uint)i < (uint)span.Length; i++)
                {
                    result.Append(separatorChar);
                    result.Append(span[i]);
                }
            }
            else
            {
                result.Append(span[0]);
                for (int i = 1; (uint)i < (uint)span.Length; i++)
                {
                    result.Append(separator);
                    result.Append(span[i]);
                }
            }

            return result.ToStringAndClear();
        }
        else
        {
            if (!e.TryGetNext(out var first))
            {
                return ""; // sequence has no value
            }

            if (!e.TryGetNext(out var second))
            {
                return first!.ToString() ?? ""; // return only first
            }

            var result = new ValueStringBuilder(stackalloc char[StackallocCharBufferSizeLimit]);
            if (separator.Length == 0)
            {
                result.Append(first);
                result.Append(second);

                while (e.TryGetNext(out var value))
                {
                    result.Append(value);
                }
            }
            else if (separator.Length == 1)
            {
                var charSeparator = separator[0];

                result.Append(first);
                result.Append(charSeparator);
                result.Append(second);

                while (e.TryGetNext(out var value))
                {
                    result.Append(charSeparator);
                    result.Append(value);
                }
            }
            else
            {
                result.Append(first);
                result.Append(separator);
                result.Append(second);

                while (e.TryGetNext(out var value))
                {
                    result.Append(separator);
                    result.Append(value);
                }
            }

            return result.ToStringAndClear();
        }
    }

    static string JoinToString(ReadOnlySpan<string> source, ReadOnlySpan<char> separator)
    {
        if (source.Length == 0) return "";
        if (source.Length == 1) return source[0];

        // calculate final length
        var finalLength = 0;
        for (int i = 0; (uint)i < (uint)source.Length; i++)
        {
            finalLength += (source[i]?.Length ?? 0);
        }
        // add separator length
        finalLength += ((source.Length - 1) * separator.Length);

        var str = FastAllocateString(finalLength);

        unsafe
        {
            fixed (char* destPointer = str.AsSpan())
            {
                Span<char> dest = new Span<char>(destPointer, finalLength);
                source[0]?.CopyTo(dest);
                dest = dest.Slice(source[0]?.Length ?? 0);

                if (separator.Length == 0)
                {
                    for (int i = 1; (uint)i < (uint)source.Length; i++)
                    {
                        source[i]?.CopyTo(dest);
                        dest = dest.Slice(source[i]?.Length ?? 0);
                    }
                }
                else if (separator.Length == 1)
                {
                    var charSeparator = separator[0];
                    for (int i = 1; (uint)i < (uint)source.Length; i++)
                    {
                        dest[0] = charSeparator;
                        source[i]?.CopyTo(dest.Slice(1));
                        dest = dest.Slice((source[i]?.Length ?? 0) + 1);
                    }
                }
                else
                {
                    for (int i = 1; (uint)i < (uint)source.Length; i++)
                    {
                        separator.CopyTo(dest);
                        dest = dest.Slice(separator.Length);
                        source[i]?.CopyTo(dest);
                        dest = dest.Slice(source[i]?.Length ?? 0);
                    }
                }
            }
        }

        return str;
    }

    #region for special types(FromArray, FromList)

    public static string JoinToString<TSource>(this ValueEnumerable<FromArray<TSource>, TSource> source, string separator)
    {
        return JoinToString(source, separator.AsSpan());
    }

    public static string JoinToString<TSource>(this ValueEnumerable<FromArray<TSource>, TSource> source, char separator)
    {
#if NET8_0_OR_GREATER
        var span = new ReadOnlySpan<char>(ref separator);
#else
        ReadOnlySpan<char> span = stackalloc char[] { separator };
#endif
        return JoinToString(source, span);
    }

    public static string JoinToString<TSource>(this ValueEnumerable<FromArray<TSource>, TSource> source, ReadOnlySpan<char> separator)
    {
        var span = source.Enumerator.GetSource(); // not span, it is array:)

        if (typeof(TSource) == typeof(string))
        {
            return JoinToString(Unsafe.As<TSource[], string[]>(ref span), separator);
        }

        if (span.Length == 0) return "";
        if (span.Length == 1) return span[0]!.ToString() ?? "";

        var result = new ValueStringBuilder(stackalloc char[StackallocCharBufferSizeLimit]);

        if (separator.Length == 0)
        {
            for (int i = 0; (uint)i < (uint)span.Length; i++)
            {
                result.Append(span[i]);
            }
        }
        else if (separator.Length == 1)
        {
            var separatorChar = separator[0];

            result.Append(span[0]);
            for (int i = 1; (uint)i < (uint)span.Length; i++)
            {
                result.Append(separatorChar);
                result.Append(span[i]);
            }
        }
        else
        {
            result.Append(span[0]);
            for (int i = 1; (uint)i < (uint)span.Length; i++)
            {
                result.Append(separator);
                result.Append(span[i]);
            }
        }

        return result.ToStringAndClear();
    }

    public static string JoinToString<TSource>(this ValueEnumerable<FromList<TSource>, TSource> source, string separator)
    {
        return JoinToString(source, separator.AsSpan());
    }

    public static string JoinToString<TSource>(this ValueEnumerable<FromList<TSource>, TSource> source, char separator)
    {
#if NET8_0_OR_GREATER
        var span = new ReadOnlySpan<char>(ref separator);
#else
        ReadOnlySpan<char> span = stackalloc char[] { separator };
#endif
        return JoinToString(source, span);
    }

    public static string JoinToString<TSource>(this ValueEnumerable<FromList<TSource>, TSource> source, ReadOnlySpan<char> separator)
    {
        var list = source.Enumerator.GetSource();

        if (typeof(TSource) == typeof(string))
        {
            return JoinToString(CollectionsMarshal.AsSpan(Unsafe.As<List<TSource>, List<string>>(ref list)), separator);
        }

        var span = CollectionsMarshal.AsSpan(list);

        if (span.Length == 0) return "";
        if (span.Length == 1) return span[0]!.ToString() ?? "";

        var result = new ValueStringBuilder(stackalloc char[StackallocCharBufferSizeLimit]);

        if (separator.Length == 0)
        {
            for (int i = 0; (uint)i < (uint)span.Length; i++)
            {
                result.Append(span[i]);
            }
        }
        else if (separator.Length == 1)
        {
            var separatorChar = separator[0];

            result.Append(span[0]);
            for (int i = 1; (uint)i < (uint)span.Length; i++)
            {
                result.Append(separatorChar);
                result.Append(span[i]);
            }
        }
        else
        {
            result.Append(span[0]);
            for (int i = 1; (uint)i < (uint)span.Length; i++)
            {
                result.Append(separator);
                result.Append(span[i]);
            }
        }

        return result.ToStringAndClear();
    }

    #endregion

    #region for Array(Select/Where/WhereSelect)

    public static string JoinToString<TSource, TResult>(this ValueEnumerable<ArraySelect<TSource, TResult>, TResult> source, string separator)
    {
        return JoinToString(source, separator.AsSpan());
    }

    public static string JoinToString<TSource, TResult>(this ValueEnumerable<ArraySelect<TSource, TResult>, TResult> source, char separator)
    {
#if NET8_0_OR_GREATER
        var span = new ReadOnlySpan<char>(ref separator);
#else
        ReadOnlySpan<char> span = stackalloc char[] { separator };
#endif
        return JoinToString(source, span);
    }

    public static string JoinToString<TSource, TResult>(this ValueEnumerable<ArraySelect<TSource, TResult>, TResult> source, ReadOnlySpan<char> separator)
    {
        var span = source.Enumerator.source; // array:)
        var selector = source.Enumerator.selector;

        if (span.Length == 0) return "";
        if (span.Length == 1) return selector(span[0])?.ToString() ?? "";

        var result = new ValueStringBuilder(stackalloc char[StackallocCharBufferSizeLimit]);
        result.Append(selector(span[0]));
        var i = 1;

        if (separator.Length == 0)
        {
            for (; (uint)i < (uint)span.Length; i++)
            {
                result.Append(selector(span[i]));
            }
        }
        else if (separator.Length == 1)
        {
            var charSeparator = separator[0];
            for (; (uint)i < (uint)span.Length; i++)
            {
                result.Append(charSeparator);
                result.Append(selector(span[i]));
            }
        }
        else
        {
            for (; (uint)i < (uint)span.Length; i++)
            {
                result.Append(separator);
                result.Append(selector(span[i]));
            }
        }

        return result.ToStringAndClear();
    }

    public static string JoinToString<TSource>(this ValueEnumerable<ArrayWhere<TSource>, TSource> source, string separator)
    {
        return JoinToString(source, separator.AsSpan());
    }

    public static string JoinToString<TSource>(this ValueEnumerable<ArrayWhere<TSource>, TSource> source, char separator)
    {
#if NET8_0_OR_GREATER
        var span = new ReadOnlySpan<char>(ref separator);
#else
        ReadOnlySpan<char> span = stackalloc char[] { separator };
#endif
        return JoinToString(source, span);
    }

    public static string JoinToString<TSource>(this ValueEnumerable<ArrayWhere<TSource>, TSource> source, ReadOnlySpan<char> separator)
    {
        var span = source.Enumerator.GetSource(); // array
        var predicate = source.Enumerator.Predicate;

        if (span.Length == 0) return "";

        var result = new ValueStringBuilder(stackalloc char[StackallocCharBufferSizeLimit]);
        var i = 0;
        for (; (uint)i < (uint)span.Length; i++)
        {
            if (predicate(span[i]))
            {
                result.Append(span[i]);
                i++;
                break;
            }
        }

        if (separator.Length == 0)
        {
            for (; (uint)i < (uint)span.Length; i++)
            {
                if (predicate(span[i]))
                {
                    result.Append(span[i]);
                }
            }
        }
        else if (separator.Length == 1)
        {
            var charSeparator = separator[0];
            for (; (uint)i < (uint)span.Length; i++)
            {
                if (predicate(span[i]))
                {
                    result.Append(charSeparator);
                    result.Append(span[i]);
                }
            }
        }
        else
        {
            for (; (uint)i < (uint)span.Length; i++)
            {
                if (predicate(span[i]))
                {
                    result.Append(separator);
                    result.Append(span[i]);
                }
            }
        }

        return result.ToStringAndClear();
    }

    public static string JoinToString<TSource, TResult>(this ValueEnumerable<ArrayWhereSelect<TSource, TResult>, TResult> source, string separator)
    {
        return JoinToString(source, separator.AsSpan());
    }

    public static string JoinToString<TSource, TResult>(this ValueEnumerable<ArrayWhereSelect<TSource, TResult>, TResult> source, char separator)
    {
#if NET8_0_OR_GREATER
        var span = new ReadOnlySpan<char>(ref separator);
#else
        ReadOnlySpan<char> span = stackalloc char[] { separator };
#endif
        return JoinToString(source, span);
    }

    public static string JoinToString<TSource, TResult>(this ValueEnumerable<ArrayWhereSelect<TSource, TResult>, TResult> source, ReadOnlySpan<char> separator)
    {
        var span = source.Enumerator.GetSource(); // array
        var predicate = source.Enumerator.Predicate;
        var selector = source.Enumerator.Selector;

        if (span.Length == 0) return "";

        var result = new ValueStringBuilder(stackalloc char[StackallocCharBufferSizeLimit]);
        var i = 0;
        for (; (uint)i < (uint)span.Length; i++)
        {
            if (predicate(span[i]))
            {
                result.Append(selector(span[i]));
                i++;
                break;
            }
        }

        if (separator.Length == 0)
        {
            for (; (uint)i < (uint)span.Length; i++)
            {
                if (predicate(span[i]))
                {
                    result.Append(selector(span[i]));
                }
            }
        }
        else if (separator.Length == 1)
        {
            var charSeparator = separator[0];
            for (; (uint)i < (uint)span.Length; i++)
            {
                if (predicate(span[i]))
                {
                    result.Append(charSeparator);
                    result.Append(selector(span[i]));
                }
            }
        }
        else
        {
            for (; (uint)i < (uint)span.Length; i++)
            {
                if (predicate(span[i]))
                {
                    result.Append(separator);
                    result.Append(selector(span[i]));
                }
            }
        }

        return result.ToStringAndClear();
    }

    #endregion

    #region for List(Select/Where/WhereSelect)

    public static string JoinToString<TSource, TResult>(this ValueEnumerable<ListSelect<TSource, TResult>, TResult> source, string separator)
    {
        return JoinToString(source, separator.AsSpan());
    }

    public static string JoinToString<TSource, TResult>(this ValueEnumerable<ListSelect<TSource, TResult>, TResult> source, char separator)
    {
#if NET8_0_OR_GREATER
        var span = new ReadOnlySpan<char>(ref separator);
#else
        ReadOnlySpan<char> span = stackalloc char[] { separator };
#endif
        return JoinToString(source, span);
    }

    public static string JoinToString<TSource, TResult>(this ValueEnumerable<ListSelect<TSource, TResult>, TResult> source, ReadOnlySpan<char> separator)
    {
        var list = source.Enumerator.source;
        var selector = source.Enumerator.selector;

        var span = CollectionsMarshal.AsSpan(list);

        if (span.Length == 0) return "";
        if (span.Length == 1) return selector(span[0])?.ToString() ?? "";

        var result = new ValueStringBuilder(stackalloc char[StackallocCharBufferSizeLimit]);
        result.Append(selector(span[0]));
        var i = 1;

        if (separator.Length == 0)
        {
            for (; (uint)i < (uint)span.Length; i++)
            {
                result.Append(selector(span[i]));
            }
        }
        else if (separator.Length == 1)
        {
            var charSeparator = separator[0];
            for (; (uint)i < (uint)span.Length; i++)
            {
                result.Append(charSeparator);
                result.Append(selector(span[i]));
            }
        }
        else
        {
            for (; (uint)i < (uint)span.Length; i++)
            {
                result.Append(separator);
                result.Append(selector(span[i]));
            }
        }

        return result.ToStringAndClear();
    }

    public static string JoinToString<TSource>(this ValueEnumerable<ListWhere<TSource>, TSource> source, string separator)
    {
        return JoinToString(source, separator.AsSpan());
    }

    public static string JoinToString<TSource>(this ValueEnumerable<ListWhere<TSource>, TSource> source, char separator)
    {
#if NET8_0_OR_GREATER
        var span = new ReadOnlySpan<char>(ref separator);
#else
        ReadOnlySpan<char> span = stackalloc char[] { separator };
#endif
        return JoinToString(source, span);
    }

    public static string JoinToString<TSource>(this ValueEnumerable<ListWhere<TSource>, TSource> source, ReadOnlySpan<char> separator)
    {
        var list = source.Enumerator.GetSource();
        var predicate = source.Enumerator.Predicate;

        var span = CollectionsMarshal.AsSpan(list);

        if (span.Length == 0) return "";

        var result = new ValueStringBuilder(stackalloc char[StackallocCharBufferSizeLimit]);
        var i = 0;
        for (; (uint)i < (uint)span.Length; i++)
        {
            if (predicate(span[i]))
            {
                result.Append(span[i]);
                i++;
                break;
            }
        }

        if (separator.Length == 0)
        {
            for (; (uint)i < (uint)span.Length; i++)
            {
                if (predicate(span[i]))
                {
                    result.Append(span[i]);
                }
            }
        }
        else if (separator.Length == 1)
        {
            var charSeparator = separator[0];
            for (; (uint)i < (uint)span.Length; i++)
            {
                if (predicate(span[i]))
                {
                    result.Append(charSeparator);
                    result.Append(span[i]);
                }
            }
        }
        else
        {
            for (; (uint)i < (uint)span.Length; i++)
            {
                if (predicate(span[i]))
                {
                    result.Append(separator);
                    result.Append(span[i]);
                }
            }
        }

        return result.ToStringAndClear();
    }

    public static string JoinToString<TSource, TResult>(this ValueEnumerable<ListWhereSelect<TSource, TResult>, TResult> source, string separator)
    {
        return JoinToString(source, separator.AsSpan());
    }

    public static string JoinToString<TSource, TResult>(this ValueEnumerable<ListWhereSelect<TSource, TResult>, TResult> source, char separator)
    {
#if NET8_0_OR_GREATER
        var span = new ReadOnlySpan<char>(ref separator);
#else
        ReadOnlySpan<char> span = stackalloc char[] { separator };
#endif
        return JoinToString(source, span);
    }

    public static string JoinToString<TSource, TResult>(this ValueEnumerable<ListWhereSelect<TSource, TResult>, TResult> source, ReadOnlySpan<char> separator)
    {
        var list = source.Enumerator.GetSource();
        var predicate = source.Enumerator.Predicate;
        var selector = source.Enumerator.Selector;

        var span = CollectionsMarshal.AsSpan(list);

        if (span.Length == 0) return "";

        var result = new ValueStringBuilder(stackalloc char[StackallocCharBufferSizeLimit]);
        var i = 0;
        for (; (uint)i < (uint)span.Length; i++)
        {
            if (predicate(span[i]))
            {
                result.Append(selector(span[i]));
                i++;
                break;
            }
        }

        if (separator.Length == 0)
        {
            for (; (uint)i < (uint)span.Length; i++)
            {
                if (predicate(span[i]))
                {
                    result.Append(selector(span[i]));
                }
            }
        }
        else if (separator.Length == 1)
        {
            var charSeparator = separator[0];
            for (; (uint)i < (uint)span.Length; i++)
            {
                if (predicate(span[i]))
                {
                    result.Append(charSeparator);
                    result.Append(selector(span[i]));
                }
            }
        }
        else
        {
            for (; (uint)i < (uint)span.Length; i++)
            {
                if (predicate(span[i]))
                {
                    result.Append(separator);
                    result.Append(selector(span[i]));
                }
            }
        }

        return result.ToStringAndClear();
    }

    #endregion
}
