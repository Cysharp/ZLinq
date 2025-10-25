namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<Index<TEnumerator, TSource>, (int Index, TSource Item)> Index<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator));

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
    struct Index<TEnumerator, TSource>(TEnumerator source)
        : IValueEnumerator<(int Index, TSource Item)>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        int index = -1;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            return source.TryGetNonEnumeratedCount(out count);
        }

        public bool TryGetSpan(out ReadOnlySpan<(int Index, TSource Item)> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<(int Index, TSource Item)> destination, Index offset)
        {
            // Iterate inlining
            if (source.TryGetSpan(out var span))
            {
                if (EnumeratorHelper.TryGetSlice(span, offset, destination.Length, out var slice))
                {
                    var index = offset.GetOffset(span.Length);
                    for (var i = 0; (uint)i < (uint)slice.Length; i++)
                    {
                        destination[i] = (index, slice[i]);
                        checked { index++; }
                    }
                    return true;
                }
            }

            return false;
        }

        public bool TryGetNext(out (int Index, TSource Item) current)
        {
            if (source.TryGetNext(out var value))
            {
                checked { index++; }
                current = (index, value);
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
}
