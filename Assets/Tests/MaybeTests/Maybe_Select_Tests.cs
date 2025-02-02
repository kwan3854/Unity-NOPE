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
    /// MaybeSelectExtensions (Select 메서드)의 모든 오버로드를 테스트합니다.
    /// 테스트는 동기/비동기 및 소스가 동기/비동기인 경우를 모두 커버합니다.
    /// </summary>
    public class Maybe_Select_Tests
    {
        #region 동기 테스트 (sync -> sync)

        [Test]
        public void Select_Sync_WithValue_ReturnsTransformedValue()
        {
            // Arrange: 값이 있는 Maybe 생성
            var maybe = Maybe<int>.From(10);
            // selector: 입력값에 3을 곱함 → 10 * 3 = 30

            // Act
            var result = maybe.Select(x => x * 3);

            // Assert
            Assert.IsTrue(result.HasValue, "값이 있는 경우 결과에 값이 있어야 합니다.");
            Assert.AreEqual(30, result.Value, "변환된 값이 올바르게 반환되어야 합니다.");
        }

        [Test]
        public void Select_Sync_WithNone_ReturnsNone()
        {
            // Arrange: None인 Maybe 생성
            var maybe = Maybe<int>.None;

            // Act
            var result = maybe.Select(x => x * 3);

            // Assert
            Assert.IsFalse(result.HasValue, "None인 경우 결과도 None이어야 합니다.");
            Assert.Throws<InvalidOperationException>(() => { var _ = result.Value; },
                "None인 경우 Value 접근 시 예외가 발생해야 합니다.");
        }

        #endregion

#if NOPE_UNITASK
        #region UniTask 기반 비동기 테스트

        // ────────────── sync → async ──────────────
        // (Maybe<T> 소스, 비동기 selector)
        [UnityTest]
        public IEnumerator Select_UniTask_SyncSource_AsyncSelector_WithValue_ReturnsTransformedValue() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.From(7);
            bool selectorCalled = false;
            // 비동기 selector: 입력값에서 3을 빼서 반환 (7 - 3 = 4)
            Func<int, UniTask<int>> selectorAsync = async x =>
            {
                selectorCalled = true;
                await UniTask.Yield();
                return x - 3;
            };

            // Act
            var result = await maybe.Select(selectorAsync);

            // Assert
            Assert.IsTrue(selectorCalled, "값이 있을 경우 비동기 selector는 호출되어야 합니다.");
            Assert.IsTrue(result.HasValue, "변환 결과에 값이 있어야 합니다.");
            Assert.AreEqual(4, result.Value);
        });

        [UnityTest]
        public IEnumerator Select_UniTask_SyncSource_AsyncSelector_WithNone_ReturnsNone() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.None;
            bool selectorCalled = false;
            Func<int, UniTask<int>> selectorAsync = async x =>
            {
                selectorCalled = true;
                await UniTask.Yield();
                return x - 3;
            };

            // Act
            var result = await maybe.Select(selectorAsync);

            // Assert
            Assert.IsFalse(selectorCalled, "None인 경우 비동기 selector는 호출되지 않아야 합니다.");
            Assert.IsFalse(result.HasValue, "None인 경우 결과도 None이어야 합니다.");
        });

        // ────────────── async → sync ──────────────
        // (UniTask<Maybe<T>> 소스, 동기 selector)
        [UnityTest]
        public IEnumerator Select_UniTask_AsyncSource_SyncSelector_WithValue_ReturnsTransformedValue() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(25));
            // 동기 selector: 25에서 5를 빼서 → 20
            Func<int, int> selector = x => x - 5;

            // Act
            var result = await asyncMaybe.Select(selector);

            // Assert
            Assert.IsTrue(result.HasValue, "async 소스에서 값이 있을 경우 결과에 값이 있어야 합니다.");
            Assert.AreEqual(20, result.Value);
        });

        [UnityTest]
        public IEnumerator Select_UniTask_AsyncSource_SyncSelector_WithNone_ReturnsNone() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var asyncMaybe = UniTask.FromResult(Maybe<int>.None);
            Func<int, int> selector = x => x - 5;

            // Act
            var result = await asyncMaybe.Select(selector);

            // Assert
            Assert.IsFalse(result.HasValue, "async 소스가 None인 경우 결과도 None이어야 합니다.");
        });

        // ────────────── async → async ──────────────
        // (UniTask<Maybe<T>> 소스, 비동기 selector)
        [UnityTest]
        public IEnumerator Select_UniTask_AsyncSource_AsyncSelector_WithValue_ReturnsTransformedValue() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(30));
            // 비동기 selector: 30을 2배 → 60
            Func<int, UniTask<int>> selectorAsync = async x =>
            {
                await UniTask.Yield();
                return x * 2;
            };

            // Act
            var result = await asyncMaybe.Select(selectorAsync);

            // Assert
            Assert.IsTrue(result.HasValue, "async 소스에서 값이 있을 경우 결과에 값이 있어야 합니다.");
            Assert.AreEqual(60, result.Value);
        });

        [UnityTest]
        public IEnumerator Select_UniTask_AsyncSource_AsyncSelector_WithNone_ReturnsNone() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var asyncMaybe = UniTask.FromResult(Maybe<int>.None);
            Func<int, UniTask<int>> selectorAsync = async x =>
            {
                await UniTask.Yield();
                return x * 2;
            };

            // Act
            var result = await asyncMaybe.Select(selectorAsync);

            // Assert
            Assert.IsFalse(result.HasValue, "async 소스가 None인 경우 결과도 None이어야 합니다.");
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
        // (Maybe<T> 소스, Awaitable selector)
        [Test]
        public async System.Threading.Tasks.Task Select_Awaitable_SyncSource_AwaitableSelector_WithValue_ReturnsTransformedValue()
        {
            // Arrange
            var maybe = Maybe<int>.From(50);
            bool selectorCalled = false;
            Func<int, Awaitable<int>> selectorAwaitable = async x =>
            {
                selectorCalled = true;
                return await FromResult(x + 20); // 50 + 20 = 70
            };

            // Act
            var result = await maybe.Select(selectorAwaitable);

            // Assert
            Assert.IsTrue(selectorCalled, "값이 있을 경우 Awaitable selector는 호출되어야 합니다.");
            Assert.IsTrue(result.HasValue, "변환 결과에 값이 있어야 합니다.");
            Assert.AreEqual(70, result.Value);
        }

        [Test]
        public async System.Threading.Tasks.Task Select_Awaitable_SyncSource_AwaitableSelector_WithNone_ReturnsNone()
        {
            // Arrange
            var maybe = Maybe<int>.None;
            bool selectorCalled = false;
            Func<int, Awaitable<int>> selectorAwaitable = async x =>
            {
                selectorCalled = true;
                return await FromResult(x + 20);
            };

            // Act
            var result = await maybe.Select(selectorAwaitable);

            // Assert
            Assert.IsFalse(selectorCalled, "None인 경우 Awaitable selector는 호출되지 않아야 합니다.");
            Assert.IsFalse(result.HasValue, "None인 경우 결과도 None이어야 합니다.");
        }

        // ────────────── async → sync ──────────────
        // (Awaitable<Maybe<T>> 소스, 동기 selector)
        [Test]
        public async System.Threading.Tasks.Task Select_Awaitable_AsyncSource_SyncSelector_WithValue_ReturnsTransformedValue()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.From(90));
            Func<int, int> selector = x => x / 3; // 90 / 3 = 30

            // Act
            var result = await asyncMaybe.Select(selector);

            // Assert
            Assert.IsTrue(result.HasValue, "Awaitable async 소스에서 값이 있을 경우 결과에 값이 있어야 합니다.");
            Assert.AreEqual(30, result.Value);
        }

        [Test]
        public async System.Threading.Tasks.Task Select_Awaitable_AsyncSource_SyncSelector_WithNone_ReturnsNone()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.None);
            Func<int, int> selector = x => x / 3;

            // Act
            var result = await asyncMaybe.Select(selector);

            // Assert
            Assert.IsFalse(result.HasValue, "Awaitable async 소스가 None인 경우 결과도 None이어야 합니다.");
        }

        // ────────────── async → async ──────────────
        // (Awaitable<Maybe<T>> 소스, Awaitable selector)
        [Test]
        public async System.Threading.Tasks.Task Select_Awaitable_AsyncSource_AwaitableSelector_WithValue_ReturnsTransformedValue()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.From(100));
            Func<int, Awaitable<int>> selectorAwaitable = async x =>
            {
                return await FromResult(x - 50); // 100 - 50 = 50
            };

            // Act
            var result = await asyncMaybe.Select(selectorAwaitable);

            // Assert
            Assert.IsTrue(result.HasValue, "Awaitable async 소스에서 값이 있을 경우 결과에 값이 있어야 합니다.");
            Assert.AreEqual(50, result.Value);
        }

        [Test]
        public async System.Threading.Tasks.Task Select_Awaitable_AsyncSource_AwaitableSelector_WithNone_ReturnsNone()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.None);
            Func<int, Awaitable<int>> selectorAwaitable = async x =>
            {
                return await FromResult(x - 50);
            };

            // Act
            var result = await asyncMaybe.Select(selectorAwaitable);

            // Assert
            Assert.IsFalse(result.HasValue, "Awaitable async 소스가 None인 경우 결과도 None이어야 합니다.");
        }

        #endregion
#endif // NOPE_AWAITABLE
    }
}