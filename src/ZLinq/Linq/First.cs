using System;

namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Returns the first element of a sequence.</summary>
        /// <param name="source">The source sequence.</param>
        /// <returns>The first element in the sequence.</returns>
        /// <remarks>Throws <see cref="System.InvalidOperationException"/> if the sequence is empty.</remarks>
        public static TSource First<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            var enumerator = source.Enumerator;
            try
            {
                return TryGetFirst<TEnumerator, TSource>(ref enumerator, out var value)
                    ? value
                    : Throws.NoElements<TSource>();
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        /// <summary>Returns the first element in a sequence that satisfies a specified condition.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The first element in the sequence that passes the test in the specified predicate function.</returns>
        /// <remarks>Throws <see cref="System.InvalidOperationException"/> if no element satisfies the condition.</remarks>
        public static TSource First<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Boolean> predicate)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var enumerator = source.Enumerator;
            try
            {
                return TryGetFirst<TEnumerator, TSource>(ref enumerator, predicate, out var value)
                ? value
                : Throws.NoMatch<TSource>();
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        /// <summary>Returns the first element of a sequence, or a default value if the sequence contains no elements.</summary>
        /// <param name="source">The source sequence.</param>
        /// <returns>The default value if the sequence is empty; otherwise, the first element.</returns>
        public static TSource? FirstOrDefault<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            var enumerator = source.Enumerator;
            try
            {
                return TryGetFirst<TEnumerator, TSource>(ref enumerator, out var value)
                ? value
                : default;
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        /// <summary>Returns the first element of a sequence, or a default value if the sequence contains no elements.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="defaultValue">The default value to return if the sequence is empty.</param>
        /// <returns>The default value if the sequence is empty; otherwise, the first element.</returns>
        public static TSource FirstOrDefault<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, TSource defaultValue)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            var enumerator = source.Enumerator;
            try
            {
                return TryGetFirst<TEnumerator, TSource>(ref enumerator, out var value)
                ? value
                : defaultValue;
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        /// <summary>Returns the first element of the sequence that satisfies a condition, or a default value if no such element is found.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The default value if no element passes the test; otherwise, the first element that passes the test.</returns>
        public static TSource? FirstOrDefault<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Boolean> predicate)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var enumerator = source.Enumerator;
            try
            {
                return TryGetFirst<TEnumerator, TSource>(ref enumerator, predicate, out var value)
                ? value
                : default;
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        /// <summary>Returns the first element of the sequence that satisfies a condition, or a default value if no such element is found.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="defaultValue">The default value to return if no element satisfies the condition.</param>
        /// <returns>The default value if no element passes the test; otherwise, the first element that passes the test.</returns>
        public static TSource FirstOrDefault<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Boolean> predicate, TSource defaultValue)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var enumerator = source.Enumerator;
            try
            {
                return TryGetFirst<TEnumerator, TSource>(ref enumerator, predicate, out var value)
                ? value
                : defaultValue;
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        static bool TryGetFirst<TEnumerator, TSource>(ref TEnumerator source, out TSource value)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            var current = default(TSource)!;
#if NETSTANDARD2_0
            var span = SingleSpan.Create<TSource>();
            if (source.TryCopyTo(span, 0))
            {
                value = span[0];
                span.Clear();
                return true;
            }
#else
            if (source.TryCopyTo(SingleSpan.Create(ref current), 0))
            {
                value = current;
                return true;
            }
#endif
            else if (EnumeratorHelper.TryConsumeGetAt<TEnumerator, TSource>(ref source, 0, out current))
            {
                value = current!;
                return true;
            }

            value = default!;
            return false;
        }

        static bool TryGetFirst<TEnumerator, TSource>(ref TEnumerator source, Func<TSource, Boolean> predicate, out TSource value)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            if (source.TryGetSpan(out var span))
            {
                foreach (var item in span)
                {
                    if (predicate(item))
                    {
                        value = item;
                        return true;
                    }
                }

                value = default!;
                return false;
            }

            while (source.TryGetNext(out var current))
            {
                if (predicate(current))
                {
                    value = current;
                    return true;
                }
            }

            value = default!;
            return false;
        }
    }
}
