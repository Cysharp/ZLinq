// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ZLinq.Linq;

namespace ZLinq.Tests
{
    public class RangeTests : RangeTestsBase
    {
        protected override ValueEnumerable<FromRange, int> GetRange(int start, int count) => ValueEnumerable.Range(start, count);

        [Fact]
        public void Range_ThrowExceptionOnNegativeCount()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => GetRange(1, -1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => GetRange(1, int.MinValue));
        }

        [Fact]
        public void Range_ThrowExceptionOnOverflow()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => GetRange(1000, int.MaxValue));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => GetRange(int.MaxValue, 1000));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => GetRange(int.MaxValue - 10, 20));
        }
    }

    // SequenceAsRangeTests is remnoved.
    // Because there is no implicit convertion from `ValueEnumerable<FromRange, int>` to `ValueEnumerable<FromInt32Sequence, int>`

    public abstract class RangeTestsBase : EnumerableTests
    {
        protected abstract ValueEnumerable<FromRange, int> GetRange(int start, int count);

        [Fact]
        public void Range_ProduceCorrectSequence()
        {
            var rangeSequence = GetRange(1, 100);
            int expected = 0;
            foreach (var val in rangeSequence)
            {
                expected++;
                Assert.Equal(expected, val);
            }

            Assert.Equal(100, expected);
        }

        public static IEnumerable<object[]> Range_ToArray_ProduceCorrectResult_MemberData()
        {
            for (int i = 0; i < 64; i++)
            {
                yield return [i];
            }
        }

        [Theory]
        [MemberData(nameof(Range_ToArray_ProduceCorrectResult_MemberData))]
        public void Range_ToArray_ProduceCorrectResult(int length)
        {
            var array = GetRange(1, length).ToArray();
            Assert.Equal(length, array.Length);
            for (var i = 0; i < array.Length; i++)
                Assert.Equal(i + 1, array[i]);
        }

        [Fact]
        public void Range_ToList_ProduceCorrectResult()
        {
            var list = GetRange(1, 100).ToList();
            Assert.Equal(100, list.Count);
            for (var i = 0; i < list.Count; i++)
                Assert.Equal(i + 1, list[i]);
        }

        [Fact]
        public void Range_ZeroCountLeadToEmptySequence()
        {
            var array = GetRange(1, 0).ToArray();
            var array2 = GetRange(int.MinValue, 0).ToArray();
            var array3 = GetRange(int.MaxValue, 0).ToArray();
            Assert.Equal(0, array.Length);
            Assert.Equal(0, array2.Length);
            Assert.Equal(0, array3.Length);
        }

        [Fact]
        public void Range_NotEnumerateAfterEnd()
        {
            using var rangeEnum = GetRange(1, 1).GetEnumerator();
            Assert.True(rangeEnum.MoveNext());
            Assert.False(rangeEnum.MoveNext());
            Assert.False(rangeEnum.MoveNext());
        }

        [Fact(Skip = SkipReason.RefStruct)]
        public void Range_EnumerableAndEnumeratorAreSame()
        {
            var rangeEnumerable = GetRange(1, 1);
            using var rangeEnumerator = rangeEnumerable.GetEnumerator();
            Assert.Same(rangeEnumerable, rangeEnumerator);
        }

        [Fact(Skip = SkipReason.RefStruct)]
        public void Range_GetEnumeratorReturnUniqueInstances()
        {
            var rangeEnumerable = GetRange(1, 1);
            using var enum1 = rangeEnumerable.GetEnumerator();
            using var enum2 = rangeEnumerable.GetEnumerator();
            Assert.NotSame(enum1, enum2);
        }

        [Fact]
        public void Range_ToInt32MaxValue()
        {
            int from = int.MaxValue - 3;
            int count = 4;
            var rangeEnumerable = GetRange(from, count);

            Assert.Equal(count, rangeEnumerable.Count());

            int[] expected = [int.MaxValue - 3, int.MaxValue - 2, int.MaxValue - 1, int.MaxValue];
            Assert.Equal(expected, rangeEnumerable);
        }

        [Fact]
        public void RepeatedCallsSameResults()
        {
            Assert.Equal(GetRange(-1, 2), GetRange(-1, 2));
            Assert.Equal(GetRange(0, 0), GetRange(0, 0));
        }

        [Fact]
        public void NegativeStart()
        {
            int start = -5;
            int count = 1;
            int[] expected = [-5];

            Assert.Equal(expected, GetRange(start, count));
        }

        [Fact]
        public void ArbitraryStart()
        {
            int start = 12;
            int count = 6;
            int[] expected = [12, 13, 14, 15, 16, 17];

            Assert.Equal(expected, GetRange(start, count));
        }

        [Fact]
        public void Take()
        {
            Assert.Equal(GetRange(0, 10), GetRange(0, 20).Take(10));
        }

        [Fact]
        public void TakeExcessive()
        {
            Assert.Equal(GetRange(0, 10), GetRange(0, 10).Take(int.MaxValue));
        }

        [Fact]
        public void Skip()
        {
            Assert.Equal(GetRange(10, 10).ToArray(), GetRange(0, 20).Skip(10));
        }

        [Fact]
        public void SkipExcessive()
        {
            Assert.Empty(GetRange(10, 10).Skip(20));
        }

        [Fact]
        public void SkipTakeCanOnlyBeOne()
        {
            Assert.Equal([1], GetRange(1, 10).Take(1));
            Assert.Equal([2], GetRange(1, 10).Skip(1).Take(1));
            Assert.Equal([3], GetRange(1, 10).Take(3).Skip(2));
            Assert.Equal([1], GetRange(1, 10).Take(3).Take(1));
        }

        [Fact]
        public void ElementAt()
        {
            Assert.Equal(4, GetRange(0, 10).ElementAt(4));
        }

        [Fact]
        public void ElementAtExcessiveThrows()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => GetRange(0, 10).ElementAt(100));
        }

        [Fact]
        public void ElementAtOrDefault()
        {
            Assert.Equal(4, GetRange(0, 10).ElementAtOrDefault(4));
        }

        [Fact]
        public void ElementAtOrDefaultExcessiveIsDefault()
        {
            Assert.Equal(0, GetRange(52, 10).ElementAtOrDefault(100));
        }

        [Fact]
        public void First()
        {
            Assert.Equal(57, GetRange(57, 1000000000).First());
        }

        [Fact]
        public void FirstOrDefault()
        {
            Assert.Equal(-100, GetRange(-100, int.MaxValue).FirstOrDefault());
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsLinqSpeedOptimized))]
        public void Last()
        {
            Assert.Equal(1000000056, GetRange(57, 1000000000).Last());
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsLinqSpeedOptimized))]
        public void LastOrDefault()
        {
            Assert.Equal(int.MaxValue - 101, GetRange(-100, int.MaxValue).LastOrDefault());
        }

        // IListImplementationIsValid test is removed.
        // Because cast from `ValueEnumerable<FromRange, int>` to `IList<int>`/IReadOnlyList<int> is not supported.
    }
}
