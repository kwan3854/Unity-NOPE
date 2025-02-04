using NOPE.Runtime.Core.Maybe;
using NUnit.Framework;

namespace NOPE.Tests.MaybeTests
{
    [TestFixture]
    public class MaybeEqualityTests
    {
        [Test]
        public void Maybe_WithSameValue_ShouldBeEqual()
        {
            // Arrange
            var a = Maybe<int>.From(42);
            var b = Maybe<int>.From(42);

            // Act & Assert
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a == b);
            Assert.IsFalse(a != b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Maybe_WithDifferentValue_ShouldNotBeEqual()
        {
            // Arrange
            var a = Maybe<int>.From(42);
            var b = Maybe<int>.From(43);

            // Act & Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [Test]
        public void Maybe_None_ShouldBeEqual()
        {
            // Arrange
            var a = Maybe<string>.None;
            var b = Maybe<string>.None;

            // Act & Assert
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a == b);
            Assert.IsFalse(a != b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Maybe_ValueAndNone_ShouldNotBeEqual()
        {
            // Arrange
            var a = Maybe<string>.From("Hello");
            var b = Maybe<string>.None;

            // Act & Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }
    }
}