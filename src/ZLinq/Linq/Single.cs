namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Returns the only element of a sequence, and throws an exception if there is not exactly one element in the sequence.</summary>
        /// <param name="source">The source sequence.</param>
        /// <returns>The single element of the sequence.</returns>
        public static TSource Single<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            var enumerator = source.Enumerator;
            try
            {
                return TryGetSingle<TEnumerator, TSource>(ref enumerator, out var value)
                    ? value
                    : Throws.NoElements<TSource>();
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        /// <summary>Returns the only element of a sequence that satisfies a specified condition, and throws an exception if more than one such element exists.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The single element of the sequence that satisfies the condition.</returns>
        public static TSource Single<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Boolean> predicate)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var enumerator = source.Enumerator;
            try
            {
                return TryGetSingle<TEnumerator, TSource>(ref enumerator, predicate, out var value)
                ? value
                : Throws.NoMatch<TSource>();
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        /// <summary>Returns the only element of a sequence, or a default value if the sequence is empty; this method throws an exception if there is more than one element in the sequence.</summary>
        /// <param name="source">The source sequence.</param>
        /// <returns>The single element of the sequence, or the default value if the sequence contains no elements.</returns>
        public static TSource? SingleOrDefault<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
    where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
        {
            var enumerator = source.Enumerator;
            try
            {
                return TryGetSingle<TEnumerator, TSource>(ref enumerator, out var value)
                    ? value
                    : default;
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        /// <summary>Returns the only element of a sequence, or a default value if the sequence is empty; this method throws an exception if there is more than one element in the sequence.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="defaultValue">The default value to return if the sequence is empty.</param>
        /// <returns>The single element of the sequence, or the default value if the sequence contains no elements.</returns>
        public static TSource SingleOrDefault<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, TSource defaultValue)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            var enumerator = source.Enumerator;
            try
            {
                return TryGetSingle<TEnumerator, TSource>(ref enumerator, out var value)
                    ? value
                    : defaultValue;
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        /// <summary>Returns the only element of a sequence that satisfies a specified condition, or a default value if no such element exists; this method throws an exception if more than one element satisfies the condition.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The single element of the sequence that satisfies the condition, or the default value if no such element is found.</returns>
        public static TSource? SingleOrDefault<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Boolean> predicate)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var enumerator = source.Enumerator;
            try
            {
                return TryGetSingle<TEnumerator, TSource>(ref enumerator, predicate, out var value)
                ? value
                : default;
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        /// <summary>Returns the only element of a sequence that satisfies a specified condition, or a default value if no such element exists; this method throws an exception if more than one element satisfies the condition.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="defaultValue">The default value to return if no element satisfies the condition.</param>
        /// <returns>The single element of the sequence that satisfies the condition, or the default value if no such element is found.</returns>
        public static TSource SingleOrDefault<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Boolean> predicate, TSource defaultValue)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var enumerator = source.Enumerator;
            try
            {
                return TryGetSingle<TEnumerator, TSource>(ref enumerator, predicate, out var value)
                ? value
                : defaultValue;
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        // Try... but throws exception when there are more than one element.
        static bool TryGetSingle<TEnumerator, TSource>(ref TEnumerator source, out TSource value)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            if (source.TryGetSpan(out var span))
            {
                if (span.Length == 1)
                {
                    value = span[0];
                    return true;
                }
                else if (span.Length != 0)
                {
                    Throws.MoreThanOneElement();
                }
            }
            else
            {
                if (source.TryGetNext(out value))
                {
                    if (source.TryGetNext(out _))
                    {
                        Throws.MoreThanOneElement();
                    }
                    return true;
                }
            }

            value = default!;
            return false;
        }

        static bool TryGetSingle<TEnumerator, TSource>(ref TEnumerator source, Func<TSource, Boolean> predicate, out TSource value)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            value = default!;
            bool found = false;
            if (source.TryGetSpan(out var span))
            {
                foreach (var item in span)
                {
                    if (predicate(item))
                    {
                        if (found)
                        {
                            Throws.MoreThanOneMatch();
                        }
                        found = true;
                        value = item;
                    }
                }
                if (found)
                {
                    return true;
                }
            }
            else
            {
                while (source.TryGetNext(out var item))
                {
                    if (predicate(item))
                    {
                        if (found)
                        {
                            Throws.MoreThanOneMatch();
                        }
                        found = true;
                        value = item;
                    }
                }
                if (found)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
