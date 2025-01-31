using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NOPE.Runtime.Core.Result;
using NUnit.Framework;
using UnityEngine;

namespace NOPE.Tests.ResultTests
{
    [TestFixture]
    public class Result_BindSafe_Tests
    {
        private Exception _exception = new("Error message");

        private Result<int, string> BindFunctionSuccess(int value)
        {
            return Result<int, string>.Success(value * 2);
        }

        private Result<int, string> BindFunctionFailure(int value)
        {
            return Result<int, string>.Failure("Error message");
        }

        private Result<int, Exception> BindFunctionThrows(int value)
        {
            throw _exception;

#pragma warning disable CS0162 // Unreachable code detected
            return Result<int, Exception>.Success(value * 2);
#pragma warning restore CS0162 // Unreachable code detected
        }

        [Test]
        public void Result_BindSafe_ShouldReturnSuccess_WhenResultIsSuccess()
        {
            // Arrange
            Result<int, string> result = 42;

            // Act
            var bindResult = result.BindSafe(
                BindFunctionSuccess,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsSuccess);
            Assert.AreEqual(84, bindResult.Value);
        }

        [Test]
        public void Result_BindSafe_ShouldReturnFailure_WhenResultIsFailure()
        {
            // Arrange
            Result<int, string> result = "Error message";

            // Act
            var bindResult = result.BindSafe(
                BindFunctionSuccess,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Error message", bindResult.Error);
        }

        [Test]
        public void Result_BindSafe_ShouldReturnFailure_WhenStartResultIsFailure()
        {
            // Arrange
            Result<int, string> result = "Start error message";

            // Act
            var bindResult = result.BindSafe(
                BindFunctionSuccess,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Start error message", bindResult.Error);
        }

        [Test]
        public void Result_BindSafe_ShouldReturnFailure_WhenBindFunctionThrows()
        {
            // Arrange
            Result<int, Exception> result = 42;

            // Act
            var bindResult = result.BindSafe(
                BindFunctionThrows,
                ex => ex);

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual(_exception, bindResult.Error);
        }

        [Test]
        public void Result_BindSafe_ShouldReturnFailure_WhenBindFunctionReturnsFailure()
        {
            // Arrange
            Result<int, string> result = 42;

            // Act
            var bindResult = result.BindSafe(
                BindFunctionFailure,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Error message", bindResult.Error);
        }
#if NOPE_UNITASK
        private async UniTask<Result<int, string>> BindFunctionSuccessUniTask(int value)
        {
            await UniTask.Delay(1);
            return Result<int, string>.Success(value * 2);
        }

        private async UniTask<Result<int, string>> BindFunctionFailureUniTask(int value)
        {
            await UniTask.Delay(1);
            return Result<int, string>.Failure("Error message");
        }

        private async UniTask<Result<int, Exception>> BindFunctionThrowsUniTask(int value)
        {
            await UniTask.Delay(1);
            throw _exception;
        }

        [Test]
        public async Task Result_BindSafe_ShouldReturnSuccess_WhenResultIsSuccess_Async()
        {
            // Arrange
            Result<int, string> result = 42;

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionSuccessUniTask,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsSuccess);
            Assert.AreEqual(84, bindResult.Value);
        }

        [Test]
        public async Task Result_BindSafe_ShouldReturnFailure_WhenResultIsFailure_Async()
        {
            // Arrange
            Result<int, string> result = "Error message";

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionSuccessUniTask,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Error message", bindResult.Error);
        }

        [Test]
        public async Task Result_BindSafe_ShouldReturnFailure_WhenStartResultIsFailure_Async()
        {
            // Arrange
            Result<int, string> result = "Start error message";

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionSuccessUniTask,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Start error message", bindResult.Error);
        }

        [Test]
        public async Task Result_BindSafe_ShouldReturnFailure_WhenBindFunctionThrows_Async()
        {
            // Arrange
            Result<int, Exception> result = 42;

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionThrowsUniTask,
                ex => ex);

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual(_exception, bindResult.Error);
        }

        [Test]
        public async Task Result_BindSafe_ShouldReturnFailure_WhenBindFunctionReturnsFailure_Async()
        {
            // Arrange
            Result<int, string> result = 42;

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionFailureUniTask,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Error message", bindResult.Error);
        }

        [Test]
        public async Task Result_BindSafe_ShouldReturnSuccess_WhenAsyncResultIsSuccess_Async()
        {
            // Arrange
            var result = UniTask.FromResult(Result<int, string>.Success(42));

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionSuccessUniTask,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsSuccess);
            Assert.AreEqual(84, bindResult.Value);
        }

        [Test]
        public async Task Result_BindSafe_ShouldReturnFailure_WhenAsyncResultIsFailure_Async()
        {
            // Arrange
            var result = UniTask.FromResult(Result<int, string>.Failure("Error message"));

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionSuccessUniTask,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Error message", bindResult.Error);
        }

        [Test]
        public async Task Result_BindSafe_ShouldReturnFailure_WhenAsyncBindFunctionThrows_Async()
        {
            // Arrange
            var result = UniTask.FromResult(Result<int, Exception>.Success(42));

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionThrowsUniTask,
                ex => ex);

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual(_exception, bindResult.Error);
        }

        [Test]
        public async Task Result_BindSafe_ShouldReturnFailure_WhenAsyncBindFunctionReturnsFailure_Async()
        {
            // Arrange
            var result = UniTask.FromResult(Result<int, string>.Success(42));

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionFailureUniTask,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Error message", bindResult.Error);
        }
        
        
        [Test]
        public async Task Result_BindSafe_ShouldReturnSuccess_WhenAsyncResultIsSuccess_Async_Sync()
        {
            // Arrange
            var result = UniTask.FromResult(Result<int, string>.Success(42));

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionSuccess,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsSuccess);
            Assert.AreEqual(84, bindResult.Value);
        }

        [Test]
        public async Task Result_BindSafe_ShouldReturnFailure_WhenAsyncResultIsFailure_Async_Sync()
        {
            // Arrange
            var result = UniTask.FromResult(Result<int, string>.Failure("Error message"));

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionSuccess,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Error message", bindResult.Error);
        }

        [Test]
        public async Task Result_BindSafe_ShouldReturnFailure_WhenAsyncStartResultIsFailure_Async_Sync()
        {
            // Arrange
            var result = UniTask.FromResult(Result<int, string>.Failure("Start error message"));

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionSuccess,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Start error message", bindResult.Error);
        }

        [Test]
        public async Task Result_BindSafe_ShouldReturnFailure_WhenAsyncBindFunctionThrows_Async_Sync()
        {
            // Arrange
            var result = UniTask.FromResult(Result<int, Exception>.Success(42));

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionThrows,
                ex => ex);

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual(_exception, bindResult.Error);
        }

        [Test]
        public async Task Result_BindSafe_ShouldReturnFailure_WhenAsyncBindFunctionReturnsFailure_Async_Sync()
        {
            // Arrange
            var result = UniTask.FromResult(Result<int, string>.Success(42));

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionFailure,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Error message", bindResult.Error);
        }
#endif

#if NOPE_AWAITABLE
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Awaitable<Result<int, string>> BindFunctionSuccessAwaitable(int value)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return Result<int, string>.Success(value * 2);
        }
        
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Awaitable<Result<int, string>> BindFunctionFailureAwaitable(int value)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return Result<int, string>.Failure("Error message");
        }
        
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Awaitable<Result<int, Exception>> BindFunctionThrowsAwaitable(int value)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            throw _exception;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Awaitable<Result<int, string>> CreateSuccessResult(int value)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return Result<int, string>.Success(value);
        }
        
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Awaitable<Result<int, string>> CreateFailureResult(string errorMessage)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return Result<int, string>.Failure(errorMessage);
        }
        
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Awaitable<Result<int, Exception>> CreateSuccessExceptionResult(int value)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return Result<int, Exception>.Success(value);
        }
        
                [Test]
        public async Task Result_BindSafe_ShouldReturnSuccess_WhenResultIsSuccess_Async()
        {
            // Arrange
            Result<int, string> result = 42;

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionSuccessAwaitable,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsSuccess);
            Assert.AreEqual(84, bindResult.Value);
        }

        [Test]
        public async Task Result_BindSafe_ShouldReturnFailure_WhenResultIsFailure_Async()
        {
            // Arrange
            Result<int, string> result = "Error message";

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionSuccessAwaitable,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Error message", bindResult.Error);
        }

        [Test]
        public async Task Result_BindSafe_ShouldReturnFailure_WhenStartResultIsFailure_Async()
        {
            // Arrange
            Result<int, string> result = "Start error message";

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionSuccessAwaitable,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Start error message", bindResult.Error);
        }

        [Test]
        public async Task Result_BindSafe_ShouldReturnFailure_WhenBindFunctionThrows_Async()
        {
            // Arrange
            Result<int, Exception> result = 42;

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionThrowsAwaitable,
                ex => ex);

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual(_exception, bindResult.Error);
        }

        [Test]
        public async Task Result_BindSafe_ShouldReturnFailure_WhenBindFunctionReturnsFailure_Async()
        {
            // Arrange
            Result<int, string> result = 42;

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionFailureAwaitable,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Error message", bindResult.Error);
        }

        [Test]
        public async Task Result_BindSafe_ShouldReturnSuccess_WhenAsyncResultIsSuccess_Async()
        {
            // Arrange
            var result = CreateSuccessResult(42);

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionSuccessAwaitable,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsSuccess);
            Assert.AreEqual(84, bindResult.Value);
        }

        [Test]
        public async Task Result_BindSafe_ShouldReturnFailure_WhenAsyncResultIsFailure_Async()
        {
            // Arrange
            var result = CreateFailureResult("Error message");

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionSuccessAwaitable,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Error message", bindResult.Error);
        }

        [Test]
        public async Task Result_BindSafe_ShouldReturnFailure_WhenAsyncBindFunctionThrows_Async()
        {
            // Arrange
            var result = CreateSuccessExceptionResult(42);

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionThrowsAwaitable,
                ex => ex);

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual(_exception, bindResult.Error);
        }

        [Test]
        public async Task Result_BindSafe_ShouldReturnFailure_WhenAsyncBindFunctionReturnsFailure_Async()
        {
            // Arrange
            var result = CreateSuccessResult(42);

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionFailureAwaitable,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Error message", bindResult.Error);
        }
        
        
        [Test]
        public async Task Result_BindSafe_ShouldReturnSuccess_WhenAsyncResultIsSuccess_Async_Sync()
        {
            // Arrange
            var result = CreateSuccessResult(42);

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionSuccess,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsSuccess);
            Assert.AreEqual(84, bindResult.Value);
        }

        [Test]
        public async Task Result_BindSafe_ShouldReturnFailure_WhenAsyncResultIsFailure_Async_Sync()
        {
            // Arrange
            var result = CreateFailureResult("Error message");

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionSuccess,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Error message", bindResult.Error);
        }

        [Test]
        public async Task Result_BindSafe_ShouldReturnFailure_WhenAsyncStartResultIsFailure_Async_Sync()
        {
            // Arrange
            var result = CreateFailureResult("Start error message");

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionSuccess,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Start error message", bindResult.Error);
        }

        [Test]
        public async Task Result_BindSafe_ShouldReturnFailure_WhenAsyncBindFunctionThrows_Async_Sync()
        {
            // Arrange
            var result = CreateSuccessExceptionResult(42);

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionThrows,
                ex => ex);

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual(_exception, bindResult.Error);
        }

        [Test]
        public async Task Result_BindSafe_ShouldReturnFailure_WhenAsyncBindFunctionReturnsFailure_Async_Sync()
        {
            // Arrange
            var result = CreateSuccessResult(42);

            // Act
            var bindResult = await result.BindSafe(
                BindFunctionFailure,
                ex => ex + " message");

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Error message", bindResult.Error);
        }
#endif
    }
}