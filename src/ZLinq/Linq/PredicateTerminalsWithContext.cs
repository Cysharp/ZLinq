namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static TSource First<TEnumerator, TSource, TContext>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TContext, bool> predicate, TContext context)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var enumerator = source.Enumerator;
            try
            {
                return TryGetFirstWithContext<TEnumerator, TSource, TContext>(ref enumerator, predicate, context, out var value) ? value : Throws.NoMatch<TSource>();
            }
            finally { enumerator.Dispose(); }
        }

        public static TSource? FirstOrDefault<TEnumerator, TSource, TContext>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TContext, bool> predicate, TContext context)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var enumerator = source.Enumerator;
            try
            {
                return TryGetFirstWithContext<TEnumerator, TSource, TContext>(ref enumerator, predicate, context, out var value) ? value : default;
            }
            finally { enumerator.Dispose(); }
        }

        public static TSource FirstOrDefault<TEnumerator, TSource, TContext>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TContext, bool> predicate, TContext context, TSource defaultValue)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var enumerator = source.Enumerator;
            try
            {
                return TryGetFirstWithContext<TEnumerator, TSource, TContext>(ref enumerator, predicate, context, out var value) ? value : defaultValue;
            }
            finally { enumerator.Dispose(); }
        }

        public static TSource Last<TEnumerator, TSource, TContext>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TContext, bool> predicate, TContext context)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var enumerator = source.Enumerator;
            try
            {
                return TryGetLastWithContext<TEnumerator, TSource, TContext>(ref enumerator, predicate, context, out var value) ? value : Throws.NoMatch<TSource>();
            }
            finally { enumerator.Dispose(); }
        }

        public static TSource? LastOrDefault<TEnumerator, TSource, TContext>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TContext, bool> predicate, TContext context)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var enumerator = source.Enumerator;
            try
            {
                return TryGetLastWithContext<TEnumerator, TSource, TContext>(ref enumerator, predicate, context, out var value) ? value : default;
            }
            finally { enumerator.Dispose(); }
        }

        public static TSource LastOrDefault<TEnumerator, TSource, TContext>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TContext, bool> predicate, TContext context, TSource defaultValue)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var enumerator = source.Enumerator;
            try
            {
                return TryGetLastWithContext<TEnumerator, TSource, TContext>(ref enumerator, predicate, context, out var value) ? value : defaultValue;
            }
            finally { enumerator.Dispose(); }
        }

        public static TSource Single<TEnumerator, TSource, TContext>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TContext, bool> predicate, TContext context)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var enumerator = source.Enumerator;
            try
            {
                return TryGetSingleWithContext<TEnumerator, TSource, TContext>(ref enumerator, predicate, context, out var value) ? value : Throws.NoMatch<TSource>();
            }
            finally { enumerator.Dispose(); }
        }

        public static TSource? SingleOrDefault<TEnumerator, TSource, TContext>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TContext, bool> predicate, TContext context)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var enumerator = source.Enumerator;
            try
            {
                return TryGetSingleWithContext<TEnumerator, TSource, TContext>(ref enumerator, predicate, context, out var value) ? value : default;
            }
            finally { enumerator.Dispose(); }
        }

        public static TSource SingleOrDefault<TEnumerator, TSource, TContext>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TContext, bool> predicate, TContext context, TSource defaultValue)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var enumerator = source.Enumerator;
            try
            {
                return TryGetSingleWithContext<TEnumerator, TSource, TContext>(ref enumerator, predicate, context, out var value) ? value : defaultValue;
            }
            finally { enumerator.Dispose(); }
        }

        static bool TryGetFirstWithContext<TEnumerator, TSource, TContext>(ref TEnumerator source, Func<TSource, TContext, bool> predicate, TContext context, out TSource value)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            if (source.TryGetSpan(out var span))
            {
                foreach (var item in span)
                {
                    if (predicate(item, context)) { value = item; return true; }
                }
            }
            else
            {
                while (source.TryGetNext(out var item))
                {
                    if (predicate(item, context)) { value = item; return true; }
                }
            }
            value = default!;
            return false;
        }

        static bool TryGetLastWithContext<TEnumerator, TSource, TContext>(ref TEnumerator source, Func<TSource, TContext, bool> predicate, TContext context, out TSource value)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            value = default!;
            if (source.TryGetSpan(out var span))
            {
                for (var i = span.Length - 1; i >= 0; i--)
                {
                    if (predicate(span[i], context)) { value = span[i]; return true; }
                }
            }
            else
            {
                var found = false;
                while (source.TryGetNext(out var item))
                {
                    if (predicate(item, context)) { value = item; found = true; }
                }
                if (found) return true;
            }
            value = default!;
            return false;
        }

        static bool TryGetSingleWithContext<TEnumerator, TSource, TContext>(ref TEnumerator source, Func<TSource, TContext, bool> predicate, TContext context, out TSource value)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            value = default!;
            var found = false;
            if (source.TryGetSpan(out var span))
            {
                foreach (var item in span)
                {
                    if (!predicate(item, context)) continue;
                    if (found) Throws.MoreThanOneMatch();
                    value = item;
                    found = true;
                }
            }
            else
            {
                while (source.TryGetNext(out var item))
                {
                    if (!predicate(item, context)) continue;
                    if (found) Throws.MoreThanOneMatch();
                    value = item;
                    found = true;
                }
            }
            return found;
        }
    }
}
