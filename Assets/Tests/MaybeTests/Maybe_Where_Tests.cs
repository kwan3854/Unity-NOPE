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
    /// MaybeWhereExtensions의 Where 확장 메서드에 대한 유닛 테스트입니다.
    /// </summary>
    public class Maybe_Where_Tests
    {
        #region 동기 테스트

        [Test]
        public void Where_Sync_WithValue_PredicateTrue_ReturnsOriginalMaybe()
        {
            // Arrange
            var maybe = Maybe<int>.From(10);
            Func<int, bool> predicate = x => x > 5; // true for 10

            // Act
            var result = maybe.Where(predicate);

            // Assert
            Assert.IsTrue(result.HasValue, "값이 있는 경우 결과에 값이 있어야 합니다.");
            Assert.AreEqual(10, result.Value, "Predicate가 true이면 원본 Maybe가 그대로 반환되어야 합니다.");
        }

        [Test]
        public void Where_Sync_WithValue_PredicateFalse_ReturnsNone()
        {
            // Arrange
            var maybe = Maybe<int>.From(10);
            Func<int, bool> predicate = x => x > 20; // false for 10

            // Act
            var result = maybe.Where(predicate);

            // Assert
            Assert.IsFalse(result.HasValue, "Predicate가 false이면 결과는 None이어야 합니다.");
            Assert.Throws<InvalidOperationException>(() => { var _ = result.Value; },
                "None인 경우 Value 접근 시 예외가 발생해야 합니다.");
        }

        [Test]
        public void Where_Sync_WithNone_ReturnsNoneWithoutCallingPredicate()
        {
            // Arrange
            var maybe = Maybe<int>.None;
            bool predicateCalled = false;
            Func<int, bool> predicate = x => { predicateCalled = true; return true; };

            // Act
            var result = maybe.Where(predicate);

            // Assert
            Assert.IsFalse(predicateCalled, "Maybe가 None이면 predicate는 호출되지 않아야 합니다.");
            Assert.IsFalse(result.HasValue, "결과는 None이어야 합니다.");
        }

        #endregion

#if NOPE_UNITASK
        #region UniTask 기반 비동기 테스트

        // Overload: Where(this Maybe<T> maybe, Func<T, UniTask<bool>> predicateAsync)
        [UnityTest]
        public IEnumerator Where_UniTask_SyncSource_AsyncPredicate_WithValue_PredicateTrue_ReturnsOriginalMaybe() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.From(15);
            Func<int, UniTask<bool>> predicateAsync = async x =>
            {
                await UniTask.Yield();
                return x < 20; // true for 15
            };

            // Act
            var result = await maybe.Where(predicateAsync);

            // Assert
            Assert.IsTrue(result.HasValue, "값이 있을 때 비동기 predicate가 true이면 결과에 값이 있어야 합니다.");
            Assert.AreEqual(15, result.Value);
        });

        [UnityTest]
        public IEnumerator Where_UniTask_SyncSource_AsyncPredicate_WithValue_PredicateFalse_ReturnsNone() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.From(15);
            Func<int, UniTask<bool>> predicateAsync = async x =>
            {
                await UniTask.Yield();
                return x > 20; // false for 15
            };

            // Act
            var result = await maybe.Where(predicateAsync);

            // Assert
            Assert.IsFalse(result.HasValue, "비동기 predicate가 false이면 결과는 None이어야 합니다.");
        });

        [UnityTest]
        public IEnumerator Where_UniTask_SyncSource_AsyncPredicate_WithNone_ReturnsNoneWithoutCallingPredicate() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybe = Maybe<int>.None;
            bool predicateCalled = false;
            Func<int, UniTask<bool>> predicateAsync = async x =>
            {
                predicateCalled = true;
                await UniTask.Yield();
                return true;
            };

            // Act
            var result = await maybe.Where(predicateAsync);

            // Assert
            Assert.IsFalse(predicateCalled, "Maybe가 None이면 비동기 predicate는 호출되지 않아야 합니다.");
            Assert.IsFalse(result.HasValue);
        });

        // Overload: Where(this UniTask<Maybe<T>> asyncMaybe, Func<T, bool> predicate)
        [UnityTest]
        public IEnumerator Where_UniTask_AsyncSource_SyncPredicate_WithValue_PredicateTrue_ReturnsOriginalMaybe() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(25));
            Func<int, bool> predicate = x => x == 25;

            // Act
            var result = await asyncMaybe.Where(predicate);

            // Assert
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(25, result.Value);
        });

        [UnityTest]
        public IEnumerator Where_UniTask_AsyncSource_SyncPredicate_WithValue_PredicateFalse_ReturnsNone() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(25));
            Func<int, bool> predicate = x => x != 25;

            // Act
            var result = await asyncMaybe.Where(predicate);

            // Assert
            Assert.IsFalse(result.HasValue);
        });

        [UnityTest]
        public IEnumerator Where_UniTask_AsyncSource_SyncPredicate_WithNone_ReturnsNone() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var asyncMaybe = UniTask.FromResult(Maybe<int>.None);
            bool predicateCalled = false;
            Func<int, bool> predicate = x => { predicateCalled = true; return true; };

            // Act
            var result = await asyncMaybe.Where(predicate);

            // Assert
            Assert.IsFalse(predicateCalled, "Maybe가 None이면 predicate는 호출되지 않아야 합니다.");
            Assert.IsFalse(result.HasValue);
        });

        // Overload: Where(this UniTask<Maybe<T>> asyncMaybe, Func<T, UniTask<bool>> predicateAsync)
        [UnityTest]
        public IEnumerator Where_UniTask_AsyncSource_AsyncPredicate_WithValue_PredicateTrue_ReturnsOriginalMaybe() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(35));
            Func<int, UniTask<bool>> predicateAsync = async x =>
            {
                await UniTask.Yield();
                return x > 30; // true for 35
            };

            // Act
            var result = await asyncMaybe.Where(predicateAsync);

            // Assert
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(35, result.Value);
        });

        [UnityTest]
        public IEnumerator Where_UniTask_AsyncSource_AsyncPredicate_WithValue_PredicateFalse_ReturnsNone() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(35));
            Func<int, UniTask<bool>> predicateAsync = async x =>
            {
                await UniTask.Yield();
                return x < 30; // false for 35
            };

            // Act
            var result = await asyncMaybe.Where(predicateAsync);

            // Assert
            Assert.IsFalse(result.HasValue);
        });

        [UnityTest]
        public IEnumerator Where_UniTask_AsyncSource_AsyncPredicate_WithNone_ReturnsNone() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var asyncMaybe = UniTask.FromResult(Maybe<int>.None);
            bool predicateCalled = false;
            Func<int, UniTask<bool>> predicateAsync = async x =>
            {
                predicateCalled = true;
                await UniTask.Yield();
                return true;
            };

            // Act
            var result = await asyncMaybe.Where(predicateAsync);

            // Assert
            Assert.IsFalse(predicateCalled, "Predicate should not be called when maybe is None.");
            Assert.IsFalse(result.HasValue);
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

        // Overload: Where(this Maybe<T> maybe, Func<T, Awaitable<bool>> predicateAwaitable)
        [Test]
        public async System.Threading.Tasks.Task Where_Awaitable_SyncSource_AwaitablePredicate_WithValue_PredicateTrue_ReturnsOriginalMaybe()
        {
            // Arrange
            var maybe = Maybe<int>.From(45);
            Func<int, Awaitable<bool>> predicateAwaitable = async x =>
            {
                return await FromResult(x == 45);
            };

            // Act
            var result = await maybe.Where(predicateAwaitable);

            // Assert
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(45, result.Value);
        }

        [Test]
        public async System.Threading.Tasks.Task Where_Awaitable_SyncSource_AwaitablePredicate_WithValue_PredicateFalse_ReturnsNone()
        {
            // Arrange
            var maybe = Maybe<int>.From(45);
            Func<int, Awaitable<bool>> predicateAwaitable = async x =>
            {
                return await FromResult(x != 45);
            };

            // Act
            var result = await maybe.Where(predicateAwaitable);

            // Assert
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public async System.Threading.Tasks.Task Where_Awaitable_SyncSource_AwaitablePredicate_WithNone_ReturnsNoneWithoutCallingPredicate()
        {
            // Arrange
            var maybe = Maybe<int>.None;
            bool predicateCalled = false;
            Func<int, Awaitable<bool>> predicateAwaitable = async x =>
            {
                predicateCalled = true;
                return await FromResult(true);
            };

            // Act
            var result = await maybe.Where(predicateAwaitable);

            // Assert
            Assert.IsFalse(predicateCalled, "Predicate should not be called when maybe is None.");
            Assert.IsFalse(result.HasValue);
        }

        // Overload: Where(this Awaitable<Maybe<T>> asyncMaybe, Func<T, bool> predicate)
        [Test]
        public async System.Threading.Tasks.Task Where_Awaitable_AsyncSource_SyncPredicate_WithValue_PredicateTrue_ReturnsOriginalMaybe()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.From(55));
            Func<int, bool> predicate = x => x > 50;

            // Act
            var result = await asyncMaybe.Where(predicate);

            // Assert
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(55, result.Value);
        }

        [Test]
        public async System.Threading.Tasks.Task Where_Awaitable_AsyncSource_SyncPredicate_WithValue_PredicateFalse_ReturnsNone()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.From(55));
            Func<int, bool> predicate = x => x < 50;

            // Act
            var result = await asyncMaybe.Where(predicate);

            // Assert
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public async System.Threading.Tasks.Task Where_Awaitable_AsyncSource_SyncPredicate_WithNone_ReturnsNone()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.None);
            bool predicateCalled = false;
            Func<int, bool> predicate = x => { predicateCalled = true; return true; };

            // Act
            var result = await asyncMaybe.Where(predicate);

            // Assert
            Assert.IsFalse(predicateCalled, "Predicate should not be called when maybe is None.");
            Assert.IsFalse(result.HasValue);
        }

        // Overload: Where(this Awaitable<Maybe<T>> asyncMaybe, Func<T, Awaitable<bool>> predicateAwaitable)
        [Test]
        public async System.Threading.Tasks.Task Where_Awaitable_AsyncSource_AsyncPredicate_WithValue_PredicateTrue_ReturnsOriginalMaybe()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.From(65));
            Func<int, Awaitable<bool>> predicateAwaitable = async x =>
            {
                return await FromResult(x < 100);
            };

            // Act
            var result = await asyncMaybe.Where(predicateAwaitable);

            // Assert
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(65, result.Value);
        }

        [Test]
        public async System.Threading.Tasks.Task Where_Awaitable_AsyncSource_AsyncPredicate_WithValue_PredicateFalse_ReturnsNone()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.From(65));
            Func<int, Awaitable<bool>> predicateAwaitable = async x =>
            {
                return await FromResult(x > 100);
            };

            // Act
            var result = await asyncMaybe.Where(predicateAwaitable);

            // Assert
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public async System.Threading.Tasks.Task Where_Awaitable_AsyncSource_AsyncPredicate_WithNone_ReturnsNone()
        {
            // Arrange
            var asyncMaybe = FromResult(Maybe<int>.None);
            bool predicateCalled = false;
            Func<int, Awaitable<bool>> predicateAwaitable = async x =>
            {
                predicateCalled = true;
                return await FromResult(true);
            };

            // Act
            var result = await asyncMaybe.Where(predicateAwaitable);

            // Assert
            Assert.IsFalse(predicateCalled, "Predicate should not be called when maybe is None.");
            Assert.IsFalse(result.HasValue);
        }

        #endregion
#endif // NOPE_AWAITABLE
    }
}