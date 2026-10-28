using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ZLinq;

/// <summary>
/// A zero-allocation sequence that ZLinq operators are chained on. Each operator returns a new <see cref="ValueEnumerable{TEnumerator, T}"/> wrapping the enumerator of the previous one.
/// </summary>
/// <remarks>
/// It is not an <see cref="System.Collections.Generic.IEnumerable{T}"/>; use <c>foreach</c>, the ZLinq operators or a conversion method such as <c>ToArray</c> to consume it.
/// In .NET 9 or later, it is a <see langword="ref"/> struct.
/// </remarks>
[StructLayout(LayoutKind.Auto)]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
[DebuggerTypeProxy(typeof(ValueEnumerableDebugView<,>))]
#if NET9_0_OR_GREATER
public readonly ref
#else
public readonly
#endif
struct ValueEnumerable<TEnumerator, T>(TEnumerator enumerator)
    where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
{
    // enumerator is struct so it always copied, no need to create new Enumerator.
    // internal operator should use this
    /// <summary>
    /// The underlying enumerator of this sequence.
    /// </summary>
    /// <remarks>
    /// When it is used directly (e.g. calling <see cref="IValueEnumerator{T}.TryGetNext(out T)"/> in a loop), it must be disposed after use, typically with a <see langword="using"/> statement.
    /// Do not call other operators on the same sequence after enumerating it.
    /// </remarks>
    public readonly TEnumerator Enumerator = enumerator;

    // Cast and OfType are implemented as instance methods rather than extension methods to simplify type specification.
    /// <summary>Casts the elements of a sequence to the specified type.</summary>
    /// <returns>A sequence that contains each element of the source sequence cast to the specified type.</returns>
    public ValueEnumerable<Cast<TEnumerator, T, TResult>, TResult> Cast<TResult>() => new(new(Enumerator));
    /// <summary>Filters the elements of a sequence based on a specified type.</summary>
    /// <returns>A sequence that contains the elements from the source sequence of the specified type.</returns>
    public ValueEnumerable<OfType<TEnumerator, T, TResult>, TResult> OfType<TResult>() => new(new(Enumerator));

    string DebuggerDisplay => ValueEnumerableDebuggerDisplayHelper.BuildDisplayText(typeof(TEnumerator));
}

// all implement types must be struct
/// <summary>
/// The enumerator of a <see cref="ValueEnumerable{TEnumerator, T}"/>. Implement it as a struct to create a custom operator.
/// </summary>
/// <remarks>
/// It is used instead of <see cref="System.Collections.Generic.IEnumerator{T}"/>. It must be disposed after use, and the source enumerator must be disposed in <see cref="IDisposable.Dispose"/>.
/// </remarks>
public interface IValueEnumerator<T> : IDisposable
{
    /// <summary>
    /// Equivalent of IEnumerator.MoveNext + Current.
    /// </summary>
    bool TryGetNext(out T current);

    // for optimization

    /// <summary>
    /// Returns the length when processing time is not necessary.
    /// Always returns true if TryGetSpan or TryCopyTo returns true.
    /// </summary>
    bool TryGetNonEnumeratedCount(out int count);

    /// <summary>
    /// Returns true if it can return a Span.
    /// Used for SIMD and loop processing optimization.
    /// If copying the entire value is acceptable, prioritize TryGetNonEnumeratedCount -> TryCopyTo instead.
    /// </summary>
    bool TryGetSpan(out ReadOnlySpan<T> span);

    /// <summary>
    /// Unlike the semantics of normal CopyTo, this allows the destination to be smaller than the source.
    /// This serves as a TryGet function as well, e.g. single-span and ^1 is TryGetLast.
    /// </summary>
    bool TryCopyTo(scoped Span<T> destination, Index offset);
}

// generic implementation of enumerator
/// <summary>
/// The enumerator used by <c>foreach</c> over a <see cref="ValueEnumerable{TEnumerator, T}"/>.
/// </summary>
[StructLayout(LayoutKind.Auto)]
#if NET9_0_OR_GREATER
public ref
#else
public
#endif
struct ValueEnumerator<TEnumerator, T>(TEnumerator enumerator) : IDisposable
    where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
{
    TEnumerator enumerator = enumerator;
    T current = default!;

    public T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => current;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        if (enumerator.TryGetNext(out current))
        {
            return true;
        }
        else
        {
            current = default!;
            return false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() => enumerator.Dispose();
}

public static partial class ValueEnumerableExtensions // keep `public static` partial class
{
    // for foreach
    /// <summary>Returns an enumerator to iterate the sequence with <c>foreach</c>.</summary>
    /// <param name="valueEnumerable">The source sequence.</param>
    /// <returns>An enumerator for the sequence.</returns>
    /// <remarks><c>foreach</c> disposes the enumerator. When it is used directly, it must be disposed after use.</remarks>
    public static ValueEnumerator<TEnumerator, T> GetEnumerator<TEnumerator, T>(in this ValueEnumerable<TEnumerator, T> valueEnumerable)
        where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        => new(valueEnumerable.Enumerator);
}

// for custom collection
/// <summary>
/// Implement it in a custom collection to provide a zero-allocation <see cref="ValueEnumerable{TEnumerator, T}"/>.
/// If a collection implements both <see cref="System.Collections.Generic.IEnumerable{T}"/> and this interface, this one takes precedence in ZLinq.
/// </summary>
public interface IValueEnumerable<TEnumerator, T>
    where TEnumerator : struct, IValueEnumerator<T>
{
    // GetValueEnumerator + extension method AsValueEnumerable causes issues
    // with type inference, or conflicts with IEnumerable<T>'s AsValueEnumerable.
    // Since there's no compiler support for foreach,
    // it's more logical to include AsValueEnumerable in the interface.
    /// <summary>Converts the collection into a <see cref="ValueEnumerable{TEnumerator, T}"/> to apply ZLinq operators without allocations.</summary>
    /// <returns>A <see cref="ValueEnumerable{TEnumerator, T}"/> that enumerates the collection.</returns>
    ValueEnumerable<TEnumerator, T> AsValueEnumerable();
}

internal static class ValueEnumerableDebuggerDisplayHelper // avoid <T> for assembly size
{
#if NET8_0_OR_GREATER
    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2070", Justification = "Only call from debugger.")]
#endif
    public static string BuildDisplayText(Type type) // root is typeof(TEnumerator) : IValueEnumerator<T>
    {
        var sb = new StringBuilder();
        BuildCore(sb, type);
        var returnType = type.GetInterface("IValueEnumerator`1")?.GetGenericArguments()[0];
        if (returnType != null)
        {
            sb.Append(" => "); // like TypeScript definition
            sb.Append(returnType.Name);
        }
        return sb.ToString();
    }

    static void BuildCore(StringBuilder sb, Type type)
    {
        if (type.IsGenericType)
        {
            var parent = type.GenericTypeArguments[0];
            if (parent.IsGenericType || (parent.Name is "FromRange" or "FromRange2" or "FromRangeDateTime" or "FromRangeDateTimeTo"))
            {
                BuildCore(sb, parent);
                sb.Append(".");
            }

            var genericsStart = type.Name.IndexOf('`');
            if (genericsStart != -1) // always come here
            {
                var name = type.Name.Substring(0, genericsStart);
                sb.Append(name);
            }
            else
            {
                sb.Append(type.Name);
            }

            if (!parent.IsGenericType && !(parent.Name is "FromRange" or "FromRange2" or "FromRangeDateTime" or "FromRangeDateTimeTo"))
            {
                sb.Append("<");
                sb.Append(parent.Name);
                sb.Append(">");
            }
            return;
        }

        if (type.Name is "FromRange" or "FromRange2" or "FromRangeDateTime" or "FromRangeDateTimeTo")
        {
            sb.Append(type.Name);
        }
        else
        {
            sb.Append("<");
            sb.Append(type.Name);
            sb.Append(">");
        }
    }
}

internal ref struct ValueEnumerableDebugView<TEnumerator, T>
    where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
{
    readonly ValueEnumerable<TEnumerator, T> source;
    T[]? items;

    public ValueEnumerableDebugView(ValueEnumerable<TEnumerator, T> source)
    {
        this.source = source;
    }

    // avoiding side-effects so only run when opened.
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] Items
    {
        get
        {
            if (items == null)
            {
                items = source.Take(100000).ToArray(); // max 100000 to avoid infinite call
            }

            return items;
        }
    }
}
