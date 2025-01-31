using System;
using NOPE.Runtime.Core.Result;
using NUnit.Framework;

namespace NOPE.Tests.ResultTests
{
    [TestFixture]
    public class Result_Value_Tests
    {
        [Test]
        public void Result_Value_ShouldHaveValueOnSuccess()
        {
            // Arrange
            Result<int, string> result = 42;

            // Act
            var value = result.Value;

            // Assert
            Assert.AreEqual(42, value);
        }
        
        [Test]
        public void Result_Value_ShouldThrowInvalidOperationExceptionOnFailure()
        {
            // Arrange
            Result<int, string> result = "Error message";

            // Act
            TestDelegate act = () => { var value = result.Value; };

            // Assert
            Assert.Throws<InvalidOperationException>(act);
        }
    }
}