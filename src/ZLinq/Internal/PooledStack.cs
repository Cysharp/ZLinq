using System.Diagnostics;

namespace ZLinq.Internal;

internal sealed class PooledStack<T> : IDisposable where T : IDisposable
{
    #region Configuration
    private readonly bool _threadSafe;
#if DEBUG
    private readonly int _creatorThreadId;
#endif
    #endregion

    #region Static Pool Management (Thread-Safe Mode)
    private static class Pool
    {
        internal static PooledStack<T>? Last;
        internal static int Size;
        internal const int MaxSize = 32;
    }
    #endregion

    #region Instance Fields
    private PooledStack<T>? _prev;
    private T[] _array;
    private int _size;
    private volatile int _disposedValue;
    #endregion

    #region Constructors
    public PooledStack(bool threadSafe = false, int initialCapacity = 4)
    {
        _size = 0;
        _threadSafe = threadSafe;
        _disposedValue = 0;
        _array = initialCapacity > 0 ? new T[initialCapacity] : [];

        // tracking to ensure that any operation on a non-thread-safe stack
        // is performed from the same thread that created it while in DEBUG mode
#if DEBUG
        if (!threadSafe) _creatorThreadId = Environment.CurrentManagedThreadId;
#endif
    }
    #endregion

    #region Pool Management
    public static PooledStack<T> Rent(bool threadSafe = false)
    {
        if (!threadSafe)
            return new PooledStack<T>(threadSafe: false); // Not pooled

        // Lock-free pool renting logic (only for threadSafe=true)
        SpinWait spin = new();
        while (true)
        {
            PooledStack<T>? currentLast = Volatile.Read(ref Pool.Last);
            if (currentLast == null)
                return new PooledStack<T>(threadSafe: true, initialCapacity: 4);

            if (currentLast.IsDisposedVolatile())
            {
                // Attempt to remove disposed item from pool head
                Interlocked.CompareExchange(ref Pool.Last, currentLast._prev, currentLast);
                // Don't decrement Pool.Size here, as we didn't increment it for this disposed item yet.
                // Let Return() handle decrement if it was ever successfully returned.
                continue;
            }

            PooledStack<T>? newLast = currentLast._prev;
            if (Interlocked.CompareExchange(ref Pool.Last, newLast, currentLast) == currentLast)
            {
                Interlocked.Decrement(ref Pool.Size);
                currentLast._prev = null;
                return currentLast;
            }
            spin.SpinOnce(); // Contention, spin and retry
        }
    }

    public static void Return(PooledStack<T> stack)
    {
        ArgumentNullException.ThrowIfNull(stack);

        if (!stack._threadSafe || stack.IsDisposedVolatile()) return;

        stack.Reset();

        // Lock-free pool return logic
        while (true) // Loop for atomic pool size check and increment
        {
            int currentSize = Volatile.Read(ref Pool.Size);
            if (currentSize >= Pool.MaxSize) return;

            // Attempt to increment pool size atomically
            if (Interlocked.CompareExchange(ref Pool.Size, currentSize + 1, currentSize) != currentSize)
                continue; // Race condition on size, retry outer loop

            // Size increment successful, now add to pool list
            var spin = new SpinWait();
            while (true)
            {
                PooledStack<T>? currentLast = Volatile.Read(ref Pool.Last);
                stack._prev = currentLast;

                // Attempt to set the returned stack as the new head
                if (Interlocked.CompareExchange(ref Pool.Last, stack, currentLast) == currentLast)
                    break;

                spin.SpinOnce(); // Contention on list head, spin and retry inner loop
            }
            break;
        }
    }

    public static void TrimPool()
    {
        if (Volatile.Read(ref Pool.Size) > 0)
        {
            Interlocked.Exchange(ref Pool.Last, null);
            Interlocked.Exchange(ref Pool.Size, 0);
        }
    }
    #endregion

    #region Core Operations
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Push(T value)
    {
        EnsureNotDisposed();
        ArgumentNullException.ThrowIfNull(value);
        DebugAssertSingleThreadAccess();

        if (_size == _array.Length)
        {            
            Array.Resize(ref _array, _array.Length == 0 ? 4 : _array.Length * 2);
        }

        if (_threadSafe)
        {
            _array[_size] = value;            
            Volatile.Write(ref _size, _size + 1);
        }
        else
        {
            _array[_size++] = value; // Non-volatile access for single-threaded mode
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryPop(out T? result)
    {
        EnsureNotDisposed();
        DebugAssertSingleThreadAccess();

        if (_threadSafe)
        {
            int currentSize = Volatile.Read(ref _size);
            if (currentSize == 0)
            {
                result = default;
                return false;
            }

            int newSize = currentSize - 1;
            result = _array[newSize];
            _array[newSize] = default!;
            Volatile.Write(ref _size, newSize);
            return true;
        }
        else
        {
            if (_size == 0)
            {
                result = default;
                return false;
            }
            // Non-volatile access for single-threaded mode
            _size--;
            result = _array[_size];
            _array[_size] = default!;
            return true;
        }
    }

    public void Pop()
    {
        // TryPop already ensures not disposed and handles thread safety
        if (!TryPop(out T? item) || item is null) return;
        item.Dispose(); // Dispose the popped item
    }

    public ref T PeekRefOrNullRef()
    {
        EnsureNotDisposed();
        int size = Volatile.Read(ref _size);
        if (size == 0)
        {
            return ref Unsafe.NullRef<T>();
        }
        return ref _array[size - 1];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryPeek(out T? item)
    {
        EnsureNotDisposed(); // Uses volatile read internally
        DebugAssertSingleThreadAccess();

        if (_threadSafe)
        {
            int currentSize = Volatile.Read(ref _size);
            if (currentSize == 0)
            {
                item = default;
                return false;
            }
            item = _array[currentSize - 1]; // No size modification, Volatile.Read is sufficient
            return true;
        }
        else
        {
            if (_size == 0)
            {
                item = default;
                return false;
            }
            item = _array[_size - 1]; // Non-volatile access
            return true;
        }
    }
    #endregion

    #region Resource Management
    private void Reset()
    {
        // Read size consistent with the mode
        int currentSize = _threadSafe ? Volatile.Read(ref _size) : _size;

        for (int i = 0; i < currentSize; i++)
        {
            T? item = _array[i];
            item?.Dispose();
            _array[i] = default!; // Clear slot
        }

        if (_threadSafe)
            Volatile.Write(ref _size, 0); // Thread-safe reset
        else
            _size = 0; // Non-volatile reset

        // Shrinking logic
        if (_array.Length > 1024) // Consider making threshold configurable
        {
            // Maybe shrink less aggressively? Math.Max(DefaultCapacity*2, _array.Length / 2)?
            Array.Resize(ref _array, Math.Max(4, _array.Length / 2));
        }
    }

    internal static readonly PooledStack<T> DisposeSentinel = new(threadSafe: false, initialCapacity: 0)
    {
        _disposedValue = 1
    };

    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref _disposedValue, 1, 0) == 0)
        {
            Reset();

            _array = [];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsDisposedVolatile()
    {
        return _disposedValue == 1;
    }
    #endregion

    #region Debug Helpers
    [Conditional("DEBUG")]
    private void DebugAssertSingleThreadAccess()
    {
#if DEBUG
        if (!_threadSafe && _creatorThreadId != Environment.CurrentManagedThreadId)
            throw new InvalidOperationException("Cross-thread access detected in single-threaded mode");
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureNotDisposed()
    {
        if (IsDisposedVolatile())
            throw new ObjectDisposedException(nameof(PooledStack<T>));
    }
    #endregion

    #region Utility Methods
    public static ThreadLocal<PooledStack<T>> CreateThreadLocalStack(bool threadSafe = false)
    {
        // Note: If threadSafe is false, pooling is disabled anyway,
        // but ThreadLocal ensures one instance per thread.
        return new ThreadLocal<PooledStack<T>>(() => Rent(threadSafe), trackAllValues: false);
    }

    public int Capacity => _array.Length;

    public int Count => _threadSafe ? Volatile.Read(ref _size) : _size;
    #endregion
}
