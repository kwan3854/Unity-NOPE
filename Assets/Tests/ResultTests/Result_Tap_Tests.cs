using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NOPE.Runtime.Core.Result;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
#if NOPE_AWAITABLE

#endif

namespace NOPE.Tests.ResultTests
{
    /// <summary>
    /// ResultExtensions의 Tap 메서드에 대한 유닛 테스트입니다.
    /// 각 테스트는 결과(Result)가 성공인 경우 액션(또는 비동기 액션)이 실행되고,
    /// 실패인 경우 실행되지 않으며, 항상 동일한 Result가 반환되는지 확인합니다.
    /// </summary>
    [TestFixture]
    public class ResultTapTests
    {
        // ----------------------------------------------------
        // 동기 메서드 (Action<T>와 파라미터 없는 Action) 테스트
        // ----------------------------------------------------

        [Test]
        public void Tap_WithActionT_WhenResultIsSuccess_ExecutesAction()
        {
            // Arrange: 성공 결과 생성
            var result = Result<int, string>.Success(42);
            bool actionExecuted = false;

            // Act: Action<T>를 사용하여 Tap 호출
            var tappedResult = result.Tap(value => { actionExecuted = true; });

            // Assert: 액션이 실행되었고 원래의 Result가 반환되어야 함
            Assert.IsTrue(actionExecuted, "Result가 성공일 때 Action<T>가 실행되어야 합니다.");
            Assert.AreEqual(result, tappedResult, "Tap 메서드는 원래의 Result를 그대로 반환해야 합니다.");
        }

        [Test]
        public void Tap_WithActionT_WhenResultIsFailure_DoesNotExecuteAction()
        {
            // Arrange: 실패 결과 생성
            var result = Result<int, string>.Failure("error");
            bool actionExecuted = false;

            // Act: Action<T>를 사용하여 Tap 호출
            var tappedResult = result.Tap(value => { actionExecuted = true; });

            // Assert: 액션이 실행되지 않아야 하며 원래의 Result가 반환되어야 함
            Assert.IsFalse(actionExecuted, "Result가 실패일 때 Action<T>는 실행되지 않아야 합니다.");
            Assert.AreEqual(result, tappedResult, "Tap 메서드는 원래의 Result를 그대로 반환해야 합니다.");
        }

        [Test]
        public void Tap_WithAction_WhenResultIsSuccess_ExecutesAction()
        {
            // Arrange: 성공 결과 생성
            var result = Result<int, string>.Success(10);
            bool actionExecuted = false;

            // Act: 파라미터 없는 Action을 사용하여 Tap 호출
            var tappedResult = result.Tap(() => { actionExecuted = true; });

            // Assert: 액션이 실행되었고 원래의 Result가 반환되어야 함
            Assert.IsTrue(actionExecuted, "Result가 성공일 때 파라미터 없는 Action이 실행되어야 합니다.");
            Assert.AreEqual(result, tappedResult, "Tap 메서드는 원래의 Result를 그대로 반환해야 합니다.");
        }

        [Test]
        public void Tap_WithAction_WhenResultIsFailure_DoesNotExecuteAction()
        {
            // Arrange: 실패 결과 생성
            var result = Result<int, string>.Failure("error");
            bool actionExecuted = false;

            // Act: 파라미터 없는 Action을 사용하여 Tap 호출
            var tappedResult = result.Tap(() => { actionExecuted = true; });

            // Assert: 액션이 실행되지 않아야 하며 원래의 Result가 반환되어야 함
            Assert.IsFalse(actionExecuted, "Result가 실패일 때 파라미터 없는 Action은 실행되지 않아야 합니다.");
            Assert.AreEqual(result, tappedResult, "Tap 메서드는 원래의 Result를 그대로 반환해야 합니다.");
        }

#if NOPE_UNITASK
        // ----------------------------------------------------
        // UniTask 기반 비동기 메서드 테스트
        // ----------------------------------------------------

        [UnityTest]
        public IEnumerator Tap_WithAsyncActionT_WhenResultIsSuccess_ExecutesAsyncAction() => UniTask.ToCoroutine(async () =>
        {
            // Arrange: 성공 결과 생성
            var result = Result<int, string>.Success(100);
            bool asyncActionExecuted = false;

            // Act: 비동기 Action<T>를 사용하여 Tap 호출
            var tappedResult = await result.Tap(async value =>
            {
                asyncActionExecuted = true;
                await UniTask.Yield(); // 간단한 비동기 대기
            });

            // Assert
            Assert.IsTrue(asyncActionExecuted, "Result가 성공일 때 비동기 Action<T>가 실행되어야 합니다.");
            Assert.AreEqual(result, tappedResult, "Tap 메서드는 원래의 Result를 그대로 반환해야 합니다.");
        });

        [UnityTest]
        public IEnumerator Tap_WithAsyncActionT_WhenResultIsFailure_DoesNotExecuteAsyncAction() => UniTask.ToCoroutine(async () =>
        {
            // Arrange: 실패 결과 생성
            var result = Result<int, string>.Failure("error");
            bool asyncActionExecuted = false;

            // Act: 비동기 Action<T>를 사용하여 Tap 호출
            var tappedResult = await result.Tap(async value =>
            {
                asyncActionExecuted = true;
                await UniTask.Yield();
            });

            // Assert
            Assert.IsFalse(asyncActionExecuted, "Result가 실패일 때 비동기 Action<T>는 실행되지 않아야 합니다.");
            Assert.AreEqual(result, tappedResult, "Tap 메서드는 원래의 Result를 그대로 반환해야 합니다.");
        });

        [UnityTest]
        public IEnumerator Tap_WithAsyncAction_WhenResultIsSuccess_ExecutesAsyncAction() => UniTask.ToCoroutine(async () =>
        {
            // Arrange: 성공 결과 생성
            var result = Result<int, string>.Success(200);
            bool asyncActionExecuted = false;

            // Act: 파라미터 없는 비동기 Action을 사용하여 Tap 호출
            var tappedResult = await result.Tap(async () =>
            {
                asyncActionExecuted = true;
                await UniTask.Yield();
            });

            // Assert
            Assert.IsTrue(asyncActionExecuted, "Result가 성공일 때 파라미터 없는 비동기 Action이 실행되어야 합니다.");
            Assert.AreEqual(result, tappedResult, "Tap 메서드는 원래의 Result를 그대로 반환해야 합니다.");
        });

        [UnityTest]
        public IEnumerator Tap_WithAsyncAction_WhenResultIsFailure_DoesNotExecuteAsyncAction() => UniTask.ToCoroutine(async () =>
        {
            // Arrange: 실패 결과 생성
            var result = Result<int, string>.Failure("error");
            bool asyncActionExecuted = false;

            // Act: 파라미터 없는 비동기 Action을 사용하여 Tap 호출
            var tappedResult = await result.Tap(async () =>
            {
                asyncActionExecuted = true;
                await UniTask.Yield();
            });

            // Assert
            Assert.IsFalse(asyncActionExecuted, "Result가 실패일 때 파라미터 없는 비동기 Action은 실행되지 않아야 합니다.");
            Assert.AreEqual(result, tappedResult, "Tap 메서드는 원래의 Result를 그대로 반환해야 합니다.");
        });

        // UniTask<Result<T, E>>를 대상으로 한 확장 메서드 테스트
        [UnityTest]
        public IEnumerator Tap_WithAsyncActionT_OnUniTaskResult_WhenResultIsSuccess_ExecutesAsyncAction() => UniTask.ToCoroutine(async () =>
        {
            // Arrange: 성공 결과 생성
            var result = Result<int, string>.Success(300);
            bool asyncActionExecuted = false;
            var asyncResult = UniTask.FromResult(result);

            // Act
            var tappedResult = await asyncResult.Tap(async value =>
            {
                asyncActionExecuted = true;
                await UniTask.Yield();
            });

            // Assert
            Assert.IsTrue(asyncActionExecuted, "UniTask<Result>가 성공일 때 비동기 Action<T>가 실행되어야 합니다.");
            Assert.AreEqual(result, tappedResult, "Tap 메서드는 원래의 Result를 그대로 반환해야 합니다.");
        });

        [UnityTest]
        public IEnumerator Tap_WithAsyncActionT_OnUniTaskResult_WhenResultIsFailure_DoesNotExecuteAsyncAction() => UniTask.ToCoroutine(async () =>
        {
            // Arrange: 실패 결과 생성
            var result = Result<int, string>.Failure("error");
            bool asyncActionExecuted = false;
            var asyncResult = UniTask.FromResult(result);

            // Act
            var tappedResult = await asyncResult.Tap(async value =>
            {
                asyncActionExecuted = true;
                await UniTask.Yield();
            });

            // Assert
            Assert.IsFalse(asyncActionExecuted, "UniTask<Result>가 실패일 때 비동기 Action<T>는 실행되지 않아야 합니다.");
            Assert.AreEqual(result, tappedResult, "Tap 메서드는 원래의 Result를 그대로 반환해야 합니다.");
        });

        [UnityTest]
        public IEnumerator Tap_WithAsyncAction_OnUniTaskResult_WhenResultIsSuccess_ExecutesAsyncAction() => UniTask.ToCoroutine(async () =>
        {
            // Arrange: 성공 결과 생성
            var result = Result<int, string>.Success(400);
            bool asyncActionExecuted = false;
            var asyncResult = UniTask.FromResult(result);

            // Act
            var tappedResult = await asyncResult.Tap(async () =>
            {
                asyncActionExecuted = true;
                await UniTask.Yield();
            });

            // Assert
            Assert.IsTrue(asyncActionExecuted, "UniTask<Result>가 성공일 때 파라미터 없는 비동기 Action이 실행되어야 합니다.");
            Assert.AreEqual(result, tappedResult, "Tap 메서드는 원래의 Result를 그대로 반환해야 합니다.");
        });

        [UnityTest]
        public IEnumerator Tap_WithAsyncAction_OnUniTaskResult_WhenResultIsFailure_DoesNotExecuteAsyncAction() => UniTask.ToCoroutine(async () =>
        {
            // Arrange: 실패 결과 생성
            var result = Result<int, string>.Failure("error");
            bool asyncActionExecuted = false;
            var asyncResult = UniTask.FromResult(result);

            // Act
            var tappedResult = await asyncResult.Tap(async () =>
            {
                asyncActionExecuted = true;
                await UniTask.Yield();
            });

            // Assert
            Assert.IsFalse(asyncActionExecuted, "UniTask<Result>가 실패일 때 파라미터 없는 비동기 Action은 실행되지 않아야 합니다.");
            Assert.AreEqual(result, tappedResult, "Tap 메서드는 원래의 Result를 그대로 반환해야 합니다.");
        });
#endif

#if NOPE_AWAITABLE
        // ----------------------------------------------------
        // Awaitable 기반 비동기 메서드 테스트 (Awaitable 타입의 API에 맞게 수정)
        // ----------------------------------------------------
#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
        private async Awaitable EmptyAwaitable()
#pragma warning restore CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
        {
            return;
        }

        [Test]
        public async System.Threading.Tasks.Task Tap_Awaitable_WithAsyncActionT_WhenResultIsSuccess_ExecutesAction()
        {
            // Arrange: 성공 결과 생성
            var result = Result<int, string>.Success(500);
            bool asyncActionExecuted = false;

            // Act: Awaitable을 사용하는 비동기 액션 호출 (예: Awaitable.CompletedTask 사용)
            var tappedResult = await result.Tap(async value =>
            {
                asyncActionExecuted = true;
                await EmptyAwaitable();
            });

            // Assert
            Assert.IsTrue(asyncActionExecuted, "Result가 성공일 때 Awaitable 기반 비동기 Action<T>가 실행되어야 합니다.");
            Assert.AreEqual(result, tappedResult, "Tap 메서드는 원래의 Result를 그대로 반환해야 합니다.");
        }

        [Test]
        public async System.Threading.Tasks.Task Tap_Awaitable_WithAsyncActionT_WhenResultIsFailure_DoesNotExecuteAction()
        {
            // Arrange: 실패 결과 생성
            var result = Result<int, string>.Failure("error");
            bool asyncActionExecuted = false;

            // Act
            var tappedResult = await result.Tap(async value =>
            {
                asyncActionExecuted = true;
                await EmptyAwaitable();
            });

            // Assert
            Assert.IsFalse(asyncActionExecuted, "Result가 실패일 때 Awaitable 기반 비동기 Action<T>는 실행되지 않아야 합니다.");
            Assert.AreEqual(result, tappedResult, "Tap 메서드는 원래의 Result를 그대로 반환해야 합니다.");
        }
#endif
    }
}
