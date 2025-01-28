using System;
using NOPE.Runtime.Core.Maybe;
using NUnit.Framework;

namespace NOPE.Tests
{
    public class MaybeAdvancedExtensionsTests
    {
        [Test]
        public void GetValueOrThrow_HasValue_ReturnsValue()
        {
            var maybe = Maybe<int>.From(42);
            var result = maybe.GetValueOrThrow();
            Assert.AreEqual(42, result);
        }

        [Test]
        public void GetValueOrThrow_NoValue_ThrowsInvalidOperationException()
        {
            var maybe = Maybe<int>.None;
            Assert.Throws<InvalidOperationException>(() => maybe.GetValueOrThrow());
        }

        [Test]
        public void GetValueOrThrow_NoValueWithCustomException_ThrowsCustom()
        {
            var maybe = Maybe<int>.None;
            var customEx = new ArgumentNullException("Custom");
            var ex = Assert.Throws<ArgumentNullException>(() => maybe.GetValueOrThrow(customEx));
            Assert.AreEqual("Custom", ex.ParamName);
        }

        [Test]
        public void GetValueOrDefault_HasValue_ReturnsInnerValue()
        {
            var maybe = Maybe<int>.From(100);
            var result = maybe.GetValueOrDefault(-1);
            Assert.AreEqual(100, result);
        }

        [Test]
        public void GetValueOrDefault_NoValue_ReturnsDefault()
        {
            var maybe = Maybe<int>.None;
            var result = maybe.GetValueOrDefault(-1);
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void Or_HasValue_ReturnsOriginalMaybe()
        {
            var maybe = Maybe<string>.From("Hello");
            var fallback = Maybe<string>.From("Fallback");
            var result = maybe.Or(fallback);
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual("Hello", result.Value);
        }

        [Test]
        public void Or_NoValue_ReturnsFallbackMaybe()
        {
            var maybe = Maybe<string>.None;
            var fallback = Maybe<string>.From("Fallback");
            var result = maybe.Or(fallback);
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual("Fallback", result.Value);
        }

        [Test]
        public void Or_ValueOverload_HasValue_ReturnsOriginalMaybe()
        {
            var maybe = Maybe<int>.From(10);
            var result = maybe.Or(99);
            Assert.AreEqual(10, result.Value);
        }

        [Test]
        public void Or_ValueOverload_NoValue_ReturnsNewMaybe()
        {
            var maybe = Maybe<int>.None;
            var result = maybe.Or(99);
            Assert.AreEqual(99, result.Value);
        }
    }
}