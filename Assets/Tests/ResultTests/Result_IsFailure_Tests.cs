using NOPE.Runtime.Core.Result;
using NUnit.Framework;

namespace NOPE.Tests.ResultTests
{
    [TestFixture]
    public class Result_IsFailure_Tests
    {
        [Test]
        public void Result_IsFailure_ShouldBeFalseOnSuccess()
        {
            // Arrange
            Result<int, string> result = 42;

            // Act
            var isFailure = result.IsFailure;

            // Assert
            Assert.IsFalse(isFailure);
        }
        
        [Test]
        public void Result_IsFailure_ShouldBeTrueOnFailure()
        {
            // Arrange
            Result<int, string> result = "Error message";

            // Act
            var isFailure = result.IsFailure;

            // Assert
            Assert.IsTrue(isFailure);
        }
    }
}