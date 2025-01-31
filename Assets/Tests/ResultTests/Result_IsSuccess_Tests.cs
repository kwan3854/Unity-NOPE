using NOPE.Runtime.Core.Result;
using NUnit.Framework;

namespace NOPE.Tests.ResultTests
{
    [TestFixture]
    public class Result_IsSuccess_Tests
    {
        [Test]
        public void Result_IsSuccess_ShouldBeTrueOnSuccess()
        {
            // Arrange
            Result<int, string> result = 42;

            // Act
            var isSuccess = result.IsSuccess;

            // Assert
            Assert.IsTrue(isSuccess);
        }
        
        [Test]
        public void Result_IsSuccess_ShouldBeFalseOnFailure()
        {
            // Arrange
            Result<int, string> result = "Error message";

            // Act
            var isSuccess = result.IsSuccess;

            // Assert
            Assert.IsFalse(isSuccess);
        }
    }
}