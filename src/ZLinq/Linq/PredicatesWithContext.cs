namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Determines whether all elements of a sequence satisfy a condition evaluated with the supplied context.</summary>
        public static bool All<TEnumerator, TSource, TContext>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TContext, bool> predicate, TContext context)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            using var enumerator = source.Enumerator;
            if (enumerator.TryGetSpan(out var span))
            {
                foreach (var item in span)
                {
                    if (!predicate(item, context)) return false;
                }
            }
            else
            {
                while (enumerator.TryGetNext(out var item))
                {
                    if (!predicate(item, context)) return false;
                }
            }
            return true;
        }

        /// <summary>Determines whether any element of a sequence satisfies a condition evaluated with the supplied context.</summary>
        public static bool Any<TEnumerator, TSource, TContext>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TContext, bool> predicate, TContext context)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            using var enumerator = source.Enumerator;
            if (enumerator.TryGetSpan(out var span))
            {
                foreach (var item in span)
                {
                    if (predicate(item, context)) return true;
                }
            }
            else
            {
                while (enumerator.TryGetNext(out var item))
                {
                    if (predicate(item, context)) return true;
                }
            }
            return false;
        }

        /// <summary>Returns the number of elements that satisfy a condition evaluated with the supplied context.</summary>
        public static int Count<TEnumerator, TSource, TContext>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TContext, bool> predicate, TContext context)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            using var enumerator = source.Enumerator;
            var count = 0;
            if (enumerator.TryGetSpan(out var span))
            {
                foreach (var item in span)
                {
                    if (predicate(item, context)) count++;
                }
            }
            else
            {
                while (enumerator.TryGetNext(out var item))
                {
                    if (predicate(item, context)) checked { count++; }
                }
            }
            return count;
        }

        /// <summary>Returns the number of elements that satisfy a condition evaluated with the supplied context.</summary>
        public static long LongCount<TEnumerator, TSource, TContext>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TContext, bool> predicate, TContext context)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            using var enumerator = source.Enumerator;
            long count = 0;
            if (enumerator.TryGetSpan(out var span))
            {
                foreach (var item in span)
                {
                    if (predicate(item, context)) checked { count++; }
                }
            }
            else
            {
                while (enumerator.TryGetNext(out var item))
                {
                    if (predicate(item, context)) checked { count++; }
                }
            }
            return count;
        }
    }
}
