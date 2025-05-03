// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using Xunit;

namespace ZLinq.Tests
{
    public class CountTests : EnumerableTests
    {
        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            var q = from x in new[] { 9999, 0, 888, -1, 66, -777, 1, 2, -12345 }
                    where x > int.MinValue
                    select x;

            Assert.Equal(q.Count(), q.Count());
        }

        [Fact]
        public void SameResultsRepeatCallsStringQuery()
        {
            var q = from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", string.Empty }
                    where !string.IsNullOrEmpty(x)
                    select x;

            Assert.Equal(q.Count(), q.Count());
        }

        public static IEnumerable<object[]> Int_TestData()
        {
            yield return [new int[0], null, 0];

            Func<int, bool> isEvenFunc = IsEven;
            yield return [new int[0], isEvenFunc, 0];
            yield return [new int[] { 4 }, isEvenFunc, 1];
            yield return [new int[] { 5 }, isEvenFunc, 0];
            yield return [new int[] { 2, 5, 7, 9, 29, 10 }, isEvenFunc, 2];
            yield return [new int[] { 2, 20, 22, 100, 50, 10 }, isEvenFunc, 6];

            yield return [RepeatedNumberGuaranteedNotCollectionType(0, 0), null, 0];
            yield return [RepeatedNumberGuaranteedNotCollectionType(5, 1), null, 1];
            yield return [RepeatedNumberGuaranteedNotCollectionType(5, 10), null, 10];
        }

        [Theory]
        [MemberData(nameof(Int_TestData))]
        public void Int(IEnumerable<int> source, Func<int, bool> predicate, int expected)
        {
            Assert.All(CreateSources(source), source =>
            {
                if (predicate is null)
                {
                    Assert.Equal(expected, source.Count());
                }
                else
                {
                    Assert.Equal(expected, source.Count(predicate));
                }
            });
        }

        [Theory, MemberData(nameof(Int_TestData))]
        public void IntRunOnce(IEnumerable<int> source, Func<int, bool> predicate, int expected)
        {
            Assert.All(CreateSources(source), source =>
            {
                if (predicate is null)
                {
                    Assert.Equal(expected, source.RunOnce().Count());
                }
                else
                {
                    Assert.Equal(expected, source.RunOnce().Count(predicate));
                }
            });
        }

        [Fact]
        public void NullableIntArray_IncludesNullObjects()
        {
            int?[] data = [-10, 4, 9, null, 11];
            Assert.Equal(5, data.Count());
        }

        [Theory]
        [MemberData(nameof(CountsAndTallies))]
        public void CountMatchesTally<T>(int count, IEnumerable<T> enumerable)
        {
            Assert.Equal(count, enumerable.Count());
        }

        [Theory, MemberData(nameof(CountsAndTallies))]
        public void RunOnce<T>(int count, IEnumerable<T> enumerable)
        {
            Assert.Equal(count, enumerable.RunOnce().Count());
        }

        private static IEnumerable<object[]> EnumerateCollectionTypesAndCounts<T>(int count, IEnumerable<T> enumerable)
        {
            foreach (IEnumerable<T> source in CreateSources(enumerable))
            {
                yield return [count, source];
            }
        }

        public static IEnumerable<object[]> CountsAndTallies()
        {
            int count = 5;
            var range = Enumerable.Range(1, count);
            foreach (object[] variant in EnumerateCollectionTypesAndCounts(count, range))
                yield return variant;
            foreach (object[] variant in EnumerateCollectionTypesAndCounts(count, range.Select(i => (float)i).ToArray()))
                yield return variant;
            foreach (object[] variant in EnumerateCollectionTypesAndCounts(count, range.Select(i => (double)i).ToArray()))
                yield return variant;
            foreach (object[] variant in EnumerateCollectionTypesAndCounts(count, range.Select(i => (decimal)i).ToArray()))
                yield return variant;
        }

        [Fact]
        public void NullSource_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).Count());
            AssertExtensions.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).Count(i => i != 0));
        }

        [Fact]
        public void NullPredicate_ThrowsArgumentNullException()
        {
            Func<int, bool> predicate = null;
            AssertExtensions.Throws<ArgumentNullException>("predicate", () => Enumerable.Range(0, 3).Count(predicate));
        }

        [Fact]
        public void NonEnumeratedCount_NullSource_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).TryGetNonEnumeratedCount(out _));
        }

        [Fact]
        public void NonEnumeratedCount_SupportedEnumerables_ShouldReturnExpectedCount()
        {
            {
                var source = new int[] { 1, 2, 3, 4 };
                Assert.True(source.TryGetNonEnumeratedCount(out int actualCount));
                Assert.Equal(source.Length, actualCount);
            }
            {
                var source = new List<int>([1, 2, 3, 4]);
                Assert.True(source.TryGetNonEnumeratedCount(out int actualCount));
                Assert.Equal(source.Count, actualCount);
            }
            {
                var source = new Stack<int>([1, 2, 3, 4]);
                Assert.True(source.TryGetNonEnumeratedCount(out int actualCount));
                Assert.Equal(source.Count, actualCount);
            }
            {
                var source = Enumerable.Empty<string>();
                Assert.True(source.TryGetNonEnumeratedCount(out int actualCount));
                Assert.Equal(0, actualCount);
            }
            {
                var source = Enumerable.Range(1, 100);
                Assert.True(source.TryGetNonEnumeratedCount(out int actualCount));
                Assert.Equal(100, actualCount);
            }
            {
                var source = Enumerable.Repeat(1, 80);
                Assert.True(source.TryGetNonEnumeratedCount(out int actualCount));
                Assert.Equal(80, actualCount);
            }

            if (PlatformDetection.IsLinqSpeedOptimized)
            {
                {
                    var source = Enumerable.Range(1, 50).Select(x => x + 1);
                    Assert.True(source.TryGetNonEnumeratedCount(out int actualCount));
                    Assert.Equal(source.Count(), actualCount);
                }
                {
                    var source = new int[] { 1, 2, 3, 4 }.Select(x => x + 1);
                    Assert.True(source.TryGetNonEnumeratedCount(out int actualCount));
                    Assert.Equal(source.Count(), actualCount);
                }
                {
                    var source = Enumerable.Range(1, 50).Select(x => x + 1).Select(x => x - 1);
                    Assert.True(source.TryGetNonEnumeratedCount(out int actualCount));
                    Assert.Equal(source.Count(), actualCount);
                }
                {
                    var source = Enumerable.Range(1, 20).Reverse();
                    Assert.True(source.TryGetNonEnumeratedCount(out int actualCount));
                    Assert.Equal(source.Count(), actualCount);
                }
                {
                    var source = Enumerable.Range(1, 20).OrderBy(x => -x);
                    Assert.True(source.TryGetNonEnumeratedCount(out int actualCount));
                    Assert.Equal(source.Count(), actualCount);
                }
                {
                    var source = Enumerable.Range(1, 10).Concat(Enumerable.Range(11, 10));
                    Assert.True(source.TryGetNonEnumeratedCount(out int actualCount));
                    Assert.Equal(source.Count(), actualCount);
                }
            }
        }

        [Fact]
        public void NonEnumeratedCount_UnsupportedEnumerables_ShouldReturnFalse()
        {
            {
                var source = Enumerable.Range(1, 100).Where(x => x % 2 == 0);
                Assert.False(source.TryGetNonEnumeratedCount(out int actualCount));
                Assert.Equal(0, actualCount);
            }
            {
                var source = Enumerable.Range(1, 100).GroupBy(x => x % 2 == 0);
                Assert.False(source.TryGetNonEnumeratedCount(out int actualCount));
                Assert.Equal(0, actualCount);
            }
            // When using ZLinq. TryGetNonEnumeratedCount return true for Select query.
            {
                ////var source = new Stack<int>(new int[] { 1, 2, 3, 4 }).Select(x => x + 1);
                ////Assert.False(source.TryGetNonEnumeratedCount(out int actualCount));
                ////Assert.Equal(0, actualCount);
            }
            {
                var source = Enumerable.Range(1, 100).Distinct();
                Assert.False(source.TryGetNonEnumeratedCount(out int actualCount));
                Assert.Equal(0, actualCount);
            }

            if (!PlatformDetection.IsLinqSpeedOptimized)
            {
                {
                    var source = Enumerable.Range(1, 100);
                    Assert.False(source.TryGetNonEnumeratedCount(out int actualCount));
                    Assert.Equal(0, actualCount);
                }
                {
                    var source = Enumerable.Repeat(1, 80);
                    Assert.False(source.TryGetNonEnumeratedCount(out int actualCount));
                    Assert.Equal(0, actualCount);
                }
                {
                    var source = Enumerable.Range(1, 50).Select(x => x + 1);
                    Assert.False(source.TryGetNonEnumeratedCount(out int actualCount));
                    Assert.Equal(0, actualCount);
                }
                {
                    var source = new int[] { 1, 2, 3, 4 }.Select(x => x + 1);
                    Assert.False(source.TryGetNonEnumeratedCount(out int actualCount));
                    Assert.Equal(0, actualCount);
                }
                {
                    var source = Enumerable.Range(1, 50).Select(x => x + 1).Select(x => x - 1);
                    Assert.False(source.TryGetNonEnumeratedCount(out int actualCount));
                    Assert.Equal(0, actualCount);
                }
                {
                    var source = Enumerable.Range(1, 20).Reverse();
                    Assert.False(source.TryGetNonEnumeratedCount(out int actualCount));
                    Assert.Equal(0, actualCount);
                }
                {
                    var source = Enumerable.Range(1, 20).OrderBy(x => -x);
                    Assert.False(source.TryGetNonEnumeratedCount(out int actualCount));
                    Assert.Equal(0, actualCount);
                }
                {
                    var source = Enumerable.Range(1, 10).Concat(Enumerable.Range(11, 10));
                    Assert.False(source.TryGetNonEnumeratedCount(out int actualCount));
                    Assert.Equal(0, actualCount);
                }
            }
        }

        [Fact]
        public void NonEnumeratedCount_ShouldNotEnumerateSource()
        {
            bool isEnumerated = false;
            Assert.False(Source().TryGetNonEnumeratedCount(out int count));
            Assert.Equal(0, count);
            Assert.False(isEnumerated);

            IEnumerable<int> Source()
            {
                isEnumerated = true;
                yield return 42;
            }
        }
    }
}
