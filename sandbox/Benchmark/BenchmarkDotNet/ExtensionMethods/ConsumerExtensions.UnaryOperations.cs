using ZLinq;
using ZLinq.Linq;

namespace Benchmark;

internal static partial class ConsumerExtensions
{
    // Append FromArray
    public static void Consume<T>(this ValueEnumerable<Append<FromArray<T>, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // Cast FromArray
    public static void Consume<T>(this ValueEnumerable<Cast<FromArray<T>, T, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // DefaultIfEmpty FromArray
    public static void Consume<T>(this ValueEnumerable<DefaultIfEmpty<FromArray<T>, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // Distinct FromArray
    public static void Consume<T>(this ValueEnumerable<Distinct<FromArray<T>, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // DistinctBy FromArray
    public static void Consume<T>(this ValueEnumerable<DistinctBy<FromArray<T>, T, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // GroupBy FromArray
    public static void Consume<T>(this ValueEnumerable<GroupBy<FromArray<T>, T, T>, IGrouping<T, T>> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // OfType FromArray
    public static void Consume<T>(this ValueEnumerable<OfType<FromArray<T>, T, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // Order/OrderBy FromArray
    public static void Consume<T>(this ValueEnumerable<OrderBy<FromArray<T>, T, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // Prepend FromArray
    public static void Consume<T>(this ValueEnumerable<Prepend<FromArray<T>, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // Reverse FromArray
    public static void Consume<T>(this ValueEnumerable<Reverse<FromArray<T>, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    #region Select

#if !USE_ZLINQ_NUGET_PACKAGE || ZLINQ_1_3_1_OR_GREATER
    // Select FromArray(Optimized)
    public static void Consume<T>(this ValueEnumerable<ArraySelect<T, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // Select FromArray(Optimized)
    public static void Consume<T>(this ValueEnumerable<ListSelect<T, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }
#else
    // Select FromArray
    public static void Consume<T>(this ValueEnumerable<Select<FromArray<T>, T, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // Select FromList
    public static void Consume<T>(this ValueEnumerable<Select<FromList<T>, T, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }
#endif

    // Select FromEnumerable
    public static void Consume<T>(this ValueEnumerable<Select<FromEnumerable<T>, T, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // Select FromMemory
    public static void Consume<T>(this ValueEnumerable<Select<FromMemory<T>, T, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // Select FromReadOnlySequence
    public static void Consume<T>(this ValueEnumerable<Select<FromReadOnlySequence<T>, T, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

#if NET8_0_OR_GREATER
    // Select FromImmutableArray
    public static void Consume<T>(this ValueEnumerable<Select<FromImmutableArray<T>, T, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }
#endif

#if NET9_0_OR_GREATER
    // Select FromSpan
    public static void Consume<T>(this ValueEnumerable<Select<FromSpan<T>, T, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }
#endif

#if !USE_ZLINQ_NUGET_PACKAGE || ZLINQ_1_3_1_OR_GREATER
    // Select x2 FromArray
    public static void Consume<T>(this ValueEnumerable<Select<ArraySelect<T, T>, T, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // Select x3 FromArray
    public static void Consume<T>(this ValueEnumerable<Select<Select<ArraySelect<T, T>, T, T>, T, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // Select x4 FromArray
    public static void Consume<T>(this ValueEnumerable<Select<Select<Select<ArraySelect<T, T>, T, T>, T, T>, T, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }
#else    
    // Select x2 FromArray
    public static void Consume<T>(this ValueEnumerable<Select<Select<FromArray<T>, T, T>, T, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // Select x3 FromArray
    public static void Consume<T>(this ValueEnumerable<Select<Select<Select<FromArray<T>, T, T>, T, T>, T, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // Select x4 FromArray
    public static void Consume<T>(this ValueEnumerable<Select<Select<Select<Select<FromArray<T>, T, T>, T, T>, T, T>, T, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }
#endif

    #endregion

    // SelectMany FromArray
    public static void Consume<T>(this ValueEnumerable<SelectMany<FromArray<T>, T, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // Shuffle FromArray
    public static void Consume<T>(this ValueEnumerable<Shuffle<FromArray<T>, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // Skip FromArray
    public static void Consume<T>(this ValueEnumerable<Skip<FromArray<T>, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // SkipLast FromArray
    public static void Consume<T>(this ValueEnumerable<SkipLast<FromArray<T>, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // SkipWhile FromArray
    public static void Consume<T>(this ValueEnumerable<SkipWhile<FromArray<T>, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // Take FromArray
    public static void Consume<T>(this ValueEnumerable<Take<FromArray<T>, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // TakeLast FromArray
    public static void Consume<T>(this ValueEnumerable<TakeLast<FromArray<T>, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

    // TakeWhile FromArray
    public static void Consume<T>(this ValueEnumerable<TakeWhile<FromArray<T>, T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }

#if !USE_ZLINQ_NUGET_PACKAGE || ZLINQ_1_3_1_OR_GREATER
    // Where FromArray (Optimized)
    public static void Consume<T>(this ValueEnumerable<ArrayWhere<T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }
#else
    // Where FromArray (Optimized)
    public static void Consume<T>(this ValueEnumerable<WhereArray<T>, T> source, Consumer consumer)
    {
        using var e = source.Enumerator;
        while (e.TryGetNext(out var item))
            consumer.Consume(in item);
    }
#endif
}
