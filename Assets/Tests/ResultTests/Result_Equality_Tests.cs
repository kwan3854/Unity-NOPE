using NOPE.Runtime.Core.Result;
using NUnit.Framework;

namespace NOPE.Tests.ResultTests
{
    [TestFixture]
    public class ResultEqualityTests
    {
        [Test]
        public void Result_SuccessWithSameValue_ShouldBeEqual()
        {
            // Arrange
            var result1 = Result<int, string>.Success(100);
            var result2 = Result<int, string>.Success(100);

            // Act & Assert
            Assert.IsTrue(result1.Equals(result2));
            Assert.IsTrue(result1 == result2);
            Assert.IsFalse(result1 != result2);
            Assert.AreEqual(result1.GetHashCode(), result2.GetHashCode());
        }

        [Test]
        public void Result_SuccessWithDifferentValue_ShouldNotBeEqual()
        {
            // Arrange
            var result1 = Result<int, string>.Success(100);
            var result2 = Result<int, string>.Success(200);

            // Act & Assert
            Assert.IsFalse(result1.Equals(result2));
            Assert.IsFalse(result1 == result2);
            Assert.IsTrue(result1 != result2);
        }

        [Test]
        public void Result_FailureWithSameError_ShouldBeEqual()
        {
            // Arrange
            var result1 = Result<int, string>.Failure("Error occurred");
            var result2 = Result<int, string>.Failure("Error occurred");

            // Act & Assert
            Assert.IsTrue(result1.Equals(result2));
            Assert.IsTrue(result1 == result2);
            Assert.IsFalse(result1 != result2);
            Assert.AreEqual(result1.GetHashCode(), result2.GetHashCode());
        }

        [Test]
        public void Result_FailureWithDifferentError_ShouldNotBeEqual()
        {
            // Arrange
            var result1 = Result<int, string>.Failure("Error A");
            var result2 = Result<int, string>.Failure("Error B");

            // Act & Assert
            Assert.IsFalse(result1.Equals(result2));
            Assert.IsFalse(result1 == result2);
            Assert.IsTrue(result1 != result2);
        }

        [Test]
        public void Result_SuccessAndFailure_ShouldNotBeEqual()
        {
            // Arrange
            var successResult = Result<int, string>.Success(100);
            var failureResult = Result<int, string>.Failure("Error occurred");

            // Act & Assert
            Assert.IsFalse(successResult.Equals(failureResult));
            Assert.IsFalse(successResult == failureResult);
            Assert.IsTrue(successResult != failureResult);
        }
    }
}