using System;
using System.Collections.Generic;
using System.Linq;
using NOPE.Runtime;
using NOPE.Runtime.Core;
using NOPE.Runtime.Core.Maybe;
using NUnit.Framework;

namespace NOPE.Tests
{
    public class CollectionUtilitiesExtensionsTests
    {
        [Test]
        public void TryFind_NullDictionary_ThrowsArgumentNullException()
        {
            Dictionary<string, int> dict = null;
            Assert.Throws<ArgumentNullException>(() => dict.TryFind("key"));
        }

        [Test]
        public void TryFind_KeyExists_ReturnsMaybeWithValue()
        {
            var dict = new Dictionary<string, int> { { "test", 42 } };
            var result = dict.TryFind("test");
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public void TryFind_KeyDoesNotExist_ReturnsNone()
        {
            var dict = new Dictionary<string, int> { { "test", 42 } };
            var result = dict.TryFind("missing");
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void TryFirst_NullSequence_ThrowsArgumentNullException()
        {
            IEnumerable<int> sequence = null;
            Assert.Throws<ArgumentNullException>(() => sequence.TryFirst());
        }

        [Test]
        public void TryFirst_EmptySequence_ReturnsNone()
        {
            var sequence = Enumerable.Empty<int>();
            var result = sequence.TryFirst();
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void TryFirst_SingleElement_ReturnsMaybeWithValue()
        {
            var sequence = new[] { 42 };
            var result = sequence.TryFirst();
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public void TryFirst_WithPredicate_MatchExists_ReturnsMaybeWithValue()
        {
            var sequence = new[] { 1, 2, 3, 4, 5 };
            var result = sequence.TryFirst(x => x > 3);
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(4, result.Value);
        }

        [Test]
        public void TryFirst_WithPredicate_NoMatch_ReturnsNone()
        {
            var sequence = new[] { 1, 2, 3 };
            var result = sequence.TryFirst(x => x > 10);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void TryLast_NullSequence_ThrowsArgumentNullException()
        {
            IEnumerable<int> sequence = null;
            Assert.Throws<ArgumentNullException>(() => sequence.TryLast());
        }

        [Test]
        public void TryLast_EmptySequence_ReturnsNone()
        {
            var sequence = Enumerable.Empty<int>();
            var result = sequence.TryLast();
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void TryLast_SingleElement_ReturnsMaybeWithValue()
        {
            var sequence = new[] { 42 };
            var result = sequence.TryLast();
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public void TryLast_WithPredicate_MatchExists_ReturnsMaybeWithValue()
        {
            var sequence = new[] { 1, 2, 3, 4, 5 };
            var result = sequence.TryLast(x => x < 4);
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(3, result.Value);
        }

        [Test]
        public void TryLast_WithPredicate_NoMatch_ReturnsNone()
        {
            var sequence = new[] { 1, 2, 3 };
            var result = sequence.TryLast(x => x > 10);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void Choose_NullSequence_ThrowsArgumentNullException()
        {
            IEnumerable<Maybe<int>> sequence = null;
            Assert.Throws<ArgumentNullException>(() => sequence.Choose().ToList());
        }

        [Test]
        public void Choose_EmptySequence_ReturnsEmptySequence()
        {
            var sequence = Enumerable.Empty<Maybe<int>>();
            var result = sequence.Choose();
            Assert.IsFalse(result.Any());
        }

        [Test]
        public void Choose_MixedValues_ReturnsOnlyValues()
        {
            var sequence = new[]
            {
                Maybe<int>.From(1),
                Maybe<int>.None,
                Maybe<int>.From(3)
            };
            var result = sequence.Choose().ToList();
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(3, result[1]);
        }

        [Test]
        public void Choose_WithSelector_TransformsValues()
        {
            var sequence = new[]
            {
                Maybe<int>.From(1),
                Maybe<int>.None,
                Maybe<int>.From(3)
            };
            var result = sequence.Choose(x => x * 2).ToList();
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, result[0]);
            Assert.AreEqual(6, result[1]);
        }

        [Test]
        public void Execute_HasValue_ExecutesAction()
        {
            var count = 0;
            var maybe = Maybe<int>.From(42);
            maybe.Execute(_ => count++);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void Execute_NoValue_DoesNotExecuteAction()
        {
            var count = 0;
            var maybe = Maybe<int>.None;
            maybe.Execute(_ => count++);
            Assert.AreEqual(0, count);
        }

        [Test]
        public void ExecuteNoValue_HasValue_DoesNotExecuteAction()
        {
            var count = 0;
            var maybe = Maybe<int>.From(42);
            maybe.ExecuteNoValue(() => count++);
            Assert.AreEqual(0, count);
        }

        [Test]
        public void ExecuteNoValue_NoValue_ExecutesAction()
        {
            var count = 0;
            var maybe = Maybe<int>.None;
            maybe.ExecuteNoValue(() => count++);
            Assert.AreEqual(1, count);
        }
    }
}