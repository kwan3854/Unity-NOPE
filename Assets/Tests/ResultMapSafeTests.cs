using System;
using System.Threading.Tasks;
using NOPE.Runtime.Core.Result;
using NUnit.Framework;
using UnityEngine;

#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace NOPE.Tests
{
    [TestFixture]
    public class ResultMapSafeTests
    {
        //--------------------------------------------------------------------------
        // 1) 동기 버전 MapSafe
        //--------------------------------------------------------------------------

        [Test]
        public void MapSafe_Sync_SuccessToSuccess_ShouldReturnSuccess()
        {
            // Arrange
            var originalResult = Result<string, string>.Success("Hello");

            // Act
            var mappedResult = originalResult.MapSafe(
                original => original.Length,
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mappedResult.IsSuccess);
            Assert.AreEqual("Hello".Length, mappedResult.Value);
        }

        [Test]
        public void MapSafe_Sync_Success_IfSelectorThrowsException_ShouldReturnFailure()
        {
            // Arrange
            var originalResult = Result<string, string>.Success("Hello");

            // Act
            var mappedResult = originalResult.MapSafe(
                _ =>
                {
                    throw new InvalidOperationException("Selector Error!");
                    
                    // fake return value for compiler
#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.
                    return 0;
#pragma warning restore CS0162 // 접근할 수 없는 코드가 있습니다.
                },
                ex => $"Exception caught: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(mappedResult.IsSuccess);
            Assert.AreEqual("Exception caught: Selector Error!", mappedResult.Error);
        }

        [Test]
        public void MapSafe_Sync_InitialFailure_ShouldSkipSelectorAndReturnOriginalFailure()
        {
            // Arrange
            var originalResult = Result<string, string>.Failure("Original Failure");

            // Act
            var mappedResult = originalResult.MapSafe(
                _ => 999, // 이 코드는 실행되지 않아야 함
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(mappedResult.IsSuccess);
            Assert.AreEqual("Original Failure", mappedResult.Error);
        }

#if NOPE_UNITASK
        //--------------------------------------------------------------------------
        // 2) UniTask 버전 MapSafe
        //--------------------------------------------------------------------------

        [Test]
        public async Task MapSafe_UniTask_SuccessToSuccess_ShouldReturnSuccess()
        {
            // Arrange
            var originalResult = Result<string, string>.Success("UniTaskValue");

            // Act
            var mappedResult = await originalResult.MapSafe(
                async original =>
                {
                    await UniTask.DelayFrame(1);
                    return original.Length;
                },
                ex => $"UniTaskError: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mappedResult.IsSuccess);
            Assert.AreEqual("UniTaskValue".Length, mappedResult.Value);
        }

        [Test]
        public async Task MapSafe_UniTask_Success_IfSelectorThrows_ShouldReturnFailure()
        {
            // Arrange
            var originalResult = Result<string, string>.Success("UniTaskValue");

            // Act
            var mappedResult = await originalResult.MapSafe(
                async _ =>
                {
                    await UniTask.DelayFrame(1);
                    throw new Exception("Async Selector crashed!");
                    
                    // fake return value for compiler
#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.
                    return 0;
#pragma warning restore CS0162 // 접근할 수 없는 코드가 있습니다.
                },
                ex => $"CaughtException: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(mappedResult.IsSuccess);
            Assert.AreEqual("CaughtException: Async Selector crashed!", mappedResult.Error);
        }

        [Test]
        public async Task MapSafe_UniTask_InitialFailure_ShouldSkipSelectorAndReturnOriginalFailure()
        {
            // Arrange
            var originalResult = Result<string, string>.Failure("Original UniTask Failure");

            // Act
            var mappedResult = await originalResult.MapSafe(
                async _ =>
                {
                    await UniTask.DelayFrame(1);
                    return 9999;
                },
                ex => $"SomeError: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(mappedResult.IsSuccess);
            Assert.AreEqual("Original UniTask Failure", mappedResult.Error);
        }

        // 2-2) UniTask<Result<T,E>>를 직접 MapSafe
        [Test]
        public async Task MapSafe_UniTask_AsyncResult_SuccessToSuccess()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<string, string>.Success("AsyncResultValue"));

            // Act
            var mappedResult = await asyncResult.MapSafe(
                original => original.Length,
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mappedResult.IsSuccess);
            Assert.AreEqual("AsyncResultValue".Length, mappedResult.Value);
        }

        [Test]
        public async Task MapSafe_UniTask_AsyncResult_Success_IfSelectorThrows()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<string, string>.Success("AsyncResultValue"));

            // Act
            var mappedResult = await asyncResult.MapSafe(
                _ =>
                {
                    throw new Exception("Map explosion!");
                    
                    // fake return value for compiler
#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.
                    return 0;
#pragma warning restore CS0162 // 접근할 수 없는 코드가 있습니다.
                },
                ex => $"Caught: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(mappedResult.IsSuccess);
            Assert.AreEqual("Caught: Map explosion!", mappedResult.Error);
        }

        [Test]
        public async Task MapSafe_UniTask_AsyncResult_InitialFailure()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<string, string>.Failure("Already Failed"));

            // Act
            var mappedResult = await asyncResult.MapSafe(
                original => original.Length, // 실행되지 않아야 함
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(mappedResult.IsSuccess);
            Assert.AreEqual("Already Failed", mappedResult.Error);
        }

        // 2-3) UniTask<Result<T,E>>를 직접 MapSafe (비동기 selector)
        [Test]
        public async Task MapSafe_UniTask_AsyncResult_AsyncSelector_SuccessToSuccess()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<string, string>.Success("AsyncResultValue"));

            // Act
            var mappedResult = await asyncResult.MapSafe(
                async original =>
                {
                    await UniTask.DelayFrame(1);
                    return original.Length;
                },
                ex => $"SelectorError: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mappedResult.IsSuccess);
            Assert.AreEqual("AsyncResultValue".Length, mappedResult.Value);
        }

        [Test]
        public async Task MapSafe_UniTask_AsyncResult_AsyncSelector_ThrowsException()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<string, string>.Success("AsyncResultValue"));

            // Act
            var mappedResult = await asyncResult.MapSafe(
                async _ =>
                {
                    await UniTask.DelayFrame(1);
                    throw new Exception("Async map explosion!");
                    
                    // fake return value for compiler
#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.
                    return 0;
#pragma warning restore CS0162 // 접근할 수 없는 코드가 있습니다.
                },
                ex => $"SelectorError: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(mappedResult.IsSuccess);
            Assert.AreEqual("SelectorError: Async map explosion!", mappedResult.Error);
        }

        [Test]
        public async Task MapSafe_UniTask_AsyncResult_AsyncSelector_InitialFailure()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<string, string>.Failure("Already Failed"));

            // Act
            var mappedResult = await asyncResult.MapSafe(
                async original =>
                {
                    await UniTask.DelayFrame(1);
                    return original.Length; // 실행되지 않아야 함
                },
                ex => $"SelectorError: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(mappedResult.IsSuccess);
            Assert.AreEqual("Already Failed", mappedResult.Error);
        }
#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        //--------------------------------------------------------------------------
        // 3) Awaitable 버전 MapSafe
        //--------------------------------------------------------------------------

        [Test]
        public async Task MapSafe_Awaitable_SuccessToSuccess_ShouldReturnSuccess()
        {
            // Arrange
            var originalResult = Result<string, string>.Success("AwaitableValue");

            // Act
            var mappedResult = await originalResult.MapSafe(
                async original =>
                {
                    // 실제 Awaitable 구현에 맞게 대기
                    await MyTestAwaitable.Delay(1);
                    return original.Length;
                },
                ex => $"AwaitableError: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mappedResult.IsSuccess);
            Assert.AreEqual("AwaitableValue".Length, mappedResult.Value);
        }

        [Test]
        public async Task MapSafe_Awaitable_Success_IfSelectorThrows_ShouldReturnFailure()
        {
            // Arrange
            var originalResult = Result<string, string>.Success("AwaitableValue");

            // Act
            var mappedResult = await originalResult.MapSafe(
                async _ =>
                {
                    await MyTestAwaitable.Delay(1);
                    throw new Exception("Selector threw!");
                    
                    // fake return value for compiler
#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.
                    return 0;
#pragma warning restore CS0162 // 접근할 수 없는 코드가 있습니다.
                },
                ex => $"AwaitableException: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(mappedResult.IsSuccess);
            Assert.AreEqual("AwaitableException: Selector threw!", mappedResult.Error);
        }

        [Test]
        public async Task MapSafe_Awaitable_InitialFailure_ShouldSkipSelectorAndReturnOriginalFailure()
        {
            // Arrange
            var originalResult = Result<string, string>.Failure("Awaitable Original Failure");

            // Act
            var mappedResult = await originalResult.MapSafe(
                async _ =>
                {
                    await MyTestAwaitable.Delay(1);
                    return 9999; // 실행되지 않아야 함
                },
                ex => $"ShouldNotReach: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(mappedResult.IsSuccess);
            Assert.AreEqual("Awaitable Original Failure", mappedResult.Error);
        }

        // 3-2) Awaitable<Result<T,E>> 자체를 MapSafe (동기 selector)
        [Test]
        public async Task MapSafe_Awaitable_AsyncResult_SuccessToSuccess()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<string, string>.Success("AwaitableValue"));

            // Act
            var mappedResult = await asyncResult.MapSafe(
                original => original.Length,
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mappedResult.IsSuccess);
            Assert.AreEqual("AwaitableValue".Length, mappedResult.Value);
        }

        [Test]
        public async Task MapSafe_Awaitable_AsyncResult_SuccessToFailure_IfSelectorThrows()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<string, string>.Success("AwaitableValue"));

            // Act
            var mappedResult = await asyncResult.MapSafe(
                async _ =>
                {
                    await MyTestAwaitable.Delay(1);
                    throw new Exception("Awaitable map exception!");
                    
                    // fake return value for compiler
#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.
                    return 0;
#pragma warning restore CS0162 // 접근할 수 없는 코드가 있습니다.
                },
                ex => $"Caught: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(mappedResult.IsSuccess);
            Assert.AreEqual("Caught: Awaitable map exception!", mappedResult.Error);
        }

        [Test]
        public async Task MapSafe_Awaitable_AsyncResult_InitialFailure()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<string, string>.Failure("Already Failed"));

            // Act
            var mappedResult = await asyncResult.MapSafe(
                original => original.Length, // 실행되지 않아야 함
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(mappedResult.IsSuccess);
            Assert.AreEqual("Already Failed", mappedResult.Error);
        }

        // 3-3) Awaitable<Result<T,E>> 자체를 MapSafe (비동기 selector)
        [Test]
        public async Task MapSafe_Awaitable_AsyncResult_AsyncSelector_SuccessToSuccess()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<string, string>.Success("AwaitableValue"));

            // Act
            var mappedResult = await asyncResult.MapSafe(
                async original =>
                {
                    await MyTestAwaitable.Delay(1);
                    return original.Length;
                },
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mappedResult.IsSuccess);
            Assert.AreEqual("AwaitableValue".Length, mappedResult.Value);
        }

        [Test]
        public async Task MapSafe_Awaitable_AsyncResult_AsyncSelector_ThrowsException()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<string, string>.Success("AwaitableValue"));

            // Act
            var mappedResult = await asyncResult.MapSafe(
                async _ =>
                {
                    await MyTestAwaitable.Delay(1);
                    throw new Exception("Async map explosion!");
                    
                    // fake return value for compiler
#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.
                    return 0;
#pragma warning restore CS0162 // 접근할 수 없는 코드가 있습니다.
                },
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(mappedResult.IsSuccess);
            Assert.AreEqual("Error: Async map explosion!", mappedResult.Error);
        }

        [Test]
        public async Task MapSafe_Awaitable_AsyncResult_AsyncSelector_InitialFailure()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<string, string>.Failure("Already Failed"));

            // Act
            var mappedResult = await asyncResult.MapSafe(
                async original =>
                {
                    await MyTestAwaitable.Delay(1);
                    return original.Length; // 실행되지 않아야 함
                },
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(mappedResult.IsSuccess);
            Assert.AreEqual("Already Failed", mappedResult.Error);
        }

        //--------------------------------------------------------------------------
        // 간단한 MyTestAwaitable 구현
        //--------------------------------------------------------------------------
        private static class MyTestAwaitable
        {
            public static async Awaitable Delay(int frames)
            {
                // 예시로 Task.Delay 사용
                await Task.Delay(10 * frames);
            }

            public static async Awaitable<Result<T, E>> FromResult<T, E>(Result<T, E> result)
            {
                await Task.Yield();
                return result;
            }
        }
#endif // NOPE_AWAITABLE
    }
}
