// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Original code: https://github.com/dotnet/runtime/blob/v9.0.3/src/libraries/System.Linq/tests/EnumerableTests.cs

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

#nullable disable

namespace System.Linq
{
    public abstract class EnumerableTests
    {
        protected class TestCollection<T> : ICollection<T>
        {
            public T[] Items;
            public int CountTouched = 0;
            public int CopyToTouched = 0;
            public TestCollection(T[] items) { Items = items; }

            public virtual int Count { get { CountTouched++; return Items.Length; } }
            public bool IsReadOnly => false;
            public void Add(T item) { throw new NotImplementedException(); }
            public void Clear() { throw new NotImplementedException(); }
            public bool Contains(T item) => Items.Contains(item);
            public bool Remove(T item) { throw new NotImplementedException(); }
            public void CopyTo(T[] array, int arrayIndex) { CopyToTouched++; Items.CopyTo(array, arrayIndex); }
            public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)Items).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
        }

        protected class TestNonGenericCollection<T> : ICollection, IEnumerable<T>
        {
            public T[] Items;
            public int CountTouched = 0;

            public TestNonGenericCollection(T[] items) => Items = items;

            public virtual int Count { get { CountTouched++; return Items.Length; } }
            public bool IsSynchronized => false;
            public object SyncRoot => this;
            public void CopyTo(Array array, int index) => throw new NotImplementedException();

            public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)Items).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
        }

        protected class TestEnumerable<T> : IEnumerable<T>
        {
            public T[] Items;
            public TestEnumerable(T[] items) { Items = items; }

            public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)Items).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
        }

        protected class TestReadOnlyCollection<T> : IReadOnlyCollection<T>
        {
            public T[] Items;
            public int CountTouched = 0;
            public TestReadOnlyCollection(T[] items) { Items = items; }

            public int Count { get { CountTouched++; return Items.Length; } }
            public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)Items).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
        }

        protected sealed class FastInfiniteEnumerator<T> : IEnumerable<T>, IEnumerator<T>
        {
            public IEnumerator<T> GetEnumerator() => this;

            IEnumerator IEnumerable.GetEnumerator() => this;

            public bool MoveNext() => true;

            public void Reset() { }

            object IEnumerator.Current => default(T);

            public void Dispose() { }

            public T Current => default(T);
        }

        protected static bool IsEven(int num) => num % 2 == 0;

        protected class AnagramEqualityComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x is null | y is null) return false;
                int length = x.Length;
                if (length != y.Length) return false;
                using (var en = x.OrderBy(i => i).GetEnumerator())
                {
                    foreach (char c in y.OrderBy(i => i))
                    {
                        en.MoveNext();
                        if (c != en.Current) return false;
                    }
                }
                return true;
            }

            public int GetHashCode(string obj)
            {
                if (obj is null) return 0;
                int hash = obj.Length;
                foreach (char c in obj)
                    hash ^= c;
                return hash;
            }
        }

        protected static IEnumerable<int> RepeatedNumberGuaranteedNotCollectionType(int num, long count)
        {
            for (long i = 0; i < count; i++) yield return num;
        }

        protected static IEnumerable<int> NumberRangeGuaranteedNotCollectionType(int num, int count)
        {
            for (int i = 0; i < count; i++) yield return num + i;
        }

        protected static IEnumerable<int?> NullableNumberRangeGuaranteedNotCollectionType(int num, int count)
        {
            for (int i = 0; i < count; i++) yield return num + i;
        }

        protected static IEnumerable<int?> RepeatedNullableNumberGuaranteedNotCollectionType(int? num, long count)
        {
            for (long i = 0; i < count; i++) yield return num;
        }

        protected class ThrowsOnMatchEnumerable<T> : IEnumerable<T>
        {
            private readonly IEnumerable<T> _data;
            private readonly T _thrownOn;

            public ThrowsOnMatchEnumerable(IEnumerable<T> source, T thrownOn)
            {
                _data = source;
                _thrownOn = thrownOn;
            }

            public IEnumerator<T> GetEnumerator()
            {
                foreach (var datum in _data)
                {
                    if (datum.Equals(_thrownOn)) throw new Exception();
                    yield return datum;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        /// <summary>
        /// Test enumerator - returns int values from 1 to 5 inclusive.
        /// </summary>
        protected class TestEnumerator : IEnumerable<int>, IEnumerator<int>
        {
            private int _current = 0;

            public virtual int Current => _current;

            object IEnumerator.Current => Current;

            public void Dispose() { }

            public virtual IEnumerator<int> GetEnumerator() => this;

            public virtual bool MoveNext() => _current++ < 5;

            public void Reset()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        /// <summary>
        /// A test enumerator that throws an InvalidOperationException when invoking Current after MoveNext has been called exactly once.
        /// </summary>
        protected class ThrowsOnCurrentEnumerator : TestEnumerator
        {
            public override int Current
            {
                get
                {
                    var current = base.Current;
                    if (current == 1)
                    {
                        throw new InvalidOperationException();
                    }
                    return current;
                }
            }
        }

        /// <summary>
        /// A test enumerator that throws an InvalidOperationException when invoking MoveNext after MoveNext has been called exactly once.
        /// </summary>
        protected class ThrowsOnMoveNext : TestEnumerator
        {
            public override bool MoveNext()
            {
                bool baseReturn = base.MoveNext();
                if (base.Current == 1)
                {
                    throw new InvalidOperationException();
                }

                return baseReturn;
            }
        }

        /// <summary>
        /// A test enumerator that throws an InvalidOperationException when GetEnumerator is called for the first time.
        /// </summary>
        protected class ThrowsOnGetEnumerator : TestEnumerator
        {
            private int getEnumeratorCallCount;

            public override IEnumerator<int> GetEnumerator()
            {
                if (getEnumeratorCallCount++ == 0)
                {
                    throw new InvalidOperationException();
                }

                return base.GetEnumerator();
            }
        }

        protected static IEnumerable<T> ForceNotCollection<T>(IEnumerable<T> source)
        {
            foreach (T item in source) yield return item;
        }

        protected static IEnumerable<T> FlipIsCollection<T>(IEnumerable<T> source)
        {
            return source is ICollection<T> ? ForceNotCollection(source) : new List<T>(source);
        }

        protected static T[] Repeat<T>(Func<int, T> factory, int count)
        {
            T[] results = new T[count];
            for (int index = 0; index < results.Length; index++)
            {
                results[index] = factory(index);
            }

            return results;
        }

        protected static IEnumerable<T> ListPartitionOrEmpty<T>(IList<T> source) // Or Empty
        {
            var listPartition = source.Skip(0);
            return listPartition;
        }

        protected static IEnumerable<T> EnumerablePartitionOrEmpty<T>(IEnumerable<T> source) // Or Empty
        {
            var enumerablePartition = ForceNotCollection(source).Skip(0);
            return enumerablePartition;
        }

        protected struct StringWithIntArray
        {
            public string name { get; set; }
            public int?[] total { get; set; }
        }

        protected class DelegateBasedCollection<T> : ICollection<T>
        {
            public Func<int> CountWorker { get; set; }
            public Func<bool> IsReadOnlyWorker { get; set; }
            public Action<T> AddWorker { get; set; }
            public Action ClearWorker { get; set; }
            public Func<T, bool> ContainsWorker { get; set; }
            public Func<T, bool> RemoveWorker { get; set; }
            public Action<T[], int> CopyToWorker { get; set; }
            public Func<IEnumerator<T>> GetEnumeratorWorker { get; set; }
            public Func<IEnumerator> NonGenericGetEnumeratorWorker { get; set; }

            public DelegateBasedCollection()
            {
                CountWorker = () => 0;
                IsReadOnlyWorker = () => false;
                AddWorker = item => { };
                ClearWorker = () => { };
                ContainsWorker = item => false;
                RemoveWorker = item => false;
                CopyToWorker = (array, arrayIndex) => { };
                GetEnumeratorWorker = () => Enumerable.Empty<T>().GetEnumerator();
                NonGenericGetEnumeratorWorker = () => GetEnumeratorWorker();
            }

            public int Count => CountWorker();
            public bool IsReadOnly => IsReadOnlyWorker();
            public void Add(T item) => AddWorker(item);
            public void Clear() => ClearWorker();
            public bool Contains(T item) => ContainsWorker(item);
            public bool Remove(T item) => RemoveWorker(item);
            public void CopyTo(T[] array, int arrayIndex) => CopyToWorker(array, arrayIndex);
            public IEnumerator<T> GetEnumerator() => GetEnumeratorWorker();
            IEnumerator IEnumerable.GetEnumerator() => NonGenericGetEnumeratorWorker();
        }

        protected static IEnumerable<IEnumerable<T>> CreateSources<T>(IEnumerable<T> source)
        {
            foreach (Func<IEnumerable<T>, IEnumerable<T>> t in IdentityTransforms<T>())
            {
                yield return t(source);
            }
        }

        protected static IEnumerable<Func<IEnumerable<T>, IEnumerable<T>>> IdentityTransforms<T>()
        {
            // Various collection types all representing the same source.
            List<Func<IEnumerable<T>, IEnumerable<T>>> sources =
            [
                e => e, // original
                e => e.ToArray(), // T[]
                e => e.ToList(), // List<T>
                e => new ReadOnlyCollection<T>(e.ToArray()), // IList<T> that's not List<T>/T[]
                e => new TestCollection<T>(e.ToArray()), // ICollection<T> that's not IList<T>
                e => new TestReadOnlyCollection<T>(e.ToArray()), // IReadOnlyCollection<T> that's not ICollection<T>
                e => ForceNotCollection(e), // IEnumerable<T> with no other interfaces
            ];
            if (typeof(T) == typeof(char))
            {
                sources.Add(e => (IEnumerable<T>)(object)string.Concat((IEnumerable<char>)(object)e)); // string
            }

            // Various transforms that all yield the same elements as the source.
            List<Func<IEnumerable<T>, IEnumerable<T>>> transforms =
            [
                // Concat
                e => e.Concat(ForceNotCollection<T>([])),
                e => ForceNotCollection<T>([]).Concat(e),

                // Following transforms cause test failure on System.Linq tests with .NET 9 
#if NET10_0_OR_GREATER
                // Append
                e =>
                {
                    T[] values = e.ToArray();
                    return values.Length == 0 ? [] : values[0..^1].Append(values[^1]);
                },

                // Prepend
                e =>
                {
                    T[] values = e.ToArray();
                    return values.Length == 0 ? [] : values[1..].Prepend(values[0]);
                },

                // Reverse
                e => e.Reverse().Reverse(),
#endif

                // Select
                e => e.Select(i => i),

                // SelectMany
                e => e.SelectMany<T, T>(i => [i]),

                // Take
                e => e.Take(int.MaxValue),
                e => e.TakeLast(int.MaxValue),
                e => e.TakeWhile(i => true),

                // Skip
                e => e.SkipWhile(i => false),

                // Where
                e => e.Where(i => true),
            ];

            foreach (Func<IEnumerable<T>, IEnumerable<T>> source in sources)
            {
                // Yield the source itself.
                yield return source;

                foreach (Func<IEnumerable<T>, IEnumerable<T>> transform in transforms)
                {
                    // Yield a single transform on the source
                    yield return e => transform(source(e));

                    foreach (Func<IEnumerable<T>, IEnumerable<T>> transform2 in transforms)
                    {
                        // Yield a second transform on the first transform on the source.
                        yield return e => transform2(transform(source(e)));
                    }
                }
            }
        }

        protected sealed class DelegateIterator<TSource> : IEnumerable<TSource>, IEnumerator<TSource>
        {
            private readonly Func<IEnumerator<TSource>> _getEnumerator;
            private readonly Func<bool> _moveNext;
            private readonly Func<TSource> _current;
            private readonly Func<IEnumerator> _explicitGetEnumerator;
            private readonly Func<object> _explicitCurrent;
            private readonly Action _reset;
            private readonly Action _dispose;

            public DelegateIterator(
                Func<IEnumerator<TSource>> getEnumerator = null,
                Func<bool> moveNext = null,
                Func<TSource> current = null,
                Func<IEnumerator> explicitGetEnumerator = null,
                Func<object> explicitCurrent = null,
                Action reset = null,
                Action dispose = null)
            {
                _getEnumerator = getEnumerator ?? (() => this);
                _moveNext = moveNext ?? (() => { throw new NotImplementedException(); });
                _current = current ?? (() => { throw new NotImplementedException(); });
                _explicitGetEnumerator = explicitGetEnumerator ?? (() => { throw new NotImplementedException(); });
                _explicitCurrent = explicitCurrent ?? (() => { throw new NotImplementedException(); });
                _reset = reset ?? (() => { throw new NotImplementedException(); });
                _dispose = dispose ?? (() => { throw new NotImplementedException(); });
            }

            public IEnumerator<TSource> GetEnumerator() => _getEnumerator();

            public bool MoveNext() => _moveNext();

            public TSource Current => _current();

            IEnumerator IEnumerable.GetEnumerator() => _explicitGetEnumerator();

            object IEnumerator.Current => _explicitCurrent();

            void IEnumerator.Reset() => _reset();

            void IDisposable.Dispose() => _dispose();
        }
    }
}
