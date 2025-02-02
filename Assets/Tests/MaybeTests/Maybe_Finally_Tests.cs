using System.Collections;
using Cysharp.Threading.Tasks;
using NOPE.Runtime.Core.Maybe;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace NOPE.Tests.MaybeTests
{
    /// <summary>
    /// Maybe의 Finally 확장 메서드에 대한 유닛 테스트입니다.
    /// </summary>
    [TestFixture]
    public class Maybe_Finally_Tests
    {
        #region 동기 테스트

        [Test]
        public void Finally_Sync_WithValue_ExecutesFinalActionAndReturnsOriginalMaybe()
        {
            // Arrange
            var maybe = Maybe<int>.From(42);
            bool actionCalled = false;
            Maybe<int> received = default;

            // Act
            var result = maybe.Finally(m => { actionCalled = true; received = m; });

            // Assert
            Assert.IsTrue(actionCalled, "최종 액션은 값이 있을 때 호출되어야 합니다.");
            // received와 result가 원래의 maybe와 동일해야 함
            Assert.AreEqual(maybe, received, "최종 액션에 전달된 값은 원래의 Maybe와 동일해야 합니다.");
            Assert.AreEqual(maybe, result, "Finally 메서드는 원래의 Maybe를 그대로 반환해야 합니다.");
        }

        [Test]
        public void Finally_Sync_WithNone_ExecutesFinalActionAndReturnsOriginalMaybe()
        {
            // Arrange
            var maybe = Maybe<int>.None;
            bool actionCalled = false;
            Maybe<int> received = default;

            // Act
            var result = maybe.Finally(m => { actionCalled = true; received = m; });

            // Assert
            Assert.IsTrue(actionCalled, "최종 액션은 None 상태에서도 호출되어야 합니다.");
            Assert.AreEqual(maybe, received, "최종 액션에 전달된 값은 원래의 None과 동일해야 합니다.");
            Assert.AreEqual(maybe, result, "Finally 메서드는 원래의 Maybe(None)을 그대로 반환해야 합니다.");
        }

        #endregion

#if NOPE_UNITASK
        #region UniTask 기반 비동기 테스트

        // Finally(this Maybe<T> maybe, Func<Maybe<T>, UniTask> finalAction)
        [UnityTest]
        public IEnumerator Finally_UniTask_WithValue_ExecutesFinalActionAndReturnsOriginalMaybe() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.From(100);
            bool actionCalled = false;
            Maybe<int> received = default;

            // Act
            var result = await maybe.Finally(async m =>
            {
                await UniTask.Yield();
                actionCalled = true;
                received = m;
            });

            // Assert
            Assert.IsTrue(actionCalled, "비동기 최종 액션은 값이 있을 때 호출되어야 합니다.");
            Assert.AreEqual(maybe, received, "비동기 최종 액션에 전달된 값은 원래의 Maybe와 동일해야 합니다.");
            Assert.AreEqual(maybe, result, "Finally 메서드는 원래의 Maybe를 그대로 반환해야 합니다.");
        });

        [UnityTest]
        public IEnumerator Finally_UniTask_WithNone_ExecutesFinalActionAndReturnsOriginalMaybe() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.None;
            bool actionCalled = false;
            Maybe<int> received = default;

            // Act
            var result = await maybe.Finally(async m =>
            {
                await UniTask.Yield();
                actionCalled = true;
                received = m;
            });

            // Assert
            Assert.IsTrue(actionCalled, "비동기 최종 액션은 None 상태에서도 호출되어야 합니다.");
            Assert.AreEqual(maybe, received, "비동기 최종 액션에 전달된 값은 원래의 None과 동일해야 합니다.");
            Assert.AreEqual(maybe, result, "Finally 메서드는 원래의 Maybe(None)을 그대로 반환해야 합니다.");
        });

        // Finally(this UniTask<Maybe<T>> asyncMaybe, Action<Maybe<T>> finalAction)
        [UnityTest]
        public IEnumerator Finally_UniTask_FromAsyncMaybe_WithValue_ExecutesFinalActionAndReturnsOriginalMaybe() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.From(200);
            var asyncMaybe = UniTask.FromResult(maybe);
            bool actionCalled = false;
            Maybe<int> received = default;

            // Act
            var result = await asyncMaybe.Finally(m =>
            {
                actionCalled = true;
                received = m;
            });

            // Assert
            Assert.IsTrue(actionCalled, "최종 액션은 async 소스에서 값이 있을 때 호출되어야 합니다.");
            Assert.AreEqual(maybe, received, "async 소스에서 전달된 값은 원래의 Maybe와 동일해야 합니다.");
            Assert.AreEqual(maybe, result, "Finally 메서드는 원래의 Maybe를 그대로 반환해야 합니다.");
        });

        // Finally(this UniTask<Maybe<T>> asyncMaybe, Func<Maybe<T>, UniTask> finalAction)
        [UnityTest]
        public IEnumerator Finally_UniTask_FromAsyncMaybe_WithNone_ExecutesFinalActionAndReturnsOriginalMaybe() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.None;
            var asyncMaybe = UniTask.FromResult(maybe);
            bool actionCalled = false;
            Maybe<int> received = default;

            // Act
            var result = await asyncMaybe.Finally(async m =>
            {
                await UniTask.Yield();
                actionCalled = true;
                received = m;
            });

            // Assert
            Assert.IsTrue(actionCalled, "최종 액션은 async 소스에서 None 상태에서도 호출되어야 합니다.");
            Assert.AreEqual(maybe, received, "async 소스에서 전달된 값은 원래의 None과 동일해야 합니다.");
            Assert.AreEqual(maybe, result, "Finally 메서드는 원래의 Maybe(None)을 그대로 반환해야 합니다.");
        });

        #endregion
#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        #region Awaitable 기반 비동기 테스트
        
#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
        private async Awaitable<T> FromResult<T>(T value)
#pragma warning restore CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
        {
            return value;
        }

        // Finally(this Maybe<T> maybe, Func<Maybe<T>, Awaitable> finalAction)
        [Test]
        public async System.Threading.Tasks.Task Finally_Awaitable_WithValue_ExecutesFinalActionAndReturnsOriginalMaybe()
        {
            // Arrange
            var maybe = Maybe<int>.From(300);
            bool actionCalled = false;
            Maybe<int> received = default;

            // Act
            var result = await maybe.Finally(async m =>
            {
                // 가짜 비동기 작업: Awaitable.FromResult 사용
                await FromResult(0);
                actionCalled = true;
                received = m;
            });

            // Assert
            Assert.IsTrue(actionCalled, "Awaitable 최종 액션은 값이 있을 때 호출되어야 합니다.");
            Assert.AreEqual(maybe, received, "Awaitable 최종 액션에 전달된 값은 원래의 Maybe와 동일해야 합니다.");
            Assert.AreEqual(maybe, result, "Finally 메서드는 원래의 Maybe를 그대로 반환해야 합니다.");
        }

        // Finally(this Awaitable<Maybe<T>> asyncMaybe, Action<Maybe<T>> finalAction)
        [Test]
        public async System.Threading.Tasks.Task Finally_Awaitable_FromAsyncMaybe_WithNone_ExecutesFinalActionAndReturnsOriginalMaybe()
        {
            // Arrange
            var maybe = Maybe<int>.None;
            var asyncMaybe = FromResult(maybe);
            bool actionCalled = false;
            Maybe<int> received = default;

            // Act
            var result = await asyncMaybe.Finally(m =>
            {
                actionCalled = true;
                received = m;
            });

            // Assert
            Assert.IsTrue(actionCalled, "Awaitable 소스에서 None 상태에서도 최종 액션이 호출되어야 합니다.");
            Assert.AreEqual(maybe, received, "Awaitable 소스에서 전달된 값은 원래의 None과 동일해야 합니다.");
            Assert.AreEqual(maybe, result, "Finally 메서드는 원래의 Maybe(None)을 그대로 반환해야 합니다.");
        }

        // Finally(this Awaitable<Maybe<T>> asyncMaybe, Func<Maybe<T>, Awaitable> finalAction)
        [Test]
        public async System.Threading.Tasks.Task Finally_Awaitable_FromAsyncMaybe_WithValue_ExecutesFinalActionAndReturnsOriginalMaybe()
        {
            // Arrange
            var maybe = Maybe<int>.From(400);
            var asyncMaybe = FromResult(maybe);
            bool actionCalled = false;
            Maybe<int> received = default;

            // Act
            var result = await asyncMaybe.Finally(async m =>
            {
                await FromResult(0);
                actionCalled = true;
                received = m;
            });

            // Assert
            Assert.IsTrue(actionCalled, "Awaitable 소스에서 값이 있을 때 최종 액션이 호출되어야 합니다.");
            Assert.AreEqual(maybe, received, "Awaitable 소스에서 전달된 값은 원래의 Maybe와 동일해야 합니다.");
            Assert.AreEqual(maybe, result, "Finally 메서드는 원래의 Maybe를 그대로 반환해야 합니다.");
        }

        #endregion
#endif // NOPE_AWAITABLE
    }
}