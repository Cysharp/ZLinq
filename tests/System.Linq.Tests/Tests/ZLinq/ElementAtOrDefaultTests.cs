// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using Xunit;

namespace ZLinq.Tests
{
    public class ElementAtOrDefaultTests : EnumerableTests
    {
        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            var q0 = from x in new[] { 9999, 0, 888, -1, 66, -777, 1, 2, -12345 } where x > int.MinValue select x;
            var q1 = from x in new[] { 9999, 0, 888, -1, 66, -777, 1, 2, -12345 } where x > int.MinValue select x;
            var q2 = from x in new[] { 9999, 0, 888, -1, 66, -777, 1, 2, -12345 } where x > int.MinValue select x;
            Assert.Equal(q0.ElementAtOrDefault(3), q0.ElementAtOrDefault(3));
            Assert.Equal(q1.ElementAtOrDefault(new Index(3)), q1.ElementAtOrDefault(new Index(3)));
            Assert.Equal(q2.ElementAtOrDefault(^6), q2.ElementAtOrDefault(^6));
        }

        [Fact]
        public void SameResultsRepeatCallsStringQuery()
        {
            var q0 = from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", string.Empty } where !string.IsNullOrEmpty(x) select x;
            var q1 = from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", string.Empty } where !string.IsNullOrEmpty(x) select x;
            var q2 = from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", string.Empty } where !string.IsNullOrEmpty(x) select x;

            Assert.Equal(q0.ElementAtOrDefault(4), q0.ElementAtOrDefault(4));
            Assert.Equal(q1.ElementAtOrDefault(new Index(4)), q1.ElementAtOrDefault(new Index(4)));
            Assert.Equal(q2.ElementAtOrDefault(^2), q2.ElementAtOrDefault(^2));
        }

        public static IEnumerable<object[]> TestData()
        {
            yield return [NumberRangeGuaranteedNotCollectionType(9, 1), 0, 1, 9];
            yield return [NumberRangeGuaranteedNotCollectionType(9, 10), 9, 1, 18];
            yield return [NumberRangeGuaranteedNotCollectionType(-4, 10), 3, 7, -1];

            yield return [new int[] { 1, 2, 3, 4 }, 4, 0, 0];
            yield return [new int[0], 0, 0, 0];
            yield return [new int[] { -4 }, 0, 1, -4];
            yield return [new int[] { 9, 8, 0, -5, 10 }, 4, 1, 10];

            yield return [NumberRangeGuaranteedNotCollectionType(-4, 5), -1, 6, 0];
            yield return [NumberRangeGuaranteedNotCollectionType(5, 5), 5, 0, 0];
            yield return [NumberRangeGuaranteedNotCollectionType(0, 0), 0, 0, 0];
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void ElementAtOrDefault(IEnumerable<int> source, int index, int indexFromEnd, int expected)
        {
            var valueEnumerable = source;

            Assert.Equal(expected, valueEnumerable.ElementAtOrDefault(index));

            if (index >= 0)
            {
                Assert.Equal(expected, valueEnumerable.ElementAtOrDefault(new Index(index)));
            }

            Assert.Equal(expected, valueEnumerable.ElementAtOrDefault(^indexFromEnd));
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void ElementAtOrDefaultRunOnce(IEnumerable<int> source, int index, int indexFromEnd, int expected)
        {
            Assert.Equal(expected, source.RunOnce().ElementAtOrDefault(index));

            if (index >= 0)
            {
                Assert.Equal(expected, source.RunOnce().ElementAtOrDefault(new Index(index)));
            }

            Assert.Equal(expected, source.RunOnce().ElementAtOrDefault(^indexFromEnd));
        }

        [Fact]
        public void NullableArray_InvalidIndex_ReturnsNull()
        {
            var source = new int?[] { 9, 8 };
            Assert.Null(source.ElementAtOrDefault(-1));
            Assert.Null(source.ElementAtOrDefault(3));
            Assert.Null(source.ElementAtOrDefault(int.MaxValue));
            Assert.Null(source.ElementAtOrDefault(int.MinValue));

            Assert.Null(source.ElementAtOrDefault(^3));
            Assert.Null(source.ElementAtOrDefault(new Index(3)));
            Assert.Null(source.ElementAtOrDefault(new Index(int.MaxValue)));
            Assert.Null(source.ElementAtOrDefault(^int.MaxValue));
        }

        [Fact]
        public void NullableArray_ValidIndex_ReturnsCorrectObject()
        {
            var source = new int?[] { 9, 8, null, -5, 10 };

            Assert.Null(source.ElementAtOrDefault(2));
            Assert.Equal(-5, source.ElementAtOrDefault(3));

            Assert.Null(source.ElementAtOrDefault(new Index(2)));
            Assert.Equal(-5, source.ElementAtOrDefault(new Index(3)));

            Assert.Null(source.ElementAtOrDefault(^3));
            Assert.Equal(-5, source.ElementAtOrDefault(^2));
        }

        [Fact]
        public void NullSource_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).ElementAtOrDefault(2));
            Assert.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).ElementAtOrDefault(new Index(2)));
            Assert.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).ElementAtOrDefault(^2));
        }

        [Fact]
        public void MutableSource()
        {
            var source = new List<int>() { 0, 1, 2, 3, 4 };
            Assert.Equal(2, source.ElementAtOrDefault(2));
            Assert.Equal(2, source.ElementAtOrDefault(new Index(2)));
            Assert.Equal(2, source.ElementAtOrDefault(^3));

            source.InsertRange(3, [-1, -2]);
            source.RemoveAt(0);
            Assert.Equal(-1, source.ElementAtOrDefault(2));
            Assert.Equal(-1, source.ElementAtOrDefault(new Index(2)));
            Assert.Equal(-1, source.ElementAtOrDefault(^4));
        }

        [Fact]
        public void MutableSourceNotList()
        {
            var source = new List<int>() { 0, 1, 2, 3, 4 };

            {
                var query1 = ForceNotCollection(source).Select(i => i);
                var query2 = ForceNotCollection(source).Select(i => i);
                var query3 = ForceNotCollection(source).Select(i => i);
                Assert.Equal(2, query1.ElementAtOrDefault(2));
                Assert.Equal(2, query2.ElementAtOrDefault(new Index(2)));
                Assert.Equal(2, query3.ElementAtOrDefault(^3));
            }

            {
                var query1 = ForceNotCollection(source).Select(i => i);
                var query2 = ForceNotCollection(source).Select(i => i);
                var query3 = ForceNotCollection(source).Select(i => i);
                source.InsertRange(3, [-1, -2]);
                source.RemoveAt(0);
                Assert.Equal(-1, query1.ElementAtOrDefault(2));
                Assert.Equal(-1, query2.ElementAtOrDefault(new Index(2)));
                Assert.Equal(-1, query3.ElementAtOrDefault(^4));
            }
        }

        [Fact]
        public void EnumerateElements()
        {
            const int ElementCount = 10;
            int state = -1;
            int moveNextCallCount = 0;
            Func<DelegateIterator<int?>> source = () =>
            {
                state = -1;
                moveNextCallCount = 0;
                return new DelegateIterator<int?>(
                    moveNext: () => { moveNextCallCount++; return ++state < ElementCount; },
                    current: () => state,
                    dispose: () => state = -1);
            };

            Assert.Equal(0, source().ElementAtOrDefault(0));
            Assert.Equal(1, moveNextCallCount);
            Assert.Equal(0, source().ElementAtOrDefault(new Index(0)));
            Assert.Equal(1, moveNextCallCount);

            Assert.Equal(5, source().ElementAtOrDefault(5));
            Assert.Equal(6, moveNextCallCount);
            Assert.Equal(5, source().ElementAtOrDefault(new Index(5)));
            Assert.Equal(6, moveNextCallCount);

            Assert.Equal(0, source().ElementAtOrDefault(^ElementCount));
            Assert.Equal(ElementCount + 1, moveNextCallCount);
            Assert.Equal(5, source().ElementAtOrDefault(^5));
            Assert.Equal(ElementCount + 1, moveNextCallCount);

            Assert.Null(source().ElementAtOrDefault(ElementCount));
            Assert.Equal(ElementCount + 1, moveNextCallCount);
            Assert.Null(source().ElementAtOrDefault(new Index(ElementCount)));
            Assert.Equal(ElementCount + 1, moveNextCallCount);
            Assert.Null(source().ElementAtOrDefault(^0));
            Assert.Equal(0, moveNextCallCount);
        }

        [Fact]
        public void NonEmptySource_Consistency()
        {
            int?[] source = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

            Assert.Equal(5, source.ElementAtOrDefault(5));
            Assert.Equal(5, source.ElementAtOrDefault(new Index(5)));
            Assert.Equal(5, source.ElementAtOrDefault(^5));

            Assert.Equal(0, source.ElementAtOrDefault(0));
            Assert.Equal(0, source.ElementAtOrDefault(new Index(0)));
            Assert.Equal(0, source.ElementAtOrDefault(^10));

            Assert.Equal(9, source.ElementAtOrDefault(9));
            Assert.Equal(9, source.ElementAtOrDefault(new Index(9)));
            Assert.Equal(9, source.ElementAtOrDefault(^1));

            Assert.Null(source.ElementAtOrDefault(-1));
            Assert.Null(source.ElementAtOrDefault(^11));

            Assert.Null(source.ElementAtOrDefault(10));
            Assert.Null(source.ElementAtOrDefault(new Index(10)));
            Assert.Null(source.ElementAtOrDefault(^0));

            Assert.Null(source.ElementAtOrDefault(int.MinValue));
            Assert.Null(source.ElementAtOrDefault(^int.MaxValue));

            Assert.Null(source.ElementAtOrDefault(int.MaxValue));
            Assert.Null(source.ElementAtOrDefault(new Index(int.MaxValue)));
        }

        [Fact]
        public void NonEmptySource_Consistency_NotList()
        {
            int?[] source = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

            Assert.Equal(5, ForceNotCollection(source).ElementAtOrDefault(5));
            Assert.Equal(5, ForceNotCollection(source).ElementAtOrDefault(new Index(5)));
            Assert.Equal(5, ForceNotCollection(source).ElementAtOrDefault(^5));

            Assert.Equal(0, ForceNotCollection(source).ElementAtOrDefault(0));
            Assert.Equal(0, ForceNotCollection(source).ElementAtOrDefault(new Index(0)));
            Assert.Equal(0, ForceNotCollection(source).ElementAtOrDefault(^10));

            Assert.Equal(9, ForceNotCollection(source).ElementAtOrDefault(9));
            Assert.Equal(9, ForceNotCollection(source).ElementAtOrDefault(new Index(9)));
            Assert.Equal(9, ForceNotCollection(source).ElementAtOrDefault(^1));

            Assert.Null(ForceNotCollection(source).ElementAtOrDefault(-1));
            Assert.Null(ForceNotCollection(source).ElementAtOrDefault(^11));

            Assert.Null(ForceNotCollection(source).ElementAtOrDefault(10));
            Assert.Null(ForceNotCollection(source).ElementAtOrDefault(new Index(10)));
            Assert.Null(ForceNotCollection(source).ElementAtOrDefault(^0));

            const int ElementCount = 10;
            int state = -1;
            int moveNextCallCount = 0;
            Func<DelegateIterator<int?>> getSource = () =>
            {
                state = -1;
                moveNextCallCount = 0;
                return new DelegateIterator<int?>(
                    moveNext: () => { moveNextCallCount++; return ++state < ElementCount; },
                    current: () => state,
                    dispose: () => state = -1);
            };

            Assert.Null(getSource().ElementAtOrDefault(10));
            Assert.Equal(ElementCount + 1, moveNextCallCount);
            Assert.Null(getSource().ElementAtOrDefault(new Index(10)));
            Assert.Equal(ElementCount + 1, moveNextCallCount);
            Assert.Null(getSource().ElementAtOrDefault(^0));
            Assert.Equal(0, moveNextCallCount);

            Assert.Null(ForceNotCollection(source).ElementAtOrDefault(int.MinValue));
            Assert.Null(ForceNotCollection(source).ElementAtOrDefault(^int.MaxValue));

            Assert.Null(ForceNotCollection(source).ElementAtOrDefault(int.MaxValue));
            Assert.Null(ForceNotCollection(source).ElementAtOrDefault(new Index(int.MaxValue)));
        }

        [Fact]
        public void NonEmptySource_Consistency_ListPartition()
        {
            int?[] source = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

            Assert.Equal(5, ListPartitionOrEmpty(source).ElementAtOrDefault(5));
            Assert.Equal(5, ListPartitionOrEmpty(source).ElementAtOrDefault(new Index(5)));
            Assert.Equal(5, ListPartitionOrEmpty(source).ElementAtOrDefault(^5));

            Assert.Equal(0, ListPartitionOrEmpty(source).ElementAtOrDefault(0));
            Assert.Equal(0, ListPartitionOrEmpty(source).ElementAtOrDefault(new Index(0)));
            Assert.Equal(0, ListPartitionOrEmpty(source).ElementAtOrDefault(^10));

            Assert.Equal(9, ListPartitionOrEmpty(source).ElementAtOrDefault(9));
            Assert.Equal(9, ListPartitionOrEmpty(source).ElementAtOrDefault(new Index(9)));
            Assert.Equal(9, ListPartitionOrEmpty(source).ElementAtOrDefault(^1));

            Assert.Null(ListPartitionOrEmpty(source).ElementAtOrDefault(-1));
            Assert.Null(ListPartitionOrEmpty(source).ElementAtOrDefault(^11));

            Assert.Null(ListPartitionOrEmpty(source).ElementAtOrDefault(10));
            Assert.Null(ListPartitionOrEmpty(source).ElementAtOrDefault(new Index(10)));
            Assert.Null(ListPartitionOrEmpty(source).ElementAtOrDefault(^0));

            Assert.Null(ListPartitionOrEmpty(source).ElementAtOrDefault(int.MinValue));
            Assert.Null(ListPartitionOrEmpty(source).ElementAtOrDefault(^int.MaxValue));

            Assert.Null(ListPartitionOrEmpty(source).ElementAtOrDefault(int.MaxValue));
            Assert.Null(ListPartitionOrEmpty(source).ElementAtOrDefault(new Index(int.MaxValue)));
        }

        [Fact]
        public void NonEmptySource_Consistency_EnumerablePartition()
        {
            int?[] source = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

            Assert.Equal(5, EnumerablePartitionOrEmpty(source).ElementAtOrDefault(5));
            Assert.Equal(5, EnumerablePartitionOrEmpty(source).ElementAtOrDefault(new Index(5)));
            Assert.Equal(5, EnumerablePartitionOrEmpty(source).ElementAtOrDefault(^5));

            Assert.Equal(0, EnumerablePartitionOrEmpty(source).ElementAtOrDefault(0));
            Assert.Equal(0, EnumerablePartitionOrEmpty(source).ElementAtOrDefault(new Index(0)));
            Assert.Equal(0, EnumerablePartitionOrEmpty(source).ElementAtOrDefault(^10));

            Assert.Equal(9, EnumerablePartitionOrEmpty(source).ElementAtOrDefault(9));
            Assert.Equal(9, EnumerablePartitionOrEmpty(source).ElementAtOrDefault(new Index(9)));
            Assert.Equal(9, EnumerablePartitionOrEmpty(source).ElementAtOrDefault(^1));

            Assert.Null(EnumerablePartitionOrEmpty(source).ElementAtOrDefault(-1));
            Assert.Null(EnumerablePartitionOrEmpty(source).ElementAtOrDefault(^11));

            Assert.Null(EnumerablePartitionOrEmpty(source).ElementAtOrDefault(10));
            Assert.Null(EnumerablePartitionOrEmpty(source).ElementAtOrDefault(new Index(10)));
            Assert.Null(EnumerablePartitionOrEmpty(source).ElementAtOrDefault(^0));

            Assert.Null(EnumerablePartitionOrEmpty(source).ElementAtOrDefault(int.MinValue));
            Assert.Null(EnumerablePartitionOrEmpty(source).ElementAtOrDefault(^int.MaxValue));

            Assert.Null(EnumerablePartitionOrEmpty(source).ElementAtOrDefault(int.MaxValue));
            Assert.Null(EnumerablePartitionOrEmpty(source).ElementAtOrDefault(new Index(int.MaxValue)));
        }

        [Fact]
        public void EmptySource_Consistency()
        {
            int?[] source = [];

            Assert.Null(source.ElementAtOrDefault(1));
            Assert.Null(source.ElementAtOrDefault(-1));
            Assert.Null(source.ElementAtOrDefault(new Index(1)));
            Assert.Null(source.ElementAtOrDefault(^1));

            Assert.Null(source.ElementAtOrDefault(0));
            Assert.Null(source.ElementAtOrDefault(new Index(0)));
            Assert.Null(source.ElementAtOrDefault(^0));

            Assert.Null(source.ElementAtOrDefault(int.MinValue));
            Assert.Null(source.ElementAtOrDefault(^int.MaxValue));

            Assert.Null(source.ElementAtOrDefault(int.MaxValue));
            Assert.Null(source.ElementAtOrDefault(new Index(int.MaxValue)));
        }

        [Fact]
        public void EmptySource_Consistency_NotList()
        {
            int?[] source = [];

            Assert.Null(ForceNotCollection(source).ElementAtOrDefault(1));
            Assert.Null(ForceNotCollection(source).ElementAtOrDefault(-1));
            Assert.Null(ForceNotCollection(source).ElementAtOrDefault(new Index(1)));
            Assert.Null(ForceNotCollection(source).ElementAtOrDefault(^1));

            Assert.Null(ForceNotCollection(source).ElementAtOrDefault(0));
            Assert.Null(ForceNotCollection(source).ElementAtOrDefault(new Index(0)));
            Assert.Null(ForceNotCollection(source).ElementAtOrDefault(^0));

            Assert.Null(ForceNotCollection(source).ElementAtOrDefault(int.MinValue));
            Assert.Null(ForceNotCollection(source).ElementAtOrDefault(^int.MaxValue));

            Assert.Null(ForceNotCollection(source).ElementAtOrDefault(int.MaxValue));
            Assert.Null(ForceNotCollection(source).ElementAtOrDefault(new Index(int.MaxValue)));
        }

        [Fact]
        public void EmptySource_Consistency_ListPartition()
        {
            int?[] source = [];

            Assert.Null(ListPartitionOrEmpty(source).ElementAtOrDefault(1));
            Assert.Null(ListPartitionOrEmpty(source).ElementAtOrDefault(-1));
            Assert.Null(ListPartitionOrEmpty(source).ElementAtOrDefault(new Index(1)));
            Assert.Null(ListPartitionOrEmpty(source).ElementAtOrDefault(^1));

            Assert.Null(ListPartitionOrEmpty(source).ElementAtOrDefault(0));
            Assert.Null(ListPartitionOrEmpty(source).ElementAtOrDefault(new Index(0)));
            Assert.Null(ListPartitionOrEmpty(source).ElementAtOrDefault(^0));

            Assert.Null(ListPartitionOrEmpty(source).ElementAtOrDefault(int.MinValue));
            Assert.Null(ListPartitionOrEmpty(source).ElementAtOrDefault(^int.MaxValue));

            Assert.Null(ListPartitionOrEmpty(source).ElementAtOrDefault(int.MaxValue));
            Assert.Null(ListPartitionOrEmpty(source).ElementAtOrDefault(new Index(int.MaxValue)));
        }

        [Fact]
        public void EmptySource_Consistency_EnumerablePartition()
        {
            int?[] source = [];

            Assert.Null(EnumerablePartitionOrEmpty(source).ElementAtOrDefault(1));
            Assert.Null(EnumerablePartitionOrEmpty(source).ElementAtOrDefault(-1));
            Assert.Null(EnumerablePartitionOrEmpty(source).ElementAtOrDefault(new Index(1)));
            Assert.Null(EnumerablePartitionOrEmpty(source).ElementAtOrDefault(^1));

            Assert.Null(EnumerablePartitionOrEmpty(source).ElementAtOrDefault(0));
            Assert.Null(EnumerablePartitionOrEmpty(source).ElementAtOrDefault(new Index(0)));
            Assert.Null(EnumerablePartitionOrEmpty(source).ElementAtOrDefault(^0));

            Assert.Null(EnumerablePartitionOrEmpty(source).ElementAtOrDefault(int.MinValue));
            Assert.Null(EnumerablePartitionOrEmpty(source).ElementAtOrDefault(^int.MaxValue));

            Assert.Null(EnumerablePartitionOrEmpty(source).ElementAtOrDefault(int.MaxValue));
            Assert.Null(EnumerablePartitionOrEmpty(source).ElementAtOrDefault(new Index(int.MaxValue)));
        }
    }
}
