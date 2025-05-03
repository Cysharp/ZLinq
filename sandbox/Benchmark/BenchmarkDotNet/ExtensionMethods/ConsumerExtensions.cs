using ZLinq;

namespace Benchmark;

/// <summary>
/// Helper class that provide extension method that executes and consumes given <see cref="ValueEnumerable{TEnumerator, T}"/>
/// </summary>
/// <remarks>
/// This class define Consume extension method per ValueEnumerable types.
/// Because when calling TryGetNext via IValueEnumerator interface.
/// It run slightly slower because it use callvirt.
/// </remarks>
internal static partial class ConsumerExtensions
{
    [Obsolete("Consume extension methods that accept IValueEnumerator<T> should not be used. because it runs slower.")]
    public static void Consume<TEnumerator, T>(this ValueEnumerable<TEnumerator, T> enumerable, Consumer consumer)
        where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
    {
        using var e = enumerable.Enumerator;
        while (e.TryGetNext(out var item))
        {
            consumer.Consume(in item);
        }
    }
}
