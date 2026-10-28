namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Returns a <see cref="long"/> that represents the total number of elements in a sequence.</summary>
        /// <param name="source">The source sequence.</param>
        /// <returns>The number of elements in the sequence.</returns>
        public static Int64 LongCount<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using (var enumerator = source.Enumerator)
            {
                bool hasCount;
                int count;
                try
                {
                    hasCount = enumerator.TryGetNonEnumeratedCount(out count);
                }
                catch (OverflowException)
                {
                    // the count exceeds int.MaxValue (e.g. Append, Concat), System.Linq's LongCount enumerates in this case.
                    hasCount = false;
                    count = 0;
                }

                if (hasCount)
                {
                    return count;
                }

                var longCount = 0L;
                while (enumerator.TryGetNext(out _))
                {
                    checked { longCount++; }
                }
                return longCount;
            }
        }

        /// <summary>Returns a <see cref="long"/> that represents how many elements in a sequence satisfy a condition.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A number that represents how many elements in the sequence satisfy the condition in the predicate function.</returns>
        public static Int64 LongCount<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Boolean> predicate)
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
                    var longCount = 0L;
                    foreach (var current in span)
                    {
                        if (predicate(current))
                        {
                            checked { longCount++; }
                        }
                    }
                    return longCount;
                }
                else
                {
                    var longCount = 0L;
                    while (enumerator.TryGetNext(out var current))
                    {
                        if (predicate(current))
                        {
                            checked { longCount++; }
                        }
                    }
                    return longCount;
                }
            }
        }

        /// <summary>Returns a <see cref="long"/> that represents how many elements in a sequence satisfy a condition.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A number that represents how many elements in the sequence satisfy the condition in the predicate function.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int64 LongCount<TSource>(this ValueEnumerable<FromArray<TSource>, TSource> source, Func<TSource, Boolean> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            var array = source.Enumerator.GetSource();
            if (array.GetType() != typeof(TSource[]))
            {
                return LongCount(array, predicate);
            }

            var longCount = 0;

            var span = (ReadOnlySpan<TSource>)array;
            for (int i = 0; (uint)i < (uint)span.Length; i++)
            {
                if (predicate(span[i]))
                {
                    longCount++;
                }
            }

            return longCount;

            [MethodImpl(MethodImplOptions.NoInlining)]
            static Int64 LongCount(TSource[] array, Func<TSource, Boolean> predicate)
            {
                var longCount = 0;
                for (int i = 0; (uint)i < (uint)array.Length; i++)
                {
                    if (predicate(array[i]))
                    {
                        longCount++;
                    }
                }
                return longCount;
            }
        }

    }
}
