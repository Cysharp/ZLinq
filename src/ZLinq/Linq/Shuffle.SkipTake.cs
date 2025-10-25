using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<ShuffleSkipTake<TEnumerator, TSource>, TSource> Take<TEnumerator, TSource>(this ValueEnumerable<Shuffle<TEnumerator, TSource>, TSource> source, int count)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return new(new(source.Enumerator.source, skipCount: 0, takeCount: Math.Max(0, count)));
        }

        public static ValueEnumerable<ShuffleSkipTake<TEnumerator, TSource>, TSource> Skip<TEnumerator, TSource>(this ValueEnumerable<Shuffle<TEnumerator, TSource>, TSource> source, int count)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return new(new(source.Enumerator.source, skipCount: Math.Max(0, count), takeCount: int.MaxValue));
        }

        // 'Last' variants are syntax sugar. just takes specified count of items randomly from source.

        public static ValueEnumerable<ShuffleSkipTake<TEnumerator, TSource>, TSource> TakeLast<TEnumerator, TSource>(this ValueEnumerable<Shuffle<TEnumerator, TSource>, TSource> source, int count)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return new(new(source.Enumerator.source, skipCount: 0, takeCount: Math.Max(0, count)));
        }

        public static ValueEnumerable<ShuffleSkipTake<TEnumerator, TSource>, TSource> SkipLast<TEnumerator, TSource>(this ValueEnumerable<Shuffle<TEnumerator, TSource>, TSource> source, int count)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return new(new(source.Enumerator.source, skipCount: Math.Max(0, count), takeCount: int.MaxValue));
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
    struct ShuffleSkipTake<TEnumerator, TSource>(TEnumerator source, int skipCount, int takeCount)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;

        // To keep implementation simple and allow for future extensibility (such as Take.Take, etc.),
        // we will use a structure with two int fields.
        readonly int skipCount = skipCount;
        readonly int takeCount = takeCount;

        RentedArrayBox<TSource>? buffer;
        int index = 0;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            if (source.TryGetNonEnumeratedCount(out count))
            {
                count = GetLength(count);
                return true;
            }

            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            InitBuffer();
            span = buffer.Span;
            return true;
        }

        public bool TryCopyTo(scoped Span<TSource> destination, Index offset)
        {
            InitBuffer();

            if (EnumeratorHelper.TryGetSlice<TSource>(buffer.Span, offset, destination.Length, out var slice))
            {
                slice.CopyTo(destination);
                return true;
            }

            return false;
        }

        public bool TryGetNext(out TSource current)
        {
            if (buffer == null)
            {
                InitBuffer();
            }

            if ((uint)index < (uint)buffer.Length)
            {
                current = buffer.UnsafeGetAt(index);
                index++;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            buffer?.Dispose();
            source.Dispose();
        }

        int GetLength(int count)
        {
            var length = Math.Max(0, count - skipCount);
            length = Math.Min(length, takeCount);
            return length;
        }

        [MemberNotNull(nameof(buffer))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        void InitBuffer()
        {
            if (buffer == null)
            {
                var (array, count) = new ValueEnumerable<TEnumerator, TSource>(source).ToArrayPool();

                // Extract only the counts and perform partial shuffling
                var resultCount = GetLength(count);
                if (resultCount == 0)
                {
                    ArrayPool<TSource>.Shared.Return(array, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<TSource>());
                    buffer = RentedArrayBox<TSource>.Empty;
                }
                else
                {
                    RandomShared.PartialShuffle(array.AsSpan(0, count), resultCount);
                    buffer = new RentedArrayBox<TSource>(array, resultCount);
                }
            }
        }
    }
}
