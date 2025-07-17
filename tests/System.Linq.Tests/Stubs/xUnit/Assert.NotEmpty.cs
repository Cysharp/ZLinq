using System.Numerics;
using ZLinq.Linq;

namespace ZLinq.Tests;

public static partial class Assert
{
    internal static void NotEmpty<T>(
        ValueEnumerable<FromSequence<T>, T> valueEnumerable)
        where T : INumber<T>
    {
        Xunit.Assert.NotEmpty(valueEnumerable.ToArray());
    }
}
