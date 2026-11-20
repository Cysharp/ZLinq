namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Applies an accumulator function to a sequence, passing the supplied context to each invocation.</summary>
        public static TSource Aggregate<TEnumerator, TSource, TContext>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TSource, TContext, TSource> func, TContext context)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(func);
            using var enumerator = source.Enumerator;
            if (enumerator.TryGetSpan(out var span))
            {
                if (span.Length == 0) Internal.Throws.NoElements<TSource>();
                var result = span[0];
                for (var i = 1; i < span.Length; i++) result = func(result, span[i], context);
                return result;
            }

            if (!enumerator.TryGetNext(out var current)) Internal.Throws.NoElements<TSource>();
            var accumulated = current;
            while (enumerator.TryGetNext(out current)) accumulated = func(accumulated, current, context);
            return accumulated;
        }

        /// <summary>Applies an accumulator function to a sequence using a seed and the supplied context.</summary>
        public static TAccumulate Aggregate<TEnumerator, TSource, TAccumulate, TContext>(this ValueEnumerable<TEnumerator, TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TContext, TAccumulate> func, TContext context)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(func);
            using var enumerator = source.Enumerator;
            var result = seed;
            if (enumerator.TryGetSpan(out var span))
            {
                foreach (var item in span) result = func(result, item, context);
            }
            else
            {
                while (enumerator.TryGetNext(out var item)) result = func(result, item, context);
            }
            return result;
        }

        /// <summary>Applies an accumulator and result selector to a sequence, passing the supplied context to both functions.</summary>
        public static TResult Aggregate<TEnumerator, TSource, TAccumulate, TResult, TContext>(this ValueEnumerable<TEnumerator, TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TContext, TAccumulate> func, Func<TAccumulate, TContext, TResult> resultSelector, TContext context)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(func);
            ArgumentNullException.ThrowIfNull(resultSelector);
            using var enumerator = source.Enumerator;
            var result = seed;
            if (enumerator.TryGetSpan(out var span))
            {
                foreach (var item in span) result = func(result, item, context);
            }
            else
            {
                while (enumerator.TryGetNext(out var item)) result = func(result, item, context);
            }
            return resultSelector(result, context);
        }
    }
}
