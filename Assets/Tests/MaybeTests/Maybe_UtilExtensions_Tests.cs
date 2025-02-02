using System;
using System.Collections;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NOPE.Runtime.Core.Maybe;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace NOPE.Tests.MaybeTests
{
    /// <summary>
    /// MaybeUtilExtensions (GetValueOrThrow, GetValueOrDefault, Or) 메서드에 대한 유닛 테스트입니다.
    /// </summary>
    public class Maybe_UtilExtensions_Tests
    {
        #region 동기 테스트

        [Test]
        public void GetValueOrThrow_WithValue_ReturnsValue()
        {
            // Arrange
            var maybe = Maybe<int>.From(123);

            // Act
            int value = maybe.GetValueOrThrow();

            // Assert
            Assert.AreEqual(123, value, "값이 있을 경우 GetValueOrThrow는 내부 값을 반환해야 합니다.");
        }

        [Test]
        public void GetValueOrThrow_WithNone_ThrowsInvalidOperationException()
        {
            // Arrange
            var maybe = Maybe<int>.None;

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => maybe.GetValueOrThrow(),
                "값이 없을 경우 GetValueOrThrow는 InvalidOperationException을 발생해야 합니다.");
        }

        [Test]
        public void GetValueOrThrow_WithNone_CustomException_ThrowsCustomException()
        {
            // Arrange
            var maybe = Maybe<int>.None;
            var customEx = new ArgumentException("Custom error");

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => maybe.GetValueOrThrow(customEx));
            Assert.AreEqual("Custom error", ex.Message);
        }

        [Test]
        public void GetValueOrDefault_WithValue_ReturnsValue()
        {
            // Arrange
            var maybe = Maybe<string>.From("Hello");

            // Act
            string value = maybe.GetValueOrDefault("Default");

            // Assert
            Assert.AreEqual("Hello", value, "값이 있을 경우 GetValueOrDefault는 내부 값을 반환해야 합니다.");
        }

        [Test]
        public void GetValueOrDefault_WithNone_ReturnsProvidedDefault()
        {
            // Arrange
            var maybe = Maybe<string>.None;

            // Act
            string value = maybe.GetValueOrDefault("Fallback");

            // Assert
            Assert.AreEqual("Fallback", value, "값이 없을 경우 GetValueOrDefault는 지정된 기본값을 반환해야 합니다.");
        }

        [Test]
        public void Or_WithValue_ReturnsOriginalMaybe()
        {
            // Arrange
            var maybe = Maybe<int>.From(10);
            var fallback = Maybe<int>.From(20);

            // Act
            var result = maybe.Or(fallback);

            // Assert
            Assert.IsTrue(result.HasValue, "값이 있을 경우 Or는 원본 Maybe를 그대로 반환해야 합니다.");
            Assert.AreEqual(10, result.Value);
        }

        [Test]
        public void Or_WithNone_ReturnsFallbackMaybe()
        {
            // Arrange
            var maybe = Maybe<int>.None;
            var fallback = Maybe<int>.From(20);

            // Act
            var result = maybe.Or(fallback);

            // Assert
            Assert.IsTrue(result.HasValue, "값이 없을 경우 Or는 fallback Maybe를 반환해야 합니다.");
            Assert.AreEqual(20, result.Value);
        }

        [Test]
        public void Or_WithValue_FallbackValue_ReturnsOriginalMaybe()
        {
            // Arrange
            var maybe = Maybe<int>.From(30);

            // Act
            var result = maybe.Or(40);

            // Assert
            Assert.IsTrue(result.HasValue, "값이 있을 경우 Or(fallbackValue) 는 원본 Maybe를 반환해야 합니다.");
            Assert.AreEqual(30, result.Value);
        }

        [Test]
        public void Or_WithNone_FallbackValue_ReturnsMaybeFromFallbackValue()
        {
            // Arrange
            var maybe = Maybe<int>.None;

            // Act
            var result = maybe.Or(40);

            // Assert
            Assert.IsTrue(result.HasValue, "값이 없을 경우 Or(fallbackValue)는 fallback 값으로 만든 Maybe를 반환해야 합니다.");
            Assert.AreEqual(40, result.Value);
        }

        #endregion

#if NOPE_UNITASK
        #region UniTask 기반 비동기 테스트

        [UnityTest]
        public IEnumerator GetValueOrThrow_UniTask_WithValue_ReturnsValue() =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var asyncMaybe = UniTask.FromResult(Maybe<int>.From(111));

                // Act
                int value = await asyncMaybe.GetValueOrThrow();

                // Assert
                Assert.AreEqual(111, value);
            });

        [Test]
        public async Task GetValueOrThrow_UniTask_WithNone_ThrowsInvalidOperationException() =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var asyncMaybe = UniTask.FromResult(Maybe<int>.None);

                // Act & Assert
                await UniTask.Yield();
                Assert.Throws<InvalidOperationException>(async () =>
                {
                    await asyncMaybe.GetValueOrThrow();
                });
            });

        [Test]
        public async Task GetValueOrThrow_UniTask_WithNone_CustomException_ThrowsCustomException() =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var asyncMaybe = UniTask.FromResult(Maybe<int>.None);
                var customEx = new ArgumentException("Async custom error");

                // Act & Assert
                await UniTask.Yield();
                Assert.Throws<ArgumentException>(async () =>
                {
                    await asyncMaybe.GetValueOrThrow(customEx);
                });
            });

        [UnityTest]
        public IEnumerator GetValueOrDefault_UniTask_WithValue_ReturnsValue() =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var asyncMaybe = UniTask.FromResult(Maybe<string>.From("AsyncHello"));

                // Act
                string value = await asyncMaybe.GetValueOrDefault("AsyncDefault");

                // Assert
                Assert.AreEqual("AsyncHello", value);
            });

        [UnityTest]
        public IEnumerator GetValueOrDefault_UniTask_WithNone_ReturnsProvidedDefault() =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var asyncMaybe = UniTask.FromResult(Maybe<string>.None);

                // Act
                string value = await asyncMaybe.GetValueOrDefault("FallbackAsync");

                // Assert
                Assert.AreEqual("FallbackAsync", value);
            });

        [UnityTest]
        public IEnumerator Or_UniTask_WithValue_ReturnsOriginalMaybe() =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var asyncMaybe = UniTask.FromResult(Maybe<int>.From(222));
                var fallback = Maybe<int>.From(333);

                // Act
                var result = await asyncMaybe.Or(fallback);

                // Assert
                Assert.IsTrue(result.HasValue);
                Assert.AreEqual(222, result.Value);
            });

        [UnityTest]
        public IEnumerator Or_UniTask_WithNone_ReturnsFallbackMaybe() =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var asyncMaybe = UniTask.FromResult(Maybe<int>.None);
                var fallback = Maybe<int>.From(333);

                // Act
                var result = await asyncMaybe.Or(fallback);

                // Assert
                Assert.IsTrue(result.HasValue);
                Assert.AreEqual(333, result.Value);
            });

        [UnityTest]
        public IEnumerator Or_UniTask_WithValue_FallbackValue_ReturnsOriginalMaybe() =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var asyncMaybe = UniTask.FromResult(Maybe<int>.From(444));

                // Act
                var result = await asyncMaybe.Or(555);

                // Assert
                Assert.IsTrue(result.HasValue);
                Assert.AreEqual(444, result.Value);
            });

        [UnityTest]
        public IEnumerator Or_UniTask_WithNone_FallbackValue_ReturnsMaybeFromFallbackValue() =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var asyncMaybe = UniTask.FromResult(Maybe<int>.None);

                // Act
                var result = await asyncMaybe.Or(555);

                // Assert
                Assert.IsTrue(result.HasValue);
                Assert.AreEqual(555, result.Value);
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
        
        [Test]
        public async Task GetValueOrThrow_Awaitable_WithValue_ReturnsValue()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.From(666));

            // Act
            int value = await asyncMaybe.GetValueOrThrow();

            // Assert
            Assert.AreEqual(666, value);
        }

        [Test]
        public async Task GetValueOrThrow_Awaitable_WithNone_ThrowsInvalidOperationException()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.None);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await asyncMaybe.GetValueOrThrow();
            });
        }

        [Test]
        public async Task GetValueOrThrow_Awaitable_WithNone_CustomException_ThrowsCustomException()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.None);
            var customEx = new ArgumentException("Awaitable custom error");

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await asyncMaybe.GetValueOrThrow(customEx);
            });
        }

        [Test]
        public async Task GetValueOrDefault_Awaitable_WithValue_ReturnsValue()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<string>.From("AwaitableHello"));

            // Act
            string value = await asyncMaybe.GetValueOrDefault("AwaitableDefault");

            // Assert
            Assert.AreEqual("AwaitableHello", value);
        }

        [Test]
        public async Task GetValueOrDefault_Awaitable_WithNone_ReturnsProvidedDefault()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<string>.None);

            // Act
            string value = await asyncMaybe.GetValueOrDefault("FallbackAwaitable");

            // Assert
            Assert.AreEqual("FallbackAwaitable", value);
        }

        [Test]
        public async Task Or_Awaitable_WithValue_ReturnsOriginalMaybe()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.From(777));
            var fallback = Maybe<int>.From(888);

            // Act
            var result = await asyncMaybe.Or(fallback);

            // Assert
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(777, result.Value);
        }

        [Test]
        public async Task Or_Awaitable_WithNone_ReturnsFallbackMaybe()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.None);
            var fallback = Maybe<int>.From(888);

            // Act
            var result = await asyncMaybe.Or(fallback);

            // Assert
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(888, result.Value);
        }

        [Test]
        public async Task Or_Awaitable_WithValue_FallbackValue_ReturnsOriginalMaybe()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.From(999));

            // Act
            var result = await asyncMaybe.Or(1000);

            // Assert
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(999, result.Value);
        }

        [Test]
        public async Task Or_Awaitable_WithNone_FallbackValue_ReturnsMaybeFromFallbackValue()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.None);

            // Act
            var result = await asyncMaybe.Or(1000);

            // Assert
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(1000, result.Value);
        }

        #endregion
#endif
    }
}