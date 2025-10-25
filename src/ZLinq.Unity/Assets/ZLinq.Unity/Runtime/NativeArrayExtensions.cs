#pragma warning disable CS9074
#nullable enable

using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using ZLinq.Internal;
using ZLinq.Linq;

namespace ZLinq
{
    public static class NativeArrayExtensions
    {
        public static ValueEnumerable<FromNativeArray<T>, T> AsValueEnumerable<T>(this NativeArray<T> source)
            where T : struct
        {
            return new(new(source.AsReadOnly()));
        }

        public static ValueEnumerable<FromNativeArray<T>, T> AsValueEnumerable<T>(this NativeArray<T>.ReadOnly source)
            where T : struct
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromNativeSlice<T>, T> AsValueEnumerable<T>(this NativeSlice<T> source)
            where T : struct
        {
            return new(new(source));
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    public struct FromNativeArray<T> : IValueEnumerator<T>
        where T : struct
    {
        public FromNativeArray(NativeArray<T>.ReadOnly source)
        {
            this.source = source;
            this.index = 0;
        }

        NativeArray<T>.ReadOnly source;
        int index;

        public void Dispose()
        {
        }

        public bool TryCopyTo(Span<T> destination, Index offset)
        {
#if UNITY_2022_1_OR_NEWER
            if (EnumeratorHelper.TryGetSlice<T>(source, offset, destination.Length, out var slice))
            {
                slice.CopyTo(destination);
                return true;
            }
#else
            unsafe
            {
                if (EnumeratorHelper.TryGetSlice<T>(new ReadOnlySpan<T>(source.GetUnsafeReadOnlyPtr(), source.Length), offset, destination.Length, out var slice))
                {
                    slice.CopyTo(destination);
                    return true;
                }
            }
#endif
            return false;
        }

        public bool TryGetNext(out T current)
        {
            if ((uint)index < (uint)source.Length)
            {
                current = source[index++];
                return true;
            }

            current = default!;
            return false;
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Length;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
#if UNITY_2022_1_OR_NEWER
            span = source;
            return true;
#else
            unsafe
            {
                span = new ReadOnlySpan<T>(source.GetUnsafeReadOnlyPtr(), source.Length);
            }
            return true;
#endif
        }
    }

    [StructLayout(LayoutKind.Auto)]
    public struct FromNativeSlice<T> : IValueEnumerator<T>
        where T : struct
    {
        NativeSlice<T> source;
        int index;

        public FromNativeSlice(NativeSlice<T> source)
        {
            this.source = source;
            this.index = 0;
        }

        public void Dispose()
        {
        }

        public unsafe bool TryCopyTo(Span<T> destination, Index offset)
        {
            if (EnumeratorHelper.TryGetSlice(new ReadOnlySpan<T>(source.GetUnsafePtr(), source.Length), offset, destination.Length, out var slice))
            {
                slice.CopyTo(destination);
                return true;
            }
            return false;
        }

        public bool TryGetNext(out T current)
        {
            if ((uint)index < (uint)source.Length)
            {
                current = source[index++];
                return true;
            }

            current = default!;
            return false;
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Length;
            return true;
        }

        public unsafe bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = new ReadOnlySpan<T>(source.GetUnsafePtr(), source.Length);
            return true;
        }
    }
}

#pragma warning restore CS9074
