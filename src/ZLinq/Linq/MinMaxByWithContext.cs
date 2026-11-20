namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Returns the element with the minimum key produced using the supplied context.</summary>
        public static TSource? MinBy<TEnumerator, TSource, TKey, TContext>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TContext, TKey> keySelector, TContext context, IComparer<TKey>? comparer = null)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => MinMaxByWithContext<TEnumerator, TSource, TKey, TContext>(source, keySelector, context, comparer, false);

        /// <summary>Returns the element with the maximum key produced using the supplied context.</summary>
        public static TSource? MaxBy<TEnumerator, TSource, TKey, TContext>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TContext, TKey> keySelector, TContext context, IComparer<TKey>? comparer = null)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => MinMaxByWithContext<TEnumerator, TSource, TKey, TContext>(source, keySelector, context, comparer, true);

        static TSource? MinMaxByWithContext<TEnumerator, TSource, TKey, TContext>(ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TContext, TKey> keySelector, TContext context, IComparer<TKey>? comparer, bool maximum)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(keySelector);
            comparer ??= Comparer<TKey>.Default;

            using var enumerator = source.Enumerator;
            if (!enumerator.TryGetNext(out var value))
            {
                if (default(TSource) is null) return default;
                Throws.NoElements();
            }

            var key = keySelector(value, context);
            if (default(TKey) is null)
            {
                if (key is null)
                {
                    var firstValue = value;
                    do
                    {
                        if (!enumerator.TryGetNext(out var current)) return firstValue;
                        value = current;
                        key = keySelector(value, context);
                    }
                    while (key is null);
                }

                while (enumerator.TryGetNext(out var current))
                {
                    var currentKey = keySelector(current, context);
                    if (currentKey is not null && IsBetter(comparer.Compare(currentKey, key), maximum))
                    {
                        key = currentKey;
                        value = current;
                    }
                }
            }
            else
            {
                while (enumerator.TryGetNext(out var current))
                {
                    var currentKey = keySelector(current, context);
                    if (IsBetter(comparer.Compare(currentKey, key), maximum))
                    {
                        key = currentKey;
                        value = current;
                    }
                }
            }

            return value;
        }

        static bool IsBetter(int comparison, bool maximum) => maximum ? comparison > 0 : comparison < 0;
    }
}
