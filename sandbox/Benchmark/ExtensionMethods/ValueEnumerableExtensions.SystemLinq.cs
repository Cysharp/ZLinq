namespace Benchmark.ZLinq;

#if USE_SYSTEM_LINQ
// Define methods to replace ZLinq's method to System.Linq.
public static partial class ValueEnumerable
{
    public static IEnumerable<T> AsValueEnumerable<T>(this IEnumerable<T> source)
    {
        return source;
    }

#if NET9_0_OR_GREATER
    public static Span<T> AsValueEnumerable<T>(this Span<T> source)
    {
        return source;
    }

    public static ReadOnlySpan<T> AsValueEnumerable<T>(this ReadOnlySpan<T> source)
    {
        return source;
    }
#endif

    public static IEnumerable<int> Range(int start, int count)
    => Enumerable.Range(start, count);

    public static IEnumerable<TResult> Repeat<TResult>(TResult element, int count)
       => Enumerable.Repeat(element, count);

    public static IEnumerable<TResult> Empty<TResult>()
       => Enumerable.Empty<TResult>();

    public static IEnumerable<T> AsValueEnumerableFromArray<T>(this IEnumerable<T> source) => source;

    public static IEnumerable<T> AsValueEnumerableFromList<T>(this IEnumerable<T> source) => source;
}
#endif
