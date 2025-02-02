using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using NOPE.Runtime.Core.Maybe;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace NOPE.Tests.MaybeTests
{
    /// <summary>
    /// MaybeExtensions의 Tap 확장 메서드에 대한 유닛 테스트입니다.
    /// </summary>
    public class Maybe_Tap_Tests
    {
        #region 동기 테스트

        [Test]
        public void Tap_Sync_WithValue_ExecutesActionAndReturnsOriginalMaybe()
        {
            // Arrange
            var maybe = Maybe<int>.From(42);
            bool actionCalled = false;
            int receivedValue = 0;

            // Act
            var result = maybe.Tap(val =>
            {
                actionCalled = true;
                receivedValue = val;
            });

            // Assert
            Assert.IsTrue(actionCalled, "값이 있는 경우 액션이 호출되어야 합니다.");
            Assert.AreEqual(42, receivedValue, "액션에 전달된 값은 원본 값과 동일해야 합니다.");
            Assert.AreEqual(maybe, result, "Tap 메서드는 원본 Maybe를 그대로 반환해야 합니다.");
        }

        [Test]
        public void Tap_Sync_WithNone_DoesNotExecuteActionAndReturnsNone()
        {
            // Arrange
            var maybe = Maybe<int>.None;
            bool actionCalled = false;

            // Act
            var result = maybe.Tap(val => actionCalled = true);

            // Assert
            Assert.IsFalse(actionCalled, "None인 경우 액션이 호출되어서는 안 됩니다.");
            Assert.IsFalse(result.HasValue, "None인 경우 결과도 None이어야 합니다.");
            Assert.Throws<InvalidOperationException>(() => { var _ = result.Value; },
                "None에서 Value 접근 시 예외가 발생해야 합니다.");
        }

        #endregion

#if NOPE_UNITASK
        #region UniTask 기반 비동기 테스트

        // ────────── Maybe<T> 소스, 비동기 액션 ──────────
        [UnityTest]
        public IEnumerator Tap_UniTask_SyncSource_AsyncAction_WithValue_ExecutesActionAndReturnsOriginalMaybe() =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var maybe = Maybe<int>.From(100);
                bool actionCalled = false;
                int receivedValue = 0;
                Func<int, UniTask> asyncAction = async x =>
                {
                    await UniTask.Yield();
                    actionCalled = true;
                    receivedValue = x;
                };

                // Act
                var result = await maybe.Tap(asyncAction);

                // Assert
                Assert.IsTrue(actionCalled, "값이 있을 때 비동기 액션이 호출되어야 합니다.");
                Assert.AreEqual(100, receivedValue, "액션에 전달된 값은 원본 값과 동일해야 합니다.");
                Assert.AreEqual(maybe, result, "비동기 Tap은 원본 Maybe를 그대로 반환해야 합니다.");
            });

        [UnityTest]
        public IEnumerator Tap_UniTask_SyncSource_AsyncAction_WithNone_DoesNotExecuteActionAndReturnsNone() =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var maybe = Maybe<int>.None;
                bool actionCalled = false;
                Func<int, UniTask> asyncAction = async x =>
                {
                    actionCalled = true;
                    await UniTask.Yield();
                };

                // Act
                var result = await maybe.Tap(asyncAction);

                // Assert
                Assert.IsFalse(actionCalled, "None인 경우 비동기 액션은 호출되어서는 안 됩니다.");
                Assert.IsFalse(result.HasValue, "None인 경우 결과도 None이어야 합니다.");
            });

        // ────────── UniTask<Maybe<T>> 소스, 동기 액션 ──────────
        [UnityTest]
        public IEnumerator Tap_UniTask_AsyncSource_SyncAction_WithValue_ExecutesActionAndReturnsOriginalMaybe() =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var asyncMaybe = UniTask.FromResult(Maybe<int>.From(200));
                bool actionCalled = false;
                int receivedValue = 0;

                // Act
                var result = await asyncMaybe.Tap(val =>
                {
                    actionCalled = true;
                    receivedValue = val;
                });

                // Assert
                Assert.IsTrue(actionCalled, "async 소스에서 값이 있을 때 동기 액션이 호출되어야 합니다.");
                Assert.AreEqual(200, receivedValue, "액션에 전달된 값은 원본 값과 동일해야 합니다.");
                Assert.AreEqual(await asyncMaybe, result, "Tap 메서드는 원본 Maybe를 그대로 반환해야 합니다.");
            });

        [UnityTest]
        public IEnumerator Tap_UniTask_AsyncSource_SyncAction_WithNone_DoesNotExecuteActionAndReturnsNone() =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var asyncMaybe = UniTask.FromResult(Maybe<int>.None);
                bool actionCalled = false;

                // Act
                var result = await asyncMaybe.Tap(val => actionCalled = true);

                // Assert
                Assert.IsFalse(actionCalled, "async 소스가 None인 경우 동기 액션은 호출되어서는 안 됩니다.");
                Assert.IsFalse(result.HasValue, "결과는 None이어야 합니다.");
            });

        // ────────── UniTask<Maybe<T>> 소스, 비동기 액션 ──────────
        [UnityTest]
        public IEnumerator Tap_UniTask_AsyncSource_AsyncAction_WithValue_ExecutesActionAndReturnsOriginalMaybe() =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var asyncMaybe = UniTask.FromResult(Maybe<int>.From(300));
                bool actionCalled = false;
                int receivedValue = 0;
                Func<int, UniTask> asyncAction = async x =>
                {
                    await UniTask.Yield();
                    actionCalled = true;
                    receivedValue = x;
                };

                // Act
                var result = await asyncMaybe.Tap(asyncAction);

                // Assert
                Assert.IsTrue(actionCalled, "비동기 액션이 호출되어야 합니다.");
                Assert.AreEqual(300, receivedValue, "액션에 전달된 값은 원본 값과 동일해야 합니다.");
                Assert.AreEqual(await asyncMaybe, result, "Tap 메서드는 원본 Maybe를 그대로 반환해야 합니다.");
            });

        [UnityTest]
        public IEnumerator Tap_UniTask_AsyncSource_AsyncAction_WithNone_DoesNotExecuteActionAndReturnsNone() =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var asyncMaybe = UniTask.FromResult(Maybe<int>.None);
                bool actionCalled = false;
                Func<int, UniTask> asyncAction = async x =>
                {
                    actionCalled = true;
                    await UniTask.Yield();
                };

                // Act
                var result = await asyncMaybe.Tap(asyncAction);

                // Assert
                Assert.IsFalse(actionCalled, "None인 경우 비동기 액션은 호출되어서는 안 됩니다.");
                Assert.IsFalse(result.HasValue, "결과는 None이어야 합니다.");
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

        // ────────── Maybe<T> 소스, Awaitable 액션 ──────────
        [Test]
        public async System.Threading.Tasks.Task Tap_Awaitable_SyncSource_AwaitableAction_WithValue_ExecutesActionAndReturnsOriginalMaybe()
        {
            // Arrange
            var maybe = Maybe<int>.From(400);
            bool actionCalled = false;
            int receivedValue = 0;
            Func<int, Awaitable> asyncAction = async x =>
            {
                // 가짜 Awaitable: await 가능한 FromResult 사용
                await FromResult(0);
                actionCalled = true;
                receivedValue = x;
            };

            // Act
            var result = await maybe.Tap(asyncAction);

            // Assert
            Assert.IsTrue(actionCalled, "값이 있을 때 Awaitable 액션이 호출되어야 합니다.");
            Assert.AreEqual(400, receivedValue, "액션에 전달된 값은 원본 값과 동일해야 합니다.");
            Assert.AreEqual(maybe, result, "Tap 메서드는 원본 Maybe를 그대로 반환해야 합니다.");
        }

        [Test]
        public async System.Threading.Tasks.Task Tap_Awaitable_SyncSource_AwaitableAction_WithNone_DoesNotExecuteActionAndReturnsNone()
        {
            // Arrange
            var maybe = Maybe<int>.None;
            bool actionCalled = false;
            Func<int, Awaitable> asyncAction = async x =>
            {
                actionCalled = true;
                await FromResult(0);
            };

            // Act
            var result = await maybe.Tap(asyncAction);

            // Assert
            Assert.IsFalse(actionCalled, "None인 경우 Awaitable 액션은 호출되어서는 안 됩니다.");
            Assert.IsFalse(result.HasValue, "결과는 None이어야 합니다.");
        }

        // ────────── Awaitable<Maybe<T>> 소스, 동기 액션 ──────────
        [Test]
        public async System.Threading.Tasks.Task Tap_Awaitable_AsyncSource_SyncAction_WithValue_ExecutesActionAndReturnsOriginalMaybe()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.From(500));
            bool actionCalled = false;
            int receivedValue = 0;

            // Act
            var result = await asyncMaybe.Tap(val =>
            {
                actionCalled = true;
                receivedValue = val;
            });

            // Assert
            Assert.IsTrue(actionCalled, "Awaitable async 소스에서 값이 있을 경우 동기 액션이 호출되어야 합니다.");
            Assert.AreEqual(500, receivedValue, "액션에 전달된 값은 원본 값과 동일해야 합니다.");
        }

        [Test]
        public async System.Threading.Tasks.Task Tap_Awaitable_AsyncSource_SyncAction_WithNone_DoesNotExecuteActionAndReturnsNone()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.None);
            bool actionCalled = false;

            // Act
            var result = await asyncMaybe.Tap(val => actionCalled = true);

            // Assert
            Assert.IsFalse(actionCalled, "Awaitable async 소스가 None인 경우 동기 액션은 호출되어서는 안 됩니다.");
            Assert.IsFalse(result.HasValue, "결과는 None이어야 합니다.");
        }

        // ────────── Awaitable<Maybe<T>> 소스, Awaitable 액션 ──────────
        [Test]
        public async System.Threading.Tasks.Task Tap_Awaitable_AsyncSource_AwaitableAction_WithValue_ExecutesActionAndReturnsOriginalMaybe()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.From(600));
            bool actionCalled = false;
            int receivedValue = 0;
            Func<int, Awaitable> asyncAction = async x =>
            {
                await FromResult(0);
                actionCalled = true;
                receivedValue = x;
            };

            // Act
            var result = await asyncMaybe.Tap(asyncAction);

            // Assert
            Assert.IsTrue(actionCalled, "Awaitable async 소스에서 값이 있을 경우 Awaitable 액션이 호출되어야 합니다.");
            Assert.AreEqual(600, receivedValue, "액션에 전달된 값은 원본 값과 동일해야 합니다.");
        }

        [Test]
        public async System.Threading.Tasks.Task Tap_Awaitable_AsyncSource_AwaitableAction_WithNone_DoesNotExecuteActionAndReturnsNone()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.None);
            bool actionCalled = false;
            Func<int, Awaitable> asyncAction = async x =>
            {
                actionCalled = true;
                await FromResult(0);
            };

            // Act
            var result = await asyncMaybe.Tap(asyncAction);

            // Assert
            Assert.IsFalse(actionCalled, "Awaitable async 소스가 None인 경우 Awaitable 액션은 호출되어서는 안 됩니다.");
            Assert.IsFalse(result.HasValue, "결과는 None이어야 합니다.");
        }

        #endregion
#endif // NOPE_AWAITABLE
    }
}