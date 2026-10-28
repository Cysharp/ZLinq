using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;
using System.Collections;

#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif

namespace ZLinq.Tests.Linq
{
    public class AsValueEnumerableTest
    {
        [Fact]
        public void FromIEnumerable_BasicFunctionality()
        {
            // Arrange
            var source = new[] { 1, 2, 3, 4, 5 };

            // Act
            var result = source.AsValueEnumerable();

            // Assert
            result.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
            count.ShouldBe(source.Length);

            var array = result.ToArray();
            array.ShouldBe(source);
        }

        [Fact]
        public void FromIEnumerable_EmptyCollection()
        {
            // Arrange
            var source = Array.Empty<int>();

            // Act
            var result = source.AsValueEnumerable();

            // Assert
            result.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
            count.ShouldBe(0);

            result.ToArray().ShouldBe(Array.Empty<int>());
        }

        [Fact]
        public void FromIEnumerable_Enumeration()
        {
            // Arrange
            var source = new[] { 1, 2, 3, 4, 5 };
            var result = source.AsValueEnumerable();

            // Act
            var collected = new List<int>();
            foreach (var item in result)
            {
                collected.Add(item);
            }

            // Assert
            collected.ShouldBe(source);
        }

        [Fact]
        public void FromArray_BasicFunctionality()
        {
            // Arrange
            var source = new[] { 1, 2, 3, 4, 5 };

            // Act
            var result = source.AsValueEnumerable();

            // Assert
            result.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
            count.ShouldBe(source.Length);

            result.TryGetSpan(out var span).ShouldBeTrue();
            span.ToArray().ShouldBe(source);

            var array = result.ToArray();
            array.ShouldBe(source);
        }

        [Fact]
        public void FromArray_EmptyArray()
        {
            // Arrange
            var source = Array.Empty<int>();

            // Act
            var result = source.AsValueEnumerable();

            // Assert
            result.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
            count.ShouldBe(0);

            result.TryGetSpan(out var span).ShouldBeTrue();
            span.IsEmpty.ShouldBeTrue();

            result.ToArray().ShouldBe(Array.Empty<int>());
        }

        [Fact]
        public void FromList_BasicFunctionality()
        {
            // Arrange
            var source = new List<int> { 1, 2, 3, 4, 5 };

            // Act
            var result = source.AsValueEnumerable();

            // Assert
            result.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
            count.ShouldBe(source.Count);

#if NET8_0_OR_GREATER
            result.TryGetSpan(out var span).ShouldBeTrue();
            span.ToArray().ShouldBe(source.ToArray());
#else
            // netstandard builds have no public API to access the backing array of List<T>.
            result.TryGetSpan(out _).ShouldBeFalse();
#endif

            var array = result.ToArray();
            array.ShouldBe(source.ToArray());
        }

        [Fact]
        public void FromList_EmptyList()
        {
            // Arrange
            var source = new List<int>();

            // Act
            var result = source.AsValueEnumerable();

            // Assert
            result.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
            count.ShouldBe(0);

#if NET8_0_OR_GREATER
            result.TryGetSpan(out var span).ShouldBeTrue();
            span.IsEmpty.ShouldBeTrue();
#else
            // netstandard builds have no public API to access the backing array of List<T>.
            result.TryGetSpan(out _).ShouldBeFalse();
#endif

            result.ToArray().ShouldBe(Array.Empty<int>());
        }

        [Fact]
        public void FromArraySegment_BasicFunctionality()
        {
            // Arrange
            var array = new[] { 1, 2, 3, 4, 5 };
            var source = new ArraySegment<int>(array, 1, 3); // Elements: 2, 3, 4

            // Act
            var result = source.AsValueEnumerable();

            // Assert
            result.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
            count.ShouldBe(source.Count);

            result.TryGetSpan(out var span).ShouldBeTrue();
            span.ToArray().ShouldBe(new[] { 2, 3, 4 });

            var resultArray = result.ToArray();
            resultArray.ShouldBe(new[] { 2, 3, 4 });
        }

        [Fact]
        public void FromMemory_BasicFunctionality()
        {
            // Arrange
            var array = new[] { 1, 2, 3, 4, 5 };
            var source = new Memory<int>(array, 1, 3); // Elements: 2, 3, 4

            // Act
            var result = source.AsValueEnumerable();

            // Assert
            result.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
            count.ShouldBe(source.Length);

            result.TryGetSpan(out var span).ShouldBeTrue();
            span.ToArray().ShouldBe(new[] { 2, 3, 4 });

            var resultArray = result.ToArray();
            resultArray.ShouldBe(new[] { 2, 3, 4 });
        }

        [Fact]
        public void FromReadOnlyMemory_BasicFunctionality()
        {
            // Arrange
            var array = new[] { 1, 2, 3, 4, 5 };
            var source = new ReadOnlyMemory<int>(array, 1, 3); // Elements: 2, 3, 4

            // Act
            var result = source.AsValueEnumerable();

            // Assert
            result.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
            count.ShouldBe(source.Length);

            result.TryGetSpan(out var span).ShouldBeTrue();
            span.ToArray().ShouldBe(new[] { 2, 3, 4 });

            var resultArray = result.ToArray();
            resultArray.ShouldBe(new[] { 2, 3, 4 });
        }

        [Fact]
        public void FromReadOnlySequence_BasicFunctionality()
        {
            // Arrange
            var array = new[] { 1, 2, 3, 4, 5 };
            var memory = new ReadOnlyMemory<int>(array);
            var source = new ReadOnlySequence<int>(memory);

            // Act
            var result = source.AsValueEnumerable();

            // Assert
            result.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
            count.ShouldBe((int)source.Length);

            result.TryGetSpan(out var span).ShouldBeTrue();
            span.ToArray().ShouldBe(array);

            var resultArray = result.ToArray();
            resultArray.ShouldBe(array);
        }

        [Fact]
        public void FromDictionary_BasicFunctionality()
        {
            // Arrange
            var source = new Dictionary<string, int>
            {
                ["one"] = 1,
                ["two"] = 2,
                ["three"] = 3
            };

            // Act
            var result = source.AsValueEnumerable();

            // Assert
            result.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
            count.ShouldBe(source.Count);

            result.TryGetSpan(out var span).ShouldBeFalse();

            var resultArray = result.ToArray();
            resultArray.Length.ShouldBe(source.Count);
            resultArray.Select(kv => kv.Key).ShouldBe(source.Keys, ignoreOrder: true);
            resultArray.Select(kv => kv.Value).ShouldBe(source.Values, ignoreOrder: true);
        }

        [Fact]
        public void FromSortedDictionary_BasicFunctionality()
        {
            // Arrange
            var source = new SortedDictionary<string, int>
            {
                ["one"] = 1,
                ["two"] = 2,
                ["three"] = 3
            };

            // Act
            var result = source.AsValueEnumerable();

            // Assert
            result.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
            count.ShouldBe(source.Count);

            result.TryGetSpan(out var span).ShouldBeFalse();

            var resultArray = result.ToArray();
            resultArray.Length.ShouldBe(source.Count);
            resultArray.Select(kv => kv.Key).ShouldBe(source.Keys, ignoreOrder: true);
            resultArray.Select(kv => kv.Value).ShouldBe(source.Values, ignoreOrder: true);
        }

        [Fact]
        public void FromQueue_BasicFunctionality()
        {
            // Arrange
            var source = new Queue<int>(new[] { 1, 2, 3, 4, 5 });

            // Act
            var result = source.AsValueEnumerable();

            // Assert
            result.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
            count.ShouldBe(source.Count);

            result.TryGetSpan(out var span).ShouldBeFalse();

            var resultArray = result.ToArray();
            resultArray.ShouldBe(source.ToArray());
        }

        [Fact]
        public void FromStack_BasicFunctionality()
        {
            // Arrange
            var source = new Stack<int>(new[] { 1, 2, 3, 4, 5 });

            // Act
            var result = source.AsValueEnumerable();

            // Assert
            result.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
            count.ShouldBe(source.Count);

            result.TryGetSpan(out var span).ShouldBeFalse();

            var resultArray = result.ToArray();
            resultArray.ShouldBe(source.ToArray());
        }

        [Fact]
        public void FromLinkedList_BasicFunctionality()
        {
            // Arrange
            var source = new LinkedList<int>(new[] { 1, 2, 3, 4, 5 });

            // Act
            var result = source.AsValueEnumerable();

            // Assert
            result.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
            count.ShouldBe(source.Count);

            result.TryGetSpan(out var span).ShouldBeFalse();

            var resultArray = result.ToArray();
            resultArray.ShouldBe(source.ToArray());
        }

        [Fact]
        public void FromHashSet_BasicFunctionality()
        {
            // Arrange
            var source = new HashSet<int>(new[] { 1, 2, 3, 4, 5 });

            // Act
            var result = source.AsValueEnumerable();

            // Assert
            result.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
            count.ShouldBe(source.Count);

            result.TryGetSpan(out var span).ShouldBeFalse();

            var resultArray = result.ToArray();
            resultArray.ShouldBe(source.ToArray(), ignoreOrder: true);
        }

        [Fact]
        public void FromSortedSet_BasicFunctionality()
        {
            // Arrange
            var source = new SortedSet<int>(new[] { 5, 2, 1, 4, 3 });

            // Act
            var result = source.AsValueEnumerable();

            // Assert
            result.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
            count.ShouldBe(source.Count);

            result.TryGetSpan(out var span).ShouldBeFalse();

            var resultArray = result.ToArray();
            resultArray.ShouldBe(new[] { 1, 2, 3, 4, 5 }); // SortedSet should be ordered
        }

        [Fact]
        public void FromSortedSet_ContainsOptimization()
        {
            // Arrange
            var source = new SortedSet<int>(new[] { 1, 2, 3, 4, 5 });
            var result = source.AsValueEnumerable();

            // Act & Assert
            result.Contains(3).ShouldBeTrue();
            result.Contains(6).ShouldBeFalse();
        }

#if NET8_0_OR_GREATER
        [Fact]
        public void FromImmutableArray_BasicFunctionality()
        {
            // Arrange
            var source = ImmutableArray.Create(1, 2, 3, 4, 5);

            // Act
            var result = source.AsValueEnumerable();

            // Assert
            result.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
            count.ShouldBe(source.Length);

            result.TryGetSpan(out var span).ShouldBeTrue();
            span.ToArray().ShouldBe(source.ToArray());

            var resultArray = result.ToArray();
            resultArray.ShouldBe(source.ToArray());
        }

        [Fact]
        public void FromImmutableHashSet_BasicFunctionality()
        {
            // Arrange
            var source = ImmutableHashSet.Create(5, 2, 1, 4, 3);

            // Act
            var result = source.AsValueEnumerable();

            // Assert
            result.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
            count.ShouldBe(source.Count);

            result.TryGetSpan(out var span).ShouldBeFalse();

            var resultArray = result.ToArray();
            resultArray.ShouldBe(new[] { 1, 2, 3, 4, 5 }); // SortedSet should be ordered
        }

        [Fact]
        public void FromImmutableHashSet_ContainsOptimization()
        {
            // Arrange
            var source = ImmutableHashSet.Create(5, 2, 1, 4, 3);
            var result = source.AsValueEnumerable();

            // Act & Assert
            result.Contains(3).ShouldBeTrue();
            result.Contains(6).ShouldBeFalse();
        }

#endif

#if NET9_0_OR_GREATER
        [Fact]
        public void FromSpan_BasicFunctionality()
        {
            // Arrange
            Span<int> source = [1, 2, 3, 4, 5];

            // Act
            var result = source.AsValueEnumerable();

            // Assert
            result.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
            count.ShouldBe(source.Length);

            result.TryGetSpan(out var span).ShouldBeTrue();

            var resultArray = result.ToArray();
            resultArray.ShouldBe(source.ToArray());
        }

        [Fact]
        public void FromReadOnlySpan_BasicFunctionality()
        {
            // Arrange
            ReadOnlySpan<int> source = [1, 2, 3, 4, 5];

            // Act
            var result = source.AsValueEnumerable();

            // Assert
            result.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
            count.ShouldBe(source.Length);

            result.TryGetSpan(out var span).ShouldBeTrue();

            var resultArray = result.ToArray();
            resultArray.ShouldBe(source.ToArray());
        }
#endif

        [Fact]
        public void TryCopyTo_Array()
        {
            // Arrange
            var source = new[] { 1, 2, 3, 4, 5 };
            var result = source.AsValueEnumerable();
            var destination = new int[5];

            // Act
            var success = result.TryCopyTo(destination, 0);

            // Assert
            success.ShouldBeTrue();
            destination.ShouldBe(source);
        }

        [Fact]
        public void TryCopyTo_Array_WithOffset()
        {
            // Arrange
            var source = new[] { 1, 2, 3, 4, 5 };
            var result = source.AsValueEnumerable();
            var destination = new int[3];

            // Act
            var success = result.TryCopyTo(destination, 2);

            // Assert
            success.ShouldBeTrue();
            destination.ShouldBe(new[] { 3, 4, 5 });
        }

        [Fact]
        public void TryCopyTo_List()
        {
            // Arrange
            var source = new List<int> { 1, 2, 3, 4, 5 };
            var result = source.AsValueEnumerable();
            var destination = new int[5];

            // Act
            var success = result.TryCopyTo(destination, 0);

            // Assert
            success.ShouldBeTrue();
            destination.ShouldBe(source.ToArray());
        }

        [Fact]
        public void ComparisonWithStandardLinq_WhereSelect()
        {
            // Arrange
            var source = new[] { 1, 2, 3, 4, 5 };

            // Act - Standard LINQ
            var expected = source
                .Where(x => x % 2 == 0)
                .Select(x => x * 2)
                .ToArray();

            // Act - ZLinq
            var actual = source
                .AsValueEnumerable()
                .Where(x => x % 2 == 0)
                .Select(x => x * 2)
                .ToArray();

            // Assert
            actual.ShouldBe(expected);
        }

        [Fact]
        public void ArrayVariance()
        {
            B[] arrayOfB = new[] { new B(), new B() };
            A[] arrayOfA = arrayOfB;

            var z = arrayOfA.AsValueEnumerable()
                .Select(x => x)
                .ToArray();

            z.Length.ShouldBe(2);
        }

        [Fact]
        public void ArrayVariance2()
        {
            B[] arrayOfB = new[] { new B(), new B() };
            A[] arrayOfA = arrayOfB;

            var z = arrayOfA.AsValueEnumerable()
                .Where(_ => true)
                .ToArray();

            z.Length.ShouldBe(2);
        }

        [Fact]
        public void ArrayVariance3()
        {
            B[] arrayOfB = new[] { new B(), new B() };
            A[] arrayOfA = arrayOfB;

            var z = arrayOfA.AsValueEnumerable()
                .Where(_ => true)
                .Select(x => x)
                .ToArray();

            z.Length.ShouldBe(2);
        }

        [Fact]
        public void ArrayVariance4()
        {
            B[] arrayOfB = new[] { new B(), new B() };
            IEnumerable<A> arrayOfA = arrayOfB;

            var z = arrayOfA.AsValueEnumerable()
                .Select(x => x)
                .ToArray();

            z.Length.ShouldBe(2);
        }

        // NonGeneric

        [Fact]
        public void NonGeneric1()
        {
            IEnumerable source = new[] { 1, 2, 3, 4, 5 };
            var objects = source.AsValueEnumerable().ToArray();
            objects[0].ShouldBe(1);
            objects[1].ShouldBe(2);
            objects[2].ShouldBe(3);
            objects[3].ShouldBe(4);
            objects[4].ShouldBe(5);
        }

        [Fact]
        public void NonGeneric2()
        {
            IEnumerable source = new[] { 1, 2, 3, 4, 5 };
            var objects = source.AsValueEnumerable<int>().ToArray();
            objects.ShouldBe([1, 2, 3, 4, 5]);
        }

        // Helper class for testing disposal
        private class DisposableTestEnumerable<T> : IEnumerable<T>
        {
            private readonly T[] _items;
            private readonly Action _onDispose;

            public DisposableTestEnumerable(T[] items, Action onDispose)
            {
                _items = items;
                _onDispose = onDispose;
            }

            public IEnumerator<T> GetEnumerator() => new DisposableTestEnumerator<T>(_items, _onDispose);

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class DisposableTestEnumerator<T> : IEnumerator<T>
        {
            private readonly T[] _items;
            private readonly Action _onDispose;
            private int _index = -1;

            public DisposableTestEnumerator(T[] items, Action onDispose)
            {
                _items = items;
                _onDispose = onDispose;
            }

            public T Current => _index >= 0 && _index < _items.Length ? _items[_index] : default!;

            object System.Collections.IEnumerator.Current => Current!;

            public bool MoveNext()
            {
                _index++;
                return _index < _items.Length;
            }

            public void Reset() => _index = -1;

            public void Dispose() => _onDispose();
        }


        class A;
        class B : A;
    }
}
