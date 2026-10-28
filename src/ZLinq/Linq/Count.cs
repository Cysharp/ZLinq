using System;

namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Returns the number of elements in a sequence.</summary>
        /// <param name="source">The source sequence.</param>
        /// <returns>The number of elements in the sequence.</returns>
        /// <remarks>If the count can be determined without enumeration (e.g. arrays, lists and collections), the sequence is not enumerated.</remarks>
        public static Int32 Count<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using (var enumerator = source.Enumerator)
            {
                if (enumerator.TryGetNonEnumeratedCount(out var count))
                {
                    return count;
                }

                // if Span exists, already TryGetNonEnumeratedCount returns true so no need to get Span.

                count = 0;
                while (enumerator.TryGetNext(out _))
                {
                    checked { count++; }
                }
                return count;
            }
        }

        // Where Count

        /// <summary>Returns the number of elements in a sequence.</summary>
        /// <param name="source">The source sequence.</param>
        /// <returns>The number of elements in the sequence.</returns>
        /// <remarks>If the count can be determined without enumeration (e.g. arrays, lists and collections), the sequence is not enumerated.</remarks>
        public static Int32 Count<TEnumerator, TSource>(this ValueEnumerable<Where<TEnumerator, TSource>, TSource> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using var enumeratorSource = source.Enumerator.GetSource();
            var predicate = source.Enumerator.Predicate;

            var count = 0;
            if (enumeratorSource.TryGetSpan(out var span))
            {
                for (int i = 0; (uint)i < (uint)span.Length; i++)
                {
                    if (predicate(span[i]))
                    {
                        count++; // no need to use checked
                    }
                }
            }
            else
            {
                while (enumeratorSource.TryGetNext(out var current))
                {
                    if (predicate(current))
                    {
                        checked { count++; }
                    }
                }
            }

            return count;
        }

        /// <summary>Returns the number of elements in a sequence.</summary>
        /// <param name="source">The source sequence.</param>
        /// <returns>The number of elements in the sequence.</returns>
        /// <remarks>If the count can be determined without enumeration (e.g. arrays, lists and collections), the sequence is not enumerated.</remarks>
        public static Int32 Count<TSource>(this ValueEnumerable<ArrayWhere<TSource>, TSource> source)
        {
            var array = source.Enumerator.GetSource();
            var predicate = source.Enumerator.Predicate;
            var span = (ReadOnlySpan<TSource>)array;

            var count = 0;
            for (int i = 0; (uint)i < (uint)span.Length; i++)
            {
                if (predicate(span[i]))
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>Returns the number of elements in a sequence.</summary>
        /// <param name="source">The source sequence.</param>
        /// <returns>The number of elements in the sequence.</returns>
        /// <remarks>If the count can be determined without enumeration (e.g. arrays, lists and collections), the sequence is not enumerated.</remarks>
        public static Int32 Count<TSource>(this ValueEnumerable<ListWhere<TSource>, TSource> source)
        {
            var list = source.Enumerator.GetSource();
            var predicate = source.Enumerator.Predicate;

            var count = 0;
            if (ListMarshal.UseChunks(list))
            {
                foreach (var span in new ListChunks<TSource>(list))
                {
                    for (int i = 0; (uint)i < (uint)span.Length; i++)
                    {
                        if (predicate(span[i]))
                        {
                            count++;
                        }
                    }
                }
            }
            else
            {
                var span = ListMarshal.AsSpan(list);
                for (int i = 0; (uint)i < (uint)span.Length; i++)
                {
                    if (predicate(span[i]))
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>Returns a number that represents how many elements in the specified sequence satisfy a condition.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A number that represents how many elements in the sequence satisfy the condition in the predicate function.</returns>
        public static Int32 Count<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Boolean> predicate)
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
                for (int i = 0; (uint)i < (uint)span.Length; i++)
                {
                    if (predicate(span[i]))
                    {
                        count++; // no need to use checked
                    }
                }
            }
            else
            {
                while (enumerator.TryGetNext(out var current))
                {
                    if (predicate(current))
                    {
                        checked { count++; }
                    }
                }
            }

            return count;
        }

        /// <summary>Returns a number that represents how many elements in the specified sequence satisfy a condition.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A number that represents how many elements in the sequence satisfy the condition in the predicate function.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 Count<TSource>(this ValueEnumerable<FromArray<TSource>, TSource> source, Func<TSource, Boolean> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            var array = source.Enumerator.GetSource();
            if (array.GetType() != typeof(TSource[]))
            {
                return Count(array, predicate);
            }

            var count = 0;

            // The assembly output differs slightly when iterating through array and span.
            // According to microbenckmark results, iterating through span was faster for the logic passed to predicate.
            var span = (ReadOnlySpan<TSource>)array;
            for (int i = 0; (uint)i < (uint)span.Length; i++)
            {
                if (predicate(span[i]))
                {
                    count++;
                }
            }

            return count;

            [MethodImpl(MethodImplOptions.NoInlining)]
            static int Count(TSource[] array, Func<TSource, Boolean> predicate)
            {
                var count = 0;
                for (int i = 0; (uint)i < (uint)array.Length; i++)
                {
                    if (predicate(array[i]))
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        /// <summary>Returns a number that represents how many elements in the specified sequence satisfy a condition.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A number that represents how many elements in the sequence satisfy the condition in the predicate function.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 Count<TSource>(this ValueEnumerable<FromList<TSource>, TSource> source, Func<TSource, Boolean> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            var list = source.Enumerator.GetSource();
            var count = 0;

            if (ListMarshal.UseChunks(list))
            {
                foreach (var span in new ListChunks<TSource>(list))
                {
                    for (int i = 0; (uint)i < (uint)span.Length; i++)
                    {
                        if (predicate(span[i]))
                        {
                            count++;
                        }
                    }
                }
            }
            else
            {
                var span = ListMarshal.AsSpan(list);
                for (int i = 0; (uint)i < (uint)span.Length; i++)
                {
                    if (predicate(span[i]))
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    }
}
