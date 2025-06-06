// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xunit;

namespace ZLinq.Tests
{
    public class SelectTests : EnumerableTests
    {
        [Fact]
        public void SameResultsRepeatCallsStringQuery()
        {
            var q1 = from x1 in new string[] { "Alen", "Felix", null, null, "X", "Have Space", "Clinton", "" }
                     select x1;

            var q = from x3 in q1
                    from x4 in (from x2 in new int[] { 55, 49, 9, -100, 24, 25, -1, 0 } select x2)
                    select new { a1 = x3, a2 = x4 };

            Assert.Equal(q.Select(e => e.a1), q.Select(e => e.a1));
        }

        [Fact]
        public void SingleElement()
        {
            var source = new[]
            {
                new  { name = "Prakash", custID = 98088 }
            };
            string[] expected = ["Prakash"];

            Assert.Equal(expected, source.Select(e => e.name));
        }

        [Fact]
        public void SelectProperty()
        {
            var source = new[]{
                new { name="Prakash", custID=98088 },
                new { name="Bob", custID=29099 },
                new { name="Chris", custID=39033 },
                new { name=(string)null, custID=30349 },
                new { name="Prakash", custID=39030 }
            };
            string[] expected = ["Prakash", "Bob", "Chris", null, "Prakash"];
            Assert.Equal(expected, source.Select(e => e.name));
        }

        [Fact]
        public void RunOnce()
        {
            var source = new[]{
                new { name="Prakash", custID=98088 },
                new { name="Bob", custID=29099 },
                new { name="Chris", custID=39033 },
                new { name=(string)null, custID=30349 },
                new { name="Prakash", custID=39030 }
            };
            string[] expected = ["Prakash", "Bob", "Chris", null, "Prakash"];
            Assert.Equal(expected, source.RunOnce().Select(e => e.name));
            Assert.Equal(expected, source.ToArray().RunOnce().Select(e => e.name));
            Assert.Equal(expected, source.ToList().RunOnce().Select(e => e.name));
        }

        [Fact]
        public void EmptyWithIndexedSelector()
        {
            Assert.Equal([], Enumerable.Empty<string>().Select((s, i) => s.Length + i));
        }

        [Fact(Skip = SkipReason.RefStruct)]
        public void EnumerateFromDifferentThread()
        {
            var selected = Enumerable.Range(0, 100).Where(i => i > 3).Select(i => i.ToString());
            Task[] tasks = new Task[4];

            //for (int i = 0; i != 4; ++i)
            //    tasks[i] = Task.Run(() => selected.ToList()); // CS8175: Cannot use ref local 'selected' inside an anonymous method, lambda expression, or query expression
            //Task.WaitAll(tasks);
        }

        [Fact]
        public void SingleElementIndexedSelector()
        {
            var source = new[]
            {
                new  { name = "Prakash", custID = 98088 }
            };
            string[] expected = ["Prakash"];

            Assert.Equal(expected, source.Select((e, index) => e.name));
        }

        [Fact]
        public void SelectPropertyPassingIndex()
        {
            var source = new[]{
                new { name="Prakash", custID=98088 },
                new { name="Bob", custID=29099 },
                new { name="Chris", custID=39033 },
                new { name=(string)null, custID=30349 },
                new { name="Prakash", custID=39030 }
            };
            string[] expected = ["Prakash", "Bob", "Chris", null, "Prakash"];
            Assert.Equal(expected, source.Select((e, i) => e.name));
        }

        [Fact]
        public void SelectPropertyUsingIndex()
        {
            var source = new[]{
                new { name="Prakash", custID=98088 },
                new { name="Bob", custID=29099 },
                new { name="Chris", custID=39033 }
            };
            string[] expected = ["Prakash", null, null];
            Assert.Equal(expected, source.Select((e, i) => i == 0 ? e.name : null));
        }

        [Fact]
        public void SelectPropertyPassingIndexOnLast()
        {
            var source = new[]{
                new { name="Prakash", custID=98088},
                new { name="Bob", custID=29099 },
                new { name="Chris", custID=39033 },
                new { name="Robert", custID=39033 },
                new { name="Allen", custID=39033 },
                new { name="Chuck", custID=39033 }
            };
            string[] expected = [null, null, null, null, null, "Chuck"];
            Assert.Equal(expected, source.Select((e, i) => i == 5 ? e.name : null));
        }

        [ConditionalFact(typeof(TestEnvironment), nameof(TestEnvironment.IsStressModeEnabled))]
        public void Overflow()
        {
            Assert.Throws<OverflowException>(() =>
            {
                var selected = new FastInfiniteEnumerator<int>().Select((e, i) => e);
                using var en = selected.GetEnumerator();
                while (en.MoveNext())
                {
                }
            });
        }

        [Fact]
        public void Select_SourceIsNull_ArgumentNullExceptionThrown()
        {
            IEnumerable<int> source = null;
            Func<int, int> selector = i => i + 1;

            AssertExtensions.Throws<ArgumentNullException>("source", () => source.Select(selector));
        }

        [Fact]
        public void Select_SelectorIsNull_ArgumentNullExceptionThrown_Indexed()
        {
            IEnumerable<int> source = Enumerable.Range(1, 10);
            Func<int, int, int> selector = null;

            AssertExtensions.Throws<ArgumentNullException>("selector", () => source.Select(selector));
        }

        [Fact]
        public void Select_SourceIsNull_ArgumentNullExceptionThrown_Indexed()
        {
            IEnumerable<int> source = null;
            Func<int, int, int> selector = (e, i) => i + 1;

            AssertExtensions.Throws<ArgumentNullException>("source", () => source.Select(selector));
        }

        [Fact]
        public void Select_SelectorIsNull_ArgumentNullExceptionThrown()
        {
            IEnumerable<int> source = Enumerable.Range(1, 10);
            Func<int, int> selector = null;

            AssertExtensions.Throws<ArgumentNullException>("selector", () => source.Select(selector));
        }
        [Fact]
        public void Select_SourceIsAnArray_ExecutionIsDeferred()
        {
            bool funcCalled = false;
            Func<int>[] source = [() => { funcCalled = true; return 1; }];

            var query = source.Select(d => d());
            Assert.False(funcCalled);
        }

        [Fact]
        public void Select_SourceIsAList_ExecutionIsDeferred()
        {
            bool funcCalled = false;
            List<Func<int>> source = new List<Func<int>>() { () => { funcCalled = true; return 1; } };

            var query = source.Select(d => d());
            Assert.False(funcCalled);
        }

        [Fact]
        public void Select_SourceIsIReadOnlyCollection_ExecutionIsDeferred()
        {
            bool funcCalled = false;
            IReadOnlyCollection<Func<int>> source = new ReadOnlyCollection<Func<int>>(new List<Func<int>>() { () => { funcCalled = true; return 1; } });

            var query = source.Select(d => d());
            Assert.False(funcCalled);
        }

        [Fact]
        public void Select_SourceIsICollection_ExecutionIsDeferred()
        {
            bool funcCalled = false;
            ICollection<Func<int>> source = new LinkedList<Func<int>>(new List<Func<int>>() { () => { funcCalled = true; return 1; } });

            var query = source.Select(d => d());
            Assert.False(funcCalled);
        }

        [Fact]
        public void Select_SourceIsIEnumerable_ExecutionIsDeferred()
        {
            bool funcCalled = false;
            IEnumerable<Func<int>> source = Enumerable.Repeat((Func<int>)(() => { funcCalled = true; return 1; }), 1);

            var query = source.Select(d => d());
            Assert.False(funcCalled);
        }

        [Fact]
        public void SelectSelect_SourceIsAnArray_ExecutionIsDeferred()
        {
            bool funcCalled = false;
            Func<int>[] source = [() => { funcCalled = true; return 1; }];

            var query = source.Select(d => d).Select(d => d());
            Assert.False(funcCalled);
        }

        [Fact]
        public void SelectSelect_SourceIsAList_ExecutionIsDeferred()
        {
            bool funcCalled = false;
            List<Func<int>> source = new List<Func<int>>() { () => { funcCalled = true; return 1; } };

            var query = source.Select(d => d).Select(d => d());
            Assert.False(funcCalled);
        }

        [Fact]
        public void SelectSelect_SourceIsIReadOnlyCollection_ExecutionIsDeferred()
        {
            bool funcCalled = false;
            IReadOnlyCollection<Func<int>> source = new ReadOnlyCollection<Func<int>>(new List<Func<int>>() { () => { funcCalled = true; return 1; } });

            var query = source.Select(d => d).Select(d => d());
            Assert.False(funcCalled);
        }

        [Fact]
        public void SelectSelect_SourceIsICollection_ExecutionIsDeferred()
        {
            bool funcCalled = false;
            ICollection<Func<int>> source = new LinkedList<Func<int>>(new List<Func<int>>() { () => { funcCalled = true; return 1; } });

            var query = source.Select(d => d).Select(d => d());
            Assert.False(funcCalled);
        }

        [Fact]
        public void SelectSelect_SourceIsIEnumerable_ExecutionIsDeferred()
        {
            bool funcCalled = false;
            IEnumerable<Func<int>> source = Enumerable.Repeat((Func<int>)(() => { funcCalled = true; return 1; }), 1);

            var query = source.Select(d => d).Select(d => d());
            Assert.False(funcCalled);
        }

        [Fact]
        public void Select_SourceIsAnArray_ReturnsExpectedValues()
        {
            int[] source = [1, 2, 3, 4, 5];
            Func<int, int> selector = i => i + 1;

            var query = source.Select(selector);

            int index = 0;
            foreach (var item in query)
            {
                var expected = selector(source[index]);
                Assert.Equal(expected, item);
                index++;
            }

            Assert.Equal(source.Length, index);
        }

        [Fact]
        public void Select_SourceIsAList_ReturnsExpectedValues()
        {
            List<int> source = [1, 2, 3, 4, 5];
            Func<int, int> selector = i => i + 1;

            var query = source.Select(selector);

            int index = 0;
            foreach (var item in query)
            {
                var expected = selector(source[index]);
                Assert.Equal(expected, item);
                index++;
            }

            Assert.Equal(source.Count, index);
        }

        [Fact]
        public void Select_SourceIsIReadOnlyCollection_ReturnsExpectedValues()
        {
            IReadOnlyCollection<int> source = new ReadOnlyCollection<int>(new List<int> { 1, 2, 3, 4, 5 });
            Func<int, int> selector = i => i + 1;

            var query = source.Select(selector);

            int index = 0;
            foreach (var item in query)
            {
                index++;
                var expected = selector(index);
                Assert.Equal(expected, item);
            }

            Assert.Equal(source.Count, index);
        }

        [Fact]
        public void Select_SourceIsICollection_ReturnsExpectedValues()
        {
            ICollection<int> source = new LinkedList<int>(new List<int> { 1, 2, 3, 4, 5 });
            Func<int, int> selector = i => i + 1;

            var query = source.Select(selector);

            int index = 0;
            foreach (var item in query)
            {
                index++;
                var expected = selector(index);
                Assert.Equal(expected, item);
            }

            Assert.Equal(source.Count, index);
        }

        [Fact]
        public void Select_SourceIsIEnumerable_ReturnsExpectedValues()
        {
            int nbOfItems = 5;
            IEnumerable<int> source = Enumerable.Range(1, nbOfItems);
            Func<int, int> selector = i => i + 1;

            var query = source.Select(selector);

            int index = 0;
            foreach (var item in query)
            {
                index++;
                var expected = selector(index);
                Assert.Equal(expected, item);
            }

            Assert.Equal(nbOfItems, index);
        }

        [Fact]
        public void Select_SourceIsAnArray_CurrentIsDefaultOfTAfterEnumeration()
        {
            int[] source = [1];
            Func<int, int> selector = i => i + 1;

            var query = source.Select(selector);

            using var enumerator = query.GetEnumerator();
            while (enumerator.MoveNext()) ;

            Assert.Equal(default(int), enumerator.Current);
        }

        [Fact]
        public void Select_SourceIsAList_CurrentIsDefaultOfTAfterEnumeration()
        {
            List<int> source = [1];
            Func<int, int> selector = i => i + 1;

            var query = source.Select(selector);

            using var enumerator = query.GetEnumerator();
            while (enumerator.MoveNext()) ;

            Assert.Equal(default(int), enumerator.Current);
        }

        [Fact]
        public void Select_SourceIsIReadOnlyCollection_CurrentIsDefaultOfTAfterEnumeration()
        {
            IReadOnlyCollection<int> source = new ReadOnlyCollection<int>(new List<int>() { 1 });
            Func<int, int> selector = i => i + 1;

            var query = source.Select(selector);

            using var enumerator = query.GetEnumerator();
            while (enumerator.MoveNext()) ;

            Assert.Equal(default(int), enumerator.Current);
        }

        [Fact]
        public void Select_SourceIsICollection_CurrentIsDefaultOfTAfterEnumeration()
        {
            ICollection<int> source = new LinkedList<int>(new List<int>() { 1 });
            Func<int, int> selector = i => i + 1;

            var query = source.Select(selector);

            using var enumerator = query.GetEnumerator();
            while (enumerator.MoveNext()) ;

            Assert.Equal(default(int), enumerator.Current);
        }

        [Fact]
        public void Select_SourceIsIEnumerable_CurrentIsDefaultOfTAfterEnumeration()
        {
            var source = Enumerable.Repeat(1, 1);
            Func<int, int> selector = i => i + 1;

            var query = source.Select(selector);

            using var enumerator = query.GetEnumerator();
            while (enumerator.MoveNext()) ;

            Assert.Equal(default(int), enumerator.Current);
        }

        [Fact]
        public void SelectSelect_SourceIsAnArray_ReturnsExpectedValues()
        {
            Func<int, int> selector = i => i + 1;
            int[] source = [1, 2, 3, 4, 5];

            var query = source.Select(selector).Select(selector);

            int index = 0;
            foreach (var item in query)
            {
                var expected = selector(selector(source[index]));
                Assert.Equal(expected, item);
                index++;
            }

            Assert.Equal(source.Length, index);
        }

        [Fact]
        public void SelectSelect_SourceIsAList_ReturnsExpectedValues()
        {
            List<int> source = new List<int> { 1, 2, 3, 4, 5 };
            Func<int, int> selector = i => i + 1;

            var query = source.Select(selector).Select(selector);

            int index = 0;
            foreach (var item in query)
            {
                var expected = selector(selector(source[index]));
                Assert.Equal(expected, item);
                index++;
            }

            Assert.Equal(source.Count, index);
        }

        [Fact]
        public void SelectSelect_SourceIsIReadOnlyCollection_ReturnsExpectedValues()
        {
            IReadOnlyCollection<int> source = new ReadOnlyCollection<int>(new List<int> { 1, 2, 3, 4, 5 });
            Func<int, int> selector = i => i + 1;

            var query = source.Select(selector).Select(selector);

            int index = 0;
            foreach (var item in query)
            {
                index++;
                var expected = selector(selector(index));
                Assert.Equal(expected, item);
            }

            Assert.Equal(source.Count, index);
        }

        [Fact]
        public void SelectSelect_SourceIsICollection_ReturnsExpectedValues()
        {
            ICollection<int> source = new LinkedList<int>(new List<int> { 1, 2, 3, 4, 5 });
            Func<int, int> selector = i => i + 1;

            var query = source.Select(selector).Select(selector);

            int index = 0;
            foreach (var item in query)
            {
                index++;
                var expected = selector(selector(index));
                Assert.Equal(expected, item);
            }

            Assert.Equal(source.Count, index);
        }

        [Fact]
        public void SelectSelect_SourceIsIEnumerable_ReturnsExpectedValues()
        {
            int nbOfItems = 5;
            IEnumerable<int> source = Enumerable.Range(1, 5);
            Func<int, int> selector = i => i + 1;

            var query = source.Select(selector).Select(selector);

            int index = 0;
            foreach (var item in query)
            {
                index++;
                var expected = selector(selector(index));
                Assert.Equal(expected, item);
            }

            Assert.Equal(nbOfItems, index);
        }

        [Fact]
        public void Select_SourceIsEmptyEnumerable_ReturnedCollectionHasNoElements()
        {
            IEnumerable<int> source = [];
            bool wasSelectorCalled = false;

            var result = source.Select(i => { wasSelectorCalled = true; return i + 1; });

            bool hadItems = false;
            foreach (var item in result)
            {
                hadItems = true;
            }

            Assert.False(hadItems);
            Assert.False(wasSelectorCalled);
        }

        [Fact]
        public void Select_ExceptionThrownFromSelector_ExceptionPropagatedToTheCaller()
        {
            int[] source = [1, 2, 3, 4, 5];
            Func<int, int> selector = i => { throw new InvalidOperationException(); };

            Assert.Throws<InvalidOperationException>(() =>
            {
                var result = source.Select(selector);
                using var enumerator = result.GetEnumerator();
                enumerator.MoveNext();
            });
        }

        [Fact]
        public void Select_ExceptionThrownFromSelector_IteratorCanBeUsedAfterExceptionIsCaught()
        {
            int[] source = [1, 2, 3, 4, 5];
            Func<int, int> selector = i =>
            {
                if (i == 1)
                    throw new InvalidOperationException();
                return i + 1;
            };

            var result = source.Select(selector);
            using var enumerator = result.GetEnumerator();

            try
            {
                enumerator.MoveNext();
            }
            catch (Exception ex)
            {
                ex.ShouldBeOfType<InvalidOperationException>();
            }

            enumerator.MoveNext();
            Assert.Equal(3 /* 2 + 1 */, enumerator.Current);
        }

        [Fact]
        public void Select_ExceptionThrownFromCurrentOfSourceIterator_ExceptionPropagatedToTheCaller()
        {
            IEnumerable<int> source = new ThrowsOnCurrent();
            Func<int, int> selector = i => i + 1;

            Assert.Throws<InvalidOperationException>(() =>
            {
                var result = source.Select(selector);
                using var enumerator = result.GetEnumerator();
                enumerator.MoveNext();
            });
        }

        [Fact]
        public void Select_ExceptionThrownFromCurrentOfSourceIterator_IteratorCanBeUsedAfterExceptionIsCaught()
        {
            IEnumerable<int> source = new ThrowsOnCurrent();
            Func<int, int> selector = i => i + 1;

            var result = source.Select(selector);
            using var enumerator = result.GetEnumerator();

            try
            {
                enumerator.MoveNext();
            }
            catch (Exception ex)
            {
                ex.ShouldBeOfType<InvalidOperationException>();
            }
            enumerator.MoveNext();
            Assert.Equal(3 /* 2 + 1 */, enumerator.Current);
        }

        /// <summary>
        /// Test enumerator - throws InvalidOperationException from Current after MoveNext called once.
        /// </summary>
        private class ThrowsOnCurrent : TestEnumerator
        {
            public override int Current
            {
                get
                {
                    var current = base.Current;
                    if (current == 1)
                        throw new InvalidOperationException();
                    return current;
                }
            }
        }

        [Fact]
        public void Select_ExceptionThrownFromMoveNextOfSourceIterator_ExceptionPropagatedToTheCaller()
        {
            IEnumerable<int> source = new ThrowsOnMoveNext();
            Func<int, int> selector = i => i + 1;

            Assert.Throws<InvalidOperationException>(() =>
            {
                var result = source.Select(selector);
                using var enumerator = result.GetEnumerator();
                enumerator.MoveNext();
            });
        }

        [Fact]
        public void Select_ExceptionThrownFromMoveNextOfSourceIterator_IteratorCanBeUsedAfterExceptionIsCaught()
        {
            IEnumerable<int> source = new ThrowsOnMoveNext();
            Func<int, int> selector = i => i + 1;

            var result = source.Select(selector);
            using var enumerator = result.GetEnumerator();

            try
            {
                enumerator.MoveNext();
            }
            catch (Exception ex)
            {
                ex.ShouldBeOfType<InvalidOperationException>();
            }
            enumerator.MoveNext();
            Assert.Equal(3 /* 2 + 1 */, enumerator.Current);
        }

        [Fact]
        public void Select_ExceptionThrownFromGetEnumeratorOnSource_ExceptionPropagatedToTheCaller()
        {
            IEnumerable<int> source = new ThrowsOnGetEnumerator();
            Func<int, int> selector = i => i + 1;

            Assert.Throws<InvalidOperationException>(() =>
            {
                var result = source.Select(selector);
                using var enumerator = result.GetEnumerator();
                enumerator.MoveNext();
            });
        }

        [Fact]
        public void Select_ExceptionThrownFromGetEnumeratorOnSource_CurrentIsSetToDefaultOfItemTypeAndIteratorCanBeUsedAfterExceptionIsCaught()
        {
            IEnumerable<int> source = new ThrowsOnGetEnumerator();
            Func<int, string> selector = i => i.ToString();

            var result = source.Select(selector);
            using var enumerator = result.GetEnumerator();

            try
            {
                enumerator.MoveNext();
            }
            catch (Exception ex)
            {
                ex.ShouldBeOfType<InvalidOperationException>();
            }

            string currentValue = enumerator.Current;
            Assert.Equal(default(string), currentValue);

            Assert.True(enumerator.MoveNext());
            Assert.Equal("1", enumerator.Current);
        }

        [Fact]
        public void Select_SourceListGetsModifiedDuringIteration_ExceptionIsPropagated()
        {
            List<int> source = new List<int>() { 1, 2, 3, 4, 5 };
            Func<int, int> selector = i => i + 1;

            var result = source.Select(selector);
            using var enumerator = result.GetEnumerator();

            Assert.True(enumerator.MoveNext());
            Assert.Equal(2 /* 1 + 1 */, enumerator.Current);

            source.Add(6);
            try
            {
                enumerator.MoveNext();
            }
            catch (Exception ex)
            {
                ex.ShouldBeOfType<InvalidOperationException>();
            }
        }

        [Fact(Skip = SkipReason.RefStruct)]
        public void Select_GetEnumeratorCalledTwice_DifferentInstancesReturned()
        {
            int[] source = [1, 2, 3, 4, 5];
            var query = source.Select(i => i + 1);

            using var enumerator1 = query.GetEnumerator();
            using var enumerator2 = query.GetEnumerator();

            // ZLinq use ref struct.
            //Assert.Same(query, enumerator1);
            //Assert.NotSame(enumerator1, enumerator2);

            enumerator1.Dispose();
            enumerator2.Dispose();
        }

        [Fact(Skip = SkipReason.EnumeratorReset)]
        public void Select_ResetCalledOnEnumerator_ThrowsException()
        {
            int[] source = [1, 2, 3, 4, 5];
            Func<int, int> selector = i => i + 1;

            Assert.Throws<NotSupportedException>(() =>
            {
                var result = source.Select(selector);
                using var enumerator = result.GetEnumerator();
                // enumerator.Reset(); // ZLinq don't support Reset()
            });
        }

        [Fact(Skip = SkipReason.EnumeratorBehaviorDifference)]
        public void ForcedToEnumeratorDoesntEnumerate()
        {
            var valueEnumerable = NumberRangeGuaranteedNotCollectionType(0, 3).Select(i => i);
            // Don't insist on this behaviour, but check it's correct if it happens
            var en = valueEnumerable.Enumerator;
            Assert.False(en.TryGetNext(out _));
        }

        [Fact(Skip = SkipReason.EnumeratorBehaviorDifference)]
        public void ForcedToEnumeratorDoesntEnumerateIndexed()
        {
            var valueEnumerable = NumberRangeGuaranteedNotCollectionType(0, 3).Select((e, i) => i);
            // Don't insist on this behaviour, but check it's correct if it happens
            using var en = valueEnumerable.Enumerator;
            Assert.False(en.TryGetNext(out _));
        }

        [Fact(Skip = SkipReason.EnumeratorBehaviorDifference)]
        public void ForcedToEnumeratorDoesntEnumerateArray()
        {
            var valueEnumerable = NumberRangeGuaranteedNotCollectionType(0, 3).ToArray().Select(i => i);
            using var en = valueEnumerable.Enumerator;
            Assert.False(en.TryGetNext(out _));
        }

        [Fact(Skip = SkipReason.EnumeratorBehaviorDifference)]
        public void ForcedToEnumeratorDoesntEnumerateList()
        {
            var valueEnumerable = NumberRangeGuaranteedNotCollectionType(0, 3).ToList().Select(i => i);
            using var en = valueEnumerable.Enumerator;
            Assert.False(en.TryGetNext(out _));
        }

        [Fact(Skip = SkipReason.EnumeratorBehaviorDifference)]
        public void ForcedToEnumeratorDoesntEnumerateIList()
        {
            var valueEnumerable = NumberRangeGuaranteedNotCollectionType(0, 3).ToList().AsReadOnly().Select(i => i);
            using var en = valueEnumerable.Enumerator;
            Assert.False(en.TryGetNext(out _));
        }

        [Fact(Skip = SkipReason.EnumeratorBehaviorDifference)]
        public void ForcedToEnumeratorDoesntEnumerateIPartition()
        {
            var valueEnumerable = NumberRangeGuaranteedNotCollectionType(0, 3).ToList().AsReadOnly().Select(i => i).Skip(1);
            using var en = valueEnumerable.Enumerator;
            Assert.False(en.TryGetNext(out _));
        }

        [Fact]
        public void Select_SourceIsArray_Count()
        {
            var source = new[] { 1, 2, 3, 4 };
            Assert.Equal(source.Length, source.Select(i => i * 2).Count());
        }

        [Fact]
        public void Select_SourceIsAList_Count()
        {
            var source = new List<int> { 1, 2, 3, 4 };
            Assert.Equal(source.Count, source.Select(i => i * 2).Count());
        }

        [Fact]
        public void Select_SourceIsAnIList_Count()
        {
            var source = new List<int> { 1, 2, 3, 4 }.AsReadOnly();
            Assert.Equal(source.Count, source.Select(i => i * 2).Count());
        }

        [Fact]
        public void Select_SourceIsArray_Skip()
        {
            var source = new[] { 1, 2, 3, 4 }.Select(i => i * 2);
            Assert.Equal([6, 8], source.Skip(2));
            Assert.Equal([6, 8], source.Skip(2).Skip(-1));
            Assert.Equal([6, 8], source.Skip(1).Skip(1));
            Assert.Equal([2, 4, 6, 8], source.Skip(-1));
            Assert.Empty(source.Skip(4));
            Assert.Empty(source.Skip(20));
        }

        [Fact]
        public void Select_SourceIsList_Skip()
        {
            var source = new List<int> { 1, 2, 3, 4 }.Select(i => i * 2);
            Assert.Equal([6, 8], source.Skip(2));
            Assert.Equal([6, 8], source.Skip(2).Skip(-1));
            Assert.Equal([6, 8], source.Skip(1).Skip(1));
            Assert.Equal([2, 4, 6, 8], source.Skip(-1));
            Assert.Empty(source.Skip(4));
            Assert.Empty(source.Skip(20));
        }

        [Fact]
        public void Select_SourceIsIList_Skip()
        {
            var source = new List<int> { 1, 2, 3, 4 }.AsReadOnly().Select(i => i * 2);
            Assert.Equal([6, 8], source.Skip(2));
            Assert.Equal([6, 8], source.Skip(2).Skip(-1));
            Assert.Equal([6, 8], source.Skip(1).Skip(1));
            Assert.Equal([2, 4, 6, 8], source.Skip(-1));
            Assert.Empty(source.Skip(4));
            Assert.Empty(source.Skip(20));
        }

        [Fact]
        public void Select_SourceIsArray_Take()
        {
            var source = new[] { 1, 2, 3, 4 }.Select(i => i * 2);
            Assert.Equal([2, 4], source.Take(2));
            Assert.Equal([2, 4], source.Take(3).Take(2));
            Assert.Empty(source.Take(-1));
            Assert.Equal([2, 4, 6, 8], source.Take(4));
            Assert.Equal([2, 4, 6, 8], source.Take(40));
            Assert.Equal([2], source.Take(1));
            Assert.Equal([4], source.Skip(1).Take(1));
            Assert.Equal([6], source.Take(3).Skip(2));
            Assert.Equal([2], source.Take(3).Take(1));
        }

        [Fact]
        public void Select_SourceIsList_Take()
        {
            var source = new List<int> { 1, 2, 3, 4 }.Select(i => i * 2);
            Assert.Equal([2, 4], source.Take(2));
            Assert.Equal([2, 4], source.Take(3).Take(2));
            Assert.Empty(source.Take(-1));
            Assert.Equal([2, 4, 6, 8], source.Take(4));
            Assert.Equal([2, 4, 6, 8], source.Take(40));
            Assert.Equal([2], source.Take(1));
            Assert.Equal([4], source.Skip(1).Take(1));
            Assert.Equal([6], source.Take(3).Skip(2));
            Assert.Equal([2], source.Take(3).Take(1));
        }

        [Fact]
        public void Select_SourceIsIList_Take()
        {
            var source = new List<int> { 1, 2, 3, 4 }.AsReadOnly().Select(i => i * 2);
            Assert.Equal([2, 4], source.Take(2));
            Assert.Equal([2, 4], source.Take(3).Take(2));
            Assert.Empty(source.Take(-1));
            Assert.Equal([2, 4, 6, 8], source.Take(4));
            Assert.Equal([2, 4, 6, 8], source.Take(40));
            Assert.Equal([2], source.Take(1));
            Assert.Equal([4], source.Skip(1).Take(1));
            Assert.Equal([6], source.Take(3).Skip(2));
            Assert.Equal([2], source.Take(3).Take(1));
        }

        [Fact]
        public void Select_SourceIsArray_ElementAt()
        {
            var source = new[] { 1, 2, 3, 4 }.Select(i => i * 2);
            for (int i = 0; i != 4; ++i)
                Assert.Equal(i * 2 + 2, source.ElementAt(i));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(() => new[] { 1, 2, 3, 4 }.Select(i => i * 2).ElementAt(-1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(() => new[] { 1, 2, 3, 4 }.Select(i => i * 2).ElementAt(4));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(() => new[] { 1, 2, 3, 4 }.Select(i => i * 2).ElementAt(40));

            Assert.Equal(6, source.Skip(1).ElementAt(1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(() => new[] { 1, 2, 3, 4 }.Select(i => i * 2).Skip(2).ElementAt(9));
        }

        [Fact]
        public void Select_SourceIsList_ElementAt()
        {
            var source = new List<int> { 1, 2, 3, 4 }.Select(i => i * 2);
            for (int i = 0; i != 4; ++i)
                Assert.Equal(i * 2 + 2, source.ElementAt(i));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(() => new List<int> { 1, 2, 3, 4 }.Select(i => i * 2).ElementAt(-1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(() => new List<int> { 1, 2, 3, 4 }.Select(i => i * 2).ElementAt(4));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(() => new List<int> { 1, 2, 3, 4 }.Select(i => i * 2).ElementAt(40));

            Assert.Equal(6, source.Skip(1).ElementAt(1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(() => new List<int> { 1, 2, 3, 4 }.Select(i => i * 2).Skip(2).ElementAt(9));
        }

        [Fact]
        public void Select_SourceIsIList_ElementAt()
        {
            var source = new List<int> { 1, 2, 3, 4 }.AsReadOnly().Select(i => i * 2);
            for (int i = 0; i != 4; ++i)
                Assert.Equal(i * 2 + 2, source.ElementAt(i));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(() => new List<int> { 1, 2, 3, 4 }.AsReadOnly().Select(i => i * 2).ElementAt(-1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(() => new List<int> { 1, 2, 3, 4 }.AsReadOnly().Select(i => i * 2).ElementAt(4));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(() => new List<int> { 1, 2, 3, 4 }.AsReadOnly().Select(i => i * 2).ElementAt(40));

            Assert.Equal(6, source.Skip(1).ElementAt(1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(() => new List<int> { 1, 2, 3, 4 }.AsReadOnly().Select(i => i * 2).Skip(2).ElementAt(9));
        }

        [Fact]
        public void Select_SourceIsArray_ElementAtOrDefault()
        {
            var source = new[] { 1, 2, 3, 4 }.Select(i => i * 2);
            for (int i = 0; i != 4; ++i)
                Assert.Equal(i * 2 + 2, source.ElementAtOrDefault(i));
            Assert.Equal(0, source.ElementAtOrDefault(-1));
            Assert.Equal(0, source.ElementAtOrDefault(4));
            Assert.Equal(0, source.ElementAtOrDefault(40));

            Assert.Equal(6, source.Skip(1).ElementAtOrDefault(1));
            Assert.Equal(0, source.Skip(2).ElementAtOrDefault(9));
        }

        [Fact]
        public void Select_SourceIsList_ElementAtOrDefault()
        {
            var source = new List<int> { 1, 2, 3, 4 }.Select(i => i * 2);
            for (int i = 0; i != 4; ++i)
                Assert.Equal(i * 2 + 2, source.ElementAtOrDefault(i));
            Assert.Equal(0, source.ElementAtOrDefault(-1));
            Assert.Equal(0, source.ElementAtOrDefault(4));
            Assert.Equal(0, source.ElementAtOrDefault(40));

            Assert.Equal(6, source.Skip(1).ElementAtOrDefault(1));
            Assert.Equal(0, source.Skip(2).ElementAtOrDefault(9));
        }

        [Fact]
        public void Select_SourceIsIList_ElementAtOrDefault()
        {
            var source = new List<int> { 1, 2, 3, 4 }.AsReadOnly().Select(i => i * 2);
            for (int i = 0; i != 4; ++i)
                Assert.Equal(i * 2 + 2, source.ElementAtOrDefault(i));
            Assert.Equal(0, source.ElementAtOrDefault(-1));
            Assert.Equal(0, source.ElementAtOrDefault(4));
            Assert.Equal(0, source.ElementAtOrDefault(40));

            Assert.Equal(6, source.Skip(1).ElementAtOrDefault(1));
            Assert.Equal(0, source.Skip(2).ElementAtOrDefault(9));
        }

        [Fact]
        public void Select_SourceIsArray_First()
        {
            var source = new[] { 1, 2, 3, 4 }.Select(i => i * 2);
            Assert.Equal(2, source.First());
            Assert.Equal(2, source.FirstOrDefault());

            Assert.Equal(6, source.Skip(2).First());
            Assert.Equal(6, source.Skip(2).FirstOrDefault());
            Assert.Throws<InvalidOperationException>(() => new[] { 1, 2, 3, 4 }.Select(i => i * 2).Skip(4).First());
            Assert.Throws<InvalidOperationException>(() => new[] { 1, 2, 3, 4 }.Select(i => i * 2).Skip(14).First());
            Assert.Equal(0, source.Skip(4).FirstOrDefault());
            Assert.Equal(0, source.Skip(14).FirstOrDefault());

            var empty = new int[0].Select(i => i * 2);
            Assert.Throws<InvalidOperationException>(() => new int[0].Select(i => i * 2).First());
            Assert.Equal(0, empty.FirstOrDefault());
        }

        [Fact]
        public void Select_SourceIsList_First()
        {
            var source = new List<int> { 1, 2, 3, 4 }.Select(i => i * 2);
            Assert.Equal(2, source.First());
            Assert.Equal(2, source.FirstOrDefault());

            Assert.Equal(6, source.Skip(2).First());
            Assert.Equal(6, source.Skip(2).FirstOrDefault());
            Assert.Throws<InvalidOperationException>(() => new List<int> { 1, 2, 3, 4 }.Select(i => i * 2).Skip(4).First());
            Assert.Throws<InvalidOperationException>(() => new List<int> { 1, 2, 3, 4 }.Select(i => i * 2).Skip(14).First());
            Assert.Equal(0, source.Skip(4).FirstOrDefault());
            Assert.Equal(0, source.Skip(14).FirstOrDefault());

            var empty = new List<int>().Select(i => i * 2);
            Assert.Throws<InvalidOperationException>(() => new List<int>().Select(i => i * 2).First());
            Assert.Equal(0, empty.FirstOrDefault());
        }

        [Fact]
        public void Select_SourceIsIList_First()
        {
            var source = new List<int> { 1, 2, 3, 4 }.AsReadOnly().Select(i => i * 2);
            Assert.Equal(2, source.First());
            Assert.Equal(2, source.FirstOrDefault());

            Assert.Equal(6, source.Skip(2).First());
            Assert.Equal(6, source.Skip(2).FirstOrDefault());
            Assert.Throws<InvalidOperationException>(() => new List<int> { 1, 2, 3, 4 }.AsReadOnly().Select(i => i * 2).Skip(4).First());
            Assert.Throws<InvalidOperationException>(() => new List<int> { 1, 2, 3, 4 }.AsReadOnly().Select(i => i * 2).Skip(14).First());
            Assert.Equal(0, source.Skip(4).FirstOrDefault());
            Assert.Equal(0, source.Skip(14).FirstOrDefault());

            var empty = new List<int>().AsReadOnly().Select(i => i * 2);
            Assert.Throws<InvalidOperationException>(() => new List<int>().AsReadOnly().Select(i => i * 2).First());
            Assert.Equal(0, empty.FirstOrDefault());
        }

        [Fact]
        public void Select_SourceIsArray_Last()
        {
            var source = new[] { 1, 2, 3, 4 }.Select(i => i * 2);
            Assert.Equal(8, source.Last());
            Assert.Equal(8, source.LastOrDefault());

            Assert.Equal(6, source.Take(3).Last());
            Assert.Equal(6, source.Take(3).LastOrDefault());

            var empty = new int[0].Select(i => i * 2);
            Assert.Throws<InvalidOperationException>(() => new int[0].Select(i => i * 2).Last());
            Assert.Equal(0, empty.LastOrDefault());
            Assert.Throws<InvalidOperationException>(() => new int[0].Select(i => i * 2).Skip(1).Last());
            Assert.Equal(0, empty.Skip(1).LastOrDefault());
        }

        [Fact]
        public void Select_SourceIsList_Last()
        {
            var source = new List<int> { 1, 2, 3, 4 }.Select(i => i * 2);
            Assert.Equal(8, source.Last());
            Assert.Equal(8, source.LastOrDefault());

            Assert.Equal(6, source.Take(3).Last());
            Assert.Equal(6, source.Take(3).LastOrDefault());

            var empty = new List<int>().Select(i => i * 2);
            Assert.Throws<InvalidOperationException>(() => new List<int>().Select(i => i * 2).Last());
            Assert.Equal(0, empty.LastOrDefault());
            Assert.Throws<InvalidOperationException>(() => new List<int>().Select(i => i * 2).Skip(1).Last());
            Assert.Equal(0, empty.Skip(1).LastOrDefault());
        }

        [Fact]
        public void Select_SourceIsIList_Last()
        {
            var source = new List<int> { 1, 2, 3, 4 }.AsReadOnly().Select(i => i * 2);
            Assert.Equal(8, source.Last());
            Assert.Equal(8, source.LastOrDefault());

            Assert.Equal(6, source.Take(3).Last());
            Assert.Equal(6, source.Take(3).LastOrDefault());

            var empty = new List<int>().AsReadOnly().Select(i => i * 2);
            Assert.Throws<InvalidOperationException>(() => new List<int>().AsReadOnly().Select(i => i * 2).Last());
            Assert.Equal(0, empty.LastOrDefault());
            Assert.Throws<InvalidOperationException>(() => new List<int>().AsReadOnly().Select(i => i * 2).Skip(1).Last());
            Assert.Equal(0, empty.Skip(1).LastOrDefault());
        }

        [Fact]
        public void Select_SourceIsArray_SkipRepeatCalls()
        {
            var source = new[] { 1, 2, 3, 4 }.Select(i => i * 2).Skip(1);
            Assert.Equal(source, source);
        }

        [Fact]
        public void Select_SourceIsArraySkipSelect()
        {
            var source = new[] { 1, 2, 3, 4 }.Select(i => i * 2).Skip(1).Select(i => i + 1);
            Assert.Equal([5, 7, 9], source);
        }

        [Fact]
        public void Select_SourceIsArrayTakeTake()
        {
            var source = new[] { 1, 2, 3, 4 }.Select(i => i * 2).Take(2).Take(1);
            Assert.Equal([2], source);
            Assert.Equal([2], source.Take(10));
        }

        [Fact]
        public void Select_SourceIsIPartitionToArray()
        {
            Assert.Equal([], new List<int>().Order().Select(i => i * 2).ToArray());
            Assert.Equal([2, 4, 6, 8], new List<int> { 1, 2, 3, 4 }.Order().Select(i => i * 2).ToArray());
        }

        [Fact]
        public void Select_SourceIsListSkipTakeCount()
        {
            Assert.Equal(3, new List<int> { 1, 2, 3, 4 }.Select(i => i * 2).Take(3).Count());
            Assert.Equal(4, new List<int> { 1, 2, 3, 4 }.Select(i => i * 2).Take(9).Count());
            Assert.Equal(2, new List<int> { 1, 2, 3, 4 }.Select(i => i * 2).Skip(2).Count());
            Assert.Empty(new List<int> { 1, 2, 3, 4 }.Select(i => i * 2).Skip(8));
        }

        [Fact]
        public void Select_SourceIsListSkipTakeToArray()
        {
            Assert.Equal([2, 4, 6], new List<int> { 1, 2, 3, 4 }.Select(i => i * 2).Take(3).ToArray());
            Assert.Equal([2, 4, 6, 8], new List<int> { 1, 2, 3, 4 }.Select(i => i * 2).Take(9).ToArray());
            Assert.Equal([6, 8], new List<int> { 1, 2, 3, 4 }.Select(i => i * 2).Skip(2).ToArray());
            Assert.Empty(new List<int> { 1, 2, 3, 4 }.Select(i => i * 2).Skip(8).ToArray());
        }

        [Fact]
        public void Select_SourceIsListSkipTakeToList()
        {
            Assert.Equal([2, 4, 6], new List<int> { 1, 2, 3, 4 }.Select(i => i * 2).Take(3).ToList());
            Assert.Equal([2, 4, 6, 8], new List<int> { 1, 2, 3, 4 }.Select(i => i * 2).Take(9).ToList());
            Assert.Equal([6, 8], new List<int> { 1, 2, 3, 4 }.Select(i => i * 2).Skip(2).ToList());
            Assert.Empty(new List<int> { 1, 2, 3, 4 }.Select(i => i * 2).Skip(8).ToList());
        }

        [Fact]
        public void Select_SourceIsIPartitionToList()
        {
            Assert.Equal([], new List<int>().Order().Select(i => i * 2).ToList());
            Assert.Equal([2, 4, 6, 8], new List<int> { 1, 2, 3, 4 }.Order().Select(i => i * 2).ToList());
        }

        [Theory]
        [MemberData(nameof(MoveNextAfterDisposeData))]
        public void MoveNextAfterDispose(IEnumerable<int> source)
        {
            // Select is specialized for a bunch of different types, so we want
            // to make sure this holds true for all of them.
            var identityTransforms = new List<Func<IEnumerable<int>, IEnumerable<int>>>
            {
                e => e,
                e => ForceNotCollection(e),
                e => e.ToArray(),
                e => e.ToList(),
                e => new LinkedList<int>(e), // IList<T> that's not a List
                e => e.Select(i => i).ToArray() // Multiple Select() chains are optimized
            };

            foreach (IEnumerable<int> equivalentSource in identityTransforms.Select(t => t(source)))
            {
                var result = equivalentSource.Select(i => i);
                using var e = result.GetEnumerator();

                while (e.MoveNext()) ; // Loop until we reach the end of the iterator, @ which pt it gets disposed.
                Assert.False(e.MoveNext()); // MoveNext should not throw an exception after Dispose.

            }
        }

        public static IEnumerable<object[]> MoveNextAfterDisposeData()
        {
            yield return [Array.Empty<int>()];
            yield return [new int[1]];
            yield return [Enumerable.Range(1, 30)];
        }

        [Theory(Skip = SkipReason.Issue0082)]
        [MemberData(nameof(RunSelectorDuringCountData))]
        public void RunSelectorDuringCount(IEnumerable<int> source)
        {
            int timesRun = 0;
            var selected = source.Select(i => timesRun++);
            selected.Count();

            // ZLinq don't enumerate items when source support `TryGetNonEnumeratedCount`. See: https://github.com/Cysharp/ZLinq/issues/82
            Assert.Equal(source.Count(), timesRun);
        }

        public static IEnumerable<object[]> RunSelectorDuringCountData()
        {
            var transforms = new Func<IEnumerable<int>, IEnumerable<int>>[]
            {
                e => e,
                e => ForceNotCollection(e),
                e => ForceNotCollection(e).Skip(1).ToArray(),
                e => ForceNotCollection(e).Where(i => true).ToArray(),
                e => e.ToArray().Where(i => true).ToArray(),
                e => e.ToList().Where(i => true).ToArray(),
                e => new LinkedList<int>(e).Where(i => true).ToArray(),
                e => e.Select(i => i).ToArray(),
                e => e.Take(e.Count()).ToArray(),
                e => e.ToArray(),
                e => e.ToList(),
                e => new LinkedList<int>(e) // Implements IList<T>.
            };

            var r = new Random(42);

            for (int i = 0; i <= 5; i++)
            {
                var enumerable = Enumerable.Range(1, i).Select(_ => r.Next()).ToArray();

                foreach (var transform in transforms)
                {
                    yield return [transform(enumerable)];
                }
            }
        }
    }
}
