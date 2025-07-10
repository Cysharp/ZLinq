#if NET8_0_OR_GREATER
using System;
using System.Buffers;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.Intrinsics;
#endif

namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Boolean Contains<TSource>(this ValueEnumerable<FromEnumerable<TSource>, TSource> source, TSource value)
        {
            // When a collection like HashSet internally holds an IEqualityComparer, it's necessary to call the collection's own Contains method.
            // This measure is also important for compatibility with LINQ to Objects.
            var innerCollection = source.Enumerator.GetSource();
            if (innerCollection is ICollection<TSource> collection)
            {
                return collection.Contains(value);
            }

            return ContainsCore(ref source, value);
        }

        public static Boolean Contains<TSource>(this ValueEnumerable<FromHashSet<TSource>, TSource> source, TSource value)
        {
            var innerCollection = source.Enumerator.GetSource();
            return innerCollection.Contains(value);
        }

        public static Boolean Contains<TSource>(this ValueEnumerable<FromSortedSet<TSource>, TSource> source, TSource value)
        {
            var innerCollection = source.Enumerator.GetSource();
            return innerCollection.Contains(value);
        }

#if NET8_0_OR_GREATER

        public static Boolean Contains<TSource>(this ValueEnumerable<FromImmutableHashSet<TSource>, TSource> source, TSource value)
        {
            var innerCollection = source.Enumerator.GetSource();
            return innerCollection.Contains(value);
        }
#endif

        public static Boolean Contains<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, TSource value)
    where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return ContainsCore(ref source, value);
        }

        static Boolean ContainsCore<TEnumerator, TSource>(ref ValueEnumerable<TEnumerator, TSource> source, TSource value)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using (var enumerator = source.Enumerator)
            {
                if (enumerator.TryGetSpan(out var span))
                {

#if NET10_0_OR_GREATER
                    return span.Contains(value);
#elif NET8_0_OR_GREATER
                    return InvokeSpanContains(span, value);
#else
                    foreach (var item in span)
                    {
                        if (EqualityComparer<TSource>.Default.Equals(item, value))
                        {
                            return true;
                        }
                    }
                    return false;
#endif
                }
                else
                {
                    while (enumerator.TryGetNext(out var item))
                    {
                        if (EqualityComparer<TSource>.Default.Equals(item, value))
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }
        }

        public static Boolean Contains<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, TSource value, IEqualityComparer<TSource>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            // In FromEnumerable and FromHashSet, when null is passed, there's a possibility of different behavior from when a non-IEqualityComparer is passed (possibility of ICollection.Contains).
            // However, this behavior is identical to System.Linq.
            comparer ??= EqualityComparer<TSource>.Default;

            using (var enumerator = source.Enumerator)
            {
                if (enumerator.TryGetSpan(out var span))
                {
                    foreach (var item in span)
                    {
                        if (comparer.Equals(item, value))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    while (enumerator.TryGetNext(out var item))
                    {
                        if (comparer.Equals(item, value))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

#if NET8_0_OR_GREATER && (NET8_0 || NET9_0)

        // Hack to avoid where constraints of MemoryExtensions.Contains.
        // .NET 10 removed it so no needs this hack. https://github.com/dotnet/runtime/pull/110197
        internal static unsafe bool InvokeSpanContains<T>(ReadOnlySpan<T> source, T value)
        {
            // Generate code from FileGen.TypeOfContains
            // float, double, decimal and string are `IsBitwiseEquatable<T> == false` so don't use SIMD(but uses unroll search, it slightly faster than handwritten).
            if (typeof(T) == typeof(byte))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, byte>(ref value));
            }
            else if (typeof(T) == typeof(sbyte))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, sbyte>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, sbyte>(ref value));
            }
            else if (typeof(T) == typeof(short))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, short>(ref value));
            }
            else if (typeof(T) == typeof(ushort))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, ushort>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, ushort>(ref value));
            }
            else if (typeof(T) == typeof(int))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, int>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, int>(ref value));
            }
            else if (typeof(T) == typeof(uint))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, uint>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, uint>(ref value));
            }
            else if (typeof(T) == typeof(long))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, long>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, long>(ref value));
            }
            else if (typeof(T) == typeof(ulong))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, ulong>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, ulong>(ref value));
            }
            else if (typeof(T) == typeof(float))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, float>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, float>(ref value));
            }
            else if (typeof(T) == typeof(double))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, double>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, double>(ref value));
            }
            else if (typeof(T) == typeof(bool))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, bool>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, bool>(ref value));
            }
            else if (typeof(T) == typeof(char))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, char>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, char>(ref value));
            }
            else if (typeof(T) == typeof(decimal))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, decimal>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, decimal>(ref value));
            }
            else if (typeof(T) == typeof(nint))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, nint>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, nint>(ref value));
            }
            else if (typeof(T) == typeof(nuint))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, nuint>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, nuint>(ref value));
            }
            else if (typeof(T) == typeof(string))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, string>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, string>(ref value));
            }
            else
            {
                foreach (var item in source)
                {
                    if (EqualityComparer<T>.Default.Equals(item, value))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

#endif
    }
}
