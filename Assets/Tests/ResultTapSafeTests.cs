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
    public class ResultTapSafeTests
    {
        //--------------------------------------------------------------------------
        // 1) 동기 TapSafe 테스트
        //--------------------------------------------------------------------------

        [Test]
        public void TapSafe_Sync_Success_ShouldInvokeAction()
        {
            // Arrange
            var initialResult = Result<int, string>.Success(42);
            bool sideEffectCalled = false;

            // Act
            var finalResult = initialResult.TapSafe(
                value =>
                {
                    sideEffectCalled = true; 
                    Assert.AreEqual(42, value);
                },
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(finalResult.IsSuccess);
            Assert.AreEqual(42, finalResult.Value);
            Assert.IsTrue(sideEffectCalled, "Side effect action should have been called on success.");
        }

        [Test]
        public void TapSafe_Sync_SuccessActionThrows_ShouldReturnFailure()
        {
            // Arrange
            var initialResult = Result<int, string>.Success(42);

            // Act
            var finalResult = initialResult.TapSafe(
                value =>
                {
                    throw new InvalidOperationException("Test Exception");
                    return;
                },
                ex => $"Error captured: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(finalResult.IsSuccess);
            Assert.AreEqual("Error captured: Test Exception", finalResult.Error);
        }

        [Test]
        public void TapSafe_Sync_Failure_ShouldNotInvokeAction()
        {
            // Arrange
            var initialResult = Result<int, string>.Failure("Original Failure");
            bool sideEffectCalled = false;

            // Act
            var finalResult = initialResult.TapSafe(
                value => sideEffectCalled = true,
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(finalResult.IsSuccess);
            Assert.AreEqual("Original Failure", finalResult.Error);
            Assert.IsFalse(sideEffectCalled, "Side effect should not be called on failure.");
        }

        [Test]
        public void TapSafe_Sync_Success_ActionWithoutValue()
        {
            // Arrange
            var initialResult = Result<int, string>.Success(100);
            bool sideEffectCalled = false;

            // Act
            var finalResult = initialResult.TapSafe(
                action: () => { sideEffectCalled = true; },
                errorHandler: ex => $"Error captured: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(finalResult.IsSuccess);
            Assert.AreEqual(100, finalResult.Value);
            Assert.IsTrue(sideEffectCalled);
        }

        [Test]
        public void TapSafe_Sync_Success_ActionWithoutValue_ThrowsException()
        {
            // Arrange
            var initialResult = Result<int, string>.Success(123);

            // Act
            var finalResult = initialResult.TapSafe(
                action: () => throw new InvalidOperationException("No value action error!"),
                errorHandler: ex => $"Error captured: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(finalResult.IsSuccess);
            Assert.AreEqual("Error captured: No value action error!", finalResult.Error);
        }

        [Test]
        public void TapSafe_Sync_Failure_ActionWithoutValue_NotCalled()
        {
            // Arrange
            var initialResult = Result<int, string>.Failure("Failure!");
            bool sideEffectCalled = false;

            // Act
            var finalResult = initialResult.TapSafe(
                action: () => sideEffectCalled = true,
                errorHandler: ex => $"ShouldNotBeCalled: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(finalResult.IsSuccess);
            Assert.AreEqual("Failure!", finalResult.Error);
            Assert.IsFalse(sideEffectCalled);
        }

#if NOPE_UNITASK
        //--------------------------------------------------------------------------
        // 2) UniTask TapSafe 테스트
        //--------------------------------------------------------------------------

        [Test]
        public async Task TapSafe_UniTask_Success_ShouldInvokeAsyncAction()
        {
            // Arrange
            var initialResult = Result<int, string>.Success(999);
            bool sideEffectCalled = false;

            // Act
            var finalResult = await initialResult.TapSafe(
                asyncAction: async value =>
                {
                    await UniTask.DelayFrame(1);
                    sideEffectCalled = true;
                    Assert.AreEqual(999, value);
                },
                errorHandler: ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(finalResult.IsSuccess);
            Assert.AreEqual(999, finalResult.Value);
            Assert.IsTrue(sideEffectCalled);
        }

        [Test]
        public async Task TapSafe_UniTask_Success_AsyncActionThrows_ShouldReturnFailure()
        {
            // Arrange
            var initialResult = Result<int, string>.Success(123);

            // Act
            var finalResult = await initialResult.TapSafe(
                asyncAction: async value =>
                {
                    await UniTask.DelayFrame(1);
                    throw new InvalidOperationException("Async tap error!");
                },
                errorHandler: ex => $"Handled: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(finalResult.IsSuccess);
            Assert.AreEqual("Handled: Async tap error!", finalResult.Error);
        }

        [Test]
        public async Task TapSafe_UniTask_Failure_ShouldNotInvokeAsyncAction()
        {
            // Arrange
            var initialResult = Result<int, string>.Failure("Already failed");
            bool sideEffectCalled = false;

            // Act
            var finalResult = await initialResult.TapSafe(
                asyncAction: async value =>
                {
                    await UniTask.DelayFrame(1);
                    sideEffectCalled = true;
                },
                errorHandler: ex => $"ShouldNotRun: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(finalResult.IsSuccess);
            Assert.AreEqual("Already failed", finalResult.Error);
            Assert.IsFalse(sideEffectCalled);
        }

        [Test]
        public async Task TapSafe_UniTask_AsyncResult_Success_ShouldInvokeActionWithValue()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(777));
            bool sideEffectCalled = false;

            // Act
            var finalResult = await asyncResult.TapSafe(
                action: value =>
                {
                    sideEffectCalled = true;
                    Assert.AreEqual(777, value);
                },
                errorHandler: ex => $"Error caught: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(finalResult.IsSuccess);
            Assert.AreEqual(777, finalResult.Value);
            Assert.IsTrue(sideEffectCalled);
        }

        [Test]
        public async Task TapSafe_UniTask_AsyncResult_Success_ActionThrows()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(12));

            // Act
            var finalResult = await asyncResult.TapSafe(
                action: _ => throw new Exception("Tap error!"),
                errorHandler: ex => $"Caught: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(finalResult.IsSuccess);
            Assert.AreEqual("Caught: Tap error!", finalResult.Error);
        }

        [Test]
        public async Task TapSafe_UniTask_AsyncResult_Failure_NoActionInvoke()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Failure("Async Failure"));
            bool sideEffectCalled = false;

            // Act
            var finalResult = await asyncResult.TapSafe(
                action: value => sideEffectCalled = true,
                errorHandler: ex => $"NotUsed: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(finalResult.IsSuccess);
            Assert.AreEqual("Async Failure", finalResult.Error);
            Assert.IsFalse(sideEffectCalled);
        }

        [Test]
        public async Task TapSafe_UniTask_AsyncResult_AsyncAction_Success()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(42));
            bool sideEffectCalled = false;

            // Act
            var finalResult = await asyncResult.TapSafe(
                asyncAction: async value =>
                {
                    await UniTask.DelayFrame(1);
                    sideEffectCalled = true;
                },
                errorHandler: ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(finalResult.IsSuccess);
            Assert.AreEqual(42, finalResult.Value);
            Assert.IsTrue(sideEffectCalled);
        }

        [Test]
        public async Task TapSafe_UniTask_AsyncResult_AsyncAction_Throws()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(101));

            // Act
            var finalResult = await asyncResult.TapSafe(
                asyncAction: async value =>
                {
                    await UniTask.DelayFrame(1);
                    throw new Exception("AsyncAction error!");
                },
                errorHandler: ex => $"Handled: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(finalResult.IsSuccess);
            Assert.AreEqual("Handled: AsyncAction error!", finalResult.Error);
        }

        [Test]
        public async Task TapSafe_UniTask_AsyncResult_AsyncAction_Failure_NoInvoke()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Failure("UniTask Failure"));
            bool sideEffectCalled = false;

            // Act
            var finalResult = await asyncResult.TapSafe(
                asyncAction: async value =>
                {
                    await UniTask.DelayFrame(1);
                    sideEffectCalled = true;
                },
                errorHandler: ex => $"NotUsed: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(finalResult.IsSuccess);
            Assert.AreEqual("UniTask Failure", finalResult.Error);
            Assert.IsFalse(sideEffectCalled);
        }

        [Test]
        public async Task TapSafe_UniTask_Success_ActionWithoutValue()
        {
            // Arrange
            var initialResult = Result<int, string>.Success(555);
            bool sideEffectCalled = false;

            // Act
            var finalResult = await initialResult.TapSafe(
                asyncAction: async () =>
                {
                    await UniTask.DelayFrame(1);
                    sideEffectCalled = true;
                },
                errorHandler: ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(finalResult.IsSuccess);
            Assert.AreEqual(555, finalResult.Value);
            Assert.IsTrue(sideEffectCalled);
        }

        [Test]
        public async Task TapSafe_UniTask_Failure_ActionWithoutValue_NotCalled()
        {
            // Arrange
            var initialResult = Result<int, string>.Failure("Error 999");
            bool sideEffectCalled = false;

            // Act
            var finalResult = await initialResult.TapSafe(
                asyncAction: async () =>
                {
                    await UniTask.DelayFrame(1);
                    sideEffectCalled = true;
                },
                errorHandler: ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(finalResult.IsSuccess);
            Assert.AreEqual("Error 999", finalResult.Error);
            Assert.IsFalse(sideEffectCalled);
        }

#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        //--------------------------------------------------------------------------
        // 3) Awaitable TapSafe 테스트
        //--------------------------------------------------------------------------

        [Test]
        public async Task TapSafe_Awaitable_Success_ShouldInvokeAsyncActionWithValue()
        {
            // Arrange
            var initialResult = Result<int, string>.Success(2025);
            bool sideEffectCalled = false;

            // Act
            var finalResult = await initialResult.TapSafe(
                asyncAction: async value =>
                {
                    await MyTestAwaitable.Delay(1);
                    sideEffectCalled = true;
                    Assert.AreEqual(2025, value);
                },
                errorHandler: ex => $"AwaitableError: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(finalResult.IsSuccess);
            Assert.AreEqual(2025, finalResult.Value);
            Assert.IsTrue(sideEffectCalled);
        }

        [Test]
        public async Task TapSafe_Awaitable_Success_AsyncActionThrowsException_ShouldReturnFailure()
        {
            // Arrange
            var initialResult = Result<int, string>.Success(999);

            // Act
            var finalResult = await initialResult.TapSafe(
                asyncAction: async value =>
                {
                    await MyTestAwaitable.Delay(1);
                    throw new InvalidOperationException("Awaitable tap error!");
                },
                errorHandler: ex => $"Handled: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(finalResult.IsSuccess);
            Assert.AreEqual("Handled: Awaitable tap error!", finalResult.Error);
        }

        [Test]
        public async Task TapSafe_Awaitable_Failure_ShouldNotInvokeAsyncAction()
        {
            // Arrange
            var initialResult = Result<int, string>.Failure("Awaitable Failure");
            bool sideEffectCalled = false;

            // Act
            var finalResult = await initialResult.TapSafe(
                asyncAction: async value =>
                {
                    await MyTestAwaitable.Delay(1);
                    sideEffectCalled = true;
                },
                errorHandler: ex => $"NotUsed: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(finalResult.IsSuccess);
            Assert.AreEqual("Awaitable Failure", finalResult.Error);
            Assert.IsFalse(sideEffectCalled);
        }

        // 3-2) Awaitable<Result<T,E>> 를 TapSafe
        [Test]
        public async Task TapSafe_Awaitable_AsyncResult_Success_TapWithValue()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Success(777));
            bool sideEffectCalled = false;

            // Act
            var finalResult = await asyncResult.TapSafe(
                action: value =>
                {
                    sideEffectCalled = true;
                    Assert.AreEqual(777, value);
                },
                errorHandler: ex => $"Handled: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(finalResult.IsSuccess);
            Assert.AreEqual(777, finalResult.Value);
            Assert.IsTrue(sideEffectCalled);
        }

        [Test]
        public async Task TapSafe_Awaitable_AsyncResult_Success_TapWithValue_Throws()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Success(111));

            // Act
            var finalResult = await asyncResult.TapSafe(
                action: value => throw new Exception("Tap error!"),
                errorHandler: ex => $"Caught: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(finalResult.IsSuccess);
            Assert.AreEqual("Caught: Tap error!", finalResult.Error);
        }

        [Test]
        public async Task TapSafe_Awaitable_AsyncResult_Failure_NoInvoke()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Failure("No tap"));

            bool sideEffectCalled = false;
            // Act
            var finalResult = await asyncResult.TapSafe(
                action: value => sideEffectCalled = true,
                errorHandler: ex => $"ShouldNotAppear: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(finalResult.IsSuccess);
            Assert.AreEqual("No tap", finalResult.Error);
            Assert.IsFalse(sideEffectCalled);
        }

        [Test]
        public async Task TapSafe_Awaitable_AsyncResult_TapWithAsyncValueAction_Success()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Success(1234));
            bool sideEffectCalled = false;

            // Act
            var finalResult = await asyncResult.TapSafe(
                asyncAction: async value =>
                {
                    await MyTestAwaitable.Delay(1);
                    sideEffectCalled = true;
                    Assert.AreEqual(1234, value);
                },
                errorHandler: ex => $"Err: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(finalResult.IsSuccess);
            Assert.AreEqual(1234, finalResult.Value);
            Assert.IsTrue(sideEffectCalled);
        }

        [Test]
        public async Task TapSafe_Awaitable_AsyncResult_TapWithAsyncValueAction_Throws()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Success(33));

            // Act
            var finalResult = await asyncResult.TapSafe(
                asyncAction: async value =>
                {
                    await MyTestAwaitable.Delay(1);
                    throw new Exception("Async tap error!");
                },
                errorHandler: ex => $"Handled: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(finalResult.IsSuccess);
            Assert.AreEqual("Handled: Async tap error!", finalResult.Error);
        }

        [Test]
        public async Task TapSafe_Awaitable_AsyncResult_Failure_AsyncValueActionNoInvoke()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Failure("Failure!"));
            bool sideEffectCalled = false;

            // Act
            var finalResult = await asyncResult.TapSafe(
                asyncAction: async value =>
                {
                    await MyTestAwaitable.Delay(1);
                    sideEffectCalled = true;
                },
                errorHandler: ex => $"NotUsed: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(finalResult.IsSuccess);
            Assert.AreEqual("Failure!", finalResult.Error);
            Assert.IsFalse(sideEffectCalled);
        }

        [Test]
        public async Task TapSafe_Awaitable_Success_ActionWithoutValue()
        {
            // Arrange
            var initialResult = Result<int, string>.Success(9999);
            bool sideEffectCalled = false;

            // Act
            var finalResult = await initialResult.TapSafe(
                asyncAction: async () =>
                {
                    await MyTestAwaitable.Delay(1);
                    sideEffectCalled = true;
                },
                errorHandler: ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(finalResult.IsSuccess);
            Assert.AreEqual(9999, finalResult.Value);
            Assert.IsTrue(sideEffectCalled);
        }

        [Test]
        public async Task TapSafe_Awaitable_Failure_ActionWithoutValue_NotCalled()
        {
            // Arrange
            var initialResult = Result<int, string>.Failure("Fail on 2025");
            bool sideEffectCalled = false;

            // Act
            var finalResult = await initialResult.TapSafe(
                asyncAction: async () =>
                {
                    await MyTestAwaitable.Delay(1);
                    sideEffectCalled = true;
                },
                errorHandler: ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsFalse(finalResult.IsSuccess);
            Assert.AreEqual("Fail on 2025", finalResult.Error);
            Assert.IsFalse(sideEffectCalled);
        }

        //--------------------------------------------------------------------------
        // MyTestAwaitable 예시 구현
        //--------------------------------------------------------------------------
        private static class MyTestAwaitable
        {
            public static async Awaitable Delay(int frames)
            {
                // 단순 Task.Delay() 예시
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
