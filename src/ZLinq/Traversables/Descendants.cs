namespace ZLinq.Traversables;

[StructLayout(LayoutKind.Auto)]
public struct Descendants<TTraverser, T>(TTraverser traverser, bool withSelf)
    : IValueEnumerator<T>
    where TTraverser : struct, ITraverser<TTraverser, T>
{
    PooledStack<Children<TTraverser, T>>? recursiveStack = null;

    public bool TryGetNonEnumeratedCount(out int count)
    {
        count = 0;
        return false;
    }

    public bool TryGetSpan(out ReadOnlySpan<T> span)
    {
        span = default;
        return false;
    }

    public bool TryCopyTo(Span<T> destination, Index offset) => false;

    public bool TryGetNext(out T current)
    {
        // IsDisposed
        if (recursiveStack == PooledStack<Children<TTraverser, T>>.DisposeSentinel)
        {
            Unsafe.SkipInit(out current);
            return false;
        }

        if (withSelf)
        {
            current = traverser.Origin;
            withSelf = false;
            return true;
        }

        // initial setup
        if (recursiveStack == null)
        {
            // mutable struct(enumerator) must use from stack ref
            var children = traverser.Children<TTraverser, T>();
            recursiveStack = PooledStack<Children<TTraverser, T>>.Rent();
            recursiveStack.Push(children.Enumerator);
        }

        // traverse depth first search
        {
            ref var enumerator = ref recursiveStack.PeekRefOrNullRef();
            while (!Unsafe.IsNullRef(ref enumerator))
            {
                while (enumerator.TryGetNext(out var value))
                {
                    current = value; // set result

                    var subTraversable = traverser.ConvertToTraverser(value);
                    try
                    {
                        if (!subTraversable.TryGetHasChild(out var hasChild) || hasChild)
                        {
                            var children = subTraversable.Children<TTraverser, T>();
                            recursiveStack.Push(children.Enumerator);
                        }
                    }
                    finally
                    {
                        subTraversable.Dispose();
                    }

                    return true; // ok.
                }

                enumerator.Dispose(); // dispose before pop
                recursiveStack.Pop(); // end current iteration, Pop
                enumerator = ref recursiveStack.PeekRefOrNullRef(); // Peek Again
            }

            Unsafe.SkipInit(out current);
            return false;
        }
    }

    public void Dispose()
    {
        if (recursiveStack != null)
        {
            PooledStack<Children<TTraverser, T>>.Return(recursiveStack);
            recursiveStack = PooledStack<Children<TTraverser, T>>.DisposeSentinel;
        }
    }
}
