namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        // The original dotnet implementation uses a Dictionary so it has TKey : notnull constraints.
        // However, I think it would be better without any null constraints.
        // https://github.com/dotnet/runtime/issues/98259
        // I'm going to use DictionarySlim instead of Dictionary so removed TKey : notnull constraints.

        /// <summary>Applies an accumulator function over a sequence, grouping results by key.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <param name="seed">The initial accumulator value for each key.</param>
        /// <param name="func">An accumulator function to be invoked on each element.</param>
        /// <returns>A sequence that contains the aggregates corresponding to each key.</returns>
        public static ValueEnumerable<AggregateBy<TEnumerator, TSource, TKey, TAccumulate>, KeyValuePair<TKey, TAccumulate>> AggregateBy<TEnumerator, TSource, TKey, TAccumulate>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(keySelector), seed, Throws.IfNull(func), null));

        /// <summary>Applies an accumulator function over a sequence, grouping results by key.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <param name="seed">The initial accumulator value for each key.</param>
        /// <param name="func">An accumulator function to be invoked on each element.</param>
        /// <param name="keyComparer">An equality comparer to compare keys, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>A sequence that contains the aggregates corresponding to each key.</returns>
        public static ValueEnumerable<AggregateBy<TEnumerator, TSource, TKey, TAccumulate>, KeyValuePair<TKey, TAccumulate>> AggregateBy<TEnumerator, TSource, TKey, TAccumulate>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, IEqualityComparer<TKey>? keyComparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(keySelector), seed, Throws.IfNull(func), keyComparer));

        /// <summary>Applies an accumulator function over a sequence, grouping results by key.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <param name="seedSelector">A function to create the initial accumulator value for each key.</param>
        /// <param name="func">An accumulator function to be invoked on each element.</param>
        /// <returns>A sequence that contains the aggregates corresponding to each key.</returns>
        public static ValueEnumerable<AggregateBy2<TEnumerator, TSource, TKey, TAccumulate>, KeyValuePair<TKey, TAccumulate>> AggregateBy<TEnumerator, TSource, TKey, TAccumulate>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TAccumulate> seedSelector, Func<TAccumulate, TSource, TAccumulate> func)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(keySelector), Throws.IfNull(seedSelector), Throws.IfNull(func), null));

        /// <summary>Applies an accumulator function over a sequence, grouping results by key.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <param name="seedSelector">A function to create the initial accumulator value for each key.</param>
        /// <param name="func">An accumulator function to be invoked on each element.</param>
        /// <param name="keyComparer">An equality comparer to compare keys, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>A sequence that contains the aggregates corresponding to each key.</returns>
        public static ValueEnumerable<AggregateBy2<TEnumerator, TSource, TKey, TAccumulate>, KeyValuePair<TKey, TAccumulate>> AggregateBy<TEnumerator, TSource, TKey, TAccumulate>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TAccumulate> seedSelector, Func<TAccumulate, TSource, TAccumulate> func, IEqualityComparer<TKey>? keyComparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(keySelector), Throws.IfNull(seedSelector), Throws.IfNull(func), keyComparer));
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
    struct AggregateBy<TEnumerator, TSource, TKey, TAccumulate>(TEnumerator source, Func<TSource, TKey> keySelector, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, IEqualityComparer<TKey>? keyComparer)
        : IValueEnumerator<KeyValuePair<TKey, TAccumulate>>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;

        DictionarySlim<TKey, TAccumulate>? dictionary;
        DictionarySlim<TKey, TAccumulate>.Enumerator enumerator;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<KeyValuePair<TKey, TAccumulate>> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<KeyValuePair<TKey, TAccumulate>> destination, Index offset) => false;

        public bool TryGetNext(out KeyValuePair<TKey, TAccumulate> current)
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
                ? new DictionarySlim<TKey, TAccumulate>(keyComparer)
                : new DictionarySlim<TKey, TAccumulate>();

            using (source)
            {
                if (source.TryGetSpan(out var span))
                {
                    foreach (var item in span)
                    {
                        var key = keySelector(item);
                        ref var accumulate = ref dict.GetValueRefOrAddDefault(key, out var exists);
                        accumulate = func(exists ? accumulate! : seed, item);
                    }
                }
                else
                {
                    while (source.TryGetNext(out var item))
                    {
                        var key = keySelector(item);
                        ref var accumulate = ref dict.GetValueRefOrAddDefault(key, out var exists);
                        accumulate = func(exists ? accumulate! : seed, item);
                    }
                }
            }

            this.dictionary = dict;
            this.enumerator = dict.GetEnumerator();
        }

        public void Dispose()
        {
            dictionary?.Dispose();
            source.Dispose();
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct AggregateBy2<TEnumerator, TSource, TKey, TAccumulate>(TEnumerator source, Func<TSource, TKey> keySelector, Func<TKey, TAccumulate> seedSelector, Func<TAccumulate, TSource, TAccumulate> func, IEqualityComparer<TKey>? keyComparer)
        : IValueEnumerator<KeyValuePair<TKey, TAccumulate>>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;

        DictionarySlim<TKey, TAccumulate>? dictionary;
        DictionarySlim<TKey, TAccumulate>.Enumerator enumerator;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<KeyValuePair<TKey, TAccumulate>> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<KeyValuePair<TKey, TAccumulate>> destination, Index offset) => false;

        public bool TryGetNext(out KeyValuePair<TKey, TAccumulate> current)
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
                ? new DictionarySlim<TKey, TAccumulate>(keyComparer)
                : new DictionarySlim<TKey, TAccumulate>();

            using (source)
            {
                if (source.TryGetSpan(out var span))
                {
                    foreach (var item in span)
                    {
                        var key = keySelector(item);
                        ref var accumulate = ref dict.GetValueRefOrAddDefault(key, out var exists);
                        accumulate = func(exists ? accumulate! : seedSelector(key), item);
                    }
                }
                else
                {
                    while (source.TryGetNext(out var item))
                    {
                        var key = keySelector(item);
                        ref var accumulate = ref dict.GetValueRefOrAddDefault(key, out var exists);
                        accumulate = func(exists ? accumulate! : seedSelector(key), item);
                    }
                }
            }

            this.dictionary = dict;
            this.enumerator = dict.GetEnumerator();
        }

        public void Dispose()
        {
            dictionary?.Dispose();
            source.Dispose();
        }
    }
}
