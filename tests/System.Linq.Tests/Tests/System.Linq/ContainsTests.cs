// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using Xunit;

#pragma warning disable xUnit2017

namespace System.Linq.Tests
{
    public class ContainsTests : EnumerableTests
    {
        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            var q = from x in new[] { 9999, 0, 888, -1, 66, -777, 1, 2, -12345 }
                    where x > int.MinValue
                    select x;

            Assert.Equal(q.Contains(-1), q.Contains(-1));
        }

        [Fact]
        public void SameResultsRepeatCallsStringQuery()
        {
            var q = from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", string.Empty }
                    where !string.IsNullOrEmpty(x)
                    select x;

            Assert.Equal(q.Contains("X"), q.Contains("X"));
        }

        public static IEnumerable<object[]> Int_TestData()
        {
            foreach (Func<IEnumerable<int>, IEnumerable<int>> transform in IdentityTransforms<int>())
            {
                yield return [transform(new int[0]), 6, false];
                yield return [transform([8, 10, 3, 0, -8]), 6, false];
                yield return [transform([8, 10, 3, 0, -8]), 8, true];
                yield return [transform([8, 10, 3, 0, -8]), -8, true];
                yield return [transform([8, 0, 10, 3, 0, -8, 0]), 0, true];

                yield return [transform(Enumerable.Range(0, 0)), 0, false];
                yield return [transform(Enumerable.Range(4, 5)), 3, false];
                yield return [transform(Enumerable.Range(3, 5)), 3, true];
                yield return [transform(Enumerable.Range(3, 5)), 7, true];
                yield return [transform(Enumerable.Range(10, 3)), 10, true];
            }
        }

        [Theory]
        [MemberData(nameof(Int_TestData))]
        public void Int(IEnumerable<int> source, int value, bool expected)
        {
            Assert.Equal(expected, source.Contains(value));
            Assert.Equal(expected, source.Contains(value, null));
        }

        [Theory, MemberData(nameof(Int_TestData))]
        public void IntRunOnce(IEnumerable<int> source, int value, bool expected)
        {
            Assert.Equal(expected, source.RunOnce().Contains(value));
            Assert.Equal(expected, source.RunOnce().Contains(value, null));
        }

        public static IEnumerable<object[]> String_TestData()
        {
            yield return [new string[] { null }, StringComparer.Ordinal, null, true];
            yield return [new string[] { "Bob", "Robert", "Tim" }, null, "trboeR", false];
            yield return [new string[] { "Bob", "Robert", "Tim" }, null, "Tim", true];
            yield return [new string[] { "Bob", "Robert", "Tim" }, new AnagramEqualityComparer(), "trboeR", true];
            yield return [new string[] { "Bob", "Robert", "Tim" }, new AnagramEqualityComparer(), "nevar", false];
        }

        [Theory]
        [MemberData(nameof(String_TestData))]
        public void String(IEnumerable<string> source, IEqualityComparer<string> comparer, string value, bool expected)
        {
            if (comparer is null)
            {
                Assert.Equal(expected, source.Contains(value));
            }
            Assert.Equal(expected, source.Contains(value, comparer));
        }

        [Theory, MemberData(nameof(String_TestData))]
        public void StringRunOnce(IEnumerable<string> source, IEqualityComparer<string> comparer, string value, bool expected)
        {
            if (comparer is null)
            {
                Assert.Equal(expected, source.RunOnce().Contains(value));
            }
            Assert.Equal(expected, source.RunOnce().Contains(value, comparer));
        }

        public static IEnumerable<object[]> NullableInt_TestData()
        {
            yield return [new int?[] { 8, 0, 10, 3, 0, -8, 0 }, null, false];
            yield return [new int?[] { 8, 0, 10, null, 3, 0, -8, 0 }, null, true];

            yield return [NullableNumberRangeGuaranteedNotCollectionType(3, 4), null, false];
            yield return [RepeatedNullableNumberGuaranteedNotCollectionType(null, 5), null, true];
        }

        [Theory]
        [MemberData(nameof(NullableInt_TestData))]
        public void NullableInt(IEnumerable<int?> source, int? value, bool expected)
        {
            Assert.Equal(expected, source.Contains(value));
            Assert.Equal(expected, source.Contains(value, null));
        }

        [Fact]
        public void NullSource_ThrowsArgumentNullException()
        {
            IEnumerable<int> source = null;

            AssertExtensions.Throws<ArgumentNullException>("source", () => source.Contains(42));
            AssertExtensions.Throws<ArgumentNullException>("source", () => source.Contains(42, EqualityComparer<int>.Default));
        }

        [Fact]
        public void ExplicitNullComparerDoesNotDeferToCollection()
        {
            IEnumerable<string> source = new HashSet<string>(new AnagramEqualityComparer()) { "ABC" };

            var hasValue = source.Contains("BAC", null);
            Assert.False(hasValue);
        }

        [Fact]
        public void ExplicitComparerDoesNotDeferToCollection()
        {
            IEnumerable<string> source = new HashSet<string> { "ABC" };
            Assert.Contains("abc", source, StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void ExplicitComparerDoestNotDeferToCollectionWithComparer()
        {
            IEnumerable<string> source = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "ABC" };
            Assert.Contains("BAC", source, new AnagramEqualityComparer());
        }

        [Fact]
        public void NoComparerDoesDeferToCollection()
        {
            IEnumerable<string> source = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "ABC" };
            Assert.Contains("abc", source);
        }

        [Fact]
        public void FollowingVariousOperators()
        {
            IEnumerable<int> source = Enumerable.Range(1, 3);
            foreach (var transform in IdentityTransforms<int>())
            {
                IEnumerable<int> transformedSource = transform(source);
                IEnumerable<int> transformedEmpty = transform([]);

                Assert.Contains(1, transformedSource);
                Assert.Contains(2, transformedSource);
                Assert.Contains(3, transformedSource);
                Assert.DoesNotContain(0, transformedSource);
                Assert.DoesNotContain(4, transformedSource);

                // Append/Prepend
                var ap = transformedSource.Append(4).Prepend(5).Append(6).Prepend(7);
                Assert.Contains(3, ap);
                Assert.Contains(4, ap);
                Assert.Contains(5, ap);
                Assert.Contains(6, ap);
                Assert.Contains(7, ap);
                Assert.DoesNotContain(8, ap);

                // Concat
                Assert.Contains(2, transform([4, 5, 6]).Concat(transformedSource));
                Assert.DoesNotContain(7, transform([4, 5, 6]).Concat(transformedSource));
                Assert.Contains(2, transform([4, 5, 6]).Concat(transform([7, 8, 9])).Concat(transformedSource));
                Assert.DoesNotContain(10, transform([4, 5, 6]).Concat(transform([7, 8, 9])).Concat(transformedSource));

                // DefaultIfEmpty
                Assert.Contains(1, transformedSource.DefaultIfEmpty(4));
                Assert.DoesNotContain(0, transformedEmpty.DefaultIfEmpty(4));
                Assert.Contains(4, transformedEmpty.DefaultIfEmpty(4));
                Assert.DoesNotContain(4, transformedSource.DefaultIfEmpty(4));
                Assert.DoesNotContain(4, transformedSource.DefaultIfEmpty(0));
                Assert.DoesNotContain(0, transformedSource.DefaultIfEmpty());
                Assert.Contains(0, transformedEmpty.DefaultIfEmpty());
                Assert.DoesNotContain(4, transformedSource.DefaultIfEmpty());

                // Distinct
                Assert.Contains(2, transform(source.Concat(source)).Distinct());
                Assert.DoesNotContain(4, transform(source.Concat(source)).Distinct());
                Assert.Contains(1, transform(source.Concat(source)).Distinct());
                Assert.Contains(1, transform(source.Concat(source)).Distinct(EqualityComparer<int>.Create((x, y) => true, x => 0)));
                Assert.DoesNotContain(2, transform(source.Concat(source)).Distinct(EqualityComparer<int>.Create((x, y) => true, x => 0)));
                Assert.DoesNotContain(0, transform(source.Concat(source)).Distinct(EqualityComparer<int>.Create((x, y) => true, x => 0)));

                // OrderBy
                Assert.Contains(2, transformedSource.OrderBy(x => x));
                Assert.Contains(2, transformedSource.OrderBy(x => x).ThenBy(x => x));
                Assert.DoesNotContain(4, transformedSource.OrderBy(x => x));
                Assert.DoesNotContain(4, transformedSource.OrderBy(x => x).ThenBy(x => x));

                // OrderByDescending
                Assert.Contains(2, transformedSource.OrderByDescending(x => x));
                Assert.Contains(2, transformedSource.OrderByDescending(x => x).ThenByDescending(x => x));
                Assert.DoesNotContain(4, transformedSource.OrderByDescending(x => x));
                Assert.DoesNotContain(4, transformedSource.OrderByDescending(x => x).ThenByDescending(x => x));

                // Where/Select
                Assert.Contains(2, transformedSource.Where(x => x > 1));
                Assert.DoesNotContain(2, transformedSource.Where(x => x > 3));
                Assert.Contains(6, transformedSource.Select(x => x * 2));
                Assert.DoesNotContain(3, transformedSource.Select(x => x * 2));
                Assert.Contains(4, transformedSource.Where(x => x % 2 == 0).Select(x => x * 2));
                Assert.DoesNotContain(6, transformedSource.Where(x => x % 2 == 0).Select(x => x * 2));

                // SelectMany
                Assert.Contains(2, transformedSource.SelectMany(x => new[] { x }));
                Assert.Contains(2, transformedSource.SelectMany(x => new List<int> { x, x * 2 }));
                Assert.DoesNotContain(4, transformedSource.SelectMany(x => new[] { x }));
                Assert.Contains(4, transformedSource.SelectMany(x => new List<int> { x, x * 2 }));
                Assert.DoesNotContain(5, transformedSource.SelectMany(x => new List<int> { x, x * 2 }));

                // Shuffle
                Assert.Contains(2, transformedSource.Shuffle());
                Assert.DoesNotContain(4, transformedSource.Shuffle());
                Assert.DoesNotContain(4, transformedSource.Shuffle().Take(1));
                Assert.Contains(2, transformedSource.Shuffle().Take(3));
                Assert.False(transformedSource.Shuffle().Take(1).Contains(4));
                for (int trial = 0; trial < 100 && !transformedSource.Shuffle().Take(1).Contains(3); trial++)
                {
                    if (trial == 99)
                    {
                        Assert.Fail("Shuffle().Take() didn't contain value after 100 tries. The chances of that are infinitesimal with a correct implementation.");
                    }
                }

                // Skip/Take
                Assert.True(transformedSource.Skip(2).Contains(3));
                Assert.True(transformedSource.Skip(2).Take(1).Contains(3));
                Assert.True(transformedSource.Take(1).Contains(1));
                Assert.False(transformedSource.Take(1).Contains(2));
                Assert.False(transformedSource.Take(1).Contains(2));
                Assert.False(transformedSource.Take(2).Contains(3));
                Assert.False(transformedSource.Skip(1).Take(1).Contains(1));
                Assert.True(transformedSource.Skip(1).Take(1).Contains(2));
                Assert.False(transformedSource.Skip(1).Take(1).Contains(3));

                // Union
                Assert.True(transformedSource.Union(transform([4])).Contains(4));
                Assert.True(transformedSource.Union(transform([4]), EqualityComparer<int>.Create((x, y) => true, x => 0)).Contains(1));
                Assert.False(transformedSource.Union(transform([4]), EqualityComparer<int>.Create((x, y) => true, x => 0)).Contains(4));
                Assert.False(transformedSource.Union(transform([3])).Contains(4));
            }

            // DefaultIfEmpty
            Assert.True(Enumerable.Empty<int>().DefaultIfEmpty(1).Contains(1));
            Assert.False(Enumerable.Empty<int>().DefaultIfEmpty(1).Contains(0));

            // Distinct
            Assert.True(new string[] { "a", "A" }.Distinct().Contains("a"));
            Assert.True(new string[] { "a", "A" }.Distinct().Contains("A"));
            Assert.True(new string[] { "a", "A" }.Distinct(StringComparer.OrdinalIgnoreCase).Contains("a"));
            Assert.False(new string[] { "a", "A" }.Distinct(StringComparer.OrdinalIgnoreCase).Contains("A"));

            // Repeat
            Assert.True(Enumerable.Repeat(1, 5).Contains(1));
            Assert.False(Enumerable.Repeat(1, 5).Contains(2));

            // Cast
            Assert.True(new int[] { 1, 2, 3 }.Cast<object>().Contains(2));
            Assert.True(new object[] { 1, 2, 3 }.Cast<int>().Contains(2));
            Assert.False(new object[] { 1, 2, 3 }.Cast<int>().Contains(4));

            // OfType
            Assert.True(new object[] { 1, "2", 3 }.OfType<int>().Contains(3));
            Assert.False(new object[] { 1, "2", 3 }.OfType<int>().Contains(4));
            Assert.False(new object[] { 1, "2", 3 }.OfType<int>().Contains(2));
            Assert.True(new object[] { 1, "2", 3 }.OfType<string>().Contains("2"));
            Assert.False(new object[] { 1, "2", 3 }.OfType<string>().Contains("4"));

            // Union
            Assert.True(new string[] { "a" }.Union(new string[] { "A" }).Contains("a"));
            Assert.True(new string[] { "a" }.Union(new string[] { "A" }).Contains("A"));
            Assert.True(new string[] { "a" }.Union(new string[] { "A" }, StringComparer.OrdinalIgnoreCase).Contains("a"));
            Assert.False(new string[] { "a" }.Union(new string[] { "A" }, StringComparer.OrdinalIgnoreCase).Contains("A"));
        }
    }
}
