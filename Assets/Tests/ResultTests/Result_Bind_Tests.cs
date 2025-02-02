using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NOPE.Runtime.Core.Result;
using NUnit.Framework;
using UnityEngine;

namespace NOPE.Tests.ResultTests
{
    [TestFixture]
    public class Result_Bind_Tests
    {
        [Test]
        public void Result_Bind_ShouldReturnSuccess_WhenResultIsSuccess()
        {
            // Arrange
            Result<int, string> result = 42;

            // Act
            var bindResult = result.Bind(value => Result.Of(() => value * 2, ex => ex + " message"));

            // Assert
            Assert.IsTrue(bindResult.IsSuccess);
            Assert.AreEqual(84, bindResult.Value);
        }

        [Test]
        public void Result_Bind_ShouldReturnFailure_WhenResultIsFailure()
        {
            // Arrange
            Result<int, string> result = "Error message";

            // Act
            var bindResult = result.Bind(value => Result.Of(() => value * 2, ex => ex + " message"));

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Error message", bindResult.Error);
        }
        
#if NOPE_UNITASK
        [Test]
        public async Task Result_Bind_ShouldReturnSuccess_WhenResultIsSuccess_Sync_to_UniTask()
        {
            // Arrange
            Result<int, string> result = 42;

            // Act
            var bindResult = await result.Bind(value => Result.Of(async () =>
            {
                await UniTask.Delay(1);
                return value * 2;
            }, ex => ex + " message"));

            // Assert
            Assert.IsTrue(bindResult.IsSuccess);
            Assert.AreEqual(84, bindResult.Value);
        }
        
        [Test]
        public async Task Result_Bind_ShouldReturnFailure_WhenResultIsFailure_Sync_to_UniTask()
        {
            // Arrange
            Result<int, string> result = "Error message";

            // Act
            var bindResult = await result.Bind(value => Result.Of(async () =>
            {
                await UniTask.Delay(1);
                return value * 2;
            }, ex => ex + " message"));

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Error message", bindResult.Error);
        }
        
        [Test]
        public async Task Result_Bind_ShouldReturnSuccess_WhenResultIsSuccess_UniTask_to_Sync()
        {
            // Arrange
            UniTask<Result<int, string>> result = UniTask.FromResult(Result<int, string>.Success(42));

            // Act
            var bindResult = await result.Bind(value => Result.Of(() => value * 2, ex => ex + " message"));

            // Assert
            Assert.IsTrue(bindResult.IsSuccess);
            Assert.AreEqual(84, bindResult.Value);
        }
        
        [Test]
        public async Task Result_Bind_ShouldReturnFailure_WhenResultIsFailure_UniTask_to_Sync()
        {
            // Arrange
            UniTask<Result<int, string>> result = UniTask.FromResult(Result<int, string>.Failure("Error message"));

            // Act
            var bindResult = await result.Bind(value => Result.Of(() => value * 2, ex => ex + " message"));

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Error message", bindResult.Error);
        }
        
        [Test]
        public async Task Result_Bind_ShouldReturnSuccess_WhenResultIsSuccess_UniTask_to_UniTask()
        {
            // Arrange
            UniTask<Result<int, string>> result = UniTask.FromResult(Result<int, string>.Success(42));

            // Act
            var bindResult = await result.Bind(value => Result.Of(async () =>
            {
                await UniTask.Delay(1);
                return value * 2;
            }, ex => ex + " message"));

            // Assert
            Assert.IsTrue(bindResult.IsSuccess);
            Assert.AreEqual(84, bindResult.Value);
        }
        
        [Test]
        public async Task Result_Bind_ShouldReturnFailure_WhenResultIsFailure_UniTask_to_UniTask()
        {
            // Arrange
            UniTask<Result<int, string>> result = UniTask.FromResult(Result<int, string>.Failure("Error message"));

            // Act
            var bindResult = await result.Bind(value => Result.Of(async () =>
            {
                await UniTask.Delay(1);
                return value * 2;
            }, ex => ex + " message"));

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Error message", bindResult.Error);
        }
#endif

#if NOPE_AWAITABLE
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Awaitable<Result<int, string>> TaskOfSuccessResult(int value)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return Result<int, string>.Success(value);
        }
        
        private async Awaitable<Result<int, string>> TaskOfFailureResult(string error)
        {
            return Result<int, string>.Failure(error);
        }
        
        [Test]
        public async Task Result_Bind_ShouldReturnSuccess_WhenResultIsSuccess_Sync_to_Awaitable()
        {
            // Arrange
            Result<int, string> result = 42;

            // Act
            var bindResult = await result.Bind(async value => await Result.Of(async () =>
            {
                await Task.Delay(1);
                return value * 2;
            }, ex => ex + " message"));

            // Assert
            Assert.IsTrue(bindResult.IsSuccess);
            Assert.AreEqual(84, bindResult.Value);
        }
        
        [Test]
        public async Task Result_Bind_ShouldReturnFailure_WhenResultIsFailure_Sync_to_Awaitable()
        {
            // Arrange
            Result<int, string> result = "Error message";

            // Act
            var bindResult = await result.Bind(async value => await Result.Of(async () =>
            {
                await Task.Delay(1);
                return value * 2;
            }, ex => ex + " message"));

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Error message", bindResult.Error);
        }
        
        [Test]
        public async Task Result_Bind_ShouldReturnSuccess_WhenResultIsSuccess_Awaitable_to_Sync()
        {
            // Arrange
            var result = TaskOfSuccessResult(42);

            // Act
            var bindResult = await result.Bind(value => Result.Of(() => value * 2, ex => ex + " message"));

            // Assert
            Assert.IsTrue(bindResult.IsSuccess);
            Assert.AreEqual(84, bindResult.Value);
        }
        
        [Test]
        public async Task Result_Bind_ShouldReturnFailure_WhenResultIsFailure_Awaitable_to_Sync()
        {
            // Arrange
            var result = TaskOfFailureResult("Error message");

            // Act
            var bindResult = await result.Bind(value => Result.Of(() => value * 2, ex => ex + " message"));

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Error message", bindResult.Error);
        }
        
        [Test]
        public async Task Result_Bind_ShouldReturnSuccess_WhenResultIsSuccess_Awaitable_to_Awaitable()
        {
            // Arrange
            var result = TaskOfSuccessResult(42);

            // Act
            var bindResult = await result.Bind(value => Result.Of(async () =>
            {
                await Task.Delay(1);
                return value * 2;
            }, ex => ex + " message"));

            // Assert
            Assert.IsTrue(bindResult.IsSuccess);
            Assert.AreEqual(84, bindResult.Value);
        }
        
        [Test]
        public async Task Result_Bind_ShouldReturnFailure_WhenResultIsFailure_Awaitable_to_Awaitable()
        {
            // Arrange
            var result = TaskOfFailureResult("Error message");

            // Act
            var bindResult = await result.Bind(value => Result.Of(async () =>
            {
                await Task.Delay(1);
                return value * 2;
            }, ex => ex + " message"));

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Error message", bindResult.Error);
        }
#endif
    

#if NOPE_UNITASK
        #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async UniTask<Result<int, string>> TaskOfSuccessResult(int value)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return Result<int, string>.Success(value);
        }
        
#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
        private async UniTask<Result<int, string>> TaskOfFailureResult(string error)
#pragma warning restore CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
        {
            return Result<int, string>.Failure(error);
        }
        
        [Test]
        public async Task Result_Bind_ShouldReturnSuccess_WhenResultIsSuccess_Sync_to_Awaitable()
        {
            // Arrange
            Result<int, string> result = 42;

            // Act
            var bindResult = await result.Bind(async value => await Result.Of(async () =>
            {
                await Task.Delay(1);
                return value * 2;
            }, ex => ex + " message"));

            // Assert
            Assert.IsTrue(bindResult.IsSuccess);
            Assert.AreEqual(84, bindResult.Value);
        }
        
        [Test]
        public async Task Result_Bind_ShouldReturnFailure_WhenResultIsFailure_Sync_to_Awaitable()
        {
            // Arrange
            Result<int, string> result = "Error message";

            // Act
            var bindResult = await result.Bind(async value => await Result.Of(async () =>
            {
                await Task.Delay(1);
                return value * 2;
            }, ex => ex + " message"));

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Error message", bindResult.Error);
        }
        
        [Test]
        public async Task Result_Bind_ShouldReturnSuccess_WhenResultIsSuccess_Awaitable_to_Sync()
        {
            // Arrange
            var result = TaskOfSuccessResult(42);

            // Act
            var bindResult = await result.Bind(value => Result.Of(() => value * 2, ex => ex + " message"));

            // Assert
            Assert.IsTrue(bindResult.IsSuccess);
            Assert.AreEqual(84, bindResult.Value);
        }
        
        [Test]
        public async Task Result_Bind_ShouldReturnFailure_WhenResultIsFailure_Awaitable_to_Sync()
        {
            // Arrange
            var result = TaskOfFailureResult("Error message");

            // Act
            var bindResult = await result.Bind(value => Result.Of(() => value * 2, ex => ex + " message"));

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Error message", bindResult.Error);
        }
        
        [Test]
        public async Task Result_Bind_ShouldReturnSuccess_WhenResultIsSuccess_Awaitable_to_Awaitable()
        {
            // Arrange
            var result = TaskOfSuccessResult(42);

            // Act
            var bindResult = await result.Bind(value => Result.Of(async () =>
            {
                await Task.Delay(1);
                return value * 2;
            }, ex => ex + " message"));

            // Assert
            Assert.IsTrue(bindResult.IsSuccess);
            Assert.AreEqual(84, bindResult.Value);
        }
        
        [Test]
        public async Task Result_Bind_ShouldReturnFailure_WhenResultIsFailure_Awaitable_to_Awaitable()
        {
            // Arrange
            var result = TaskOfFailureResult("Error message");

            // Act
            var bindResult = await result.Bind(value => Result.Of(async () =>
            {
                await Task.Delay(1);
                return value * 2;
            }, ex => ex + " message"));

            // Assert
            Assert.IsTrue(bindResult.IsFailure);
            Assert.AreEqual("Error message", bindResult.Error);
        }
#endif
    }
}