using NOPE.Runtime;
using NUnit.Framework;

namespace NOPE.Tests
{
    public class BasicTests
    {
        [Test]
        public void Result_Success_ShouldCreateValidResult()
        {
            // Arrange & Act
            var r = Result<int>.Success(5);

            // Assert
            Assert.IsTrue(r.IsSuccess);
            Assert.IsFalse(r.IsFailure);
            Assert.AreEqual(5, r.Value);
            // IsSuccess 시 Error 프로퍼티에 접근하면 예외가 떠야 함
            Assert.Throws<System.InvalidOperationException>(() =>
            {
                var _ = r.Error;
            });
        }

        [Test]
        public void Result_Failure_ShouldCreateFailedResult()
        {
            // Arrange & Act
            var r = Result<int>.Failure("Something went wrong");

            // Assert
            Assert.IsFalse(r.IsSuccess);
            Assert.IsTrue(r.IsFailure);
            Assert.AreEqual("Something went wrong", r.Error);
            // IsFailure 시 Value 프로퍼티에 접근하면 예외가 떠야 함
            Assert.Throws<System.InvalidOperationException>(() =>
            {
                var _ = r.Value;
            });
        }

        [Test]
        public void Result_ImplicitConversions_ShouldWork()
        {
            // 암시적 변환: T -> Success
            Result<int> r1 = 123;
            Assert.IsTrue(r1.IsSuccess);
            Assert.AreEqual(123, r1.Value);

            // 암시적 변환: string -> Failure
            Result<int> r2 = "Error occurred";
            Assert.IsFalse(r2.IsSuccess);
            Assert.AreEqual("Error occurred", r2.Error);
        }

        [Test]
        public void Maybe_From_ShouldCreateCorrectMaybe()
        {
            // Arrange & Act
            var maybeWithValue = Maybe<string>.From("Hello");
            var maybeNone = Maybe<string>.From(null);

            // Assert
            Assert.IsTrue(maybeWithValue.HasValue);
            Assert.AreEqual("Hello", maybeWithValue.Value);

            Assert.IsFalse(maybeNone.HasValue);
            Assert.Throws<System.InvalidOperationException>(() =>
            {
                var _ = maybeNone.Value;
            });
        }

        [Test]
        public void Maybe_None_ShouldBeNoValue()
        {
            // Arrange
            var none = Maybe<int>.None;

            // Assert
            Assert.IsFalse(none.HasValue);
            Assert.IsTrue(none.HasNoValue);
        }

        [Test]
        public void Maybe_ImplicitConversion_ShouldWork()
        {
            // Arrange & Act
            Maybe<int> maybe123 = 123;  // implicit
            Maybe<int> maybeNone = Maybe<int>.None;

            // Assert
            Assert.IsTrue(maybe123.HasValue);
            Assert.AreEqual(123, maybe123.Value);

            Assert.IsFalse(maybeNone.HasValue);
        }
    }
}