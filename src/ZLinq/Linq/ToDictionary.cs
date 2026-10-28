namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Creates a <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/> from a sequence of key/value pairs.</summary>
        /// <param name="source">The source sequence.</param>
        /// <returns>A dictionary that contains the keys and values from the sequence.</returns>
        public static Dictionary<TKey, TValue> ToDictionary<TEnumerator, TKey, TValue>(this ValueEnumerable<TEnumerator, KeyValuePair<TKey, TValue>> source)
            where TKey : notnull
            where TEnumerator : struct, IValueEnumerator<KeyValuePair<TKey, TValue>>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return ToDictionary(source, null);
        }

        /// <summary>Creates a <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/> from a sequence of key/value pairs.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="comparer">An equality comparer to compare keys, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>A dictionary that contains the keys and values from the sequence.</returns>
        public static Dictionary<TKey, TValue> ToDictionary<TEnumerator, TKey, TValue>(this ValueEnumerable<TEnumerator, KeyValuePair<TKey, TValue>> source, IEqualityComparer<TKey>? comparer)
            where TKey : notnull
            where TEnumerator : struct, IValueEnumerator<KeyValuePair<TKey, TValue>>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using var enumerator = source.Enumerator;
            if (enumerator.TryGetSpan(out var span))
            {
                var dict = new Dictionary<TKey, TValue>(span.Length, comparer);
                foreach (var item in span)
                {
                    dict.Add(item.Key, item.Value);
                }
                return dict;
            }
            else
            {
                var dict = enumerator.TryGetNonEnumeratedCount(out var count)
                    ? new Dictionary<TKey, TValue>(count, comparer)
                    : new Dictionary<TKey, TValue>(comparer);
                while (enumerator.TryGetNext(out var item))
                {
                    dict.Add(item.Key, item.Value);
                }
                return dict;
            }
        }

        /// <summary>Creates a <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/> from a sequence of key/value pairs.</summary>
        /// <param name="source">The source sequence.</param>
        /// <returns>A dictionary that contains the keys and values from the sequence.</returns>
        public static Dictionary<TKey, TValue> ToDictionary<TEnumerator, TKey, TValue>(this ValueEnumerable<TEnumerator, (TKey Key, TValue Value)> source)
where TKey : notnull
where TEnumerator : struct, IValueEnumerator<(TKey Key, TValue Value)>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return ToDictionary(source, null);
        }

        /// <summary>Creates a <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/> from a sequence of key/value pairs.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="comparer">An equality comparer to compare keys, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>A dictionary that contains the keys and values from the sequence.</returns>
        public static Dictionary<TKey, TValue> ToDictionary<TEnumerator, TKey, TValue>(this ValueEnumerable<TEnumerator, (TKey Key, TValue Value)> source, IEqualityComparer<TKey>? comparer)
            where TKey : notnull
            where TEnumerator : struct, IValueEnumerator<(TKey Key, TValue Value)>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using var enumerator = source.Enumerator;
            if (enumerator.TryGetSpan(out var span))
            {
                var dict = new Dictionary<TKey, TValue>(span.Length, comparer);
                foreach (var item in span)
                {
                    dict.Add(item.Key, item.Value);
                }
                return dict;
            }
            else
            {
                var dict = enumerator.TryGetNonEnumeratedCount(out var count)
                    ? new Dictionary<TKey, TValue>(count, comparer)
                    : new Dictionary<TKey, TValue>(comparer);
                while (enumerator.TryGetNext(out var item))
                {
                    dict.Add(item.Key, item.Value);
                }
                return dict;
            }
        }

        /// <summary>Creates a <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/> from a sequence according to a specified key selector function.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <returns>A dictionary that contains keys and values.</returns>
        public static Dictionary<TKey, TSource> ToDictionary<TEnumerator, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector)
            where TKey : notnull
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return ToDictionary(source, keySelector, null);
        }

        /// <summary>Creates a <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/> from a sequence according to a specified key selector function.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <param name="comparer">An equality comparer to compare keys, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>A dictionary that contains keys and values.</returns>
        public static Dictionary<TKey, TSource> ToDictionary<TEnumerator, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
            where TKey : notnull
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(keySelector);

            using var enumerator = source.Enumerator;
            if (enumerator.TryGetSpan(out var span))
            {
                var dict = new Dictionary<TKey, TSource>(span.Length, comparer);
                foreach (var item in span)
                {
                    dict.Add(keySelector(item), item);
                }
                return dict;
            }
            else
            {
                var dict = enumerator.TryGetNonEnumeratedCount(out var count)
                    ? new Dictionary<TKey, TSource>(count, comparer)
                    : new Dictionary<TKey, TSource>(comparer);
                while (enumerator.TryGetNext(out var item))
                {
                    dict.Add(keySelector(item), item);
                }
                return dict;
            }
        }

        /// <summary>Creates a <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/> from a sequence according to a specified key selector function.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>A dictionary that contains keys and values.</returns>
        public static Dictionary<TKey, TElement> ToDictionary<TEnumerator, TSource, TKey, TElement>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
            where TKey : notnull
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return ToDictionary(source, keySelector, elementSelector, null!);
        }

        /// <summary>Creates a <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/> from a sequence according to a specified key selector function.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="comparer">An equality comparer to compare keys, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>A dictionary that contains keys and values.</returns>
        public static Dictionary<TKey, TElement> ToDictionary<TEnumerator, TSource, TKey, TElement>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey>? comparer)
            where TKey : notnull
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(keySelector);
            ArgumentNullException.ThrowIfNull(elementSelector);

            using var enumerator = source.Enumerator;
            if (enumerator.TryGetSpan(out var span))
            {
                var dict = new Dictionary<TKey, TElement>(span.Length, comparer);
                foreach (var item in span)
                {
                    dict.Add(keySelector(item), elementSelector(item));
                }
                return dict;
            }
            else
            {
                var dict = enumerator.TryGetNonEnumeratedCount(out var count)
                    ? new Dictionary<TKey, TElement>(count, comparer)
                    : new Dictionary<TKey, TElement>(comparer);
                while (enumerator.TryGetNext(out var item))
                {
                    dict.Add(keySelector(item), elementSelector(item));
                }
                return dict;
            }
        }
    }
}
