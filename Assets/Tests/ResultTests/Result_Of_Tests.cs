using System;
using System.Threading.Tasks;
using NOPE.Runtime.Core.Result;
using NUnit.Framework;

#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace NOPE.Tests.ResultTests
{
    [TestFixture]
    public class Result_Of_Tests
    {
        [Test]
        public void Result_Of_ShouldReturnSuccess_WhenFunctionDoesNotThrow()
        {
            // Act
            var result = Result.Of(() => 42, ex => "Error message");

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public void Result_Of_ShouldReturnFailure_WhenFunctionThrows()
        {
            // Act
            var result = Result.Of<int, string>(() =>
            {
                throw new Exception("Error");
#pragma warning disable CS0162 // Unreachable code detected
                return 42;
#pragma warning restore CS0162 // Unreachable code detected
            }, ex => "Error message");

            // Assert
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual("Error message", result.Error);
        }
        
        [Test]
        public void Result_Of_ShouldReturnExceptionMessage_WhenFunctionThrows()
        {
            var exception = new Exception("Error");
            // Act
            var result = Result.Of<int, string>(() =>
            {
                throw exception;
#pragma warning disable CS0162 // Unreachable code detected
                return 42;
#pragma warning restore CS0162 // Unreachable code detected
            }, ex => ex.Message);

            // Assert
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(exception.Message, result.Error);
        }

#if NOPE_UNITASK
        [Test]
        public async Task Result_Of_ShouldReturnSuccess_WhenFunctionDoesNotThrow_Async()
        {
            // Act
            var result = await Result.Of(async () =>
            {
                await UniTask.Delay(1);
                return 42;
            }, ex => "Error message");

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public async Task Result_Of_ShouldReturnFailure_WhenFunctionThrows_Async()
        {
            // Act
            var result = await Result.Of<int, string>(async () =>
            {
                throw new Exception("Error");
#pragma warning disable CS0162 // Unreachable code detected
                await UniTask.Delay(1);
                return 42;
#pragma warning restore CS0162 // Unreachable code detected
            }, ex => "Error message");

            // Assert
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual("Error message", result.Error);
        }
        
        [Test]
        public async Task Result_Of_ShouldReturnExceptionMessage_WhenFunctionThrows_Async()
        {
            var exception = new Exception("Error");
            // Act
            var result = await Result.Of<int, string>(async () =>
            {
                throw exception;
#pragma warning disable CS0162 // Unreachable code detected
                await Task.Delay(1);
                return 42;
#pragma warning restore CS0162 // Unreachable code detected
            }, ex => ex.Message);

            // Assert
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(exception.Message, result.Error);
        }
#endif

#if NOPE_AWAITABLE
        [Test]
        public async Task Result_Of_ShouldReturnSuccess_WhenFunctionDoesNotThrow_Awaitable()
        {
            // Act
            var result = await Result.Of(async () =>
            {
                await Task.Delay(1);
                return 42;
            }, ex => "Error message");
        
            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public async Task Result_Of_ShouldReturnFailure_WhenFunctionThrows_Awaitable()
        {
            var exception = new Exception("Error");
            // Act
            var result = await Result.Of(async () =>
            {
                throw exception;
#pragma warning disable CS0162 // Unreachable code detected
                await Task.Delay(1);
                return 42;
#pragma warning restore CS0162 // Unreachable code detected
            }, ex => ex);
        
            // Assert
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(exception, result.Error);
        }
        
        [Test]
        public async Task Result_Of_ShouldReturnExceptionMessage_WhenFunctionThrows_Async()
        {
            var exception = new Exception("Error");
            // Act
            var result = await Result.Of(async () =>
            {
                throw exception;
#pragma warning disable CS0162 // Unreachable code detected
                await Task.Delay(1);
                return 42;
#pragma warning restore CS0162 // Unreachable code detected
            }, ex => ex);

            // Assert
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(exception, result.Error);
        }
#endif
    }
}