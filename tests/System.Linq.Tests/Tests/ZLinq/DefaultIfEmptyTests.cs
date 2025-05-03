// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using Xunit;

namespace ZLinq.Tests
{
    public class DefaultIfEmptyTests : EnumerableTests
    {
        [Fact]
        public void SameResultsRepeatCallsNonEmptyQuery()
        {
            var q = from x in new[] { 9999, 0, 888, -1, 66, -777, 1, 2, -12345 }
                    where x > int.MinValue
                    select x;

            Assert.Equal(q.DefaultIfEmpty(5), q.DefaultIfEmpty(5));
        }

        [Fact]
        public void SameResultsRepeatCallsEmptyQuery()
        {
            var q = from x in NumberRangeGuaranteedNotCollectionType(0, 0)
                    select x;

            Assert.Equal(q.DefaultIfEmpty(88), q.DefaultIfEmpty(88));

        }

        public static IEnumerable<object[]> TestData()
        {
            yield return [new int[0], 0, new int[] { 0 }];
            yield return [new int[] { 3 }, 0, new int[] { 3 }];
            yield return [new int[] { 3, -1, 0, 10, 15 }, 0, new int[] { 3, -1, 0, 10, 15 }];

            yield return [new int[0], -10, new int[] { -10 }];
            yield return [new int[] { 3 }, 9, new int[] { 3 }];
            yield return [new int[] { 3, -1, 0, 10, 15 }, 9, new int[] { 3, -1, 0, 10, 15 }];
            yield return [Enumerable.Empty<int>(), 0, new int[] { 0 }];
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public static void DefaultIfEmpty(IEnumerable<int> source, int defaultValue, int[] expected)
        {
            if (defaultValue == 0)
            {
                var result = source.DefaultIfEmpty();
                Assert.Equal(result, result);
                Assert.Equal(expected, result);
                Assert.Equal(expected.Length, result.Count());
                Assert.Equal(expected, result.ToList());
                Assert.Equal(expected, result.ToArray());
            }

            {
                var result = source.DefaultIfEmpty(defaultValue);
                Assert.Equal(result, result);
                Assert.Equal(expected, result);
                Assert.Equal(expected.Length, result.Count());
                Assert.Equal(expected, result.ToList());
                Assert.Equal(expected, result.ToArray());
            }
        }

        [Theory, MemberData(nameof(TestData))]
        public static void DefaultIfEmptyRunOnce(IEnumerable<int> source, int defaultValue, int[] expected)
        {
            if (defaultValue == 0)
            {
                Assert.Equal(expected, source.RunOnce().DefaultIfEmpty());
            }

            Assert.Equal(expected, source.RunOnce().DefaultIfEmpty(defaultValue));
        }

        [Fact]
        public void NullableArray_Empty_WithoutDefaultValue()
        {
            int?[] source = [];
            Assert.Equal([null], source.DefaultIfEmpty());
        }

        [Fact]
        public void NullableArray_Empty_WithDefaultValue()
        {
            int?[] source = [];
            int? defaultValue = 9;
            Assert.Equal([defaultValue], source.DefaultIfEmpty(defaultValue));
        }

        [Fact]
        public void NullSource_ThrowsArgumentNullException()
        {
            IEnumerable<int> source = null;

            AssertExtensions.Throws<ArgumentNullException>("source", () => source.DefaultIfEmpty());
            AssertExtensions.Throws<ArgumentNullException>("source", () => source.DefaultIfEmpty(42));
        }

        [Fact(Skip = SkipReason.EnumeratorBehaviorDifference)]
        public void ForcedToEnumeratorDoesntEnumerate()
        {
            var valueEnumerable = NumberRangeGuaranteedNotCollectionType(0, 3).DefaultIfEmpty();
            // Don't insist on this behaviour, but check it's correct if it happens
            using var en = valueEnumerable.Enumerator;
            Assert.False(en.TryGetNext(out _));
        }

        [Fact]
        public void First_Last_ElementAt()
        {
            IEnumerable<int> nonEmpty = Enumerable.Range(1, 3);
            Assert.Equal(1, nonEmpty.First());
            Assert.Equal(3, nonEmpty.Last());
            Assert.Equal(1, nonEmpty.ElementAt(0));
            Assert.Equal(2, nonEmpty.ElementAt(1));
            Assert.Equal(3, nonEmpty.ElementAt(2));
            Assert.Throws<ArgumentOutOfRangeException>(() => nonEmpty.ElementAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => nonEmpty.ElementAt(4));

            IEnumerable<int> empty = [];
            Assert.Equal(42, empty.DefaultIfEmpty(42).First());
            Assert.Equal(42, empty.DefaultIfEmpty(42).Last());
            Assert.Equal(42, empty.DefaultIfEmpty(42).ElementAt(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => empty.DefaultIfEmpty(42).ElementAt(1));
        }
    }
}
