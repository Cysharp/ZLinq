using System.Buffers;

namespace ZLinq.Internal;

[StructLayout(LayoutKind.Auto)]
internal class RentedRingBuffer<T>(int capacity) : IDisposable
{
    public T[]? Buffer = ArrayPool<T>.Shared.Rent(4);
    public readonly int Capacity = capacity;
    int head = 0;
    public int Count = 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Enqueue(T item)
    {
        if (head == Buffer!.Length && Count != Capacity)
        {
            Expand();
        }


        Buffer![head] = item;
        var newHead = (head + 1);
        head = newHead % Capacity;
        Count = Math.Min(Count + 1, Capacity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryDequeue(out T item)
    {
        if (Count == 0)
        {
            item = default!;
            return false;
        }

        var tail = ((long)head - Count + Capacity) % Capacity;
        ref var tailRef = ref Buffer![tail];
        item = tailRef;
        tailRef = default!;
        Count--;
        return true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    void Expand()
    {
        var newBuffer = ArrayPool<T>.Shared.Rent(Buffer!.Length * 2);
        var prevSpan = Buffer.AsSpan();
        prevSpan.CopyTo(newBuffer);
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            prevSpan.Clear();
        }

        ArrayPool<T>.Shared.Return(Buffer);
        Buffer = newBuffer;
    }

    public void Dispose()
    {
        if (Buffer != null)
        {
            // Clear the buffer if T contains references
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                Buffer.AsSpan(0, Count).Clear();
            }

            ArrayPool<T>.Shared.Return(Buffer);
            Buffer = null;
        }
    }
}
