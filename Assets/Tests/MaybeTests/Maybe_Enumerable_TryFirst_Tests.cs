using System;
using System.Collections.Generic;
using NOPE.Runtime.Core.Maybe;
using NUnit.Framework;
using UnityEngine;
#if NOPE_UNITASK
using System.Collections;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
#endif

#if NOPE_AWAITABLE
// Awaitable 관련 네임스페이스(예: NOPE.Awaitable)가 있다면 추가하세요.
#endif

namespace NOPE.Tests.MaybeTests
{
    /// <summary>
    /// TryFirst 확장 메서드에 대한 유닛 테스트입니다.
    /// </summary>
    [TestFixture]
    public class Maybe_Enumerable_TryFirst_Tests
    {
        #region 동기 테스트

        [Test]
        public void TryFirst_ReturnsMaybeWithFirstElement_WhenSequenceNotEmpty()
        {
            // Arrange
            var list = new List<int> { 10, 20, 30 };

            // Act
            var maybe = list.TryFirst();

            // Assert
            Assert.IsTrue(maybe.HasValue, "비어있지 않은 시퀀스의 경우 첫 번째 요소가 존재해야 합니다.");
            Assert.AreEqual(10, maybe.Value, "첫 번째 요소가 올바르게 반환되어야 합니다.");
        }

        [Test]
        public void TryFirst_ReturnsMaybeNone_WhenSequenceIsEmpty()
        {
            // Arrange
            var list = new List<int>();

            // Act
            var maybe = list.TryFirst();

            // Assert
            Assert.IsFalse(maybe.HasValue, "빈 시퀀스의 경우 Maybe.None이어야 합니다.");
            Assert.Throws<InvalidOperationException>(() => { var _ = maybe.Value; },
                "값이 없는 Maybe에서 Value 접근 시 예외가 발생해야 합니다.");
        }

        [Test]
        public void TryFirst_WithPredicate_ReturnsMaybeWithFirstMatchingElement()
        {
            // Arrange
            var list = new List<int> { 5, 8, 13, 21 };

            // Act
            var maybe = list.TryFirst(x => x > 10);

            // Assert
            Assert.IsTrue(maybe.HasValue, "조건을 만족하는 요소가 존재해야 합니다.");
            Assert.AreEqual(13, maybe.Value, "조건을 만족하는 첫 번째 요소가 반환되어야 합니다.");
        }

        [Test]
        public void TryFirst_WithPredicate_ReturnsMaybeNone_WhenNoElementMatches()
        {
            // Arrange
            var list = new List<int> { 5, 8, 13, 21 };

            // Act
            var maybe = list.TryFirst(x => x > 100);

            // Assert
            Assert.IsFalse(maybe.HasValue, "조건을 만족하는 요소가 없으면 Maybe.None이어야 합니다.");
            Assert.Throws<InvalidOperationException>(() => { var _ = maybe.Value; },
                "값이 없는 Maybe에서 Value 접근 시 예외가 발생해야 합니다.");
        }

        [Test]
        public void TryFirst_ThrowsArgumentNullException_WhenSourceIsNull()
        {
            // Arrange
            List<int> list = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => list.TryFirst(),
                "null 소스 전달 시 ArgumentNullException이 발생해야 합니다.");
        }

        [Test]
        public void TryFirst_WithPredicate_ThrowsArgumentNullException_WhenSourceIsNull()
        {
            // Arrange
            List<int> list = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => list.TryFirst(x => true),
                "null 소스 전달 시 ArgumentNullException이 발생해야 합니다.");
        }

        [Test]
        public void TryFirst_WithPredicate_ThrowsArgumentNullException_WhenPredicateIsNull()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3 };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => list.TryFirst((Func<int, bool>)null),
                "null predicate 전달 시 ArgumentNullException이 발생해야 합니다.");
        }

        #endregion

#if NOPE_UNITASK
        #region UniTask 기반 비동기 테스트

        [UnityTest]
        public IEnumerator TryFirstAsync_WithAsyncPredicate_ReturnsMaybeWithFirstMatchingElement() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var list = new List<int> { 10, 20, 30 };
            // 비동기 predicate: x가 15보다 작으면 true
            var maybe = await list.TryFirst(async x => { await UniTask.Yield(); return x < 15; });

            // Assert
            Assert.IsTrue(maybe.HasValue, "비동기 predicate를 사용하여 조건을 만족하는 첫 번째 요소가 존재해야 합니다.");
            Assert.AreEqual(10, maybe.Value, "조건에 맞는 첫 번째 요소가 반환되어야 합니다.");
        });

        [UnityTest]
        public IEnumerator TryFirstAsync_WithAsyncPredicate_ReturnsMaybeNone_WhenNoMatch() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var list = new List<int> { 10, 20, 30 };
            var maybe = await list.TryFirst(async x => { await UniTask.Yield(); return x > 100; });

            // Assert
            Assert.IsFalse(maybe.HasValue, "조건에 맞는 요소가 없으면 Maybe.None이어야 합니다.");
            Assert.Throws<InvalidOperationException>(() => { var _ = maybe.Value; });
        });

        [UnityTest]
        public IEnumerator TryFirstAsync_FromAsyncSource_ReturnsMaybeWithFirstElement() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var list = new List<int> { 100, 200, 300 };
            var asyncSource = UniTask.FromResult<IEnumerable<int>>(list);

            // Act
            var maybe = await asyncSource.TryFirst();

            // Assert
            Assert.IsTrue(maybe.HasValue, "비동기 소스에서 첫 번째 요소가 반환되어야 합니다.");
            Assert.AreEqual(100, maybe.Value, "첫 번째 요소가 올바르게 반환되어야 합니다.");
        });

        [UnityTest]
        public IEnumerator TryFirstAsync_FromAsyncSource_WithPredicate_ReturnsMaybeWithMatchingElement() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var list = new List<int> { 50, 75, 100 };
            var asyncSource = UniTask.FromResult<IEnumerable<int>>(list);

            // Act
            var maybe = await asyncSource.TryFirst(x => x > 60);

            // Assert
            Assert.IsTrue(maybe.HasValue, "조건을 만족하는 요소가 있어야 합니다.");
            Assert.AreEqual(75, maybe.Value, "조건에 맞는 첫 번째 요소가 반환되어야 합니다.");
        });

        [UnityTest]
        public IEnumerator TryFirstAsync_FromAsyncSource_WithAsyncPredicate_ReturnsMaybeWithMatchingElement() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var list = new List<int> { 50, 75, 100 };
            var asyncSource = UniTask.FromResult<IEnumerable<int>>(list);

            // Act
            var maybe = await asyncSource.TryFirst(async x => { await UniTask.Yield(); return x > 60; });

            // Assert
            Assert.IsTrue(maybe.HasValue, "비동기 predicate 조건을 만족하는 요소가 있어야 합니다.");
            Assert.AreEqual(75, maybe.Value, "조건에 맞는 첫 번째 요소가 반환되어야 합니다.");
        });

        [Test]
        public async Task TryFirstAsync_FromAsyncSource_ThrowsArgumentNullException_WhenSourceIsNull() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            UniTask<IEnumerable<int>> asyncSource = UniTask.FromResult<IEnumerable<int>>(null);

            // Act & Assert
            await UniTask.Yield();
            Assert.Throws<ArgumentNullException>(async () =>
            {
                await asyncSource.TryFirst();
            });
        });

        [Test]
        public async Task TryFirstAsync_FromAsyncSource_WithPredicate_ThrowsArgumentNullException_WhenPredicateIsNull() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var list = new List<int> { 1, 2, 3 };
            var asyncSource = UniTask.FromResult<IEnumerable<int>>(list);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(async () =>
            {
                await asyncSource.TryFirst((Func<int, bool>)null);
            });
        });

        #endregion
#endif

#if NOPE_AWAITABLE
        #region Awaitable 기반 비동기 테스트
        
#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
        private async Awaitable<T> FromResult<T>(T value)
#pragma warning restore CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
        {
            return value;
        }

        [Test]
        public async System.Threading.Tasks.Task TryFirst_Awaitable_WithAwaitablePredicate_ReturnsMaybeWithFirstMatchingElement()
        {
            // Arrange
            var list = new List<int> { 10, 20, 30 };
            // Fake awaitable predicate: x가 15보다 작으면 true
            var maybe = await list.TryFirst(async x => await FromResult(x < 15));

            // Assert
            Assert.IsTrue(maybe.HasValue, "Awaitable predicate 조건에 맞는 요소가 있어야 합니다.");
            Assert.AreEqual(10, maybe.Value, "조건에 맞는 첫 번째 요소가 반환되어야 합니다.");
        }

        [Test]
        public async System.Threading.Tasks.Task TryFirst_Awaitable_WithAwaitablePredicate_ReturnsMaybeNone_WhenNoMatch()
        {
            // Arrange
            var list = new List<int> { 10, 20, 30 };
            var maybe = await list.TryFirst(async x => await FromResult(x > 100));

            // Assert
            Assert.IsFalse(maybe.HasValue, "조건을 만족하는 요소가 없으면 Maybe.None이어야 합니다.");
            Assert.Throws<InvalidOperationException>(() => { var _ = maybe.Value; });
        }

        [Test]
        public async System.Threading.Tasks.Task TryFirst_Awaitable_FromAsyncSource_ReturnsMaybeWithFirstElement()
        {
            // Arrange
            var list = new List<int> { 100, 200, 300 };
            var asyncSource = FromResult<IEnumerable<int>>(list);

            // Act
            var maybe = await asyncSource.TryFirst();

            // Assert
            Assert.IsTrue(maybe.HasValue, "Awaitable 소스에서 첫 번째 요소가 반환되어야 합니다.");
            Assert.AreEqual(100, maybe.Value, "첫 번째 요소가 올바르게 반환되어야 합니다.");
        }

        [Test]
        public async System.Threading.Tasks.Task TryFirst_Awaitable_FromAsyncSource_WithPredicate_ReturnsMaybeWithMatchingElement()
        {
            // Arrange
            var list = new List<int> { 50, 75, 100 };
            var asyncSource = FromResult<IEnumerable<int>>(list);

            // Act
            var maybe = await asyncSource.TryFirst(x => x > 60);

            // Assert
            Assert.IsTrue(maybe.HasValue, "조건을 만족하는 요소가 있어야 합니다.");
            Assert.AreEqual(75, maybe.Value, "조건에 맞는 첫 번째 요소가 반환되어야 합니다.");
        }

        [Test]
        public async System.Threading.Tasks.Task TryFirst_Awaitable_FromAsyncSource_WithAwaitablePredicate_ReturnsMaybeWithMatchingElement()
        {
            // Arrange
            var list = new List<int> { 50, 75, 100 };
            var asyncSource = FromResult<IEnumerable<int>>(list);

            // Act
            var maybe = await asyncSource.TryFirst(async x => await FromResult(x > 60));

            // Assert
            Assert.IsTrue(maybe.HasValue, "Awaitable predicate 조건을 만족하는 요소가 있어야 합니다.");
            Assert.AreEqual(75, maybe.Value, "조건에 맞는 첫 번째 요소가 반환되어야 합니다.");
        }

        [Test]
        public async System.Threading.Tasks.Task TryFirst_Awaitable_FromAsyncSource_ThrowsArgumentNullException_WhenSourceIsNull()
        {
            // Arrange
            Awaitable<IEnumerable<int>> asyncSource = FromResult<IEnumerable<int>>(null);

            // Act & Assert
            await System.Threading.Tasks.Task.Yield();
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await asyncSource.TryFirst();
            });
        }

        [Test]
        public async System.Threading.Tasks.Task TryFirst_Awaitable_FromAsyncSource_WithPredicate_ThrowsArgumentNullException_WhenPredicateIsNull()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3 };
            var asyncSource = FromResult<IEnumerable<int>>(list);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await asyncSource.TryFirst((Func<int, bool>)null);
            });
        }

        #endregion
#endif
    }
}
