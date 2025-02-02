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
    /// MaybeSelectManyExtensions (SelectMany 메서드)의 모든 오버로드를 테스트합니다.
    /// </summary>
    [TestFixture]
    public class Maybe_SelectMany_Tests
    {
        #region 동기 테스트 (source: Maybe<T>, binder: sync, resultSelector: sync)

        [Test]
        public void SelectMany_Sync_WithValue_ReturnsCombinedResult()
        {
            // Arrange
            var maybe = Maybe<int>.From(10);
            // binder: int -> Maybe<string>
            Func<int, Maybe<string>> binder = x => Maybe<string>.From("A" + x);
            // resultSelector: (int, string) -> string
            Func<int, string, string> resultSelector = (x, s) => s + "!";
            // Expected: "A10!"

            // Act
            var result = maybe.SelectMany(binder, resultSelector);

            // Assert
            Assert.IsTrue(result.HasValue, "값이 있는 경우 결과에 값이 있어야 합니다.");
            Assert.AreEqual("A10!", result.Value);
        }

        [Test]
        public void SelectMany_Sync_WithBinderReturnsNone_ReturnsNone()
        {
            // Arrange
            var maybe = Maybe<int>.From(10);
            // binder: 항상 None 반환
            Func<int, Maybe<string>> binder = x => Maybe<string>.None;
            Func<int, string, string> resultSelector = (x, s) => s + "!";
            
            // Act
            var result = maybe.SelectMany(binder, resultSelector);

            // Assert
            Assert.IsFalse(result.HasValue, "binder가 None을 반환하면 결과도 None이어야 합니다.");
        }

        [Test]
        public void SelectMany_Sync_WithNoneSource_ReturnsNone()
        {
            // Arrange
            var maybe = Maybe<int>.None;
            bool binderCalled = false;
            Func<int, Maybe<string>> binder = x => { binderCalled = true; return Maybe<string>.From("ShouldNotCall"); };
            Func<int, string, string> resultSelector = (x, s) => s + "!";
            
            // Act
            var result = maybe.SelectMany(binder, resultSelector);

            // Assert
            Assert.IsFalse(binderCalled, "소스가 None이면 binder는 호출되지 않아야 합니다.");
            Assert.IsFalse(result.HasValue, "소스가 None이면 결과는 None이어야 합니다.");
        }

        #endregion

#if NOPE_UNITASK
        #region UniTask - 동기 소스 (Maybe<T>) → 비동기/동기 혼합

        // (1) source: Maybe<T>, binderAsync: async, resultSelector: sync
        [UnityTest]
        public IEnumerator SelectMany_UniTask_SyncSource_AsyncBinder_SyncResultSelector_WithValue_ReturnsCombinedResult() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.From(20);
            Func<int, UniTask<Maybe<string>>> binderAsync = async x =>
            {
                await UniTask.Yield();
                return Maybe<string>.From("B" + x);
            };
            Func<int, string, string> resultSelector = (x, s) => s + "?"; // Expected: "B20?"
            
            // Act
            var result = await maybe.SelectMany(binderAsync, resultSelector);

            // Assert
            Assert.IsTrue(result.HasValue, "값이 있을 경우 결과에 값이 있어야 합니다.");
            Assert.AreEqual("B20?", result.Value);
        });

        [UnityTest]
        public IEnumerator SelectMany_UniTask_SyncSource_AsyncBinder_SyncResultSelector_WithBinderReturnsNone_ReturnsNone() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.From(20);
            Func<int, UniTask<Maybe<string>>> binderAsync = async x =>
            {
                await UniTask.Yield();
                return Maybe<string>.None;
            };
            Func<int, string, string> resultSelector = (x, s) => s + "?";
            
            // Act
            var result = await maybe.SelectMany(binderAsync, resultSelector);

            // Assert
            Assert.IsFalse(result.HasValue, "binderAsync가 None을 반환하면 결과도 None이어야 합니다.");
        });

        [UnityTest]
        public IEnumerator SelectMany_UniTask_SyncSource_AsyncBinder_SyncResultSelector_WithNoneSource_ReturnsNone() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.None;
            bool binderCalled = false;
            Func<int, UniTask<Maybe<string>>> binderAsync = async x =>
            {
                binderCalled = true;
                await UniTask.Yield();
                return Maybe<string>.From("ShouldNotCall");
            };
            Func<int, string, string> resultSelector = (x, s) => s + "?";
            
            // Act
            var result = await maybe.SelectMany(binderAsync, resultSelector);

            // Assert
            Assert.IsFalse(binderCalled, "소스가 None이면 binderAsync는 호출되지 않아야 합니다.");
            Assert.IsFalse(result.HasValue, "소스가 None이면 결과는 None이어야 합니다.");
        });

        // (2) source: Maybe<T>, binder: sync, resultSelectorAsync: async
        [UnityTest]
        public IEnumerator SelectMany_UniTask_SyncSource_SyncBinder_AsyncResultSelector_WithValue_ReturnsCombinedResult() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.From(30);
            Func<int, Maybe<string>> binder = x => Maybe<string>.From("C" + x);
            Func<int, string, UniTask<string>> resultSelectorAsync = async (x, s) =>
            {
                await UniTask.Yield();
                return s + "$"; // Expected: "C30$"
            };
            
            // Act
            var result = await maybe.SelectMany(binder, resultSelectorAsync);

            // Assert
            Assert.IsTrue(result.HasValue, "값이 있을 경우 결과에 값이 있어야 합니다.");
            Assert.AreEqual("C30$", result.Value);
        });

        [UnityTest]
        public IEnumerator SelectMany_UniTask_SyncSource_SyncBinder_AsyncResultSelector_WithBinderReturnsNone_ReturnsNone() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.From(30);
            Func<int, Maybe<string>> binder = x => Maybe<string>.None;
            Func<int, string, UniTask<string>> resultSelectorAsync = async (x, s) =>
            {
                await UniTask.Yield();
                return s + "$";
            };

            // Act
            var result = await maybe.SelectMany(binder, resultSelectorAsync);

            // Assert
            Assert.IsFalse(result.HasValue, "binder가 None이면 결과는 None이어야 합니다.");
        });

        // (3) source: Maybe<T>, binderAsync: async, resultSelectorAsync: async
        [UnityTest]
        public IEnumerator SelectMany_UniTask_SyncSource_AsyncBinder_AsyncResultSelector_WithValue_ReturnsCombinedResult() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.From(40);
            Func<int, UniTask<Maybe<string>>> binderAsync = async x =>
            {
                await UniTask.Yield();
                return Maybe<string>.From("D" + x);
            };
            Func<int, string, UniTask<string>> resultSelectorAsync = async (x, s) =>
            {
                await UniTask.Yield();
                return s + "%"; // Expected: "D40%"
            };

            // Act
            var result = await maybe.SelectMany(binderAsync, resultSelectorAsync);

            // Assert
            Assert.IsTrue(result.HasValue, "값이 있을 경우 결과에 값이 있어야 합니다.");
            Assert.AreEqual("D40%", result.Value);
        });

        [UnityTest]
        public IEnumerator SelectMany_UniTask_SyncSource_AsyncBinder_AsyncResultSelector_WithBinderReturnsNone_ReturnsNone() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.From(40);
            Func<int, UniTask<Maybe<string>>> binderAsync = async x =>
            {
                await UniTask.Yield();
                return Maybe<string>.None;
            };
            Func<int, string, UniTask<string>> resultSelectorAsync = async (x, s) =>
            {
                await UniTask.Yield();
                return s + "%";
            };

            // Act
            var result = await maybe.SelectMany(binderAsync, resultSelectorAsync);

            // Assert
            Assert.IsFalse(result.HasValue, "binderAsync가 None이면 결과는 None이어야 합니다.");
        });

        // ────────────── async 소스 (UniTask<Maybe<T>>) ──────────────

        // (4) source: UniTask<Maybe<T>>, binder: sync, resultSelector: sync
        [UnityTest]
        public IEnumerator SelectMany_UniTask_AsyncSource_SyncBinder_SyncResultSelector_WithValue_ReturnsCombinedResult() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(50));
            Func<int, Maybe<string>> binder = x => Maybe<string>.From("E" + x);
            Func<int, string, string> resultSelector = (x, s) => s + "!";
            // Expected: "E50!"
            
            // Act
            var result = await asyncMaybe.SelectMany(binder, resultSelector);

            // Assert
            Assert.IsTrue(result.HasValue, "async 소스에서 값이 있을 경우 결과에 값이 있어야 합니다.");
            Assert.AreEqual("E50!", result.Value);
        });

        // (5) source: UniTask<Maybe<T>>, binder: sync, resultSelectorAsync: async
        [UnityTest]
        public IEnumerator SelectMany_UniTask_AsyncSource_SyncBinder_AsyncResultSelector_WithValue_ReturnsCombinedResult() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(60));
            Func<int, Maybe<string>> binder = x => Maybe<string>.From("F" + x);
            Func<int, string, UniTask<string>> resultSelectorAsync = async (x, s) =>
            {
                await UniTask.Yield();
                return s + "@"; // Expected: "F60@"
            };

            // Act
            var result = await asyncMaybe.SelectMany(binder, resultSelectorAsync);

            // Assert
            Assert.IsTrue(result.HasValue, "async 소스에서 값이 있을 경우 결과에 값이 있어야 합니다.");
            Assert.AreEqual("F60@", result.Value);
        });

        // (6) source: UniTask<Maybe<T>>, binderAsync: async, resultSelector: sync
        [UnityTest]
        public IEnumerator SelectMany_UniTask_AsyncSource_AsyncBinder_SyncResultSelector_WithValue_ReturnsCombinedResult() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(70));
            Func<int, UniTask<Maybe<string>>> binderAsync = async x =>
            {
                await UniTask.Yield();
                return Maybe<string>.From("G" + x);
            };
            Func<int, string, string> resultSelector = (x, s) => s + "#"; // Expected: "G70#"

            // Act
            var result = await asyncMaybe.SelectMany(binderAsync, resultSelector);

            // Assert
            Assert.IsTrue(result.HasValue, "async 소스에서 값이 있을 경우 결과에 값이 있어야 합니다.");
            Assert.AreEqual("G70#", result.Value);
        });

        // (7) source: UniTask<Maybe<T>>, binderAsync: async, resultSelectorAsync: async
        [UnityTest]
        public IEnumerator SelectMany_UniTask_AsyncSource_AsyncBinder_AsyncResultSelector_WithValue_ReturnsCombinedResult() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(80));
            Func<int, UniTask<Maybe<string>>> binderAsync = async x =>
            {
                await UniTask.Yield();
                return Maybe<string>.From("H" + x);
            };
            Func<int, string, UniTask<string>> resultSelectorAsync = async (x, s) =>
            {
                await UniTask.Yield();
                return s + "&"; // Expected: "H80&"
            };

            // Act
            var result = await asyncMaybe.SelectMany(binderAsync, resultSelectorAsync);

            // Assert
            Assert.IsTrue(result.HasValue, "async 소스에서 값이 있을 경우 결과에 값이 있어야 합니다.");
            Assert.AreEqual("H80&", result.Value);
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

        // ────────────── sync → async ──────────────
        // (8) source: Maybe<T>, binderAwaitable: async, resultSelector: sync
        [Test]
        public async System.Threading.Tasks.Task SelectMany_Awaitable_SyncSource_AwaitableBinder_SyncResultSelector_WithValue_ReturnsCombinedResult()
        {
            // Arrange
            var maybe = Maybe<int>.From(90);
            Func<int, Awaitable<Maybe<string>>> binderAwaitable = async x =>
            {
                return await FromResult(Maybe<string>.From("I" + x));
            };
            Func<int, string, string> resultSelector = (x, s) => s + "*"; // Expected: "I90*"
            
            // Act
            var result = await maybe.SelectMany(binderAwaitable, resultSelector);

            // Assert
            Assert.IsTrue(result.HasValue, "값이 있을 경우 결과에 값이 있어야 합니다.");
            Assert.AreEqual("I90*", result.Value);
        }

        [Test]
        public async System.Threading.Tasks.Task SelectMany_Awaitable_SyncSource_AwaitableBinder_SyncResultSelector_WithBinderReturnsNone_ReturnsNone()
        {
            // Arrange
            var maybe = Maybe<int>.From(90);
            Func<int, Awaitable<Maybe<string>>> binderAwaitable = async x =>
            {
                return await FromResult(Maybe<string>.None);
            };
            Func<int, string, string> resultSelector = (x, s) => s + "*";
            
            // Act
            var result = await maybe.SelectMany(binderAwaitable, resultSelector);

            // Assert
            Assert.IsFalse(result.HasValue, "binderAwaitable가 None을 반환하면 결과도 None이어야 합니다.");
        }

        // ────────────── async → sync ──────────────
        // (9) source: Awaitable<Maybe<T>>, sync binder, sync resultSelector
        [Test]
        public async System.Threading.Tasks.Task SelectMany_Awaitable_AsyncSource_SyncBinder_SyncResultSelector_WithValue_ReturnsCombinedResult()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.From(100));
            Func<int, Maybe<string>> binder = x => Maybe<string>.From("J" + x);
            Func<int, string, string> resultSelector = (x, s) => s + "~"; // Expected: "J100~"
            
            // Act
            var result = await asyncMaybe.SelectMany(binder, resultSelector);

            // Assert
            Assert.IsTrue(result.HasValue, "async 소스에서 값이 있을 경우 결과에 값이 있어야 합니다.");
            Assert.AreEqual("J100~", result.Value);
        }

        // (10) source: Awaitable<Maybe<T>>, sync binder, async resultSelector
        [Test]
        public async System.Threading.Tasks.Task SelectMany_Awaitable_AsyncSource_SyncBinder_AsyncResultSelector_WithValue_ReturnsCombinedResult()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.From(110));
            Func<int, Maybe<string>> binder = x => Maybe<string>.From("K" + x);
            Func<int, string, Awaitable<string>> resultSelectorAwaitable = async (x, s) =>
            {
                return await FromResult(s + "!");
            };
            // Expected: "K110!"
            
            // Act
            var result = await asyncMaybe.SelectMany(binder, resultSelectorAwaitable);

            // Assert
            Assert.IsTrue(result.HasValue, "값이 있을 경우 결과에 값이 있어야 합니다.");
            Assert.AreEqual("K110!", result.Value);
        }

        // (11) source: Awaitable<Maybe<T>>, async binder, sync resultSelector
        [Test]
        public async System.Threading.Tasks.Task SelectMany_Awaitable_AsyncSource_AsyncBinder_SyncResultSelector_WithValue_ReturnsCombinedResult()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.From(120));
            Func<int, Awaitable<Maybe<string>>> binderAwaitable = async x =>
            {
                return await FromResult(Maybe<string>.From("L" + x));
            };
            Func<int, string, string> resultSelector = (x, s) => s + "#"; // Expected: "L120#"
            
            // Act
            var result = await asyncMaybe.SelectMany(binderAwaitable, resultSelector);

            // Assert
            Assert.IsTrue(result.HasValue, "값이 있을 경우 결과에 값이 있어야 합니다.");
            Assert.AreEqual("L120#", result.Value);
        }

        // (12) source: Awaitable<Maybe<T>>, async binder, async resultSelector
        [Test]
        public async System.Threading.Tasks.Task SelectMany_Awaitable_AsyncSource_AsyncBinder_AsyncResultSelector_WithValue_ReturnsCombinedResult()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.From(130));
            Func<int, Awaitable<Maybe<string>>> binderAwaitable = async x =>
            {
                return await FromResult(Maybe<string>.From("M" + x));
            };
            Func<int, string, Awaitable<string>> resultSelectorAwaitable = async (x, s) =>
            {
                return await FromResult(s + "%");
            };
            // Expected: "M130%"
            
            // Act
            var result = await asyncMaybe.SelectMany(binderAwaitable, resultSelectorAwaitable);

            // Assert
            Assert.IsTrue(result.HasValue, "값이 있을 경우 결과에 값이 있어야 합니다.");
            Assert.AreEqual("M130%", result.Value);
        }

        #endregion
#endif // NOPE_AWAITABLE
    }
}