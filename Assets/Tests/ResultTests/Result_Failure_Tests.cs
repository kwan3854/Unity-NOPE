using System;
using NOPE.Runtime.Core.Result;
using NUnit.Framework;

namespace NOPE.Tests.ResultTests
{
    [TestFixture]
    public class Result_Failure_Tests
    {
        [Test]
        public void Result_Failure_ShouldHaveErrorOnFailure()
        {
            // Arrange
            var result = Result<int, string>.Failure("Error message");

            // Act
            var error = result.Error;

            // Assert
            Assert.AreEqual("Error message", error);
        }
        
        [Test]
        public void Result_Failure_ShouldNotHaveValueOnFailure()
        {
            // Arrange
            var result = Result<int, string>.Failure("Error message");

            // Act
            TestDelegate act = () => { var value = result.Value; };

            // Assert
            Assert.Throws<InvalidOperationException>(act);
        }
        
        [Test]
        public void Result_Failure_ShouldNotBeSuccessful()
        {
            // Arrange
            var result = Result<int, string>.Failure("Error message");

            // Act
            var isSuccess = result.IsSuccess;

            // Assert
            Assert.IsFalse(isSuccess);
        }
        
        [Test]
        public void Result_Failure_ShouldBeFailure()
        {
            // Arrange
            var result = Result<int, string>.Failure("Error message");

            // Act
            var isFailure = result.IsFailure;

            // Assert
            Assert.IsTrue(isFailure);
        }
    }
}