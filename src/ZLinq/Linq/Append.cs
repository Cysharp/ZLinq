namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Appends a value to the end of the sequence.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="element">The value to append to the source sequence.</param>
        /// <returns>A sequence that ends with the specified value.</returns>
        public static ValueEnumerable<Append<TEnumerator, TSource>, TSource> Append<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, TSource element)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, element));
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
    struct Append<TEnumerator, TSource>(TEnumerator source, TSource element)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        byte state;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (source.TryGetNonEnumeratedCount(out count))
            {
                count = checked(count + 1); // same as System.Linq (.NET 11), LongCount falls back to enumeration on overflow
                return true;
            }
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<TSource> destination, Index offset)
        {
            if (destination.Length == 0) return false;
            if (!source.TryGetNonEnumeratedCount(out var srcCount)) return false;

            var totalLength = srcCount + 1;
            var start = offset.GetOffset(totalLength);

            if ((uint)start >= (uint)totalLength) // negative when offset is from end and larger than totalLength
            {
                return false;
            }

            var destinationOffset = 0;
            if (start < srcCount)
            {
                if (!source.TryCopyTo(destination, start))
                {
                    return false;
                }
                destinationOffset = Math.Min(srcCount - start, destination.Length);
            }

            if (destinationOffset < destination.Length && start <= srcCount)
            {
                destination[destinationOffset] = element;
            }

            return true;
        }

        public bool TryGetNext(out TSource current)
        {
            if (state == 0)
            {
                if (source.TryGetNext(out current))
                {
                    return true;
                }
                state = 1;
            }

            if (state == 1)
            {
                current = element;
                state = 2;
                return true;
            }

            current = default!;
            return false;
        }

        public void Dispose()
        {
            state = 2;
            source.Dispose();
        }
    }

}
