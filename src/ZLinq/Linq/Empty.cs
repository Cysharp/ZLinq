namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        /// <summary>Returns an empty sequence.</summary>
        /// <returns>An empty sequence.</returns>
        public static ValueEnumerable<FromEmpty<T>, T> Empty<T>()
        {
            return new(default);
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromEmpty<T> : IValueEnumerator<T>
    {
        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = [];
            return true;
        }

        public bool TryCopyTo(Span<T> destination, Index offset) => true;

        public bool TryGetNext(out T current)
        {
            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
        }
    }
}
