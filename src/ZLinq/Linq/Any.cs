using System;

namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Boolean Any<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using (var enumerator = source.Enumerator)
            {
                if (enumerator.TryGetNonEnumeratedCount(out var count))
                {
                    return count > 0;
                }

                return enumerator.TryGetNext(out _);
            }
        }

        public static Boolean Any<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Boolean> predicate)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);

            using (var enumerator = source.Enumerator)
            {
                if (enumerator.TryGetSpan(out var span))
                {
                    foreach (var item in span)
                    {
                        if (predicate(item))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    while (enumerator.TryGetNext(out var item))
                    {
                        if (predicate(item))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean Any<TSource>(this ValueEnumerable<FromArray<TSource>, TSource> source, Func<TSource, Boolean> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            var array = source.Enumerator.GetSource();
            if (array.GetType() != typeof(TSource[]))
            {
                return Any(array, predicate);
            }

            // The assembly output differs slightly when iterating through array and span.
            // According to microbenckmark results, iterating through span was faster for the logic passed to predicate.
            var span = (ReadOnlySpan<TSource>)array;
            for (int i = 0; (uint)i < (uint)span.Length; i++)
            {
                if (predicate(span[i]))
                {
                    return true;
                }
            }

            return false;

            [MethodImpl(MethodImplOptions.NoInlining)]
            static Boolean Any(TSource[] array, Func<TSource, Boolean> predicate)
            {
                for (int i = 0; (uint)i < (uint)array.Length; i++)
                {
                    if (predicate(array[i]))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
