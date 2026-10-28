namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Returns the last element of a sequence.</summary>
        /// <param name="source">The source sequence.</param>
        /// <returns>The last element in the sequence.</returns>
        /// <remarks>Throws <see cref="System.InvalidOperationException"/> if the sequence is empty.</remarks>
        public static TSource Last<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            var enumerator = source.Enumerator;
            try
            {
                return TryGetLast<TEnumerator, TSource>(ref enumerator, out var value)
                ? value
                : Throws.NoElements<TSource>();
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        /// <summary>Returns the last element of a sequence that satisfies a specified condition.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The last element in the sequence that passes the test in the specified predicate function.</returns>
        /// <remarks>Throws <see cref="System.InvalidOperationException"/> if no element satisfies the condition.</remarks>
        public static TSource Last<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Boolean> predicate)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var enumerator = source.Enumerator;
            try
            {
                return TryGetLast<TEnumerator, TSource>(ref enumerator, predicate, out var value)
                ? value
                : Throws.NoMatch<TSource>();
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        /// <summary>Returns the last element of a sequence, or a default value if the sequence contains no elements.</summary>
        /// <param name="source">The source sequence.</param>
        /// <returns>The default value if the sequence is empty; otherwise, the last element.</returns>
        public static TSource? LastOrDefault<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
    where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
        {
            var enumerator = source.Enumerator;
            try
            {
                return TryGetLast<TEnumerator, TSource>(ref enumerator, out var value)
                ? value
                : default;
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        /// <summary>Returns the last element of a sequence, or a default value if the sequence contains no elements.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="defaultValue">The default value to return if the sequence is empty.</param>
        /// <returns>The default value if the sequence is empty; otherwise, the last element.</returns>
        public static TSource LastOrDefault<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, TSource defaultValue)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            var enumerator = source.Enumerator;
            try
            {
                return TryGetLast<TEnumerator, TSource>(ref enumerator, out var value)
                ? value
                : defaultValue;
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        /// <summary>Returns the last element of a sequence that satisfies a condition, or a default value if no such element is found.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The default value if no element passes the test; otherwise, the last element that passes the test.</returns>
        public static TSource? LastOrDefault<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Boolean> predicate)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var enumerator = source.Enumerator;
            try
            {
                return TryGetLast<TEnumerator, TSource>(ref enumerator, predicate, out var value)
                ? value
                : default;
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        /// <summary>Returns the last element of a sequence that satisfies a condition, or a default value if no such element is found.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="defaultValue">The default value to return if no element satisfies the condition.</param>
        /// <returns>The default value if no element passes the test; otherwise, the last element that passes the test.</returns>
        public static TSource LastOrDefault<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Boolean> predicate, TSource defaultValue)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var enumerator = source.Enumerator;
            try
            {
                return TryGetLast<TEnumerator, TSource>(ref enumerator, predicate, out var value)
                ? value
                : defaultValue;
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        static bool TryGetLast<TEnumerator, TSource>(ref TEnumerator source, out TSource value)
           where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
           , allows ref struct
#endif
        {

            var current = default(TSource)!;
#if NETSTANDARD2_0
            var span = SingleSpan.Create<TSource>();
            if (source.TryCopyTo(span, ^1))
            {
                value = span[0];
                span.Clear();
                return true;
            }
#else
            if (source.TryCopyTo(SingleSpan.Create(ref current), ^1))
            {
                value = current;
                return true;
            }
#endif
            else if (EnumeratorHelper.TryConsumeGetAt<TEnumerator, TSource>(ref source, ^1, out current))
            {
                value = current!;
                return true;
            }

            value = default!;
            return false;
        }

        static bool TryGetLast<TEnumerator, TSource>(ref TEnumerator source, Func<TSource, Boolean> predicate, out TSource value)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            if (source.TryGetSpan(out var span))
            {
                // search from last
                for (var i = span.Length - 1; i >= 0; i--)
                {
                    ref readonly var v = ref span[i];
                    if (predicate(v))
                    {
                        value = v;
                        return true;
                    }
                }

                value = default!;
                return false;
            }

            while (source.TryGetNext(out var last))
            {
                if (predicate(last)) // found
                {
                    while (source.TryGetNext(out var current))
                    {
                        if (predicate(current))
                        {
                            last = current;
                        }
                    }

                    value = last;
                    return true;
                }
            }

            value = default!;
            return false;
        }
    }
}
