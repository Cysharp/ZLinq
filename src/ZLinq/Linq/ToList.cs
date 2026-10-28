namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static List<TSource> ToList<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using var enumerator = source.Enumerator;

            if (enumerator.TryGetNonEnumeratedCount(out var count))
            {
#if !NET8_0_OR_GREATER
                if (ListMarshal.TryGetListSource<TEnumerator, TSource>(in enumerator, out var sourceList))
                {
                    return new List<TSource>(sourceList);
                }

                if (ListMarshal.TryGetArraySource<TEnumerator, TSource>(in enumerator, out var sourceArray))
                {
                    return new List<TSource>(sourceArray);
                }
#endif

                using var filler = new ListFiller<TSource>(count);
                var span = filler.Span;
                var written = count;
                if (!enumerator.TryCopyTo(span, 0))
                {
                    var i = 0;
                    while (enumerator.TryGetNext(out var current))
                    {
                        span[i] = current;
                        i++;
                    }
                    written = i;
                }
                return filler.Commit(written);
            }
            else
            {
                // list.Add is slow, avoid it.
#if NETSTANDARD2_0
                Span<TSource> initialBufferSpan = default;
#elif NET8_0_OR_GREATER
                var initialBuffer = default(InlineArray16<TSource>);
                Span<TSource> initialBufferSpan = initialBuffer;
#else
                var initialBuffer = default(InlineArray16<TSource>);
                Span<TSource> initialBufferSpan = initialBuffer.AsSpan();
#endif
                var arrayBuilder = new SegmentedArrayProvider<TSource>(initialBufferSpan);
                var span = arrayBuilder.GetSpan();
                var i = 0;
                while (enumerator.TryGetNext(out var item))
                {
                    if (i == span.Length)
                    {
                        arrayBuilder.Advance(i);
                        span = arrayBuilder.GetSpan();
                        i = 0;
                    }

                    span[i] = item;
                    i++;
                }
                arrayBuilder.Advance(i);

                count = arrayBuilder.Count;

                using var filler = new ListFiller<TSource>(count);
                arrayBuilder.CopyToAndClear(filler.Span);
                return filler.Commit(count);
            }
        }

        // Select -> ToList is common pattern so optimize it.

        public static List<TResult> ToList<TEnumerator, TSource, TResult>(this ValueEnumerable<Select<TEnumerator, TSource, TResult>, TResult> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using var enumerator = source.Enumerator.source; // use select-source enumerator

            var selector = source.Enumerator.selector;

            if (enumerator.TryGetSpan(out var sourceSpan))
            {
                using var filler = new ListFiller<TResult>(sourceSpan.Length);
                var span = filler.Span;

                for (int i = 0; (uint)i < (uint)sourceSpan.Length; i++)
                {
                    span[i] = selector(sourceSpan[i]);
                }

                return filler.Commit(sourceSpan.Length);
            }
            else
            {
#if NETSTANDARD2_0
                Span<TResult> initialBufferSpan = default;
#elif NET8_0_OR_GREATER
                var initialBuffer = default(InlineArray16<TResult>);
                Span<TResult> initialBufferSpan = initialBuffer;
#else
                var initialBuffer = default(InlineArray16<TResult>);
                Span<TResult> initialBufferSpan = initialBuffer.AsSpan();
#endif
                var arrayBuilder = new SegmentedArrayProvider<TResult>(initialBufferSpan);
                var span = arrayBuilder.GetSpan();
                var i = 0;
                while (enumerator.TryGetNext(out var item))
                {
                    if (i == span.Length)
                    {
                        arrayBuilder.Advance(i);
                        span = arrayBuilder.GetSpan();
                        i = 0;
                    }

                    span[i] = selector(item);
                    i++;
                }
                arrayBuilder.Advance(i);

                var count = arrayBuilder.Count;

                using var filler = new ListFiller<TResult>(count);
                arrayBuilder.CopyToAndClear(filler.Span);
                return filler.Commit(count);
            }
        }

        public static List<TResult> ToList<TResult>(this ValueEnumerable<RangeSelect<TResult>, TResult> source)
        {
            var value = source.Enumerator.start;
            var count = source.Enumerator.count;
            var selector = source.Enumerator.selector;

            using var filler = new ListFiller<TResult>(count);
            var span = filler.Span;

            for (int i = 0; (uint)i < (uint)span.Length; i++)
            {
                span[i] = selector(value);
                value++;
            }

            return filler.Commit(count);
        }

        public static List<TResult> ToList<TSource, TResult>(this ValueEnumerable<ArraySelect<TSource, TResult>, TResult> source)
        {
            var sourceArray = source.Enumerator.source;
            var selector = source.Enumerator.selector;

            using var filler = new ListFiller<TResult>(sourceArray.Length);
            var span = filler.Span;

            for (int i = 0; (uint)i < (uint)sourceArray.Length; i++)
            {
                span[i] = selector(sourceArray[i]);
            }

            return filler.Commit(sourceArray.Length);
        }

        public static List<TResult> ToList<TSource, TResult>(this ValueEnumerable<ListSelect<TSource, TResult>, TResult> source)
        {
            var sourceList = source.Enumerator.source;
            var selector = source.Enumerator.selector;

            var count = sourceList.Count;

            using var filler = new ListFiller<TResult>(count);
            var span = filler.Span;

            var written = 0;
            if (ListMarshal.UseChunks(sourceList))
            {
                // Stops early if the selector shrinks the list; Commit fills the rest with default.
                foreach (var chunk in new ListChunks<TSource>(sourceList))
                {
                    var destination = span.Slice(written, chunk.Length);
                    for (int i = 0; (uint)i < (uint)chunk.Length; i++)
                    {
                        destination[i] = selector(chunk[i]);
                    }
                    written += chunk.Length;
                }
            }
            else
            {
                var elements = ListMarshal.AsSpan(sourceList);
                for (int i = 0; (uint)i < (uint)elements.Length; i++)
                {
                    span[i] = selector(elements[i]);
                }
                written = elements.Length;
            }

            return filler.Commit(written);
        }
    }
}
