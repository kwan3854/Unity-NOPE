using System;
using NOPE.Runtime.Core.Result;
using NUnit.Framework;
using System.Threading.Tasks;
using UnityEngine;

#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace NOPE.Tests
{
    [TestFixture]
    public class ResultBindSafeTests
    {
        //==========================================================================
        // 1) 동기 버전 BindSafe 테스트
        //==========================================================================

        [Test]
        public void BindSafe_Sync_SuccessToSuccess_ShouldReturnSuccess()
        {
            // Arrange
            var initialResult = Result<string, string>.Success("InitialValue");

            // Act
            var boundResult = initialResult.BindSafe(
                value => Result<int, string>.Success(value.Length),
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(boundResult.IsSuccess);
            Assert.AreEqual("InitialValue".Length, boundResult.Value);
        }

        [Test]
        public void BindSafe_Sync_SuccessToFailure_ShouldReturnFailure()
        {
            // Arrange
            var initialResult = Result<string, string>.Success("InitialValue");

            // Act
            var boundResult = initialResult.BindSafe(
                _ => Result<int, string>.Failure("Binder returned Error"),
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(boundResult.IsSuccess);
            Assert.AreEqual("Binder returned Error", boundResult.Error);
        }

        [Test]
        public void BindSafe_Sync_Success_IfBinderThrowsException_ShouldReturnFailureWithErrorSelectorMessage()
        {
            // Arrange
            var initialResult = Result<string, string>.Success("InitialValue");

            // Act
            var boundResult = initialResult.BindSafe(
                _ =>
                {
                    throw new InvalidOperationException("Oops!");
                    
                    // fake return for compiler
#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.
                    return Result<int, string>.Success(0);
#pragma warning restore CS0162 // 접근할 수 없는 코드가 있습니다.
                },
                ex => $"ExceptionCaught: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(boundResult.IsSuccess);
            Assert.AreEqual("ExceptionCaught: Oops!", boundResult.Error);
        }

        [Test]
        public void BindSafe_Sync_InitialFailure_ShouldSkipBinderAndReturnOriginalFailure()
        {
            // Arrange
            var initialResult = Result<string, string>.Failure("Original Error");

            // Act
            var boundResult = initialResult.BindSafe(
                value => Result<int, string>.Success(value.Length),
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(boundResult.IsSuccess);
            Assert.AreEqual("Original Error", boundResult.Error);
        }

#if NOPE_UNITASK
        //==========================================================================
        // 2) UniTask 비동기 버전 BindSafe 테스트
        //==========================================================================

        [Test]
        public async Task BindSafe_UniTask_SuccessToSuccess_ShouldReturnSuccess()
        {
            // Arrange
            var initialResult = Result<string, string>.Success("AsyncInitial");

            // Act
            var boundResult = await initialResult.BindSafe(
                async value =>
                {
                    await UniTask.DelayFrame(1);
                    return Result<int, string>.Success(value.Length);
                },
                ex => $"UniTaskError: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(boundResult.IsSuccess);
            Assert.AreEqual("AsyncInitial".Length, boundResult.Value);
        }

        [Test]
        public async Task BindSafe_UniTask_SuccessToFailure_ShouldReturnFailure()
        {
            // Arrange
            var initialResult = Result<string, string>.Success("AsyncInitial");

            // Act
            var boundResult = await initialResult.BindSafe(
                async _ =>
                {
                    await UniTask.DelayFrame(1);
                    return Result<int, string>.Failure("Async Binder Error");
                },
                ex => $"UniTaskError: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(boundResult.IsSuccess);
            Assert.AreEqual("Async Binder Error", boundResult.Error);
        }

        [Test]
        public async Task BindSafe_UniTask_Success_IfBinderThrowsException_ShouldReturnFailure()
        {
            // Arrange
            var initialResult = Result<string, string>.Success("AsyncInitial");

            // Act
            var boundResult = await initialResult.BindSafe(
                async _ =>
                {
                    await UniTask.DelayFrame(1);
                    throw new InvalidOperationException("UniTaskException!");
                    
                    // fake return for compiler
#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.
                    return Result<int, string>.Success(0);
#pragma warning restore CS0162 // 접근할 수 없는 코드가 있습니다.
                },
                ex => $"UniTaskExceptionCaught: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(boundResult.IsSuccess);
            Assert.AreEqual("UniTaskExceptionCaught: UniTaskException!", boundResult.Error);
        }

        [Test]
        public async Task BindSafe_UniTask_InitialFailure_ShouldSkipBinderAndReturnOriginalFailure()
        {
            // Arrange
            var initialResult = Result<string, string>.Failure("Original UniTask Error");

            // Act
            var boundResult = await initialResult.BindSafe(
                async value =>
                {
                    await UniTask.DelayFrame(1);
                    return Result<int, string>.Success(value.Length);
                },
                ex => $"UniTaskError: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(boundResult.IsSuccess);
            Assert.AreEqual("Original UniTask Error", boundResult.Error);
        }

        //==========================================================================
        // 2-2) UniTask on an asyncResult (UniTask<Result<T,E>> 자체)를 BindSafe
        //==========================================================================

        [Test]
        public async Task BindSafe_UniTask_FromAsyncResult_SuccessToSuccess_ShouldReturnSuccess()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<string, string>.Success("AsyncInitial"));

            // Act
            var boundResult = await asyncResult.BindSafe(
                value => Result<int, string>.Success(value.Length),
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(boundResult.IsSuccess);
            Assert.AreEqual("AsyncInitial".Length, boundResult.Value);
        }

        [Test]
        public async Task BindSafe_UniTask_FromAsyncResult_SuccessToFailure_ShouldReturnFailure()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<string, string>.Success("AsyncInitial"));

            // Act
            var boundResult = await asyncResult.BindSafe(
                _ => Result<int, string>.Failure("Direct Binder Failure"),
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(boundResult.IsSuccess);
            Assert.AreEqual("Direct Binder Failure", boundResult.Error);
        }

        [Test]
        public async Task BindSafe_UniTask_FromAsyncResult_Success_IfBinderThrows_ShouldReturnFailure()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<string, string>.Success("AsyncInitial"));

            // Act
            var boundResult = await asyncResult.BindSafe(
                _ =>
                {
                    throw new Exception("Binder exploded");

                    // fake return for compiler
#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.
                    return Result<int, string>.Success(0);
#pragma warning restore CS0162 // 접근할 수 없는 코드가 있습니다.
                },
                ex => $"ExceptionCaught: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(boundResult.IsSuccess);
            Assert.AreEqual("ExceptionCaught: Binder exploded", boundResult.Error);
        }

        [Test]
        public async Task BindSafe_UniTask_FromAsyncResult_InitialFailure_ShouldSkipBinder()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<string, string>.Failure("Initial Async Error"));

            // Act
            var boundResult = await asyncResult.BindSafe(
                value => Result<int, string>.Success(value.Length),
                ex => $"ShouldNotRun: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(boundResult.IsSuccess);
            Assert.AreEqual("Initial Async Error", boundResult.Error);
        }

        //==========================================================================
        // 2-3) UniTask on an asyncResult (UniTask<Result<T,E>> 자체)를 BindSafe (Async Binder)
        //==========================================================================

        [Test]
        public async Task BindSafe_UniTask_FromAsyncResult_AsyncBinder_SuccessToSuccess()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<string, string>.Success("AsyncInitial"));

            // Act
            var boundResult = await asyncResult.BindSafe(
                async value =>
                {
                    await UniTask.DelayFrame(1);
                    return Result<int, string>.Success(value.Length);
                },
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(boundResult.IsSuccess);
            Assert.AreEqual("AsyncInitial".Length, boundResult.Value);
        }

        [Test]
        public async Task BindSafe_UniTask_FromAsyncResult_AsyncBinder_Failure()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<string, string>.Success("AsyncInitial"));

            // Act
            var boundResult = await asyncResult.BindSafe(
                async _ =>
                {
                    await UniTask.DelayFrame(1);
                    return Result<int, string>.Failure("AsyncBinder Error");
                },
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(boundResult.IsSuccess);
            Assert.AreEqual("AsyncBinder Error", boundResult.Error);
        }

        [Test]
        public async Task BindSafe_UniTask_FromAsyncResult_AsyncBinder_ThrowsException()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<string, string>.Success("AsyncInitial"));

            // Act
            var boundResult = await asyncResult.BindSafe(
                async _ =>
                {
                    await UniTask.DelayFrame(1);
                    throw new Exception("AsyncBinder exploded");
                    
                    // fake return for compiler
#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.
                    return Result<int, string>.Success(0);
#pragma warning restore CS0162 // 접근할 수 없는 코드가 있습니다.
                },
                ex => $"ExceptionCaught: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(boundResult.IsSuccess);
            Assert.AreEqual("ExceptionCaught: AsyncBinder exploded", boundResult.Error);
        }

        [Test]
        public async Task BindSafe_UniTask_FromAsyncResult_AsyncBinder_InitialFailure()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<string, string>.Failure("Original Async Error"));

            // Act
            var boundResult = await asyncResult.BindSafe(
                async value =>
                {
                    await UniTask.DelayFrame(1);
                    return Result<int, string>.Success(value.Length);
                },
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(boundResult.IsSuccess);
            Assert.AreEqual("Original Async Error", boundResult.Error);
        }
#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        //==========================================================================
        // 3) Awaitable 비동기 버전 BindSafe 테스트
        //    (동일 패턴이지만 실제 Awaitable 구현체에 맞춰야 함)
        //==========================================================================

        [Test]
        public async Task BindSafe_Awaitable_SuccessToSuccess_ShouldReturnSuccess()
        {
            // Arrange
            var initialResult = Result<string, string>.Success("AwaitableInitial");

            // Act
            var boundResult = await initialResult.BindSafe(
                async value =>
                {
                    // 가상의 awaitable 동작
                    await MyTestAwaitable.Delay(1);
                    return Result<int, string>.Success(value.Length);
                },
                ex => $"AwaitableError: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(boundResult.IsSuccess);
            Assert.AreEqual("AwaitableInitial".Length, boundResult.Value);
        }

        [Test]
        public async Task BindSafe_Awaitable_SuccessToFailure_ShouldReturnFailure()
        {
            // Arrange
            var initialResult = Result<string, string>.Success("AwaitableInitial");

            // Act
            var boundResult = await initialResult.BindSafe(
                async _ =>
                {
                    await MyTestAwaitable.Delay(1);
                    return Result<int, string>.Failure("Awaitable Binder Error");
                },
                ex => $"AwaitableError: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(boundResult.IsSuccess);
            Assert.AreEqual("Awaitable Binder Error", boundResult.Error);
        }

        [Test]
        public async Task BindSafe_Awaitable_Success_IfBinderThrowsException_ShouldReturnFailure()
        {
            // Arrange
            var initialResult = Result<string, string>.Success("AwaitableInitial");

            // Act
            var boundResult = await initialResult.BindSafe(
                async _ =>
                {
                    await MyTestAwaitable.Delay(1);
                    throw new InvalidOperationException("AwaitableException!");
                    
                    // fake return for compiler
#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.
                    return Result<int, string>.Success(0);
#pragma warning restore CS0162 // 접근할 수 없는 코드가 있습니다.
                },
                ex => $"AwaitableExceptionCaught: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(boundResult.IsSuccess);
            Assert.AreEqual("AwaitableExceptionCaught: AwaitableException!", boundResult.Error);
        }

        [Test]
        public async Task BindSafe_Awaitable_InitialFailure_ShouldSkipBinderAndReturnOriginalFailure()
        {
            // Arrange
            var initialResult = Result<string, string>.Failure("Original Awaitable Error");

            // Act
            var boundResult = await initialResult.BindSafe(
                async value =>
                {
                    await MyTestAwaitable.Delay(1);
                    return Result<int, string>.Success(value.Length);
                },
                ex => $"AwaitableError: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(boundResult.IsSuccess);
            Assert.AreEqual("Original Awaitable Error", boundResult.Error);
        }

        //==========================================================================
        // 3-2) Awaitable on an asyncResult (Awaitable<Result<T,E>>) 자체를 BindSafe
        //==========================================================================

        [Test]
        public async Task BindSafe_Awaitable_FromAsyncResult_SuccessToSuccess_ShouldReturnSuccess()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<string, string>.Success("AwaitableInitial"));

            // Act
            var boundResult = await asyncResult.BindSafe(
                value => Result<int, string>.Success(value.Length),
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(boundResult.IsSuccess);
            Assert.AreEqual("AwaitableInitial".Length, boundResult.Value);
        }

        [Test]
        public async Task BindSafe_Awaitable_FromAsyncResult_SuccessToFailure_ShouldReturnFailure()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<string, string>.Success("AwaitableInitial"));

            // Act
            var boundResult = await asyncResult.BindSafe(
                _ => Result<int, string>.Failure("Direct Awaitable Binder Failure"),
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(boundResult.IsSuccess);
            Assert.AreEqual("Direct Awaitable Binder Failure", boundResult.Error);
        }

        [Test]
        public async Task BindSafe_Awaitable_FromAsyncResult_Success_IfBinderThrows_ShouldReturnFailure()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<string, string>.Success("AwaitableInitial"));

            // Act
            var boundResult = await asyncResult.BindSafe(
                _ =>
                {
                    throw new Exception("Awaitable Binder exploded");
                    
                    // fake return for compiler
#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.
                    return Result<int, string>.Success(0);
#pragma warning restore CS0162 // 접근할 수 없는 코드가 있습니다.
                },
                ex => $"ExceptionCaught: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(boundResult.IsSuccess);
            Assert.AreEqual("ExceptionCaught: Awaitable Binder exploded", boundResult.Error);
        }

        [Test]
        public async Task BindSafe_Awaitable_FromAsyncResult_InitialFailure_ShouldSkipBinder()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<string, string>.Failure("Initial Awaitable Error"));

            // Act
            var boundResult = await asyncResult.BindSafe(
                value => Result<int, string>.Success(value.Length),
                ex => $"ShouldNotRun: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(boundResult.IsSuccess);
            Assert.AreEqual("Initial Awaitable Error", boundResult.Error);
        }

        // AsyncBinder 버전
        [Test]
        public async Task BindSafe_Awaitable_FromAsyncResult_AsyncBinder_SuccessToSuccess()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<string, string>.Success("AwaitableInitial"));

            // Act
            var boundResult = await asyncResult.BindSafe(
                async value =>
                {
                    await MyTestAwaitable.Delay(1);
                    return Result<int, string>.Success(value.Length);
                },
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(boundResult.IsSuccess);
            Assert.AreEqual("AwaitableInitial".Length, boundResult.Value);
        }

        [Test]
        public async Task BindSafe_Awaitable_FromAsyncResult_AsyncBinder_Failure()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<string, string>.Success("AwaitableInitial"));

            // Act
            var boundResult = await asyncResult.BindSafe(
                async _ =>
                {
                    await MyTestAwaitable.Delay(1);
                    return Result<int, string>.Failure("Awaitable Binder Error");
                },
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(boundResult.IsSuccess);
            Assert.AreEqual("Awaitable Binder Error", boundResult.Error);
        }

        [Test]
        public async Task BindSafe_Awaitable_FromAsyncResult_AsyncBinder_ThrowsException()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<string, string>.Success("AwaitableInitial"));

            // Act
            var boundResult = await asyncResult.BindSafe(
                async _ =>
                {
                    await MyTestAwaitable.Delay(1);
                    throw new Exception("Awaitable Binder exploded");
                    
                    // fake return for compiler
#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.
                    return Result<int, string>.Success(0);
#pragma warning restore CS0162 // 접근할 수 없는 코드가 있습니다.
                },
                ex => $"ExceptionCaught: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(boundResult.IsSuccess);
            Assert.AreEqual("ExceptionCaught: Awaitable Binder exploded", boundResult.Error);
        }

        [Test]
        public async Task BindSafe_Awaitable_FromAsyncResult_AsyncBinder_InitialFailure()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<string, string>.Failure("Original Awaitable Error"));

            // Act
            var boundResult = await asyncResult.BindSafe(
                async value =>
                {
                    await MyTestAwaitable.Delay(1);
                    return Result<int, string>.Success(value.Length);
                },
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(boundResult.IsSuccess);
            Assert.AreEqual("Original Awaitable Error", boundResult.Error);
        }

        //--------------------------------------------------------------------------
        // MyTestAwaitable 예시 (실제 프로젝트의 Awaitable 구현을 사용하세요.)
        //--------------------------------------------------------------------------
        private static class MyTestAwaitable
        {
            public static async Awaitable Delay(int frames)
            {
                // Task.Delay 사용 등, 실제 대기 로직
                await Task.Delay(10 * frames);
            }

            public static async Awaitable<Result<T, E>> FromResult<T, E>(Result<T, E> result)
            {
                // 실제 구현에 맞게 await 가능하도록 리턴
                await Task.Yield();
                return result;
            }
        }
#endif // NOPE_AWAITABLE
    }
}
