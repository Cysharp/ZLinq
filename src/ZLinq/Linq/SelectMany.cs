namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Projects each element of a sequence to a sequence and flattens the resulting sequences into one sequence.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>A sequence whose elements are the result of invoking the one-to-many transform function on each element of the source sequence.</returns>
        public static ValueEnumerable<SelectMany<TEnumerator, TEnumerator2, TSource, TResult>, TResult> SelectMany<TEnumerator, TEnumerator2, TSource, TResult>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, ValueEnumerable<TEnumerator2, TResult>> selector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TResult>
#if NET9_0_OR_GREATER
           , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(selector)));

        /// <summary>Projects each element of a sequence to a sequence, and flattens the resulting sequences into one sequence. The index of each source element is used in the projected form of that element.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="selector">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param>
        /// <returns>A sequence whose elements are the result of invoking the one-to-many transform function on each element of the source sequence.</returns>
        public static ValueEnumerable<SelectMany2<TEnumerator, TEnumerator2, TSource, TResult>, TResult> SelectMany<TEnumerator, TEnumerator2, TSource, TResult>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, int, ValueEnumerable<TEnumerator2, TResult>> selector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TResult>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(selector)));

        /// <summary>Projects each element of a sequence to a sequence, flattens the resulting sequences into one sequence, and invokes a result selector function on each element therein.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="collectionSelector">A transform function to apply to each element of the source sequence.</param>
        /// <param name="resultSelector">A transform function to apply to each element of the intermediate sequence.</param>
        /// <returns>A sequence whose elements are the result of invoking the one-to-many transform function on each element of the source sequence and then mapping each of those elements and their corresponding source element to a result element.</returns>
        public static ValueEnumerable<SelectMany3<TEnumerator, TEnumerator2, TSource, TCollection, TResult>, TResult> SelectMany<TEnumerator, TEnumerator2, TSource, TCollection, TResult>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, ValueEnumerable<TEnumerator2, TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TCollection>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(collectionSelector), Throws.IfNull(resultSelector)));

        /// <summary>Projects each element of a sequence to a sequence, flattens the resulting sequences into one sequence, and invokes a result selector function on each element therein. The index of each source element is used in the intermediate projected form of that element.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="collectionSelector">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param>
        /// <param name="resultSelector">A transform function to apply to each element of the intermediate sequence.</param>
        /// <returns>A sequence whose elements are the result of invoking the one-to-many transform function on each element of the source sequence and then mapping each of those elements and their corresponding source element to a result element.</returns>
        public static ValueEnumerable<SelectMany4<TEnumerator, TEnumerator2, TSource, TCollection, TResult>, TResult> SelectMany<TEnumerator, TEnumerator2, TSource, TCollection, TResult>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, int, ValueEnumerable<TEnumerator2, TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TCollection>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(collectionSelector), Throws.IfNull(resultSelector)));

        // IEnumerable<T> validation

        /// <summary>Projects each element of a sequence to a sequence and flattens the resulting sequences into one sequence.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>A sequence whose elements are the result of invoking the one-to-many transform function on each element of the source sequence.</returns>
        public static ValueEnumerable<SelectMany<TEnumerator, TSource, TResult>, TResult> SelectMany<TEnumerator, TSource, TResult>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, IEnumerable<TResult>> selector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(selector)));

        /// <summary>Projects each element of a sequence to a sequence, and flattens the resulting sequences into one sequence. The index of each source element is used in the projected form of that element.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="selector">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param>
        /// <returns>A sequence whose elements are the result of invoking the one-to-many transform function on each element of the source sequence.</returns>
        public static ValueEnumerable<SelectMany2<TEnumerator, TSource, TResult>, TResult> SelectMany<TEnumerator, TSource, TResult>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(selector)));

        /// <summary>Projects each element of a sequence to a sequence, flattens the resulting sequences into one sequence, and invokes a result selector function on each element therein.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="collectionSelector">A transform function to apply to each element of the source sequence.</param>
        /// <param name="resultSelector">A transform function to apply to each element of the intermediate sequence.</param>
        /// <returns>A sequence whose elements are the result of invoking the one-to-many transform function on each element of the source sequence and then mapping each of those elements and their corresponding source element to a result element.</returns>
        public static ValueEnumerable<SelectMany3<TEnumerator, TSource, TCollection, TResult>, TResult> SelectMany<TEnumerator, TSource, TCollection, TResult>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(collectionSelector), Throws.IfNull(resultSelector)));

        /// <summary>Projects each element of a sequence to a sequence, flattens the resulting sequences into one sequence, and invokes a result selector function on each element therein. The index of each source element is used in the intermediate projected form of that element.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="collectionSelector">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param>
        /// <param name="resultSelector">A transform function to apply to each element of the intermediate sequence.</param>
        /// <returns>A sequence whose elements are the result of invoking the one-to-many transform function on each element of the source sequence and then mapping each of those elements and their corresponding source element to a result element.</returns>
        public static ValueEnumerable<SelectMany4<TEnumerator, TSource, TCollection, TResult>, TResult> SelectMany<TEnumerator, TSource, TCollection, TResult>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(collectionSelector), Throws.IfNull(resultSelector)));
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
    struct SelectMany<TEnumerator, TSource, TResult>(TEnumerator source, Func<TSource, IEnumerable<TResult>> selector)
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        FromEnumerable<TResult> innerEnumerator;
        bool hasInner = false;

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

        public bool TryCopyTo(scoped Span<TResult> destination, Index offset) => false;

        public bool TryGetNext(out TResult current)
        {
        BEGIN:
            if (hasInner)
            {
                if (innerEnumerator.TryGetNext(out current))
                {
                    return true;
                }

                innerEnumerator.Dispose();
                hasInner = false;
            }

            if (source.TryGetNext(out var value))
            {
                innerEnumerator = selector(value).AsValueEnumerable().Enumerator;
                hasInner = true;
                goto BEGIN;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (hasInner)
            {
                innerEnumerator.Dispose();
            }
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
    struct SelectMany2<TEnumerator, TEnumerator2, TSource, TResult>(TEnumerator source, Func<TSource, int, ValueEnumerable<TEnumerator2, TResult>> selector)
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        where TEnumerator2 : struct, IValueEnumerator<TResult>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        TEnumerator2 innerEnumerator;
        bool hasInner = false;
        int index = 0;

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

        public bool TryCopyTo(scoped Span<TResult> destination, Index offset) => false;

        public bool TryGetNext(out TResult current)
        {
        BEGIN:
            if (hasInner)
            {
                if (innerEnumerator.TryGetNext(out current))
                {
                    return true;
                }

                innerEnumerator.Dispose();
                hasInner = false;
            }

            if (source.TryGetNext(out var value))
            {
                innerEnumerator = selector(value, index++).Enumerator;
                hasInner = true;
                goto BEGIN;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (hasInner)
            {
                innerEnumerator.Dispose();
            }
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
    struct SelectMany3<TEnumerator, TEnumerator2, TSource, TCollection, TResult>(TEnumerator source, Func<TSource, ValueEnumerable<TEnumerator2, TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        where TEnumerator2 : struct, IValueEnumerator<TCollection>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        TEnumerator2 innerEnumerator;
        TSource currentSource = default!;
        bool hasInner = false;

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

        public bool TryCopyTo(scoped Span<TResult> destination, Index offset) => false;

        public bool TryGetNext(out TResult current)
        {
        BEGIN:
            if (hasInner)
            {
                if (innerEnumerator.TryGetNext(out var innerCurrent))
                {
                    current = resultSelector(currentSource, innerCurrent);
                    return true;
                }

                innerEnumerator.Dispose();
                hasInner = false;
            }

            if (source.TryGetNext(out var value))
            {
                currentSource = value;
                innerEnumerator = collectionSelector(value).Enumerator;
                hasInner = true;
                goto BEGIN;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (hasInner)
            {
                innerEnumerator.Dispose();
            }
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
    struct SelectMany4<TEnumerator, TEnumerator2, TSource, TCollection, TResult>(TEnumerator source, Func<TSource, int, ValueEnumerable<TEnumerator2, TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        where TEnumerator2 : struct, IValueEnumerator<TCollection>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        TEnumerator2 innerEnumerator;
        TSource currentSource = default!;
        int index;
        bool hasInner = false;

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

        public bool TryCopyTo(scoped Span<TResult> destination, Index offset) => false;

        public bool TryGetNext(out TResult current)
        {
        BEGIN:
            if (hasInner)
            {
                if (innerEnumerator.TryGetNext(out var innerCurrent))
                {
                    current = resultSelector(currentSource, innerCurrent);
                    return true;
                }

                innerEnumerator.Dispose();
                hasInner = false;
            }

            if (source.TryGetNext(out var value))
            {
                currentSource = value;
                innerEnumerator = collectionSelector(value, index++).Enumerator;
                hasInner = true;
                goto BEGIN;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (hasInner)
            {
                innerEnumerator.Dispose();
            }
            source.Dispose();
        }
    }

    #region IEnumerable valiation

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct SelectMany<TEnumerator, TEnumerator2, TSource, TResult>(TEnumerator source, Func<TSource, ValueEnumerable<TEnumerator2, TResult>> selector)
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        where TEnumerator2 : struct, IValueEnumerator<TResult>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        TEnumerator2 innerEnumerator;
        bool hasInner = false;

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

        public bool TryCopyTo(scoped Span<TResult> destination, Index offset) => false;

        public bool TryGetNext(out TResult current)
        {
        BEGIN:
            if (hasInner)
            {
                if (innerEnumerator.TryGetNext(out current))
                {
                    return true;
                }

                innerEnumerator.Dispose();
                hasInner = false;
            }

            if (source.TryGetNext(out var value))
            {
                innerEnumerator = selector(value).Enumerator;
                hasInner = true;
                goto BEGIN;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (hasInner)
            {
                innerEnumerator.Dispose();
            }
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
    struct SelectMany2<TEnumerator, TSource, TResult>(TEnumerator source, Func<TSource, int, IEnumerable<TResult>> selector)
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        FromEnumerable<TResult> innerEnumerator;
        bool hasInner = false;
        int index = 0;

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

        public bool TryCopyTo(scoped Span<TResult> destination, Index offset) => false;

        public bool TryGetNext(out TResult current)
        {
        BEGIN:
            if (hasInner)
            {
                if (innerEnumerator.TryGetNext(out current))
                {
                    return true;
                }

                innerEnumerator.Dispose();
                hasInner = false;
            }

            if (source.TryGetNext(out var value))
            {
                innerEnumerator = selector(value, index++).AsValueEnumerable().Enumerator;
                hasInner = true;
                goto BEGIN;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (hasInner)
            {
                innerEnumerator.Dispose();
            }
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
    struct SelectMany3<TEnumerator, TSource, TCollection, TResult>(TEnumerator source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        FromEnumerable<TCollection> innerEnumerator;
        TSource currentSource = default!;
        bool hasInner = false;

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

        public bool TryCopyTo(scoped Span<TResult> destination, Index offset) => false;

        public bool TryGetNext(out TResult current)
        {
        BEGIN:
            if (hasInner)
            {
                if (innerEnumerator.TryGetNext(out var innerCurrent))
                {
                    current = resultSelector(currentSource, innerCurrent);
                    return true;
                }

                innerEnumerator.Dispose();
                hasInner = false;
            }

            if (source.TryGetNext(out var value))
            {
                currentSource = value;
                innerEnumerator = collectionSelector(value).AsValueEnumerable().Enumerator;
                hasInner = true;
                goto BEGIN;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (hasInner)
            {
                innerEnumerator.Dispose();
            }
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
    struct SelectMany4<TEnumerator, TSource, TCollection, TResult>(TEnumerator source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        FromEnumerable<TCollection> innerEnumerator;
        TSource currentSource = default!;
        int index;
        bool hasInner = false;

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

        public bool TryCopyTo(scoped Span<TResult> destination, Index offset) => false;

        public bool TryGetNext(out TResult current)
        {
        BEGIN:
            if (hasInner)
            {
                if (innerEnumerator.TryGetNext(out var innerCurrent))
                {
                    current = resultSelector(currentSource, innerCurrent);
                    return true;
                }

                innerEnumerator.Dispose();
                hasInner = false;
            }

            if (source.TryGetNext(out var value))
            {
                currentSource = value;
                innerEnumerator = collectionSelector(value, index++).AsValueEnumerable().Enumerator;
                hasInner = true;
                goto BEGIN;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            if (hasInner)
            {
                innerEnumerator.Dispose();
            }
            source.Dispose();
        }
    }

    #endregion
}
