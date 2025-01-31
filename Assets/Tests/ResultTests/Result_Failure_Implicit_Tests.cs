using System;
using NOPE.Runtime.Core.Result;
using NUnit.Framework;

namespace NOPE.Tests.ResultTests
{
    [TestFixture]
    public class Result_Failure_Implicit_Tests
    {
        [Test]
        public void Result_Failure_Implicit_ShouldHaveErrorOnFailure()
        {
            // Arrange
            Result<int, string> result = "Error message";

            // Act
            var error = result.Error;

            // Assert
            Assert.AreEqual("Error message", error);
        }
        
        [Test]
        public void Result_Failure_Implicit_ShouldNotHaveValueOnFailure()
        {
            // Arrange
            Result<int, string> result = "Error message";

            // Act
            TestDelegate act = () => { var value = result.Value; };

            // Assert
            Assert.Throws<InvalidOperationException>(act);
        }
        
        [Test]
        public void Result_Failure_Implicit_ShouldNotBeSuccessful()
        {
            // Arrange
            Result<int, string> result = "Error message";

            // Act
            var isSuccess = result.IsSuccess;

            // Assert
            Assert.IsFalse(isSuccess);
        }
        
        [Test]
        public void Result_Failure_Implicit_ShouldBeFailure()
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