using ZLinq;
using ZLinq.Linq;

namespace Benchmark;

internal static partial class ConsumerExtensions
{
    // Concat FromArray
    public static void Consume<T>(this ValueEnumerable<Concat<FromArray<T>, FromEnumerable<T>, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // Concat FromList
    public static void Consume<T>(this ValueEnumerable<Concat<FromList<T>, FromEnumerable<T>, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // Except FromArray
    public static void Consume<T>(this ValueEnumerable<Except<FromArray<T>, FromEnumerable<T>, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // Except FromList
    public static void Consume<T>(this ValueEnumerable<Except<FromList<T>, FromEnumerable<T>, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // GroupJoin FromArray
    public static void Consume<T>(this ValueEnumerable<GroupJoin<FromArray<T>, FromEnumerable<T>, T, T, T, (T x, IEnumerable<T> y)>, (T x, IEnumerable<T> y)> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // Intersect FromArray
    public static void Consume<T>(this ValueEnumerable<Intersect<FromArray<T>, FromEnumerable<T>, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // Join FromArray
    public static void Consume<T>(this ValueEnumerable<Join<FromArray<T>, FromEnumerable<T>, T, T, T, (T x, T y)>, (T x, T y)> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // LeftJoin FromArray
    public static void Consume<T>(this ValueEnumerable<LeftJoin<FromArray<T>, FromEnumerable<T>, T, T, T, (T x, T? y)>, (T x, T? y)> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // RightJoin FromArray
    public static void Consume<T>(this ValueEnumerable<RightJoin<FromArray<T>, FromEnumerable<T>, T, T, T, (T? x, T y)>, (T? x, T y)> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // Union FromArray
    public static void Consume<T>(this ValueEnumerable<Union<FromArray<T>, FromEnumerable<T>, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // Zip FromArray
    public static void Consume<T>(this ValueEnumerable<Zip<FromArray<T>, FromEnumerable<T>, T, T, (T x, T y)>, (T x, T y)> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }
}
