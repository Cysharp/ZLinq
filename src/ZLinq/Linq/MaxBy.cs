namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Returns the maximum value in a sequence according to a specified key selector function.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <returns>The value with the maximum key in the sequence.</returns>
        public static TSource? MaxBy<TEnumerator, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(keySelector);
            return MaxBy(source, keySelector, null);
        }

        /// <summary>Returns the maximum value in a sequence according to a specified key selector function.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <param name="comparer">A comparer to compare keys, or <see langword="null"/> to use the default comparer.</param>
        /// <returns>The value with the maximum key in the sequence.</returns>
        public static TSource? MaxBy<TEnumerator, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(keySelector);
            comparer ??= Comparer<TKey>.Default;

            using (var enumerator = source.Enumerator)
            {
                if (!enumerator.TryGetNext(out var value)) // TSource value;
                {
                    if (default(TSource) is null)
                    {
                        return default;
                    }
                    else
                    {
                        Throws.NoElements();
                    }
                }

                // store first value and key.
                TKey key = keySelector(value);

                if (default(TKey) is null)
                {
                    // reference type
                    if (key is null)
                    {
                        var firstValue = value;

                        do
                        {
                            if (!enumerator.TryGetNext(out var current))
                            {
                                return firstValue; // return first-value when all keys are null.
                            }
                            value = current;
                            key = keySelector(value);
                        }
                        while (key is null);
                    }

                    while (enumerator.TryGetNext(out var current))
                    {
                        var currentKey = keySelector(current);
                        if (currentKey is not null && comparer.Compare(currentKey, key) > 0)
                        {
                            key = currentKey;
                            value = current;
                        }
                    }
                }
                else
                {
                    // value type
                    if (comparer == Comparer<TKey>.Default)
                    {
                        while (enumerator.TryGetNext(out var current))
                        {
                            var currentKey = keySelector(current);
                            if (Comparer<TKey>.Default.Compare(currentKey, key) > 0)
                            {
                                key = currentKey;
                                value = current;
                            }
                        }

                        return value;
                    }
                    else
                    {
                        while (enumerator.TryGetNext(out var current))
                        {
                            var currentKey = keySelector(current);
                            if (comparer.Compare(currentKey, key) > 0)
                            {
                                key = currentKey;
                                value = current;
                            }
                        }
                    }
                }

                return value;
            }
        }
    }
}
