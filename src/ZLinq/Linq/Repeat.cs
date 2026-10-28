namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        /// <summary>Generates a sequence that contains one repeated value.</summary>
        /// <param name="element">The value to be repeated.</param>
        /// <param name="count">The number of times to repeat the value in the generated sequence.</param>
        /// <returns>A sequence that contains a repeated value.</returns>
        public static ValueEnumerable<FromRepeat<T>, T> Repeat<T>(T element, int count)
        {
            if (count < 0)
            {
                Throws.ArgumentOutOfRange(nameof(count));
            }

            return new(new(element, count));
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromRepeat<T>(T _element, int _count) : IValueEnumerator<T>
    {
        int index;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = _count;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default!;
            return false;
        }

        public bool TryCopyTo(Span<T> destination, Index offset)
        {
            if (EnumeratorHelper.TryGetSliceRange(_count, offset, destination.Length, out var _, out var fillCount))
            {
                destination.Slice(0, fillCount).Fill(_element); // Span.Fill using SIMD
                return true;
            }

            return false;
        }

        public bool TryGetNext(out T current)
        {
            if (index++ < _count)
            {
                current = _element;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
        }
    }
}
