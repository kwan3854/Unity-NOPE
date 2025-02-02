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
    /// Maybe의 Map 확장 메서드에 대한 유닛 테스트입니다.
    /// </summary>\
    [TestFixture]
    public class Maybe_Map_Tests
    {
        #region 동기 테스트

        [Test]
        public void Map_WithValue_ReturnsTransformedValue()
        {
            // Arrange
            var maybe = Maybe<int>.From(10);
            
            // Act
            var result = maybe.Map(x => x * 2); // 10 * 2 = 20

            // Assert
            Assert.IsTrue(result.HasValue, "값이 있을 경우 Map은 변환된 값을 반환해야 합니다.");
            Assert.AreEqual(20, result.Value);
        }

        [Test]
        public void Map_WithNone_ReturnsNone()
        {
            // Arrange
            var maybe = Maybe<int>.None;
            bool selectorCalled = false;

            // Act
            var result = maybe.Map(x =>
            {
                selectorCalled = true;
                return x * 2;
            });

            // Assert
            Assert.IsFalse(selectorCalled, "None인 경우 selector는 호출되지 않아야 합니다.");
            Assert.IsFalse(result.HasValue, "None인 경우 Map은 None을 반환해야 합니다.");
            Assert.Throws<InvalidOperationException>(() => { var _ = result.Value; },
                "None인 경우 Value 접근 시 예외가 발생해야 합니다.");
        }

        #endregion

#if NOPE_UNITASK
        #region UniTask 기반 비동기 테스트

        // 1. Map(this Maybe<T> maybe, Func<T, UniTask<TNew>> asyncSelector)
        [UnityTest]
        public IEnumerator MapAsync_WithValue_ReturnsTransformedValue() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.From(15);
            bool selectorCalled = false;

            // Act
            var result = await maybe.Map(async x =>
            {
                selectorCalled = true;
                await UniTask.Yield();
                return x + 5; // 15 + 5 = 20
            });

            // Assert
            Assert.IsTrue(selectorCalled, "비동기 selector는 값이 있을 경우 호출되어야 합니다.");
            Assert.IsTrue(result.HasValue, "비동기 Map은 변환된 값을 반환해야 합니다.");
            Assert.AreEqual(20, result.Value);
        });

        [UnityTest]
        public IEnumerator MapAsync_WithNone_ReturnsNone() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.None;
            bool selectorCalled = false;

            // Act
            var result = await maybe.Map(async x =>
            {
                selectorCalled = true;
                await UniTask.Yield();
                return x + 5;
            });

            // Assert
            Assert.IsFalse(selectorCalled, "None인 경우 비동기 selector는 호출되지 않아야 합니다.");
            Assert.IsFalse(result.HasValue, "None인 경우 비동기 Map은 None을 반환해야 합니다.");
            Assert.Throws<InvalidOperationException>(() => { var _ = result.Value; });
        });

        // 2. Map(this UniTask<Maybe<T>> asyncMaybe, Func<T, TNew> selector)
        [UnityTest]
        public IEnumerator MapAsync_FromAsyncMaybe_WithSyncSelector_ReturnsTransformedValue() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(25));
            
            // Act
            var result = await asyncMaybe.Map(x => x - 5); // 25 - 5 = 20

            // Assert
            Assert.IsTrue(result.HasValue, "async 소스와 동기 selector 사용 시 값이 반환되어야 합니다.");
            Assert.AreEqual(20, result.Value);
        });

        // 3. Map(this UniTask<Maybe<T>> asyncMaybe, Func<T, UniTask<TNew>> asyncSelector)
        [UnityTest]
        public IEnumerator MapAsync_FromAsyncMaybe_WithAsyncSelector_ReturnsTransformedValue() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(30));
            
            // Act
            var result = await asyncMaybe.Map(async x =>
            {
                await UniTask.Yield();
                return x * 3; // 30 * 3 = 90
            });

            // Assert
            Assert.IsTrue(result.HasValue, "async 소스와 비동기 selector 사용 시 값이 반환되어야 합니다.");
            Assert.AreEqual(90, result.Value);
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

        // 1. Map(this Maybe<T> maybe, Func<T, Awaitable<TNew>> asyncSelector)
        [Test]
        public async System.Threading.Tasks.Task Map_Awaitable_WithValue_ReturnsTransformedValue()
        {
            // Arrange
            var maybe = Maybe<int>.From(50);
            bool selectorCalled = false;

            // Act
            var result = await maybe.Map(async x =>
            {
                selectorCalled = true;
                return await FromResult(x + 10); // 50 + 10 = 60
            });

            // Assert
            Assert.IsTrue(selectorCalled, "Awaitable selector는 값이 있을 경우 호출되어야 합니다.");
            Assert.IsTrue(result.HasValue, "Awaitable Map은 변환된 값을 반환해야 합니다.");
            Assert.AreEqual(60, result.Value);
        }

        [Test]
        public async System.Threading.Tasks.Task Map_Awaitable_WithNone_ReturnsNone()
        {
            // Arrange
            var maybe = Maybe<int>.None;
            bool selectorCalled = false;

            // Act
            var result = await maybe.Map(async x =>
            {
                selectorCalled = true;
                return await FromResult(x + 10);
            });

            // Assert
            Assert.IsFalse(selectorCalled, "None인 경우 Awaitable selector는 호출되지 않아야 합니다.");
            Assert.IsFalse(result.HasValue, "None인 경우 Awaitable Map은 None을 반환해야 합니다.");
            Assert.Throws<InvalidOperationException>(() => { var _ = result.Value; });
        }

        // 2. Map(this Awaitable<Maybe<T>> asyncMaybe, Func<T, TNew> selector)
        [Test]
        public async System.Threading.Tasks.Task Map_Awaitable_FromAsyncMaybe_WithSyncSelector_ReturnsTransformedValue()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.From(70));
            
            // Act
            var result = await asyncMaybe.Map(x => x * 2); // 70 * 2 = 140

            // Assert
            Assert.IsTrue(result.HasValue, "Awaitable async source와 동기 selector 사용 시 값이 반환되어야 합니다.");
            Assert.AreEqual(140, result.Value);
        }

        // 3. Map(this Awaitable<Maybe<T>> asyncMaybe, Func<T, Awaitable<TNew>> asyncSelector)
        [Test]
        public async System.Threading.Tasks.Task Map_Awaitable_FromAsyncMaybe_WithAsyncSelector_ReturnsTransformedValue()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.From(80));
            
            // Act
            var result = await asyncMaybe.Map(async x =>
            {
                return await FromResult(x - 30); // 80 - 30 = 50
            });

            // Assert
            Assert.IsTrue(result.HasValue, "Awaitable async source와 Awaitable selector 사용 시 값이 반환되어야 합니다.");
            Assert.AreEqual(50, result.Value);
        }

        #endregion
#endif // NOPE_AWAITABLE
    }
}
