using System;
using NOPE.Runtime.Core.Result;
using NUnit.Framework;

namespace NOPE.Tests.ResultTests
{
    [TestFixture]
    public class Result_Success_Tests
    {
        [Test]
        public void Result_Success_ShouldHaveValueOnSuccess()
        {
            // Arrange
            var result = Result<int, string>.Success(42);

            // Act
            var value = result.Value;

            // Assert
            Assert.AreEqual(42, value);
        }
        
        [Test]
        public void Result_Success_ShouldNotHaveErrorOnSuccess()
        {
            // Arrange
            var result = Result<int, string>.Success(42);

            // Act
            TestDelegate act = () => { var error = result.Error; };

            // Assert
            Assert.Throws<InvalidOperationException>(act);
        }
        
        [Test]
        public void Result_Success_ShouldBeSuccessful()
        {
            // Arrange
            var result = Result<int, string>.Success(42);

            // Act
            var isSuccess = result.IsSuccess;

            // Assert
            Assert.IsTrue(isSuccess);
        }
        
        [Test]
        public void Result_Success_ShouldNotBeFailure()
        {
            // Arrange
            var result = Result<int, string>.Success(42);

            // Act
            var isFailure = result.IsFailure;

            // Assert
            Assert.IsFalse(isFailure);
        }
    }
}
