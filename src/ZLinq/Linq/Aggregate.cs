namespace ZLinq;

partial class ValueEnumerableExtensions
{
    /// <summary>Applies an accumulator function over a sequence.</summary>
    /// <param name="source">The source sequence.</param>
    /// <param name="func">An accumulator function to be invoked on each element.</param>
    /// <returns>The final accumulator value.</returns>
    public static TSource Aggregate<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TSource, TSource> func)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        ArgumentNullException.ThrowIfNull(func);

        using (var enumerator = source.Enumerator)
        {
            if (enumerator.TryGetSpan(out var span))
            {
                if (span.Length == 0)
                {
                    Internal.Throws.NoElements<TSource>();
                }

                var result = span[0];
                for (int i = 1; (uint)i < (uint)span.Length; i++)
                {
                    result = func(result, span[i]);
                }

                return result;
            }
            else
            {
                if (!enumerator.TryGetNext(out var result))
                {
                    Internal.Throws.NoElements<TSource>();
                }

                while (enumerator.TryGetNext(out var current))
                {
                    result = func(result, current);
                }

                return result;
            }
        }
    }

    /// <summary>Applies an accumulator function over a sequence. The specified seed value is used as the initial accumulator value.</summary>
    /// <param name="source">The source sequence.</param>
    /// <param name="seed">The initial accumulator value.</param>
    /// <param name="func">An accumulator function to be invoked on each element.</param>
    /// <returns>The final accumulator value.</returns>
    public static TAccumulate Aggregate<TEnumerator, TSource, TAccumulate>(this ValueEnumerable<TEnumerator, TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        ArgumentNullException.ThrowIfNull(func);

        using (var enumerator = source.Enumerator)
        {
            if (enumerator.TryGetSpan(out var span))
            {
                var result = seed;
                foreach (var item in span)
                {
                    result = func(result, item);
                }

                return result;
            }
            else
            {

                var result = seed;
                while (enumerator.TryGetNext(out var current))
                {
                    result = func(result, current);
                }

                return result;
            }
        }
    }

    /// <summary>Applies an accumulator function over a sequence. The specified seed value is used as the initial accumulator value, and the specified function is used to select the result value.</summary>
    /// <param name="source">The source sequence.</param>
    /// <param name="seed">The initial accumulator value.</param>
    /// <param name="func">An accumulator function to be invoked on each element.</param>
    /// <param name="resultSelector">A function to transform the final accumulator value into the result value.</param>
    /// <returns>The transformed final accumulator value.</returns>
    public static TResult Aggregate<TEnumerator, TSource, TAccumulate, TResult>(this ValueEnumerable<TEnumerator, TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        ArgumentNullException.ThrowIfNull(func);
        ArgumentNullException.ThrowIfNull(resultSelector);

        using (var enumerator = source.Enumerator)
        {
            if (enumerator.TryGetSpan(out var span))
            {
                var result = seed;
                foreach (var item in span)
                {
                    result = func(result, item);
                }

                return resultSelector(result);
            }
            else
            {
                var result = seed;
                while (enumerator.TryGetNext(out var current))
                {
                    result = func(result, current);
                }

                return resultSelector(result);
            }
        }
    }
}
