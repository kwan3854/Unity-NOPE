using System.Collections;
using Cysharp.Threading.Tasks;
using NOPE.Runtime.Core.Maybe;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace NOPE.Tests.MaybeTests
{
    /// <summary>
    /// Maybe의 Match 확장 메서드에 대한 유닛 테스트입니다.
    /// </summary>
    [TestFixture]
    public class Maybe_Match_Tests
    {
        #region 동기 테스트

        [Test]
        public void Match_WithValue_InvokesOnValueAndReturnsResult()
        {
            // Arrange
            var maybe = Maybe<int>.From(10);

            // Act
            var result = maybe.Match(x => "Has " + x, () => "None");

            // Assert
            Assert.AreEqual("Has 10", result, "Maybe에 값이 있을 때 onValue가 호출되어야 합니다.");
        }

        [Test]
        public void Match_WithNone_InvokesOnNoneAndReturnsResult()
        {
            // Arrange
            var maybe = Maybe<int>.None;

            // Act
            var result = maybe.Match(x => "Has " + x, () => "None");

            // Assert
            Assert.AreEqual("None", result, "Maybe가 None일 때 onNone가 호출되어야 합니다.");
        }

        #endregion

#if NOPE_UNITASK
        #region UniTask 기반 비동기 테스트

        // onValue: 동기, onNone: 비동기
        [UnityTest]
        public IEnumerator Match_UniTask_OnValueSyncOnNoneAsync_ReturnsOnValue() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.From(5);

            // Act
            var result = await maybe.Match(x => "Value " + x, async () =>
            {
                await UniTask.Yield();
                return "NoneAsync";
            });

            // Assert
            Assert.AreEqual("Value 5", result, "Maybe에 값이 있을 때 동기 onValue가 호출되어야 합니다.");
        });

        [UnityTest]
        public IEnumerator Match_UniTask_OnValueSyncOnNoneAsync_ReturnsOnNoneAsync() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.None;

            // Act
            var result = await maybe.Match(x => "Value " + x, async () =>
            {
                await UniTask.Yield();
                return "NoneAsync";
            });

            // Assert
            Assert.AreEqual("NoneAsync", result, "Maybe가 None일 때 비동기 onNoneAsync가 호출되어야 합니다.");
        });

        // onValue: 비동기, onNone: 동기
        [UnityTest]
        public IEnumerator Match_UniTask_OnValueAsyncOnNoneSync_ReturnsOnValueAsync() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.From(7);

            // Act
            var result = await maybe.Match(async x =>
            {
                await UniTask.Yield();
                return "AsyncValue " + x;
            }, () => "NoneSync");

            // Assert
            Assert.AreEqual("AsyncValue 7", result, "Maybe에 값이 있을 때 비동기 onValueAsync가 호출되어야 합니다.");
        });

        [UnityTest]
        public IEnumerator Match_UniTask_OnValueAsyncOnNoneSync_ReturnsOnNoneSync() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.None;

            // Act
            var result = await maybe.Match(async x =>
            {
                await UniTask.Yield();
                return "AsyncValue " + x;
            }, () => "NoneSync");

            // Assert
            Assert.AreEqual("NoneSync", result, "Maybe가 None일 때 동기 onNone가 호출되어야 합니다.");
        });

        // onValue: 비동기, onNone: 비동기
        [UnityTest]
        public IEnumerator Match_UniTask_OnValueAsyncOnNoneAsync_ReturnsOnValueAsync() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.From(12);

            // Act
            var result = await maybe.Match(async x =>
            {
                await UniTask.Yield();
                return "AsyncValue " + x;
            }, async () =>
            {
                await UniTask.Yield();
                return "AsyncNone";
            });

            // Assert
            Assert.AreEqual("AsyncValue 12", result, "Maybe에 값이 있을 때 비동기 onValueAsync가 호출되어야 합니다.");
        });

        [UnityTest]
        public IEnumerator Match_UniTask_OnValueAsyncOnNoneAsync_ReturnsOnNoneAsync() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.None;

            // Act
            var result = await maybe.Match(async x =>
            {
                await UniTask.Yield();
                return "AsyncValue " + x;
            }, async () =>
            {
                await UniTask.Yield();
                return "AsyncNone";
            });

            // Assert
            Assert.AreEqual("AsyncNone", result, "Maybe가 None일 때 비동기 onNoneAsync가 호출되어야 합니다.");
        });

        // UniTask<Maybe<T>> asyncMaybe 소스
        [UnityTest]
        public IEnumerator Match_UniTask_FromAsyncMaybe_OnValue_ReturnsOnValue() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(20));

            // Act
            var result = await asyncMaybe.Match(x => "FromAsync " + x, () => "None");

            // Assert
            Assert.AreEqual("FromAsync 20", result, "UniTask 소스에서 값이 있을 때 onValue가 호출되어야 합니다.");
        });

        [UnityTest]
        public IEnumerator Match_UniTask_FromAsyncMaybe_OnNone_ReturnsOnNone() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var asyncMaybe = UniTask.FromResult(Maybe<int>.None);

            // Act
            var result = await asyncMaybe.Match(x => "FromAsync " + x, () => "None");

            // Assert
            Assert.AreEqual("None", result, "UniTask 소스에서 None일 때 onNone가 호출되어야 합니다.");
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
        public async System.Threading.Tasks.Task Match_Awaitable_OnValue_SyncOnNoneAsync_ReturnsOnValue()
        {
            // Arrange
            var maybe = Maybe<int>.From(30);

            // Act
            var result = await maybe.Match(x => "Value " + x, async () => await FromResult("NoneAsync"));

            // Assert
            Assert.AreEqual("Value 30", result, "Maybe에 값이 있을 때 동기 onValue와 비동기 onNoneAsync 조합이 올바르게 동작해야 합니다.");
        }

        [Test]
        public async System.Threading.Tasks.Task Match_Awaitable_OnValue_SyncOnNoneAsync_ReturnsOnNoneAsync()
        {
            // Arrange
            var maybe = Maybe<int>.None;

            // Act
            var result = await maybe.Match(x => "Value " + x, async () => await FromResult("NoneAsync"));

            // Assert
            Assert.AreEqual("NoneAsync", result, "Maybe가 None일 때 비동기 onNoneAsync가 호출되어야 합니다.");
        }

        [Test]
        public async System.Threading.Tasks.Task Match_Awaitable_OnValueAsync_OnNoneSync_ReturnsOnValueAsync()
        {
            // Arrange
            var maybe = Maybe<int>.From(35);

            // Act
            var result = await maybe.Match(async x => await FromResult("AsyncValue " + x), () => "NoneSync");

            // Assert
            Assert.AreEqual("AsyncValue 35", result, "Maybe에 값이 있을 때 Awaitable onValueAsync가 호출되어야 합니다.");
        }

        [Test]
        public async System.Threading.Tasks.Task Match_Awaitable_OnValueAsync_OnNoneSync_ReturnsOnNoneSync()
        {
            // Arrange
            var maybe = Maybe<int>.None;

            // Act
            var result = await maybe.Match(async x => await FromResult("AsyncValue " + x), () => "NoneSync");

            // Assert
            Assert.AreEqual("NoneSync", result, "Maybe가 None일 때 동기 onNone가 호출되어야 합니다.");
        }

        [Test]
        public async System.Threading.Tasks.Task Match_Awaitable_OnValueAsync_OnNoneAsync_ReturnsOnValueAsync()
        {
            // Arrange
            var maybe = Maybe<int>.From(40);

            // Act
            var result = await maybe.Match(async x => await FromResult("AsyncValue " + x),
                async () => await FromResult("AsyncNone"));

            // Assert
            Assert.AreEqual("AsyncValue 40", result, "Maybe에 값이 있을 때 Awaitable onValueAsync가 호출되어야 합니다.");
        }

        [Test]
        public async System.Threading.Tasks.Task Match_Awaitable_OnValueAsync_OnNoneAsync_ReturnsOnNoneAsync()
        {
            // Arrange
            var maybe = Maybe<int>.None;

            // Act
            var result = await maybe.Match(async x => await FromResult("AsyncValue " + x),
                async () => await FromResult("AsyncNone"));

            // Assert
            Assert.AreEqual("AsyncNone", result, "Maybe가 None일 때 Awaitable onNoneAsync가 호출되어야 합니다.");
        }

        // Awaitable<Maybe<T>> asyncMaybe 소스 테스트
        [Test]
        public async System.Threading.Tasks.Task Match_Awaitable_FromAsyncMaybe_OnValue_ReturnsOnValue()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.From(50));

            // Act
            var result = await asyncMaybe.Match(x => "FromAsync " + x, () => "None");

            // Assert
            Assert.AreEqual("FromAsync 50", result, "Awaitable 소스에서 값이 있을 때 onValue가 호출되어야 합니다.");
        }

        [Test]
        public async System.Threading.Tasks.Task Match_Awaitable_FromAsyncMaybe_OnNone_ReturnsOnNone()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.None);

            // Act
            var result = await asyncMaybe.Match(x => "FromAsync " + x, () => "None");

            // Assert
            Assert.AreEqual("None", result, "Awaitable 소스에서 None일 때 onNone가 호출되어야 합니다.");
        }

        #endregion
#endif // NOPE_AWAITABLE
    }
}