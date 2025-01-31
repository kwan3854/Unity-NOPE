using System;
using NOPE.Runtime.Core.Result;
using NUnit.Framework;

namespace NOPE.Tests.ResultTests
{
    [TestFixture]
    public class Result_Error_Tests
    {
        [Test]
        public void Result_Error_ShouldHaveErrorOnFailure()
        {
            // Arrange
            Result<int, string> result = "Error message";

            // Act
            var error = result.Error;

            // Assert
            Assert.AreEqual("Error message", error);
        }
        
        [Test]
        public void Result_Error_ShouldThrowInvalidOperationExceptionOnSuccess()
        {
            // Arrange
            Result<int, string> result = 42;

            // Act
            TestDelegate act = () => { var error = result.Error; };

            // Assert
            Assert.Throws<InvalidOperationException>(act);
        }
    }
}