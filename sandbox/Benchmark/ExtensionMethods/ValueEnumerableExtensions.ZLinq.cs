using System.Diagnostics;
using System.Runtime.CompilerServices;
using ZLinq;
using ZLinq.Linq;

namespace Benchmark.ZLinq;

#if !USE_SYSTEM_LINQ
internal static partial class ValueEnumerableExtensions
{
    public static ValueEnumerable<FromArray<T>, T> AsValueEnumerableFromArray<T>(this IEnumerable<T> source)
    {
        Debug.Assert(source is T[]);
        return Unsafe.As<T[]>(source).AsValueEnumerable();
    }

    public static ValueEnumerable<FromList<T>, T> AsValueEnumerableFromList<T>(this IEnumerable<T> source)
    {
        Debug.Assert(source is List<T>);
        return Unsafe.As<List<T>>(source).AsValueEnumerable();
    }
}
#endif

