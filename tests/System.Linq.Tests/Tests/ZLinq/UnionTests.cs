// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Collections;
using Xunit;

namespace ZLinq.Tests
{
    public class UnionTests : EnumerableTests
    {
        private sealed class Modulo100EqualityComparer : IEqualityComparer<int?>
        {
            public bool Equals(int? x, int? y)
            {
                if (!x.HasValue) return !y.HasValue;
                if (!y.HasValue) return false;
                return x.GetValueOrDefault() % 100 == y.GetValueOrDefault() % 100;
            }

            public int GetHashCode(int? obj)
            {
                return obj.HasValue ? obj.GetValueOrDefault() % 100 + 1 : 0;
            }

            public override bool Equals(object obj)
            {
                // Equal to all other instances.
                return obj is Modulo100EqualityComparer;
            }

            public override int GetHashCode()
            {
                return 0xAFFAB1E; // Any number as long as it's constant.
            }
        }

        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            var q1 = from x1 in new int?[] { 2, 3, null, 2, null, 4, 5 }
                     select x1;
            var q2 = from x2 in new int?[] { 1, 9, null, 4 }
                     select x2;

            Assert.Equal(q1.Union(q2), q1.Union(q2));
        }

        [Fact]
        public void SameResultsRepeatCallsStringQuery()
        {
            var q1 = from x1 in new[] { "AAA", string.Empty, "q", "C", "#", "!@#$%^", "0987654321", "Calling Twice" }
                     select x1;
            var q2 = from x2 in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS" }
                     select x2;

            Assert.Equal(q1.Union(q2), q1.Union(q2));
        }

        [Fact]
        public void SameResultsRepeatCallsMultipleUnions()
        {
            var q1 = from x1 in new int?[] { 2, 3, null, 2, null, 4, 5 }
                     select x1;
            var q2 = from x2 in new int?[] { 1, 9, null, 4 }
                     select x2;
            var q3 = from x3 in new int?[] { null, 8, 2, 2, 3 }
                     select x3;

            Assert.Equal(q1.Union(q2).Union(q3), q1.Union(q2).Union(q3));
        }

        [Fact]
        public void BothEmpty()
        {
            int[] first = [];
            int[] second = [];
            Assert.Empty(first.Union(second));
        }

        [Fact]
        public void ManyEmpty()
        {
            int[] first = [];
            int[] second = [];
            int[] third = [];
            int[] fourth = [];
            Assert.Empty(first.Union(second).Union(third).Union(fourth));
        }

        [Fact]
        public void CustomComparer()
        {
            string[] first = ["Bob", "Robert", "Tim", "Matt", "miT"];
            string[] second = ["ttaM", "Charlie", "Bbo"];
            string[] expected = ["Bob", "Robert", "Tim", "Matt", "Charlie"];

            var comparer = new AnagramEqualityComparer();

            Assert.Equal(expected, first.Union(second, comparer), comparer);
        }

        [Fact]
        public void RunOnce()
        {
            string[] first = ["Bob", "Robert", "Tim", "Matt", "miT"];
            string[] second = ["ttaM", "Charlie", "Bbo"];
            string[] expected = ["Bob", "Robert", "Tim", "Matt", "Charlie"];

            var comparer = new AnagramEqualityComparer();
            Assert.Equal(expected, first.RunOnce().Union(second.RunOnce(), comparer), comparer);
        }

        [Fact]
        public void FirstNullCustomComparer()
        {
            string[] first = null;
            string[] second = ["ttaM", "Charlie", "Bbo"];

            var ane = AssertExtensions.Throws<ArgumentNullException>(() => first.Union(second, new AnagramEqualityComparer()));
        }

        [Fact]
        public void SecondNullCustomComparer()
        {
            string[] first = ["Bob", "Robert", "Tim", "Matt", "miT"];
            string[] second = null;

            var ane = AssertExtensions.Throws<ArgumentNullException>("second", () => first.Union(second, new AnagramEqualityComparer()));
        }

        [Fact]
        public void FirstNullNoComparer()
        {
            string[] first = null;
            string[] second = ["ttaM", "Charlie", "Bbo"];

            var ane = AssertExtensions.Throws<ArgumentNullException>(() => first.Union(second));
        }

        [Fact]
        public void SecondNullNoComparer()
        {
            string[] first = ["Bob", "Robert", "Tim", "Matt", "miT"];
            string[] second = null;

            var ane = AssertExtensions.Throws<ArgumentNullException>("second", () => first.Union(second));
        }

        [Fact]
        public void SingleNullWithEmpty()
        {
            string[] first = [null];
            string[] second = [];
            string[] expected = [null];

            Assert.Equal(expected, first.Union(second, EqualityComparer<string>.Default));
        }

        [Fact]
        public void NullEmptyStringMix()
        {
            string[] first = [null, null, string.Empty];
            string[] second = [null, null];
            string[] expected = [null, string.Empty];

            Assert.Equal(expected, first.Union(second, EqualityComparer<string>.Default));
        }

        [Fact]
        public void DoubleNullWithEmpty()
        {
            string[] first = [null, null];
            string[] second = [];
            string[] expected = [null];

            Assert.Equal(expected, first.Union(second, EqualityComparer<string>.Default));
        }

        [Fact]
        public void EmptyWithNonEmpty()
        {
            int[] first = [];
            int[] second = [2, 4, 5, 3, 2, 3, 9];
            int[] expected = [2, 4, 5, 3, 9];

            Assert.Equal(expected, first.Union(second));
        }

        [Fact]
        public void NonEmptyWithEmpty()
        {
            int[] first = [2, 4, 5, 3, 2, 3, 9];
            int[] second = [];
            int[] expected = [2, 4, 5, 3, 9];

            Assert.Equal(expected, first.Union(second));
        }

        [Fact]
        public void CommonElementsShared()
        {
            int[] first = [1, 2, 3, 4, 5, 6];
            int[] second = [6, 7, 7, 7, 8, 1];
            int[] expected = [1, 2, 3, 4, 5, 6, 7, 8];

            Assert.Equal(expected, first.Union(second));
        }

        [Fact]
        public void SameElementRepeated()
        {
            int[] first = [1, 1, 1, 1, 1, 1];
            int[] second = [1, 1, 1, 1, 1, 1];
            int[] expected = [1];

            Assert.Equal(expected, first.Union(second));
        }

        [Fact]
        public void RepeatedElementsWithSingleElement()
        {
            int[] first = [1, 2, 3, 5, 3, 6];
            int[] second = [7];
            int[] expected = [1, 2, 3, 5, 6, 7];

            Assert.Equal(expected, first.Union(second));
        }

        [Fact]
        public void SingleWithAllUnique()
        {
            int?[] first = [2];
            int?[] second = [3, null, 4, 5];
            int?[] expected = [2, 3, null, 4, 5];

            Assert.Equal(expected, first.Union(second));
        }

        [Fact]
        public void EachHasRepeatsBetweenAndAmongstThemselves()
        {
            int?[] first = [1, 2, 3, 4, null, 5, 1];
            int?[] second = [6, 2, 3, 4, 5, 6];
            int?[] expected = [1, 2, 3, 4, null, 5, 6];

            Assert.Equal(expected, first.Union(second));
        }

        [Fact]
        public void EachHasRepeatsBetweenAndAmongstThemselvesMultipleUnions()
        {
            int?[] first = [1, 2, 3, 4, null, 5, 1];
            int?[] second = [6, 2, 3, 4, 5, 6];
            int?[] third = [2, 8, 2, 3, 2, 8];
            int?[] fourth = [null, 1, 7, 2, 7];
            int?[] expected = [1, 2, 3, 4, null, 5, 6, 8, 7];

            Assert.Equal(expected, first.Union(second).Union(third).Union(fourth));
        }

        [Fact]
        public void MultipleUnionsCustomComparer()
        {
            int?[] first = [1, 102, 903, 204, null, 5, 601];
            int?[] second = [6, 202, 903, 204, 5, 106];
            int?[] third = [2, 308, 2, 103, 802, 308];
            int?[] fourth = [null, 101, 207, 202, 207];
            int?[] expected = [1, 102, 903, 204, null, 5, 6, 308, 207];

            Assert.Equal(expected, first.Union(second, new Modulo100EqualityComparer()).Union(third, new Modulo100EqualityComparer()).Union(fourth, new Modulo100EqualityComparer()));
        }

        [Fact]
        public void MultipleUnionsDifferentComparers()
        {
            string[] first = ["Alpha", "Bravo", "Charlie", "Bravo", "Delta", "atleD", "ovarB"];
            string[] second = ["Charlie", "Delta", "Echo", "Foxtrot", "Foxtrot", "choE"];
            string[] third = ["trotFox", "Golf", "Alpha", "choE", "Tango"];

            string[] plainThenAnagram = ["Alpha", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot", "Golf", "Tango"];
            string[] anagramThenPlain = ["Alpha", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot", "trotFox", "Golf", "choE", "Tango"];

            Assert.Equal(plainThenAnagram, first.Union(second).Union(third, new AnagramEqualityComparer()));
            Assert.Equal(anagramThenPlain, first.Union(second, new AnagramEqualityComparer()).Union(third));
        }

        [Fact]
        public void NullEqualityComparer()
        {
            string[] first = ["Bob", "Robert", "Tim", "Matt", "miT"];
            string[] second = ["ttaM", "Charlie", "Bbo"];
            string[] expected = ["Bob", "Robert", "Tim", "Matt", "miT", "ttaM", "Charlie", "Bbo"];

            Assert.Equal(expected, first.Union(second, null));
        }

        [Fact(Skip = SkipReason.EnumeratorBehaviorDifference)]
        public void ForcedToEnumeratorDoesntEnumerate()
        {
            var valueEnumerable = NumberRangeGuaranteedNotCollectionType(0, 3).Union(Enumerable.Range(0, 3));
            // Don't insist on this behaviour, but check it's correct if it happens
            using var en = valueEnumerable.Enumerator;
            Assert.False(en.TryGetNext(out _));
        }

        [Fact(Skip = SkipReason.EnumeratorBehaviorDifference)]
        public void ForcedToEnumeratorDoesntEnumerateMultipleUnions()
        {
            var valueEnumerable = NumberRangeGuaranteedNotCollectionType(0, 3)
                .Union(Enumerable.Range(0, 3))
                .Union(Enumerable.Range(2, 4))
                .Union([9, 2, 4]);

            // Don't insist on this behaviour, but check it's correct if it happens
            using var en = valueEnumerable.Enumerator;
            Assert.False(en.TryGetNext(out _));
        }

        [Fact]
        public void ToArray()
        {
            string[] first = ["Bob", "Robert", "Tim", "Matt", "miT"];
            string[] second = ["ttaM", "Charlie", "Bbo"];
            string[] expected = ["Bob", "Robert", "Tim", "Matt", "miT", "ttaM", "Charlie", "Bbo"];

            Assert.Equal(expected, first.Union(second).ToArray());
        }

        [Fact]
        public void ToArrayMultipleUnion()
        {
            string[] first = ["Bob", "Robert", "Tim", "Matt", "miT"];
            string[] second = ["ttaM", "Charlie", "Bbo"];
            string[] third = ["Bob", "Albert", "Tim"];
            string[] expected = ["Bob", "Robert", "Tim", "Matt", "miT", "ttaM", "Charlie", "Bbo", "Albert"];

            Assert.Equal(expected, first.Union(second).Union(third).ToArray());
        }

        [Fact]
        public void ToList()
        {
            string[] first = ["Bob", "Robert", "Tim", "Matt", "miT"];
            string[] second = ["ttaM", "Charlie", "Bbo"];
            string[] expected = ["Bob", "Robert", "Tim", "Matt", "miT", "ttaM", "Charlie", "Bbo"];

            Assert.Equal(expected, first.Union(second).ToList());
        }

        [Fact]
        public void ToListMultipleUnion()
        {
            string[] first = ["Bob", "Robert", "Tim", "Matt", "miT"];
            string[] second = ["ttaM", "Charlie", "Bbo"];
            string[] third = ["Bob", "Albert", "Tim"];
            string[] expected = ["Bob", "Robert", "Tim", "Matt", "miT", "ttaM", "Charlie", "Bbo", "Albert"];

            Assert.Equal(expected, first.Union(second).Union(third).ToList());
        }

        [Fact]
        public void Count()
        {
            string[] first = ["Bob", "Robert", "Tim", "Matt", "miT"];
            string[] second = ["ttaM", "Charlie", "Bbo"];

            Assert.Equal(8, first.Union(second).Count());
        }
        [Fact]
        public void CountMultipleUnion()
        {
            string[] first = ["Bob", "Robert", "Tim", "Matt", "miT"];
            string[] second = ["ttaM", "Charlie", "Bbo"];
            string[] third = ["Bob", "Albert", "Tim"];

            Assert.Equal(9, first.Union(second).Union(third).Count());
        }

        [Fact]
        public void RepeatEnumerating()
        {
            string[] first = ["Bob", "Robert", "Tim", "Matt", "miT"];
            string[] second = ["ttaM", "Charlie", "Bbo"];

            var result = first.Union(second);

            Assert.Equal(result, result);
        }

        [Fact]
        public void RepeatEnumeratingMultipleUnions()
        {
            string[] first = ["Bob", "Robert", "Tim", "Matt", "miT"];
            string[] second = ["ttaM", "Charlie", "Bbo"];
            string[] third = ["Matt", "Albert", "Ichabod"];

            var result = first.Union(second).Union(third);

            Assert.Equal(result, result);
        }

        [Fact]
        public void HashSetWithBuiltInComparer_HashSetContainsNotUsed()
        {
            IEnumerable<string> input1 = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "a" };
            IEnumerable<string> input2 = ["A"];

            Assert.Equal(["a", "A"], input1.Union(input2));
            Assert.Equal(["a", "A"], input1.Union(input2, null));
            Assert.Equal(["a", "A"], input1.Union(input2, EqualityComparer<string>.Default));
            Assert.Equal(["a"], input1.Union(input2, StringComparer.OrdinalIgnoreCase));

            Assert.Equal(["A", "a"], input2.Union(input1));
            Assert.Equal(["A", "a"], input2.Union(input1, null));
            Assert.Equal(["A", "a"], input2.Union(input1, EqualityComparer<string>.Default));
            Assert.Equal(["A"], input2.Union(input1, StringComparer.OrdinalIgnoreCase));
        }

        [Fact]
        public void UnionBy_FirstNull_ThrowsArgumentNullException()
        {
            string[] first = null;
            string[] second = ["bBo", "shriC"];

            AssertExtensions.Throws<ArgumentNullException>(() => first.UnionBy(second, x => x));
            AssertExtensions.Throws<ArgumentNullException>(() => first.UnionBy(second, x => x, new AnagramEqualityComparer()));
        }

        [Fact]
        public void UnionBy_SecondNull_ThrowsArgumentNullException()
        {
            string[] first = ["Bob", "Tim", "Robert", "Chris"];
            string[] second = null;

            AssertExtensions.Throws<ArgumentNullException>("second", () => first.UnionBy(second, x => x));
            AssertExtensions.Throws<ArgumentNullException>("second", () => first.UnionBy(second, x => x, new AnagramEqualityComparer()));
        }

        [Fact]
        public void UnionBy_KeySelectorNull_ThrowsArgumentNullException()
        {
            string[] first = ["Bob", "Tim", "Robert", "Chris"];
            string[] second = ["bBo", "shriC"];
            Func<string, string> keySelector = null;

            AssertExtensions.Throws<ArgumentNullException>("keySelector", () => first.UnionBy(second, keySelector));
            AssertExtensions.Throws<ArgumentNullException>("keySelector", () => first.UnionBy(second, keySelector, new AnagramEqualityComparer()));
        }

        [Theory]
        [MemberData(nameof(UnionBy_TestData))]
        public static void UnionBy_HasExpectedOutput<TSource, TKey>(IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer, IEnumerable<TSource> expected)
        {
            Assert.Equal(expected, first.UnionBy(second, keySelector, comparer));
        }

        [Theory]
        [MemberData(nameof(UnionBy_TestData))]
        public static void UnionBy_RunOnce_HasExpectedOutput<TSource, TKey>(IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer, IEnumerable<TSource> expected)
        {
            Assert.Equal(expected, first.RunOnce().UnionBy(second.RunOnce(), keySelector, comparer));
        }

        public static IEnumerable<object[]> UnionBy_TestData()
        {
            yield return WrapArgs(
                first: Enumerable.Range(0, 7),
                second: Enumerable.Range(3, 7),
                keySelector: x => x,
                comparer: null,
                expected: Enumerable.Range(0, 10));

            yield return WrapArgs(
                first: Enumerable.Range(0, 10),
                second: Enumerable.Range(10, 10),
                keySelector: x => x,
                comparer: null,
                expected: Enumerable.Range(0, 20));

            yield return WrapArgs(
                first: [],
                second: Enumerable.Range(0, 5),
                keySelector: x => x,
                comparer: null,
                expected: Enumerable.Range(0, 5));

            yield return WrapArgs(
                first: Enumerable.Repeat(5, 20),
                second: [],
                keySelector: x => x,
                comparer: null,
                expected: Enumerable.Repeat(5, 1));

            yield return WrapArgs(
                first: Enumerable.Repeat(5, 20),
                second: Enumerable.Repeat(5, 3),
                keySelector: x => x,
                comparer: null,
                expected: Enumerable.Repeat(5, 1));

            yield return WrapArgs(
                first: ["Bob", "Tim", "Robert", "Chris"],
                second: ["bBo", "shriC"],
                keySelector: x => x,
                null,
                expected: ["Bob", "Tim", "Robert", "Chris", "bBo", "shriC"]);

            yield return WrapArgs(
                first: ["Bob", "Tim", "Robert", "Chris"],
                second: ["bBo", "shriC"],
                keySelector: x => x,
                new AnagramEqualityComparer(),
                expected: ["Bob", "Tim", "Robert", "Chris"]);

            yield return WrapArgs(
                first: new (string Name, int Age)[] { ("Tom", 20), ("Dick", 30), ("Harry", 40), ("Martin", 20) },
                second: new (string Name, int Age)[] { ("Peter", 21), ("John", 30), ("Toby", 33) },
                keySelector: x => x.Age,
                comparer: null,
                expected: new (string Name, int Age)[] { ("Tom", 20), ("Dick", 30), ("Harry", 40), ("Peter", 21), ("Toby", 33) });

            yield return WrapArgs(
                first: new (string Name, int Age)[] { ("Tom", 20), ("Dick", 30), ("Harry", 40), ("Martin", 20) },
                second: new (string Name, int Age)[] { ("Toby", 33), ("Harry", 35), ("tom", 67) },
                keySelector: x => x.Name,
                comparer: null,
                expected: new (string Name, int Age)[] { ("Tom", 20), ("Dick", 30), ("Harry", 40), ("Martin", 20), ("Toby", 33), ("tom", 67) });

            yield return WrapArgs(
                first: new (string Name, int Age)[] { ("Tom", 20), ("Dick", 30), ("Harry", 40), ("Martin", 20) },
                second: new (string Name, int Age)[] { ("Toby", 33), ("Harry", 35), ("tom", 67) },
                keySelector: x => x.Name,
                comparer: StringComparer.OrdinalIgnoreCase,
                expected: new (string Name, int Age)[] { ("Tom", 20), ("Dick", 30), ("Harry", 40), ("Martin", 20), ("Toby", 33) });

            object[] WrapArgs<TSource, TKey>(IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer, IEnumerable<TSource> expected)
                => [first, second, keySelector, comparer, expected];
        }
    }
}
