using System;
using NOPE.Runtime.Core.Result;
using NUnit.Framework;
using UnityEngine;
#if NOPE_UNITASK
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
#endif
#if NOPE_AWAITABLE
// Awaitable 관련 네임스페이스(예: NOPE.Awaitable)가 있다면 추가하세요.
#endif

namespace NOPE.Tests.ResultTests
{
    /// <summary>
    /// TapSafe 확장 메서드에 대한 유닛 테스트입니다.
    /// </summary>
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public class Result_TapSafe_Tests
    {
        // ----------------------------------------------------
        // 동기 메서드 테스트 (Action<T>와 파라미터 없는 Action)
        // ----------------------------------------------------

        [Test]
        public void TapSafe_WithActionT_WhenResultIsSuccess_ExecutesAction()
        {
            // Arrange: 성공 Result 생성
            var result = Result<int, string>.Success(123);
            bool actionExecuted = false;

            // Act: Action<T>를 통해 TapSafe 호출 (예외 없이 정상 실행)
            var tappedResult = result.TapSafe(_ => { actionExecuted = true; }, ex => "handled: " + ex.Message);

            // Assert: 액션이 실행되고 원래 Result가 반환됨
            Assert.IsTrue(actionExecuted, "Result가 성공일 때 Action<T>가 실행되어야 합니다.");
            Assert.AreEqual(result, tappedResult, "TapSafe는 원래의 Result를 그대로 반환해야 합니다.");
        }

        [Test]
        public void TapSafe_WithActionT_WhenActionThrows_ReturnsFailure()
        {
            // Arrange: 성공 Result 생성
            var result = Result<int, string>.Success(123);
            bool actionExecuted = false;

            // Act: Action<T> 내부에서 예외 발생시킴
            var tappedResult = result.TapSafe(_ =>
            {
                actionExecuted = true;
                throw new Exception("oops");
                
#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.
                return;
#pragma warning restore CS0162 // 접근할 수 없는 코드가 있습니다.
            }, ex => "handled: " + ex.Message);

            // Assert: 액션은 호출되었으나 예외로 인해 실패 Result가 반환됨
            Assert.IsTrue(actionExecuted, "액션이 호출되어야 합니다.");
            Assert.IsFalse(tappedResult.IsSuccess, "예외 발생 시 실패 Result여야 합니다.");
            Assert.AreEqual("handled: oops", tappedResult.Error, "errorHandler가 반환한 값이어야 합니다.");
        }

        [Test]
        public void TapSafe_WithActionT_WhenResultIsFailure_SkipsAction()
        {
            // Arrange: 실패 Result 생성
            var result = Result<int, string>.Failure("original failure");
            bool actionExecuted = false;

            // Act: 실패 Result인 경우 액션은 실행되지 않아야 함
            var tappedResult = result.TapSafe(_ => { actionExecuted = true; }, ex => "handled: " + ex.Message);

            // Assert
            Assert.IsFalse(actionExecuted, "실패 Result인 경우 Action<T>가 실행되어서는 안 됩니다.");
            Assert.IsFalse(tappedResult.IsSuccess, "원래 실패 Result여야 합니다.");
            Assert.AreEqual("original failure", tappedResult.Error, "원래 에러가 그대로 전달되어야 합니다.");
        }

        [Test]
        public void TapSafe_WithParameterlessAction_WhenResultIsSuccess_ExecutesAction()
        {
            // Arrange: 성공 Result 생성
            var result = Result<int, string>.Success(456);
            bool actionExecuted = false;

            // Act: 파라미터 없는 Action 사용
            var tappedResult = result.TapSafe(() => { actionExecuted = true; }, ex => "handled: " + ex.Message);

            // Assert
            Assert.IsTrue(actionExecuted, "성공 Result일 때 파라미터 없는 Action이 실행되어야 합니다.");
            Assert.AreEqual(result, tappedResult, "원래 Result가 그대로 반환되어야 합니다.");
        }

        [Test]
        public void TapSafe_WithParameterlessAction_WhenActionThrows_ReturnsFailure()
        {
            // Arrange: 성공 Result 생성
            var result = Result<int, string>.Success(456);
            bool actionExecuted = false;

            // Act: 파라미터 없는 Action 내부에서 예외 발생
            var tappedResult = result.TapSafe(() =>
            {
                actionExecuted = true;
                throw new Exception("oops");
                
#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.
                return;
#pragma warning restore CS0162 // 접근할 수 없는 코드가 있습니다.
            }, ex => "handled: " + ex.Message);

            // Assert
            Assert.IsTrue(actionExecuted, "액션이 호출되어야 합니다.");
            Assert.IsFalse(tappedResult.IsSuccess, "예외 발생 시 실패 Result여야 합니다.");
            Assert.AreEqual("handled: oops", tappedResult.Error, "errorHandler가 반환한 값이어야 합니다.");
        }

        [Test]
        public void TapSafe_WithParameterlessAction_WhenResultIsFailure_SkipsAction()
        {
            // Arrange: 실패 Result 생성
            var result = Result<int, string>.Failure("original failure");
            bool actionExecuted = false;

            // Act
            var tappedResult = result.TapSafe(() => { actionExecuted = true; }, ex => "handled: " + ex.Message);

            // Assert
            Assert.IsFalse(actionExecuted, "실패 Result인 경우 Action이 실행되어서는 안 됩니다.");
            Assert.IsFalse(tappedResult.IsSuccess, "원래 실패 Result여야 합니다.");
            Assert.AreEqual("original failure", tappedResult.Error, "원래 에러가 그대로 전달되어야 합니다.");
        }

#if NOPE_UNITASK
        // ----------------------------------------------------
        // UniTask 기반 비동기 메서드 테스트
        // ----------------------------------------------------

        [UnityTest]
        public IEnumerator TapSafe_WithAsyncActionT_WhenResultIsSuccess_ExecutesAsyncAction() => UniTask.ToCoroutine(async () =>
        {
            // Arrange: 성공 Result 생성
            var result = Result<int, string>.Success(789);
            bool asyncActionExecuted = false;

            // Act: 비동기 Action<T>를 사용하여 TapSafe 호출
            var tappedResult = await result.TapSafe(async val =>
            {
                asyncActionExecuted = true;
                await UniTask.Yield();
            }, ex => "handled: " + ex.Message);

            // Assert
            Assert.IsTrue(asyncActionExecuted, "성공 Result일 때 비동기 Action<T>가 실행되어야 합니다.");
            Assert.AreEqual(result, tappedResult, "원래 Result가 그대로 반환되어야 합니다.");
        });

        [UnityTest]
        public IEnumerator TapSafe_WithAsyncActionT_WhenAsyncActionThrows_ReturnsFailure() => UniTask.ToCoroutine(async () =>
        {
            // Arrange: 성공 Result 생성
            var result = Result<int, string>.Success(789);
            bool asyncActionExecuted = false;

            // Act: 비동기 Action<T> 내부에서 예외 발생
            var tappedResult = await result.TapSafe(async val =>
            {
                asyncActionExecuted = true;
                await UniTask.Yield();
                throw new Exception("async fail");
            }, ex => "handled: " + ex.Message);

            // Assert
            Assert.IsTrue(asyncActionExecuted, "액션이 호출되어야 합니다.");
            Assert.IsFalse(tappedResult.IsSuccess, "예외 발생 시 실패 Result여야 합니다.");
            Assert.AreEqual("handled: async fail", tappedResult.Error, "errorHandler가 반환한 값이어야 합니다.");
        });

        [UnityTest]
        public IEnumerator TapSafe_WithAsyncActionT_WhenResultIsFailure_SkipsAsyncAction() => UniTask.ToCoroutine(async () =>
        {
            // Arrange: 실패 Result 생성
            var result = Result<int, string>.Failure("original failure");
            bool asyncActionExecuted = false;

            // Act
            var tappedResult = await result.TapSafe(async val =>
            {
                asyncActionExecuted = true;
                await UniTask.Yield();
            }, ex => "handled: " + ex.Message);

            // Assert
            Assert.IsFalse(asyncActionExecuted, "실패 Result일 경우 비동기 Action<T>가 실행되어서는 안 됩니다.");
            Assert.IsFalse(tappedResult.IsSuccess, "원래 실패 Result여야 합니다.");
            Assert.AreEqual("original failure", tappedResult.Error, "원래 에러가 그대로 전달되어야 합니다.");
        });

        [UnityTest]
        public IEnumerator TapSafe_WithAsyncAction_WhenResultIsSuccess_ExecutesAsyncAction() => UniTask.ToCoroutine(async () =>
        {
            // Arrange: 성공 Result 생성
            var result = Result<int, string>.Success(101);
            bool asyncActionExecuted = false;

            // Act: 파라미터 없는 비동기 Action 사용
            var tappedResult = await result.TapSafe(async () =>
            {
                asyncActionExecuted = true;
                await UniTask.Yield();
            }, ex => "handled: " + ex.Message);

            // Assert
            Assert.IsTrue(asyncActionExecuted, "성공 Result일 때 파라미터 없는 비동기 Action이 실행되어야 합니다.");
            Assert.AreEqual(result, tappedResult, "원래 Result가 그대로 반환되어야 합니다.");
        });

        [UnityTest]
        public IEnumerator TapSafe_WithAsyncAction_WhenAsyncActionThrows_ReturnsFailure() => UniTask.ToCoroutine(async () =>
        {
            // Arrange: 성공 Result 생성
            var result = Result<int, string>.Success(101);
            bool asyncActionExecuted = false;

            // Act: 비동기 Action 내부에서 예외 발생
            var tappedResult = await result.TapSafe(async () =>
            {
                asyncActionExecuted = true;
                await UniTask.Yield();
                throw new Exception("async fail");
            }, ex => "handled: " + ex.Message);

            // Assert
            Assert.IsTrue(asyncActionExecuted, "액션이 호출되어야 합니다.");
            Assert.IsFalse(tappedResult.IsSuccess, "예외 발생 시 실패 Result여야 합니다.");
            Assert.AreEqual("handled: async fail", tappedResult.Error, "errorHandler가 반환한 값이어야 합니다.");
        });

        [UnityTest]
        public IEnumerator TapSafe_WithAsyncAction_OnUniTaskResult_WhenResultIsSuccess_ExecutesAsyncAction() => UniTask.ToCoroutine(async () =>
        {
            // Arrange: 성공 Result 생성 및 UniTask<Result<T, E>> 생성
            var result = Result<int, string>.Success(202);
            bool asyncActionExecuted = false;
            var asyncResult = UniTask.FromResult(result);

            // Act
            var tappedResult = await asyncResult.TapSafe(async () =>
            {
                asyncActionExecuted = true;
                await UniTask.Yield();
            }, ex => "handled: " + ex.Message);

            // Assert
            Assert.IsTrue(asyncActionExecuted, "UniTask<Result>가 성공일 때 액션이 실행되어야 합니다.");
            Assert.AreEqual(result, tappedResult, "원래 Result가 그대로 반환되어야 합니다.");
        });

        [UnityTest]
        public IEnumerator TapSafe_WithAsyncAction_OnUniTaskResult_WhenAsyncActionThrows_ReturnsFailure() => UniTask.ToCoroutine(async () =>
        {
            // Arrange: 성공 Result 생성 및 UniTask<Result<T, E>> 생성
            var result = Result<int, string>.Success(202);
            bool asyncActionExecuted = false;
            var asyncResult = UniTask.FromResult(result);

            // Act
            var tappedResult = await asyncResult.TapSafe(async () =>
            {
                asyncActionExecuted = true;
                await UniTask.Yield();
                throw new Exception("async fail");
            }, ex => "handled: " + ex.Message);

            // Assert
            Assert.IsTrue(asyncActionExecuted, "액션이 호출되어야 합니다.");
            Assert.IsFalse(tappedResult.IsSuccess, "예외 발생 시 실패 Result여야 합니다.");
            Assert.AreEqual("handled: async fail", tappedResult.Error, "errorHandler가 반환한 값이어야 합니다.");
        });
#endif

#if NOPE_AWAITABLE
        // ----------------------------------------------------
        // Awaitable 기반 비동기 메서드 테스트
        // ----------------------------------------------------
        // 가짜 EmptyAwaitable 메서드 (앞으로 이렇게 사용)
        private async Awaitable EmptyAwaitable()
        {
            await System.Threading.Tasks.Task.CompletedTask;
            // ReSharper disable once RedundantJumpStatement
            return;
        }
        
#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
        private async Awaitable<T> AwaitableFromResult<T>(T value)
#pragma warning restore CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
        {
            return value;
        }

        [Test]
        public async System.Threading.Tasks.Task TapSafe_WithAsyncActionT_WhenResultIsSuccess_ExecutesAction_Awaitable()
        {
            // Arrange: 성공 Result 생성
            var result = Result<int, string>.Success(303);
            bool asyncActionExecuted = false;

            // Act
            var tappedResult = await result.TapSafe(async _ =>
            {
                asyncActionExecuted = true;
                await EmptyAwaitable();
            }, ex => "handled: " + ex.Message);

            // Assert
            Assert.IsTrue(asyncActionExecuted, "성공 Result일 때 Awaitable 기반 Action<T>가 실행되어야 합니다.");
            Assert.AreEqual(result, tappedResult, "원래 Result가 그대로 반환되어야 합니다.");
        }

        [Test]
        public async System.Threading.Tasks.Task TapSafe_WithAsyncActionT_WhenAsyncActionThrows_ReturnsFailure_Awaitable()
        {
            // Arrange: 성공 Result 생성
            var result = Result<int, string>.Success(303);
            bool asyncActionExecuted = false;

            // Act
            var tappedResult = await result.TapSafe(async _ =>
            {
                asyncActionExecuted = true;
                await EmptyAwaitable();
                throw new Exception("awaitable fail");
            }, ex => "handled: " + ex.Message);

            // Assert
            Assert.IsTrue(asyncActionExecuted, "액션이 호출되어야 합니다.");
            Assert.IsFalse(tappedResult.IsSuccess, "예외 발생 시 실패 Result여야 합니다.");
            Assert.AreEqual("handled: awaitable fail", tappedResult.Error, "errorHandler가 반환한 값이어야 합니다.");
        }

        [Test]
        public async System.Threading.Tasks.Task TapSafe_WithAsyncActionT_WhenResultIsFailure_SkipsAction_Awaitable()
        {
            // Arrange: 실패 Result 생성
            var result = Result<int, string>.Failure("original failure");
            bool asyncActionExecuted = false;

            // Act
            var tappedResult = await result.TapSafe(async _ =>
            {
                asyncActionExecuted = true;
                await EmptyAwaitable();
            }, ex => "handled: " + ex.Message);

            // Assert
            Assert.IsFalse(asyncActionExecuted, "실패 Result인 경우 Awaitable 기반 Action<T>가 실행되어서는 안 됩니다.");
            Assert.IsFalse(tappedResult.IsSuccess, "원래 실패 Result여야 합니다.");
            Assert.AreEqual("original failure", tappedResult.Error, "원래 에러가 그대로 전달되어야 합니다.");
        }

        [Test]
        public async System.Threading.Tasks.Task TapSafe_WithAsyncAction_WhenResultIsSuccess_ExecutesAsyncAction_Awaitable()
        {
            // Arrange: 성공 Result 생성
            var result = Result<int, string>.Success(404);
            bool asyncActionExecuted = false;

            // Act: 파라미터 없는 비동기 Action 사용
            var tappedResult = await result.TapSafe(async () =>
            {
                asyncActionExecuted = true;
                await EmptyAwaitable();
            }, ex => "handled: " + ex.Message);

            // Assert
            Assert.IsTrue(asyncActionExecuted, "성공 Result일 때 파라미터 없는 Awaitable 기반 Action이 실행되어야 합니다.");
            Assert.AreEqual(result, tappedResult, "원래 Result가 그대로 반환되어야 합니다.");
        }

        [Test]
        public async System.Threading.Tasks.Task TapSafe_WithAsyncAction_WhenAsyncActionThrows_ReturnsFailure_Awaitable()
        {
            // Arrange: 성공 Result 생성
            var result = Result<int, string>.Success(404);
            bool asyncActionExecuted = false;

            // Act: 파라미터 없는 비동기 Action 내부에서 예외 발생
            var tappedResult = await result.TapSafe(async () =>
            {
                asyncActionExecuted = true;
                await EmptyAwaitable();
                throw new Exception("awaitable fail");
            }, ex => "handled: " + ex.Message);

            // Assert
            Assert.IsTrue(asyncActionExecuted, "액션이 호출되어야 합니다.");
            Assert.IsFalse(tappedResult.IsSuccess, "예외 발생 시 실패 Result여야 합니다.");
            Assert.AreEqual("handled: awaitable fail", tappedResult.Error, "errorHandler가 반환한 값이어야 합니다.");
        }

        [Test]
        public async System.Threading.Tasks.Task TapSafe_WithAsyncAction_OnAwaitableResult_WhenResultIsSuccess_ExecutesAsyncAction_Awaitable()
        {
            // Arrange: 성공 Result 생성 및 Awaitable<Result<T, E>> 생성 (Awaitable.FromResult 사용)
            var result = Result<int, string>.Success(505);
            bool asyncActionExecuted = false;
            var asyncResult = AwaitableFromResult(result);

            // Act
            var tappedResult = await asyncResult.TapSafe(async () =>
            {
                asyncActionExecuted = true;
                await EmptyAwaitable();
            }, ex => "handled: " + ex.Message);

            // Assert
            Assert.IsTrue(asyncActionExecuted, "Awaitable<Result>가 성공일 때 액션이 실행되어야 합니다.");
            Assert.AreEqual(result, tappedResult, "원래 Result가 그대로 반환되어야 합니다.");
        }

        [Test]
        public async System.Threading.Tasks.Task TapSafe_WithAsyncAction_OnAwaitableResult_WhenAsyncActionThrows_ReturnsFailure_Awaitable()
        {
            // Arrange: 성공 Result 생성 및 Awaitable<Result<T, E>> 생성
            var result = Result<int, string>.Success(505);
            bool asyncActionExecuted = false;
            var asyncResult = AwaitableFromResult(result);

            // Act
            var tappedResult = await asyncResult.TapSafe(async () =>
            {
                asyncActionExecuted = true;
                await EmptyAwaitable();
                throw new Exception("awaitable fail");
            }, ex => "handled: " + ex.Message);

            // Assert
            Assert.IsTrue(asyncActionExecuted, "액션이 호출되어야 합니다.");
            Assert.IsFalse(tappedResult.IsSuccess, "예외 발생 시 실패 Result여야 합니다.");
            Assert.AreEqual("handled: awaitable fail", tappedResult.Error, "errorHandler가 반환한 값이어야 합니다.");
        }
#endif
    }
}
