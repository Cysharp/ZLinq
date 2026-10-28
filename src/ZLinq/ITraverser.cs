namespace ZLinq;

public interface ITraverser<TTraverser, T> : IDisposable
    where TTraverser : struct, ITraverser<TTraverser, T> // self
{
    T Origin { get; }
    TTraverser ConvertToTraverser(T next); // for Descendants
    bool TryGetHasChild(out bool hasChild); // optional: optimize use for Descendants
    bool TryGetChildCount(out int count);   // optional: optimize use for Children
    bool TryGetParent(out T parent); // for Ancestors
    bool TryGetNextChild(out T child); // for Children | Descendants
    bool TryGetNextSibling(out T next); // for AfterSelf
    bool TryGetPreviousSibling(out T previous); // BeforeSelf
}

public static class TraverserExtensions
{
    /// <summary>Returns the child nodes of the origin node.</summary>
    /// <param name="traverser">The traverser that represents the origin node.</param>
    /// <returns>A sequence that contains the child nodes.</returns>
    public static ValueEnumerable<Children<TTraverser, T>, T> Children<TTraverser, T>(this TTraverser traverser)
        where TTraverser : struct, ITraverser<TTraverser, T>
    {
        return new(new(traverser, withSelf: false));
    }

    /// <summary>Returns the origin node and its child nodes.</summary>
    /// <param name="traverser">The traverser that represents the origin node.</param>
    /// <returns>A sequence that contains the origin node and the child nodes.</returns>
    public static ValueEnumerable<Children<TTraverser, T>, T> ChildrenAndSelf<TTraverser, T>(this TTraverser traverser)
        where TTraverser : struct, ITraverser<TTraverser, T>
    {
        return new(new(traverser, withSelf: true));
    }

    /// <summary>Returns the descendant nodes of the origin node in depth-first order.</summary>
    /// <param name="traverser">The traverser that represents the origin node.</param>
    /// <returns>A sequence that contains the descendant nodes.</returns>
    public static ValueEnumerable<Descendants<TTraverser, T>, T> Descendants<TTraverser, T>(this TTraverser traverser)
        where TTraverser : struct, ITraverser<TTraverser, T>
    {
        return new(new(traverser, withSelf: false));
    }

    /// <summary>Returns the origin node and its descendant nodes in depth-first order.</summary>
    /// <param name="traverser">The traverser that represents the origin node.</param>
    /// <returns>A sequence that contains the origin node and the descendant nodes.</returns>
    public static ValueEnumerable<Descendants<TTraverser, T>, T> DescendantsAndSelf<TTraverser, T>(this TTraverser traverser)
        where TTraverser : struct, ITraverser<TTraverser, T>
    {
        return new(new(traverser, withSelf: true));
    }

    /// <summary>Returns the ancestor nodes of the origin node, from the parent to the root.</summary>
    /// <param name="traverser">The traverser that represents the origin node.</param>
    /// <returns>A sequence that contains the ancestor nodes.</returns>
    public static ValueEnumerable<Ancestors<TTraverser, T>, T> Ancestors<TTraverser, T>(this TTraverser traverser)
        where TTraverser : struct, ITraverser<TTraverser, T>
    {
        return new(new(traverser, withSelf: false));
    }

    /// <summary>Returns the origin node and its ancestor nodes, from the origin node to the root.</summary>
    /// <param name="traverser">The traverser that represents the origin node.</param>
    /// <returns>A sequence that contains the origin node and the ancestor nodes.</returns>
    public static ValueEnumerable<Ancestors<TTraverser, T>, T> AncestorsAndSelf<TTraverser, T>(this TTraverser traverser)
        where TTraverser : struct, ITraverser<TTraverser, T>
    {
        return new(new(traverser, withSelf: true));
    }

    /// <summary>Returns the sibling nodes before the origin node.</summary>
    /// <param name="traverser">The traverser that represents the origin node.</param>
    /// <returns>A sequence that contains the sibling nodes before the origin node.</returns>
    public static ValueEnumerable<BeforeSelf<TTraverser, T>, T> BeforeSelf<TTraverser, T>(this TTraverser traverser)
        where TTraverser : struct, ITraverser<TTraverser, T>
    {
        return new(new(traverser, withSelf: false));
    }

    /// <summary>Returns the sibling nodes before the origin node, followed by the origin node.</summary>
    /// <param name="traverser">The traverser that represents the origin node.</param>
    /// <returns>A sequence that contains the sibling nodes before the origin node and the origin node.</returns>
    public static ValueEnumerable<BeforeSelf<TTraverser, T>, T> BeforeSelfAndSelf<TTraverser, T>(this TTraverser traverser)
        where TTraverser : struct, ITraverser<TTraverser, T>
    {
        return new(new(traverser, withSelf: true));
    }

    /// <summary>Returns the sibling nodes after the origin node.</summary>
    /// <param name="traverser">The traverser that represents the origin node.</param>
    /// <returns>A sequence that contains the sibling nodes after the origin node.</returns>
    public static ValueEnumerable<AfterSelf<TTraverser, T>, T> AfterSelf<TTraverser, T>(this TTraverser traverser)
        where TTraverser : struct, ITraverser<TTraverser, T>
    {
        return new(new(traverser, withSelf: false));
    }

    /// <summary>Returns the origin node, followed by the sibling nodes after it.</summary>
    /// <param name="traverser">The traverser that represents the origin node.</param>
    /// <returns>A sequence that contains the origin node and the sibling nodes after it.</returns>
    public static ValueEnumerable<AfterSelf<TTraverser, T>, T> AfterSelfAndSelf<TTraverser, T>(this TTraverser traverser)
        where TTraverser : struct, ITraverser<TTraverser, T>
    {
        return new(new(traverser, withSelf: true));
    }
}
