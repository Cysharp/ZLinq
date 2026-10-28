namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        /// <summary>Splits the elements of a sequence into chunks of size at most the specified size.</summary>
        /// <param name="source">The source sequence.</param>
        /// <param name="size">The maximum size of each chunk.</param>
        /// <returns>A sequence that contains the elements of the source sequence split into chunks. The last chunk can be smaller than the specified size.</returns>
        public static ValueEnumerable<Chunk<TEnumerator, TSource>, TSource[]> Chunk<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Int32 size)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            const int ArrayMaxLength = 0X7FFFFFC7;

            if (size < 1) Throws.ArgumentOutOfRange(nameof(size));

            // I think it would be better to throw an exception, but adjusting to Array.MaxLength.
            // Related discussion in https://github.com/dotnet/runtime/issues/67132
            size = Math.Min(size, ArrayMaxLength);

            return new(new(source.Enumerator, size));
        }
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
    struct Chunk<TEnumerator, TSource>(TEnumerator source, Int32 size)
        : IValueEnumerator<TSource[]>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        int index;
#if NET9_0_OR_GREATER
        ReadOnlySpan<TSource> sourceSpan;
#endif
        bool isInitialized;
        bool isCompleted;
        bool isCanGetSpan;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (source.TryGetNonEnumeratedCount(out var sourceCount))
            {
                count = sourceCount / size + (sourceCount % size == 0 ? 0 : 1);
                return true;
            }

            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource[]> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(scoped Span<TSource[]> destination, Index offset) => false;

        public bool TryGetNext(out TSource[] current)
        {
            if (isCompleted)
            {
                current = default!;
                return false;
            }

            if (!isInitialized)
            {
                isInitialized = true;

                if (source.TryGetNonEnumeratedCount(out var count))
                {
                    size = Math.Min(size, count); // no needs large buffer
                }

                if (source.TryGetSpan(out var span))
                {
                    isCanGetSpan = true;
#if NET9_0_OR_GREATER
                    sourceSpan = span;
#endif
                    if (span.Length == 0)
                    {
                        isCompleted = true;
                        current = default!;
                        return false;
                    }
                }
            }

            if (isCanGetSpan)
            {
#if NET9_0_OR_GREATER
                var span = sourceSpan;
#else
                source.TryGetSpan(out var span);
#endif

                var currentSpan = span.Slice(index, Math.Min(size, span.Length - index));
                index += currentSpan.Length;
                current = currentSpan.ToArray();
                if (index == span.Length)
                {
                    isCompleted = true;
                }
                return true;
            }
            else // noSpan
            {
                index = 0;
                current = null!;
                while (source.TryGetNext(out var value))
                {
                    if (current == null) current = new TSource[size];

                    current[index++] = value;
                    if (index == size)
                    {
                        index = 0;
                        return true;
                    }
                }

                isCompleted = true;
                if (current == null) return false;
                if (current.Length != index)
                {
                    Array.Resize(ref current, index);
                }
                return true;
            }
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }

}
