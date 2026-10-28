namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        // The original dotnet implementation uses a Dictionary so it has TKey : notnull constraints.
        // However, I think it would be better without any null constraints.
        // https://github.com/dotnet/runtime/issues/98259
        // I'm going to use DictionarySlim instead of Dictionary so removed TKey : notnull constraints.

        /// <summary>Returns the count of elements in the source sequence grouped by key.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <returns>A sequence that contains the keys and the number of elements for each key.</returns>
        public static ValueEnumerable<CountBy<TEnumerator, TSource, TKey>, KeyValuePair<TKey, int>> CountBy<TEnumerator, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(keySelector), null));

        /// <summary>Returns the count of elements in the source sequence grouped by key.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <param name="keyComparer">An equality comparer to compare keys, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>A sequence that contains the keys and the number of elements for each key.</returns>
        public static ValueEnumerable<CountBy<TEnumerator, TSource, TKey>, KeyValuePair<TKey, int>> CountBy<TEnumerator, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? keyComparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(keySelector), keyComparer));
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct CountBy<TEnumerator, TSource, TKey>(TEnumerator source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? keyComparer)
        : IValueEnumerator<KeyValuePair<TKey, int>>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;

        DictionarySlim<TKey, int>? dictionary;
        DictionarySlim<TKey, int>.Enumerator enumerator;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<KeyValuePair<TKey, int>> span)
        {
            span = default;
            return false;
        }


        public bool TryCopyTo(scoped Span<KeyValuePair<TKey, int>> destination, Index offset) => false;

        public bool TryGetNext(out KeyValuePair<TKey, int> current)
        {
            if (dictionary == null)
            {
                // if empty, don't create dictionary.
                if (source.TryGetNonEnumeratedCount(out var count) && count == 0)
                {
                    Unsafe.SkipInit(out current);
                    return false;
                }

                Initialize();
            }

            if (enumerator.TryGetNext(out current))
            {
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        void Initialize()
        {
            var dict = keyComparer != null
                ? new DictionarySlim<TKey, int>(keyComparer)
                : new DictionarySlim<TKey, int>();

            using (source)
            {
                if (source.TryGetSpan(out var span))
                {
                    foreach (var item in span)
                    {
                        var key = keySelector(item);
                        ref var count = ref dict.GetValueRefOrAddDefault(key, out var exists);
                        checked { count += 1; }
                    }
                }
                else
                {
                    while (source.TryGetNext(out var item))
                    {
                        var key = keySelector(item);
                        ref var count = ref dict.GetValueRefOrAddDefault(key, out var exists);
                        checked { count += 1; }
                    }
                }
            }

            dictionary = dict;
            enumerator = dictionary.GetEnumerator();
        }

        public void Dispose()
        {
            dictionary?.Dispose();
            source.Dispose();
        }
    }
}
