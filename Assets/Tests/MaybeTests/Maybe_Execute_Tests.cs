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
    /// MaybeUtilExtensions의 Execute/ExecuteNoValue 확장 메서드에 대한 유닛 테스트입니다.
    /// </summary>
    public class Maybe_Execute_Tests
    {
        #region 동기 테스트

        [Test]
        public void Execute_Sync_WithValue_ExecutesActionAndReturnsOriginalMaybe()
        {
            // Arrange
            var maybe = Maybe<int>.From(42);
            bool actionCalled = false;
            int received = 0;

            // Act
            var result = maybe.Execute(val =>
            {
                actionCalled = true;
                received = val;
            });

            // Assert
            Assert.IsTrue(actionCalled, "값이 있을 경우 동기 액션이 호출되어야 합니다.");
            Assert.AreEqual(42, received, "액션에 전달된 값이 원래 값과 동일해야 합니다.");
            Assert.AreEqual(maybe, result, "Execute는 원본 Maybe를 그대로 반환해야 합니다.");
        }

        [Test]
        public void Execute_Sync_WithNone_DoesNotExecuteActionAndReturnsNone()
        {
            // Arrange
            var maybe = Maybe<int>.None;
            bool actionCalled = false;

            // Act
            var result = maybe.Execute(val => actionCalled = true);

            // Assert
            Assert.IsFalse(actionCalled, "값이 없는 경우 동기 액션은 호출되지 않아야 합니다.");
            Assert.IsFalse(result.HasValue, "결과는 None이어야 합니다.");
        }

        [Test]
        public void ExecuteNoValue_Sync_WithNone_ExecutesActionAndReturnsOriginalMaybe()
        {
            // Arrange
            var maybe = Maybe<int>.None;
            bool actionCalled = false;

            // Act
            var result = maybe.ExecuteNoValue(() => actionCalled = true);

            // Assert
            Assert.IsTrue(actionCalled, "값이 없는 경우 ExecuteNoValue 동기 액션이 호출되어야 합니다.");
            Assert.IsFalse(result.HasValue, "결과는 None이어야 합니다.");
        }

        [Test]
        public void ExecuteNoValue_Sync_WithValue_DoesNotExecuteActionAndReturnsOriginalMaybe()
        {
            // Arrange
            var maybe = Maybe<int>.From(42);
            bool actionCalled = false;

            // Act
            var result = maybe.ExecuteNoValue(() => actionCalled = true);

            // Assert
            Assert.IsFalse(actionCalled, "값이 있는 경우 ExecuteNoValue 동기 액션은 호출되지 않아야 합니다.");
            Assert.IsTrue(result.HasValue, "결과는 값이 있어야 합니다.");
            Assert.AreEqual(42, result.Value);
        }

        #endregion

#if NOPE_UNITASK
        #region UniTask 기반 비동기 테스트

        // [1] 소스: Maybe<T>, 액션: Func<T, UniTask>
        [UnityTest]
        public IEnumerator Execute_UniTask_SyncSource_AsyncAction_WithValue_ExecutesActionAndReturnsOriginalMaybe() =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var maybe = Maybe<int>.From(100);
                bool actionCalled = false;
                int received = 0;
                Func<int, UniTask> asyncAction = async x =>
                {
                    await UniTask.Yield();
                    actionCalled = true;
                    received = x;
                };

                // Act
                var result = await maybe.Execute(asyncAction);

                // Assert
                Assert.IsTrue(actionCalled, "비동기 액션은 값이 있을 때 호출되어야 합니다.");
                Assert.AreEqual(100, received, "비동기 액션에 전달된 값은 원래 값과 동일해야 합니다.");
                Assert.AreEqual(maybe, result, "Execute는 원본 Maybe를 그대로 반환해야 합니다.");
            });

        [UnityTest]
        public IEnumerator Execute_UniTask_SyncSource_AsyncAction_WithNone_DoesNotExecuteActionAndReturnsNone() =>
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
                var result = await maybe.Execute(asyncAction);

                // Assert
                Assert.IsFalse(actionCalled, "값이 없는 경우 비동기 액션은 호출되어서는 안 됩니다.");
                Assert.IsFalse(result.HasValue, "결과는 None이어야 합니다.");
            });

        // [2] 소스: Maybe<T>, 액션: Func<UniTask> for ExecuteNoValue
        [UnityTest]
        public IEnumerator ExecuteNoValue_UniTask_SyncSource_AsyncAction_WithNone_ExecutesActionAndReturnsOriginalMaybe() =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var maybe = Maybe<int>.None;
                bool actionCalled = false;
                Func<UniTask> asyncAction = async () =>
                {
                    await UniTask.Yield();
                    actionCalled = true;
                };

                // Act
                var result = await maybe.ExecuteNoValue(asyncAction);

                // Assert
                Assert.IsTrue(actionCalled, "비동기 액션은 None일 때 호출되어야 합니다.");
                Assert.IsFalse(result.HasValue, "결과는 None이어야 합니다.");
            });

        [UnityTest]
        public IEnumerator ExecuteNoValue_UniTask_SyncSource_AsyncAction_WithValue_DoesNotExecuteActionAndReturnsOriginalMaybe() =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var maybe = Maybe<int>.From(100);
                bool actionCalled = false;
                Func<UniTask> asyncAction = async () =>
                {
                    actionCalled = true;
                    await UniTask.Yield();
                };

                // Act
                var result = await maybe.ExecuteNoValue(asyncAction);

                // Assert
                Assert.IsFalse(actionCalled, "값이 있을 때 비동기 ExecuteNoValue 액션은 호출되지 않아야 합니다.");
                Assert.IsTrue(result.HasValue);
                Assert.AreEqual(100, result.Value);
            });

        // [3] 소스: UniTask<Maybe<T>>, 동기 액션
        [UnityTest]
        public IEnumerator Execute_UniTask_AsyncSource_SyncAction_WithValue_ExecutesActionAndReturnsOriginalMaybe() =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var asyncMaybe = UniTask.FromResult(Maybe<int>.From(200));
                bool actionCalled = false;
                int received = 0;

                // Act
                var result = await asyncMaybe.Execute(val =>
                {
                    actionCalled = true;
                    received = val;
                });

                // Assert
                Assert.IsTrue(actionCalled, "동기 액션은 값이 있을 때 호출되어야 합니다.");
                Assert.AreEqual(200, received);
                Assert.AreEqual(await asyncMaybe, result);
            });

        [UnityTest]
        public IEnumerator Execute_UniTask_AsyncSource_SyncAction_WithNone_DoesNotExecuteActionAndReturnsNone() =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var asyncMaybe = UniTask.FromResult(Maybe<int>.None);
                bool actionCalled = false;

                // Act
                var result = await asyncMaybe.Execute(val => actionCalled = true);

                // Assert
                Assert.IsFalse(actionCalled, "동기 액션은 None인 경우 호출되어서는 안 됩니다.");
                Assert.IsFalse(result.HasValue);
            });

        // [4] 소스: UniTask<Maybe<T>>, 액션: Func<T, UniTask>
        [UnityTest]
        public IEnumerator Execute_UniTask_AsyncSource_AsyncAction_WithValue_ExecutesActionAndReturnsOriginalMaybe() =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var asyncMaybe = UniTask.FromResult(Maybe<int>.From(300));
                bool actionCalled = false;
                int received = 0;
                Func<int, UniTask> asyncAction = async x =>
                {
                    await UniTask.Yield();
                    actionCalled = true;
                    received = x;
                };

                // Act
                var result = await asyncMaybe.Execute(asyncAction);

                // Assert
                Assert.IsTrue(actionCalled);
                Assert.AreEqual(300, received);
                Assert.AreEqual(await asyncMaybe, result);
            });

        [UnityTest]
        public IEnumerator Execute_UniTask_AsyncSource_AsyncAction_WithNone_DoesNotExecuteActionAndReturnsNone() =>
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
                var result = await asyncMaybe.Execute(asyncAction);

                // Assert
                Assert.IsFalse(actionCalled);
                Assert.IsFalse(result.HasValue);
            });

        // [5] 소스: UniTask<Maybe<T>>, ExecuteNoValue overload with sync Action
        [UnityTest]
        public IEnumerator ExecuteNoValue_UniTask_AsyncSource_SyncAction_WithNone_ExecutesActionAndReturnsOriginalMaybe() =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var asyncMaybe = UniTask.FromResult(Maybe<int>.None);
                bool actionCalled = false;

                // Act
                var result = await asyncMaybe.ExecuteNoValue(() => actionCalled = true);

                // Assert
                Assert.IsTrue(actionCalled);
                Assert.IsFalse(result.HasValue);
            });

        [UnityTest]
        public IEnumerator ExecuteNoValue_UniTask_AsyncSource_SyncAction_WithValue_DoesNotExecuteActionAndReturnsOriginalMaybe() =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var asyncMaybe = UniTask.FromResult(Maybe<int>.From(400));
                bool actionCalled = false;

                // Act
                var result = await asyncMaybe.ExecuteNoValue(() => actionCalled = true);

                // Assert
                Assert.IsFalse(actionCalled);
                Assert.IsTrue(result.HasValue);
                Assert.AreEqual(400, result.Value);
            });

        // [6] 소스: UniTask<Maybe<T>>, ExecuteNoValue overload with Func<UniTask>
        [UnityTest]
        public IEnumerator ExecuteNoValue_UniTask_AsyncSource_AsyncAction_WithNone_ExecutesActionAndReturnsOriginalMaybe() =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var asyncMaybe = UniTask.FromResult(Maybe<int>.None);
                bool actionCalled = false;
                Func<UniTask> asyncAction = async () =>
                {
                    await UniTask.Yield();
                    actionCalled = true;
                };

                // Act
                var result = await asyncMaybe.ExecuteNoValue(asyncAction);

                // Assert
                Assert.IsTrue(actionCalled);
                Assert.IsFalse(result.HasValue);
            });

        [UnityTest]
        public IEnumerator ExecuteNoValue_UniTask_AsyncSource_AsyncAction_WithValue_DoesNotExecuteActionAndReturnsOriginalMaybe() =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var asyncMaybe = UniTask.FromResult(Maybe<int>.From(500));
                bool actionCalled = false;
                Func<UniTask> asyncAction = async () =>
                {
                    actionCalled = true;
                    await UniTask.Yield();
                };

                // Act
                var result = await asyncMaybe.ExecuteNoValue(asyncAction);

                // Assert
                Assert.IsFalse(actionCalled);
                Assert.IsTrue(result.HasValue);
                Assert.AreEqual(500, result.Value);
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

        // [1] 소스: Maybe<T>, 액션: Func<T, Awaitable>
        [Test]
        public async System.Threading.Tasks.Task Execute_Awaitable_SyncSource_AwaitableAction_WithValue_ExecutesActionAndReturnsOriginalMaybe()
        {
            // Arrange
            var maybe = Maybe<int>.From(600);
            bool actionCalled = false;
            int received = 0;
            Func<int, Awaitable> asyncAction = async x =>
            {
                await FromResult(0);
                actionCalled = true;
                received = x;
            };

            // Act
            var result = await maybe.Execute(asyncAction);

            // Assert
            Assert.IsTrue(actionCalled);
            Assert.AreEqual(600, received);
            Assert.AreEqual(maybe, result);
        }

        [Test]
        public async System.Threading.Tasks.Task Execute_Awaitable_SyncSource_AwaitableAction_WithNone_DoesNotExecuteActionAndReturnsNone()
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
            var result = await maybe.Execute(asyncAction);

            // Assert
            Assert.IsFalse(actionCalled);
            Assert.IsFalse(result.HasValue);
        }

        // [2] 소스: Maybe<T>, ExecuteNoValue with Func<Awaitable>
        [Test]
        public async System.Threading.Tasks.Task ExecuteNoValue_Awaitable_SyncSource_AwaitableAction_WithNone_ExecutesActionAndReturnsOriginalMaybe()
        {
            // Arrange
            var maybe = Maybe<int>.None;
            bool actionCalled = false;
            Func<Awaitable> asyncAction = async () =>
            {
                await FromResult(0);
                actionCalled = true;
            };

            // Act
            var result = await maybe.ExecuteNoValue(asyncAction);

            // Assert
            Assert.IsTrue(actionCalled);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public async System.Threading.Tasks.Task ExecuteNoValue_Awaitable_SyncSource_AwaitableAction_WithValue_DoesNotExecuteActionAndReturnsOriginalMaybe()
        {
            // Arrange
            var maybe = Maybe<int>.From(700);
            bool actionCalled = false;
            Func<Awaitable> asyncAction = async () =>
            {
                actionCalled = true;
                await FromResult(0);
            };

            // Act
            var result = await maybe.ExecuteNoValue(asyncAction);

            // Assert
            Assert.IsFalse(actionCalled);
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(700, result.Value);
        }

        // [3] 소스: Awaitable<Maybe<T>>, 동기 액션
        [Test]
        public async System.Threading.Tasks.Task Execute_Awaitable_AsyncSource_SyncAction_WithValue_ExecutesActionAndReturnsOriginalMaybe()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.From(800));
            bool actionCalled = false;
            int received = 0;

            // Act
            var result = await asyncMaybe.Execute(val =>
            {
                actionCalled = true;
                received = val;
            });

            // Assert
            Assert.IsTrue(actionCalled);
            Assert.AreEqual(800, received);
        }

        [Test]
        public async System.Threading.Tasks.Task Execute_Awaitable_AsyncSource_SyncAction_WithNone_DoesNotExecuteActionAndReturnsNone()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.None);
            bool actionCalled = false;

            // Act
            var result = await asyncMaybe.Execute(val => actionCalled = true);

            // Assert
            Assert.IsFalse(actionCalled);
            Assert.IsFalse(result.HasValue);
        }

        // [4] 소스: Awaitable<Maybe<T>>, 액션: Func<T, Awaitable>
        [Test]
        public async System.Threading.Tasks.Task Execute_Awaitable_AsyncSource_AwaitableAction_WithValue_ExecutesActionAndReturnsOriginalMaybe()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.From(900));
            bool actionCalled = false;
            int received = 0;
            Func<int, Awaitable> asyncAction = async x =>
            {
                await FromResult(0);
                actionCalled = true;
                received = x;
            };

            // Act
            var result = await asyncMaybe.Execute(asyncAction);

            // Assert
            Assert.IsTrue(actionCalled);
            Assert.AreEqual(900, received);
        }

        [Test]
        public async System.Threading.Tasks.Task Execute_Awaitable_AsyncSource_AwaitableAction_WithNone_DoesNotExecuteActionAndReturnsNone()
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
            var result = await asyncMaybe.Execute(asyncAction);

            // Assert
            Assert.IsFalse(actionCalled);
            Assert.IsFalse(result.HasValue);
        }

        // [5] 소스: Awaitable<Maybe<T>>, ExecuteNoValue overload with sync Action
        [Test]
        public async System.Threading.Tasks.Task ExecuteNoValue_Awaitable_AsyncSource_SyncAction_WithNone_ExecutesActionAndReturnsOriginalMaybe()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.None);
            bool actionCalled = false;

            // Act
            var result = await asyncMaybe.ExecuteNoValue(() => actionCalled = true);

            // Assert
            Assert.IsTrue(actionCalled);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public async System.Threading.Tasks.Task ExecuteNoValue_Awaitable_AsyncSource_SyncAction_WithValue_DoesNotExecuteActionAndReturnsOriginalMaybe()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.From(1000));
            bool actionCalled = false;

            // Act
            var result = await asyncMaybe.ExecuteNoValue(() => actionCalled = true);

            // Assert
            Assert.IsFalse(actionCalled);
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(1000, result.Value);
        }

        // [6] 소스: Awaitable<Maybe<T>>, ExecuteNoValue overload with Func<Awaitable>
        [Test]
        public async System.Threading.Tasks.Task ExecuteNoValue_Awaitable_AsyncSource_AsyncAction_WithNone_ExecutesActionAndReturnsOriginalMaybe()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.None);
            bool actionCalled = false;
            Func<Awaitable> asyncAction = async () =>
            {
                await FromResult(0);
                actionCalled = true;
            };

            // Act
            var result = await asyncMaybe.ExecuteNoValue(asyncAction);

            // Assert
            Assert.IsTrue(actionCalled);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public async System.Threading.Tasks.Task ExecuteNoValue_Awaitable_AsyncSource_AsyncAction_WithValue_DoesNotExecuteActionAndReturnsOriginalMaybe()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.From(1100));
            bool actionCalled = false;
            Func<Awaitable> asyncAction = async () =>
            {
                actionCalled = true;
                await FromResult(0);
            };

            // Act
            var result = await asyncMaybe.ExecuteNoValue(asyncAction);

            // Assert
            Assert.IsFalse(actionCalled);
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(1100, result.Value);
        }

        #endregion
#endif // NOPE_AWAITABLE
    }
}