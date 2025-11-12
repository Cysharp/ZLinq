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
                var list = new List<TSource>(count); // list with capacity set internal buffer as source size
#if NET8_0_OR_GREATER
                CollectionsMarshal.SetCount(list, count);
#else
                CollectionsMarshal.UnsafeSetCount(list, count);
#endif
                var span = CollectionsMarshal.AsSpan(list);
                if (!enumerator.TryCopyTo(span, 0))
                {
                    var i = 0;
                    while (enumerator.TryGetNext(out var current))
                    {
                        span[i] = current;
                        i++;
                    }
                }
                return list;
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

                var list = new List<TSource>(count);
#if NET8_0_OR_GREATER
                CollectionsMarshal.SetCount(list, count);
#else
                CollectionsMarshal.UnsafeSetCount(list, count);
#endif

                var listSpan = CollectionsMarshal.AsSpan(list);
                arrayBuilder.CopyToAndClear(listSpan);
                return list;
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
                var list = new List<TResult>(sourceSpan.Length);
#if NET8_0_OR_GREATER
                CollectionsMarshal.SetCount(list, sourceSpan.Length);
#else
                CollectionsMarshal.UnsafeSetCount(list, sourceSpan.Length);
#endif
                var span = CollectionsMarshal.AsSpan(list);

                for (int i = 0; (uint)i < (uint)sourceSpan.Length; i++)
                {
                    span[i] = selector(sourceSpan[i]);
                }

                return list;
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

                var list = new List<TResult>(count);
#if NET8_0_OR_GREATER
                CollectionsMarshal.SetCount(list, count);
#else
                CollectionsMarshal.UnsafeSetCount(list, count);
#endif

                var listSpan = CollectionsMarshal.AsSpan(list);
                arrayBuilder.CopyToAndClear(listSpan);
                return list;
            }
        }

        public static List<TResult> ToList<TResult>(this ValueEnumerable<RangeSelect<TResult>, TResult> source)
        {
            var value = source.Enumerator.start;
            var count = source.Enumerator.count;
            var selector = source.Enumerator.selector;

            var list = new List<TResult>(count);
#if NET8_0_OR_GREATER
            CollectionsMarshal.SetCount(list, count);
#else
            CollectionsMarshal.UnsafeSetCount(list, count);
#endif
            var span = CollectionsMarshal.AsSpan(list);

            for (int i = 0; (uint)i < (uint)span.Length; i++)
            {
                span[i] = selector(value);
                value++;
            }

            return list;
        }

        public static List<TResult> ToList<TSource, TResult>(this ValueEnumerable<ArraySelect<TSource, TResult>, TResult> source)
        {
            var sourceArray = source.Enumerator.source;
            var selector = source.Enumerator.selector;

            var list = new List<TResult>(sourceArray.Length);
#if NET8_0_OR_GREATER
            CollectionsMarshal.SetCount(list, sourceArray.Length);
#else
            CollectionsMarshal.UnsafeSetCount(list, sourceArray.Length);
#endif
            var span = CollectionsMarshal.AsSpan(list);

            for (int i = 0; (uint)i < (uint)sourceArray.Length; i++)
            {
                span[i] = selector(sourceArray[i]);
            }

            return list;
        }

        public static List<TResult> ToList<TSource, TResult>(this ValueEnumerable<ListSelect<TSource, TResult>, TResult> source)
        {
            var sourceArray = CollectionsMarshal.AsSpan(source.Enumerator.source);
            var selector = source.Enumerator.selector;

            var list = new List<TResult>(sourceArray.Length);
#if NET8_0_OR_GREATER
            CollectionsMarshal.SetCount(list, sourceArray.Length);
#else
            CollectionsMarshal.UnsafeSetCount(list, sourceArray.Length);
#endif
            var span = CollectionsMarshal.AsSpan(list);

            for (int i = 0; (uint)i < (uint)sourceArray.Length; i++)
            {
                span[i] = selector(sourceArray[i]);
            }

            return list;
        }

        // filtering(Where/OfType) -> ToList is frequently case so optimize it.

        public static List<TSource> ToList<TEnumerator, TSource>(this ValueEnumerable<Where<TEnumerator, TSource>, TSource> source)
           where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            var whereEnumerator = source.Enumerator; // no needs dispose(using)
            var predicate = whereEnumerator.Predicate;
            using var enumerator = whereEnumerator.GetSource(); // using only where source enumerator

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

            if (enumerator.TryGetSpan(out var sourceSpan))
            {
                foreach (var item in sourceSpan)
                {
                    if (predicate(item))
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
                }
                arrayBuilder.Advance(i);
            }
            else
            {
                while (enumerator.TryGetNext(out var item))
                {
                    if (predicate(item))
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
                }
                arrayBuilder.Advance(i);
            }

            var count = arrayBuilder.Count;
            var list = new List<TSource>(count);
#if NET8_0_OR_GREATER
            CollectionsMarshal.SetCount(list, count);
#else
            CollectionsMarshal.UnsafeSetCount(list, count);
#endif

            if (count != 0)
            {
                var listSpan = CollectionsMarshal.AsSpan(list);
                arrayBuilder.CopyToAndClear(listSpan);
            }
            return list;
        }

        public static List<TSource> ToList<TSource>(this ValueEnumerable<ArrayWhere<TSource>, TSource> source)
        {
            var whereEnumerator = source.Enumerator; // no needs dispose(using)
            var predicate = whereEnumerator.Predicate;
            var sourceArray = whereEnumerator.GetSource();

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
            foreach (var item in sourceArray)
            {
                if (predicate(item))
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
            }
            arrayBuilder.Advance(i);

            var count = arrayBuilder.Count;
            var list = new List<TSource>(count);
#if NET8_0_OR_GREATER
            CollectionsMarshal.SetCount(list, count);
#else
            CollectionsMarshal.UnsafeSetCount(list, count);
#endif

            if (count != 0)
            {
                var listSpan = CollectionsMarshal.AsSpan(list);
                arrayBuilder.CopyToAndClear(listSpan);
            }
            return list;
        }

        public static List<TSource> ToList<TSource>(this ValueEnumerable<ListWhere<TSource>, TSource> source)
        {
            var whereEnumerator = source.Enumerator; // no needs dispose(using)
            var predicate = whereEnumerator.Predicate;
            var sourceArray = CollectionsMarshal.AsSpan(whereEnumerator.GetSource());

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
            foreach (var item in sourceArray)
            {
                if (predicate(item))
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
            }
            arrayBuilder.Advance(i);

            var count = arrayBuilder.Count;
            var list = new List<TSource>(count);
#if NET8_0_OR_GREATER
            CollectionsMarshal.SetCount(list, count);
#else
            CollectionsMarshal.UnsafeSetCount(list, count);
#endif

            if (count != 0)
            {
                var listSpan = CollectionsMarshal.AsSpan(list);
                arrayBuilder.CopyToAndClear(listSpan);
            }
            return list;
        }

        public static List<TResult> ToList<TEnumerator, TSource, TResult>(this ValueEnumerable<WhereSelect<TEnumerator, TSource, TResult>, TResult> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            var whereEnumerator = source.Enumerator; // no needs dispose(using)
            var predicate = whereEnumerator.Predicate;
            var selector = whereEnumerator.Selector;
            using var enumerator = whereEnumerator.GetSource(); // using only where source enumerator

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

            if (enumerator.TryGetSpan(out var sourceSpan))
            {
                foreach (var item in sourceSpan)
                {
                    if (predicate(item))
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
                }
                arrayBuilder.Advance(i);
            }
            else
            {
                while (enumerator.TryGetNext(out var item))
                {
                    if (predicate(item))
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
                }
                arrayBuilder.Advance(i);
            }

            var count = arrayBuilder.Count;
            var list = new List<TResult>(count);
#if NET8_0_OR_GREATER
            CollectionsMarshal.SetCount(list, count);
#else
            CollectionsMarshal.UnsafeSetCount(list, count);
#endif

            if (count != 0)
            {
                var listSpan = CollectionsMarshal.AsSpan(list);
                arrayBuilder.CopyToAndClear(listSpan);
            }
            return list;
        }

        public static List<TResult> ToList<TSource, TResult>(this ValueEnumerable<ArrayWhereSelect<TSource, TResult>, TResult> source)
        {
            var whereEnumerator = source.Enumerator; // no needs dispose(using)
            var predicate = whereEnumerator.Predicate;
            var selector = whereEnumerator.Selector;
            var sourceArray = whereEnumerator.GetSource();

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

            foreach (var item in sourceArray)
            {
                if (predicate(item))
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
            }
            arrayBuilder.Advance(i);

            var count = arrayBuilder.Count;
            var list = new List<TResult>(count);
#if NET8_0_OR_GREATER
            CollectionsMarshal.SetCount(list, count);
#else
            CollectionsMarshal.UnsafeSetCount(list, count);
#endif

            if (count != 0)
            {
                var listSpan = CollectionsMarshal.AsSpan(list);
                arrayBuilder.CopyToAndClear(listSpan);
            }
            return list;
        }

        public static List<TResult> ToList<TSource, TResult>(this ValueEnumerable<ListWhereSelect<TSource, TResult>, TResult> source)
        {
            var whereEnumerator = source.Enumerator; // no needs dispose(using)
            var predicate = whereEnumerator.Predicate;
            var selector = whereEnumerator.Selector;
            var sourceArray = CollectionsMarshal.AsSpan(whereEnumerator.GetSource());

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
            foreach (var item in sourceArray)
            {
                if (predicate(item))
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
            }
            arrayBuilder.Advance(i);

            var count = arrayBuilder.Count;
            var list = new List<TResult>(count);
#if NET8_0_OR_GREATER
            CollectionsMarshal.SetCount(list, count);
#else
            CollectionsMarshal.UnsafeSetCount(list, count);
#endif

            if (count != 0)
            {
                var listSpan = CollectionsMarshal.AsSpan(list);
                arrayBuilder.CopyToAndClear(listSpan);
            }
            return list;
        }

        public static List<TResult> ToList<TEnumerator, TSource, TResult>(this ValueEnumerable<OfType<TEnumerator, TSource, TResult>, TResult> source)
           where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            var ofTypeEnumerator = source.Enumerator; // no needs dispose(using)
            using var enumerator = ofTypeEnumerator.GetSource(); // using only ofType source enumerator

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

            if (enumerator.TryGetSpan(out var sourceSpan))
            {
                foreach (var value in sourceSpan)
                {
                    if (value is TResult item)
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
                }
                arrayBuilder.Advance(i);
            }
            else
            {
                while (enumerator.TryGetNext(out var value))
                {
                    if (value is TResult item)
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
                }
                arrayBuilder.Advance(i);
            }

            var count = arrayBuilder.Count;
            var list = new List<TResult>(count);
#if NET8_0_OR_GREATER
            CollectionsMarshal.SetCount(list, count);
#else
            CollectionsMarshal.UnsafeSetCount(list, count);
#endif

            if (count != 0)
            {
                var listSpan = CollectionsMarshal.AsSpan(list);
                arrayBuilder.CopyToAndClear(listSpan);
            }
            return list;
        }
    }
}
