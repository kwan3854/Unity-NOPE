using System;
using NOPE.Runtime.Core.Result;
using NUnit.Framework;

namespace NOPE.Tests.ResultTests
{
    [TestFixture]
    public class Result_Success_Implicit_Tests
    {
        [Test]
        public void Result_Success_Implicit_ShouldHaveValueOnSuccess()
        {
            // Arrange
            Result<int, string> result = 42;

            // Act
            var value = result.Value;

            // Assert
            Assert.AreEqual(42, value);
        }
        
        [Test]
        public void Result_Success_Implicit_ShouldNotHaveErrorOnSuccess()
        {
            // Arrange
            Result<int, string> result = 42;

            // Act
            TestDelegate act = () => { var error = result.Error; };

            // Assert
            Assert.Throws<InvalidOperationException>(act);
        }
        
        [Test]
        public void Result_Success_Implicit_ShouldBeSuccessful()
        {
            // Arrange
            Result<int, string> result = 42;

            // Act
            var isSuccess = result.IsSuccess;

            // Assert
            Assert.IsTrue(isSuccess);
        }
        
        [Test]
        public void Result_Success_Implicit_ShouldNotBeFailure()
        {
            // Arrange
            Result<int, string> result = 42;

            // Act
            var isFailure = result.IsFailure;

            // Assert
            Assert.IsFalse(isFailure);
        }
    }
}