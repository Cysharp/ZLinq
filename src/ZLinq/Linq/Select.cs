namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<Select<TEnumerator, TSource, TResult>, TResult> Select<TEnumerator, TSource, TResult>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TResult> selector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(selector)));

        public static ValueEnumerable<Select2<TEnumerator, TSource, TResult>, TResult> Select<TEnumerator, TSource, TResult>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Int32, TResult> selector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(selector)));

        public static ValueEnumerable<RangeSelect<TResult>, TResult> Select<TResult>(this ValueEnumerable<FromRange, int> source, Func<int, TResult> selector)
            => new(new(source.Enumerator, Throws.IfNull(selector)));

        public static ValueEnumerable<SelectWhere<TEnumerator, TSource, TResult>, TResult> Where<TEnumerator, TSource, TResult>(this ValueEnumerable<Select<TEnumerator, TSource, TResult>, TResult> source, Func<TResult, bool> predicate)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source.Enumerator.Where(Throws.IfNull(predicate)));

        public static ValueEnumerable<ArraySelect<TSource, TResult>, TResult> Select<TSource, TResult>(this ValueEnumerable<FromArray<TSource>, TSource> source, Func<TSource, TResult> selector)
            => new(new(source.Enumerator.GetSource(), Throws.IfNull(selector)));

        public static ValueEnumerable<ArraySelectWhere<TSource, TResult>, TResult> Where<TSource, TResult>(this ValueEnumerable<ArraySelect<TSource, TResult>, TResult> source, Func<TResult, bool> predicate)
            => new(source.Enumerator.Where(Throws.IfNull(predicate)));

        public static ValueEnumerable<ListSelect<TSource, TResult>, TResult> Select<TSource, TResult>(this ValueEnumerable<FromList<TSource>, TSource> source, Func<TSource, TResult> selector)
            => new(new(source.Enumerator.GetSource(), Throws.IfNull(selector)));

        public static ValueEnumerable<ListSelectWhere<TSource, TResult>, TResult> Where<TSource, TResult>(this ValueEnumerable<ListSelect<TSource, TResult>, TResult> source, Func<TResult, bool> predicate)
            => new(source.Enumerator.Where(Throws.IfNull(predicate)));
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
    struct Select<TEnumerator, TSource, TResult>(TEnumerator source, Func<TSource, TResult> selector) : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        internal TEnumerator source = source;
        internal readonly Func<TSource, TResult> selector = selector;

        public bool TryGetNonEnumeratedCount(out int count) => source.TryGetNonEnumeratedCount(out count);

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<TResult> destination, Index offset)
        {
            // Iterate inlining
            if (source.TryGetSpan(out var span))
            {
                if (EnumeratorHelper.TryGetSlice(span, offset, destination.Length, out var slice))
                {
                    for (var i = 0; i < slice.Length; i++)
                    {
                        destination[i] = selector(slice[i]);
                    }
                    return true;
                }
            }

            //  First/ElementAt/Last
            if (destination.Length == 1)
            {
#if NETSTANDARD2_0
                var singleSpan = SingleSpan.Create<TSource>();
                if (source.TryCopyTo(singleSpan, offset))
                {
                    try
                    {
                        destination[0] = selector(singleSpan[0]);
                    }
                    finally
                    {
                        singleSpan.Clear();
                    }
                    return true;
                }
#else
                var current = default(TSource)!;
                if (source.TryCopyTo(SingleSpan.Create(ref current), offset))
                {
                    destination[0] = selector(current);
                    return true;
                }
#endif

                if (EnumeratorHelper.TryConsumeGetAt(ref source, offset, out TSource value))
                {
                    destination[0] = selector(value);
                    return true;
                }
            }

            return false;
        }

        public bool TryGetNext(out TResult current)
        {
            if (source.TryGetNext(out var value))
            {
                current = selector(value);
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }

        internal SelectWhere<TEnumerator, TSource, TResult> Where(Func<TResult, bool> predicate)
            => new(source, selector, predicate);
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct Select2<TEnumerator, TSource, TResult>(TEnumerator source, Func<TSource, Int32, TResult> selector)
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        int index = 0;

        public bool TryGetNonEnumeratedCount(out int count) => source.TryGetNonEnumeratedCount(out count);

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<TResult> destination, Index offset) => false;

        public bool TryGetNext(out TResult current)
        {
            if (source.TryGetNext(out var value))
            {
                current = selector(value, index++);
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
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
    struct RangeSelect<TResult>(FromRange source, Func<int, TResult> selector) : IValueEnumerator<TResult>
    {
        // Range
        internal readonly int count = source.count;
        internal readonly int start = source.start;
        internal readonly int to = source.to;
        int value = source.start;

        // Select
        internal readonly Func<int, TResult> selector = selector;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = this.count;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<TResult> destination, Index offset)
        {
            if (EnumeratorHelper.TryGetSliceRange(count, offset, destination.Length, out var fillStart, out var fillCount))
            {
                var value = start + fillStart;
                for (int i = 0; i < fillCount; i++)
                {
                    destination[i] = selector(value);
                    value++;
                }
                return true;
            }

            return false;
        }

        public bool TryGetNext(out TResult current)
        {
            if (value < to)
            {
                current = selector(value);
                value++;
                return true;
            }

            current = default!;
            return false;
        }

        public void Dispose()
        {
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct SelectWhere<TEnumerator, TSource, TResult>(TEnumerator source, Func<TSource, TResult> selector, Func<TResult, bool> predicate) // no in TEnumerator
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = default;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<TResult> destination, Index offset) => false;

        public bool TryGetNext(out TResult current)
        {
            while (source.TryGetNext(out var value))
            {
                var result = selector(value);
                if (predicate(result))
                {
                    current = result;
                    return true;
                }
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
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
    struct ArraySelect<TSource, TResult>(TSource[] source, Func<TSource, TResult> selector) : IValueEnumerator<TResult>
    {
        internal TSource[] source = source;
        internal readonly Func<TSource, TResult> selector = selector;
        int index;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Length;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<TResult> destination, Index offset)
        {
            // AsSpan is failed by array variance
            if (!typeof(TSource).IsValueType && source.GetType() != typeof(TSource[]))
            {
                return false;
            }

            if (EnumeratorHelper.TryGetSlice<TSource>(source.AsSpan(), offset, destination.Length, out var slice))
            {
                for (var i = 0; i < slice.Length; i++)
                {
                    destination[i] = selector(slice[i]);
                }
                return true;
            }

            return false;
        }

        public bool TryGetNext(out TResult current)
        {
            var length = source.Length;
#if NET8_0_OR_GREATER
            ref var firstElement = ref MemoryMarshal.GetArrayDataReference(source);

            if ((uint)index < (uint)length)
            {
                current = selector(Unsafe.Add(ref firstElement, (nint)(uint)index++)); // must be index++
                return true;
            }
#else
            if ((uint)index < (uint)length)
            {
                current = selector(source[index++]); // must be index++
                return true;
            }
#endif

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
        }

        internal ArraySelectWhere<TSource, TResult> Where(Func<TResult, bool> predicate)
        {
            return new ArraySelectWhere<TSource, TResult>(source, selector, predicate);
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct ArraySelectWhere<TSource, TResult>(TSource[] source, Func<TSource, TResult> selector, Func<TResult, bool> predicate) : IValueEnumerator<TResult>
    {
        TSource[] source = source;
        readonly Func<TSource, TResult> selector = selector;
        Func<TResult, bool> predicate = predicate;
        int index;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<TResult> destination, Index offset)
        {
            return false;
        }

        public bool TryGetNext(out TResult current)
        {
            var length = source.Length;
#if NET8_0_OR_GREATER
            ref var firstElement = ref MemoryMarshal.GetArrayDataReference(source);

            while ((uint)index < (uint)length)
            {
                current = selector(Unsafe.Add(ref firstElement, (nint)(uint)index++));
                if (predicate(current))
                {
                    return true;
                }
            }
#else
            while ((uint)index < (uint)length)
            {
                current = selector(source[index++]);
                if (predicate(current))
                {
                    return true;
                }
            }
#endif

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct ListSelect<TSource, TResult>(List<TSource> source, Func<TSource, TResult> selector) : IValueEnumerator<TResult>
    {
        internal List<TSource> source = source;
        internal readonly Func<TSource, TResult> selector = selector;
        int index;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Count;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<TResult> destination, Index offset)
        {
            if (EnumeratorHelper.TryGetSlice<TSource>(CollectionsMarshal.AsSpan(source), offset, destination.Length, out var slice))
            {
                for (var i = 0; i < slice.Length; i++)
                {
                    destination[i] = selector(slice[i]);
                }
                return true;
            }

            return false;
        }

        public bool TryGetNext(out TResult current)
        {
            if (index < source.Count)
            {
                current = selector(source[index++]);
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
        }

        internal ListSelectWhere<TSource, TResult> Where(Func<TResult, bool> predicate)
        {
            return new ListSelectWhere<TSource, TResult>(source, selector, predicate);
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct ListSelectWhere<TSource, TResult>(List<TSource> source, Func<TSource, TResult> selector, Func<TResult, bool> predicate) : IValueEnumerator<TResult>
    {
        List<TSource> source = source;
        readonly Func<TSource, TResult> selector = selector;
        Func<TResult, bool> predicate = predicate;
        int index;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<TResult> destination, Index offset)
        {
            return false;
        }

        public bool TryGetNext(out TResult current)
        {
            var span = CollectionsMarshal.AsSpan(source);
            while (index < span.Length)
            {
                current = selector(span[index++]);
                if (predicate(current))
                {
                    return true;
                }
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
        }
    }
}
