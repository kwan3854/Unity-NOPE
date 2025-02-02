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
    /// Maybe의 Bind 확장 메서드에 대한 유닛 테스트입니다.
    /// </summary>
    [TestFixture]
    public class Maybe_Bind_Tests
    {
        #region 동기 테스트

        [Test]
        public void Bind_ReturnsBinderResult_WhenMaybeHasValue()
        {
            // Arrange
            var maybe = Maybe<int>.From(10);
            bool binderCalled = false;
            Func<int, Maybe<string>> binder = x =>
            {
                binderCalled = true;
                return Maybe<string>.From("Number " + x);
            };

            // Act
            var result = maybe.Bind(binder);

            // Assert
            Assert.IsTrue(binderCalled, "Binder should be called when maybe has a value.");
            Assert.IsTrue(result.HasValue, "Result should have a value.");
            Assert.AreEqual("Number 10", result.Value);
        }

        [Test]
        public void Bind_ReturnsNone_WhenMaybeIsNone()
        {
            // Arrange
            var maybe = Maybe<int>.None;
            bool binderCalled = false;
            Func<int, Maybe<string>> binder = x =>
            {
                binderCalled = true;
                return Maybe<string>.From("Number " + x);
            };

            // Act
            var result = maybe.Bind(binder);

            // Assert
            Assert.IsFalse(binderCalled, "Binder should not be called when maybe is None.");
            Assert.IsFalse(result.HasValue, "Result should be None.");
            Assert.Throws<InvalidOperationException>(() => { var _ = result.Value; },
                "Accessing Value on a None result should throw an exception.");
        }

        #endregion

#if NOPE_UNITASK
        #region UniTask 기반 비동기 테스트

        // 1. Bind(this Maybe<T> maybe, Func<T, UniTask<Maybe<TNew>>> asyncBinder)
        [UnityTest]
        public IEnumerator BindAsync_MaybeHasValue_ReturnsBinderResult() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.From(20);
            bool binderCalled = false;
            Func<int, UniTask<Maybe<string>>> asyncBinder = async x =>
            {
                binderCalled = true;
                await UniTask.Yield();
                return Maybe<string>.From("Value " + x);
            };

            // Act
            var result = await maybe.Bind(asyncBinder);

            // Assert
            Assert.IsTrue(binderCalled, "Async binder should be called when maybe has a value.");
            Assert.IsTrue(result.HasValue, "Result should have a value.");
            Assert.AreEqual("Value 20", result.Value);
        });

        [UnityTest]
        public IEnumerator BindAsync_MaybeIsNone_ReturnsNone() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.None;
            bool binderCalled = false;
            Func<int, UniTask<Maybe<string>>> asyncBinder = async x =>
            {
                binderCalled = true;
                await UniTask.Yield();
                return Maybe<string>.From("Value " + x);
            };

            // Act
            var result = await maybe.Bind(asyncBinder);

            // Assert
            Assert.IsFalse(binderCalled, "Async binder should not be called when maybe is None.");
            Assert.IsFalse(result.HasValue, "Result should be None.");
        });

        // 2. Bind(this UniTask<Maybe<T>> asyncMaybe, Func<T, Maybe<TNew>> binder)
        [UnityTest]
        public IEnumerator BindAsync_FromUniTask_WithSyncBinder_ReturnsBinderResult() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(30));
            bool binderCalled = false;
            Func<int, Maybe<string>> binder = x =>
            {
                binderCalled = true;
                return Maybe<string>.From("Sync " + x);
            };

            // Act
            var result = await asyncMaybe.Bind(binder);

            // Assert
            Assert.IsTrue(binderCalled, "Sync binder should be called when using async source.");
            Assert.IsTrue(result.HasValue, "Result should have a value.");
            Assert.AreEqual("Sync 30", result.Value);
        });

        // 3. Bind(this UniTask<Maybe<T>> asyncMaybe, Func<T, UniTask<Maybe<TNew>>> asyncBinder)
        [UnityTest]
        public IEnumerator BindAsync_FromUniTask_WithAsyncBinder_ReturnsBinderResult() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(40));
            bool binderCalled = false;
            Func<int, UniTask<Maybe<string>>> asyncBinder = async x =>
            {
                binderCalled = true;
                await UniTask.Yield();
                return Maybe<string>.From("Async " + x);
            };

            // Act
            var result = await asyncMaybe.Bind(asyncBinder);

            // Assert
            Assert.IsTrue(binderCalled, "Async binder should be called when using async source.");
            Assert.IsTrue(result.HasValue, "Result should have a value.");
            Assert.AreEqual("Async 40", result.Value);
        });

        #endregion
#endif

#if NOPE_AWAITABLE
        #region Awaitable 기반 비동기 테스트
        
        private async Awaitable<T> FromResult<T>(T value)
        {
            await Task.Yield();
            return value;
        }

        // 1. Bind(this Maybe<T> maybe, Func<T, Awaitable<Maybe<TNew>>> asyncBinder)
        [Test]
        public async System.Threading.Tasks.Task Bind_Awaitable_MaybeHasValue_ReturnsBinderResult()
        {
            // Arrange
            var maybe = Maybe<int>.From(50);
            bool binderCalled = false;
            Func<int, Awaitable<Maybe<string>>> asyncBinder = async x =>
            {
                binderCalled = true;
                return await FromResult(Maybe<string>.From("Awaitable " + x));
            };

            // Act
            var result = await maybe.Bind(asyncBinder);

            // Assert
            Assert.IsTrue(binderCalled, "Awaitable binder should be called when maybe has a value.");
            Assert.IsTrue(result.HasValue, "Result should have a value.");
            Assert.AreEqual("Awaitable 50", result.Value);
        }

        [Test]
        public async System.Threading.Tasks.Task Bind_Awaitable_MaybeIsNone_ReturnsNone()
        {
            // Arrange
            var maybe = Maybe<int>.None;
            bool binderCalled = false;
            Func<int, Awaitable<Maybe<string>>> asyncBinder = async x =>
            {
                binderCalled = true;
                return await FromResult(Maybe<string>.From("Awaitable " + x));
            };

            // Act
            var result = await maybe.Bind(asyncBinder);

            // Assert
            Assert.IsFalse(binderCalled, "Awaitable binder should not be called when maybe is None.");
            Assert.IsFalse(result.HasValue, "Result should be None.");
        }

        // 2. Bind(this Awaitable<Maybe<T>> asyncMaybe, Func<T, Maybe<TNew>> binder)
        [Test]
        public async System.Threading.Tasks.Task Bind_Awaitable_FromAsyncMaybe_WithSyncBinder_ReturnsBinderResult()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.From(60));
            bool binderCalled = false;
            Func<int, Maybe<string>> binder = x =>
            {
                binderCalled = true;
                return Maybe<string>.From("SyncAwaitable " + x);
            };

            // Act
            var result = await asyncMaybe.Bind(binder);

            // Assert
            Assert.IsTrue(binderCalled, "Sync binder should be called for Awaitable async maybe.");
            Assert.IsTrue(result.HasValue, "Result should have a value.");
            Assert.AreEqual("SyncAwaitable 60", result.Value);
        }

        // 3. Bind(this Awaitable<Maybe<T>> asyncMaybe, Func<T, Awaitable<Maybe<TNew>>> asyncBinder)
        [Test]
        public async System.Threading.Tasks.Task Bind_Awaitable_FromAsyncMaybe_WithAsyncBinder_ReturnsBinderResult()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.From(70));
            bool binderCalled = false;
            Func<int, Awaitable<Maybe<string>>> asyncBinder = async x =>
            {
                binderCalled = true;
                return await FromResult(Maybe<string>.From("AsyncAwaitable " + x));
            };

            // Act
            var result = await asyncMaybe.Bind(asyncBinder);

            // Assert
            Assert.IsTrue(binderCalled, "Async binder should be called for Awaitable async maybe.");
            Assert.IsTrue(result.HasValue, "Result should have a value.");
            Assert.AreEqual("AsyncAwaitable 70", result.Value);
        }

        #endregion
#endif
    }
}

