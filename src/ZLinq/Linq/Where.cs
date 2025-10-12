using System;
using System.Reflection;

namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<Where<TEnumerator, TSource>, TSource> Where<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Boolean> predicate)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(predicate)));

        public static ValueEnumerable<Where2<TEnumerator, TSource>, TSource> Where<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Int32, Boolean> predicate)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(predicate)));

        public static ValueEnumerable<WhereSelect<TEnumerator, TSource, TResult>, TResult> Select<TEnumerator, TSource, TResult>(this ValueEnumerable<Where<TEnumerator, TSource>, TSource> source, Func<TSource, TResult> selector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source.Enumerator.Select(Throws.IfNull(selector)));

        // Optimize variations

        public static ValueEnumerable<ArrayWhere<TSource>, TSource> Where<TSource>(this ValueEnumerable<FromArray<TSource>, TSource> source, Func<TSource, Boolean> predicate)
            => new(new(source.Enumerator, Throws.IfNull(predicate)));

        public static ValueEnumerable<ArrayWhereSelect<TSource, TResult>, TResult> Select<TSource, TResult>(this ValueEnumerable<ArrayWhere<TSource>, TSource> source, Func<TSource, TResult> selector)
            => new(source.Enumerator.Select(Throws.IfNull(selector)));

        public static ValueEnumerable<ListWhere<TSource>, TSource> Where<TSource>(this ValueEnumerable<FromList<TSource>, TSource> source, Func<TSource, Boolean> predicate)
            => new(new(source.Enumerator, Throws.IfNull(predicate)));

        public static ValueEnumerable<ListWhereSelect<TSource, TResult>, TResult> Select<TSource, TResult>(this ValueEnumerable<ListWhere<TSource>, TSource> source, Func<TSource, TResult> selector)
            => new(source.Enumerator.Select(Throws.IfNull(selector)));
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
    struct Where<TEnumerator, TSource>(TEnumerator source, Func<TSource, Boolean> predicate)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;

        internal TEnumerator GetSource() => source;
        internal Func<TSource, bool> Predicate => predicate;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = default;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<TSource> destination, Index offset) => false;

        public bool TryGetNext(out TSource current)
        {
            while (source.TryGetNext(out var value))
            {
                if (predicate(value))
                {
                    current = value;
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

        // Optimize for common pattern: Where().Select()
        internal WhereSelect<TEnumerator, TSource, TResult> Select<TResult>(Func<TSource, TResult> selector)
            => new(source, predicate, selector);
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct Where2<TEnumerator, TSource>(TEnumerator source, Func<TSource, Int32, Boolean> predicate)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        int index = 0;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = default;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<TSource> destination, Index offset) => false;

        public bool TryGetNext(out TSource current)
        {
            while (source.TryGetNext(out var value))
            {
                if (predicate(value, index++))
                {
                    current = value;
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
    struct WhereSelect<TEnumerator, TSource, TResult>(TEnumerator source, Func<TSource, Boolean> predicate, Func<TSource, TResult> selector) // no in TEnumerator
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        internal TEnumerator GetSource() => source;
        internal Func<TSource, bool> Predicate => predicate;
        internal Func<TSource, TResult> Selector => selector;

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
                if (predicate(value))
                {
                    current = selector(value);
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
    public struct ArrayWhere<TSource>(FromArray<TSource> source, Func<TSource, Boolean> predicate) : IValueEnumerator<TSource>
    {
        TSource[] source = source.GetSource();
        internal TSource[] GetSource() => source;
        internal Func<TSource, bool> Predicate => predicate;
        int index;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = default;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<TSource> destination, Index offset) => false;

        public bool TryGetNext(out TSource current)
        {
            var length = source.Length;
#if NET8_0_OR_GREATER
            ref var firstElement = ref MemoryMarshal.GetArrayDataReference(source);

            while ((uint)index < (uint)length)
            {
                var value = Unsafe.Add(ref firstElement, (nint)(uint)index);
                index++;
                if (predicate(value))
                {
                    current = value;
                    return true;
                }
            }
#else
            while ((uint)index < (uint)length)
            {
                var value = source[index];
                index++;
                if (predicate(value))
                {
                    current = value;
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

        // Optimize for common pattern: Where().Select()
        public ArrayWhereSelect<TSource, TResult> Select<TResult>(Func<TSource, TResult> selector)
            => new(source, predicate, selector);
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct ArrayWhereSelect<TSource, TResult>(TSource[] source, Func<TSource, Boolean> predicate, Func<TSource, TResult> selector)
        : IValueEnumerator<TResult>
    {
        int index;
        TSource[] source = source;
        internal TSource[] GetSource() => source;
        internal Func<TSource, bool> Predicate => predicate;
        internal Func<TSource, TResult> Selector => selector;

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
            var length = source.Length;
#if NET8_0_OR_GREATER
            ref var firstElement = ref MemoryMarshal.GetArrayDataReference(source);

            while ((uint)index < (uint)length)
            {
                var value = Unsafe.Add(ref firstElement, (nint)(uint)index);
                index++;
                if (predicate(value))
                {
                    current = selector(value);
                    return true;
                }
            }
#else
            while ((uint)index < (uint)length)
            {
                var value = source[index];
                index++;
                if (predicate(value))
                {
                    current = selector(value);
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
    public struct ListWhere<TSource>(FromList<TSource> source, Func<TSource, Boolean> predicate) : IValueEnumerator<TSource>
    {
        List<TSource> source = source.GetSource();
        internal List<TSource> GetSource() => source;
        internal Func<TSource, bool> Predicate => predicate;
        int index;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = default;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<TSource> destination, Index offset) => false;

        public bool TryGetNext(out TSource current)
        {
            var span = CollectionsMarshal.AsSpan(source);
            while (index < span.Length)
            {
                var value = span[index];
                index++;
                if (predicate(value))
                {
                    current = value;
                    return true;
                }
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
        }

        // Optimize for common pattern: Where().Select()
        public ListWhereSelect<TSource, TResult> Select<TResult>(Func<TSource, TResult> selector)
            => new(source, predicate, selector);
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct ListWhereSelect<TSource, TResult>(List<TSource> source, Func<TSource, Boolean> predicate, Func<TSource, TResult> selector)
        : IValueEnumerator<TResult>
    {
        int index;
        List<TSource> source = source;
        internal List<TSource> GetSource() => source;
        internal Func<TSource, bool> Predicate => predicate;
        internal Func<TSource, TResult> Selector => selector;

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
            var span = CollectionsMarshal.AsSpan(source);
            while (index < span.Length)
            {
                var value = span[index];
                index++;
                if (predicate(value))
                {
                    current = selector(value);
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
