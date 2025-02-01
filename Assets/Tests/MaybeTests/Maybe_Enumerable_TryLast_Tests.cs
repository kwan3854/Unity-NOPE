using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NOPE.Runtime.Core.Maybe;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace NOPE.Tests.MaybeTests
{
    /// <summary>
    /// MaybeEnumerableExtensions의 TryLast 확장 메서드에 대한 유닛 테스트입니다.
    /// </summary>
    [TestFixture]
    public class Maybe_Enumerable_TryLast_Tests
    {
        #region 동기 테스트

        [Test]
        public void TryLast_ReturnsMaybeWithLastElement_WhenSequenceNotEmpty()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3, 4, 5 };

            // Act
            var maybe = list.TryLast();

            // Assert
            Assert.IsTrue(maybe.HasValue, "비어있지 않은 시퀀스에서 마지막 요소가 있어야 합니다.");
            Assert.AreEqual(5, maybe.Value, "마지막 요소가 올바르게 반환되어야 합니다.");
        }

        [Test]
        public void TryLast_ReturnsMaybeNone_WhenSequenceIsEmpty()
        {
            // Arrange
            var list = new List<int>();

            // Act
            var maybe = list.TryLast();

            // Assert
            Assert.IsFalse(maybe.HasValue, "빈 시퀀스의 경우 Maybe.None이어야 합니다.");
            Assert.Throws<InvalidOperationException>(() => { var _ = maybe.Value; },
                "값이 없는 Maybe에서 Value 접근 시 예외가 발생해야 합니다.");
        }

        [Test]
        public void TryLast_WithPredicate_ReturnsMaybeWithLastMatchingElement()
        {
            // Arrange
            var list = new List<int> { 2, 4, 6, 8, 10 };
            // predicate: 짝수인 경우 (모든 항목이 짝수이므로 마지막은 10)
            // 또는 조건을 조금 변경해서 마지막 짝수가 아니라 조건에 맞는 마지막 항목을 확인해도 됨
            // 여기서는 간단하게 x % 2 == 0 사용
            // 마지막 짝수는 10
            // (조건에 맞는 요소가 없으면 Maybe.None이어야 함)
            
            // Act
            var maybe = list.TryLast(x => x % 2 == 0);

            // Assert
            Assert.IsTrue(maybe.HasValue, "조건을 만족하는 요소가 있어야 합니다.");
            Assert.AreEqual(10, maybe.Value, "조건을 만족하는 마지막 요소가 올바르게 반환되어야 합니다.");
        }

        [Test]
        public void TryLast_WithPredicate_ReturnsMaybeNone_WhenNoElementMatches()
        {
            // Arrange
            var list = new List<int> { 1, 3, 5, 7 };
            
            // Act
            var maybe = list.TryLast(x => x % 2 == 0);

            // Assert
            Assert.IsFalse(maybe.HasValue, "조건을 만족하는 요소가 없으면 Maybe.None이어야 합니다.");
            Assert.Throws<InvalidOperationException>(() => { var _ = maybe.Value; },
                "값이 없는 Maybe에서 Value 접근 시 예외가 발생해야 합니다.");
        }

        [Test]
        public void TryLast_ThrowsArgumentNullException_WhenSourceIsNull()
        {
            // Arrange
            List<int> list = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => list.TryLast(),
                "null 소스 전달 시 ArgumentNullException이 발생해야 합니다.");
        }

        [Test]
        public void TryLast_WithPredicate_ThrowsArgumentNullException_WhenSourceIsNull()
        {
            // Arrange
            List<int> list = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => list.TryLast(x => true),
                "null 소스 전달 시 ArgumentNullException이 발생해야 합니다.");
        }

        [Test]
        public void TryLast_WithPredicate_ThrowsArgumentNullException_WhenPredicateIsNull()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3 };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => list.TryLast((Func<int, bool>)null),
                "null predicate 전달 시 ArgumentNullException이 발생해야 합니다.");
        }

        #endregion

#if NOPE_UNITASK
        #region UniTask 기반 비동기 테스트

        [UnityTest]
        public IEnumerator TryLastAsync_WithAsyncPredicate_ReturnsMaybeWithLastMatchingElement() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var list = new List<int> { 2, 4, 6, 8, 10 };
            // 비동기 predicate: x가 5보다 큰 경우 (마지막은 10)
            var maybe = await list.TryLast(async x =>
            {
                await UniTask.Yield();
                return x > 5;
            });

            // Assert
            Assert.IsTrue(maybe.HasValue, "비동기 predicate 조건을 만족하는 요소가 있어야 합니다.");
            Assert.AreEqual(10, maybe.Value, "조건에 맞는 마지막 요소가 올바르게 반환되어야 합니다.");
        });

        [UnityTest]
        public IEnumerator TryLastAsync_WithAsyncPredicate_ReturnsMaybeNone_WhenNoMatch() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var list = new List<int> { 1, 3, 5, 7 };
            var maybe = await list.TryLast(async x =>
            {
                await UniTask.Yield();
                return x % 2 == 0;
            });

            // Assert
            Assert.IsFalse(maybe.HasValue, "조건에 맞는 요소가 없으면 Maybe.None이어야 합니다.");
            Assert.Throws<InvalidOperationException>(() => { var _ = maybe.Value; });
        });

        [UnityTest]
        public IEnumerator TryLastAsync_FromAsyncSource_ReturnsMaybeWithLastElement() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var list = new List<int> { 10, 20, 30 };
            var asyncSource = UniTask.FromResult<IEnumerable<int>>(list);

            // Act
            var maybe = await asyncSource.TryLast();

            // Assert
            Assert.IsTrue(maybe.HasValue, "비동기 소스에서 마지막 요소가 반환되어야 합니다.");
            Assert.AreEqual(30, maybe.Value, "마지막 요소가 올바르게 반환되어야 합니다.");
        });

        [UnityTest]
        public IEnumerator TryLastAsync_FromAsyncSource_WithPredicate_ReturnsMaybeWithMatchingElement() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var list = new List<int> { 5, 10, 15, 20 };
            var asyncSource = UniTask.FromResult<IEnumerable<int>>(list);

            // Act: predicate은 x가 짝수인 경우, 마지막 짝수는 20
            var maybe = await asyncSource.TryLast(x => x % 2 == 0);

            // Assert
            Assert.IsTrue(maybe.HasValue, "조건을 만족하는 요소가 있어야 합니다.");
            Assert.AreEqual(20, maybe.Value, "조건에 맞는 마지막 요소가 반환되어야 합니다.");
        });

        [UnityTest]
        public IEnumerator TryLastAsync_FromAsyncSource_WithAsyncPredicate_ReturnsMaybeWithMatchingElement() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var list = new List<int> { 5, 10, 15, 20 };
            var asyncSource = UniTask.FromResult<IEnumerable<int>>(list);

            // Act: 비동기 predicate, x < 18이면 true → 마지막 매칭은 15
            var maybe = await asyncSource.TryLast(async x =>
            {
                await UniTask.Yield();
                return x < 18;
            });

            // Assert
            Assert.IsTrue(maybe.HasValue, "비동기 predicate 조건을 만족하는 요소가 있어야 합니다.");
            Assert.AreEqual(15, maybe.Value, "조건에 맞는 마지막 요소가 반환되어야 합니다.");
        });

        [Test]
        public async Task TryLastAsync_FromAsyncSource_ThrowsArgumentNullException_WhenSourceIsNull() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            UniTask<IEnumerable<int>> asyncSource = UniTask.FromResult<IEnumerable<int>>(null);

            // Act & Assert
            await UniTask.Yield();
            Assert.Throws<ArgumentNullException>(async () =>
            {
                await asyncSource.TryLast();
            });
        });

        [Test]
        public async Task TryLastAsync_FromAsyncSource_WithPredicate_ThrowsArgumentNullException_WhenPredicateIsNull() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var list = new List<int> { 1, 2, 3 };
            var asyncSource = UniTask.FromResult<IEnumerable<int>>(list);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(async () =>
            {
                await asyncSource.TryLast((Func<int, bool>)null);
            });
        });

        #endregion
#endif

#if NOPE_AWAITABLE
        #region Awaitable 기반 비동기 테스트

        private async Awaitable<T> FromResult<T>(T value)
        {
            await System.Threading.Tasks.Task.Yield();
            return value;
        }

        [Test]
        public async Task TryLast_Awaitable_ReturnsMaybeWithLastElement_WhenSequenceNotEmpty()
        {
            // Arrange
            var list = new List<int> { 100, 200, 300 };
            var asyncSource = FromResult<IEnumerable<int>>(list);

            // Act
            var maybe = await asyncSource.TryLast();

            // Assert
            Assert.IsTrue(maybe.HasValue, "Awaitable 소스에서 마지막 요소가 반환되어야 합니다.");
            Assert.AreEqual(300, maybe.Value, "마지막 요소가 올바르게 반환되어야 합니다.");
        }

        [Test]
        public async Task TryLast_Awaitable_ReturnsMaybeNone_WhenSequenceIsEmpty()
        {
            // Arrange
            var list = new List<int>();
            var asyncSource = FromResult<IEnumerable<int>>(list);

            // Act
            var maybe = await asyncSource.TryLast();

            // Assert
            Assert.IsFalse(maybe.HasValue, "빈 Awaitable 소스는 Maybe.None이어야 합니다.");
            Assert.Throws<InvalidOperationException>(() => { var _ = maybe.Value; });
        }

        [Test]
        public async Task TryLast_Awaitable_WithPredicate_ReturnsMaybeWithLastMatchingElement()
        {
            // Arrange
            var list = new List<int> { 10, 20, 30, 40 };
            var asyncSource = FromResult<IEnumerable<int>>(list);

            // Act: predicate은 x % 3 == 0인 경우 → 30이 마지막 매칭
            var maybe = await asyncSource.TryLast(x => x % 3 == 0);

            // Assert
            Assert.IsTrue(maybe.HasValue, "조건을 만족하는 Awaitable 요소가 있어야 합니다.");
            Assert.AreEqual(30, maybe.Value, "조건에 맞는 마지막 요소가 반환되어야 합니다.");
        }

        [Test]
        public async Task TryLast_Awaitable_WithAwaitablePredicate_ReturnsMaybeWithLastMatchingElement()
        {
            // Arrange
            var list = new List<int> { 10, 20, 30, 40 };
            var asyncSource = FromResult<IEnumerable<int>>(list);

            // Act: 비동기 predicate, x > 25인 경우 → 마지막 매칭은 40
            var maybe = await asyncSource.TryLast(async x => await FromResult(x > 25));

            // Assert
            Assert.IsTrue(maybe.HasValue, "Awaitable predicate 조건을 만족하는 요소가 있어야 합니다.");
            Assert.AreEqual(40, maybe.Value, "조건에 맞는 마지막 요소가 반환되어야 합니다.");
        }

        [Test]
        public async Task TryLast_Awaitable_FromAsyncSource_ThrowsArgumentNullException_WhenSourceIsNull()
        {
            // Arrange
            Awaitable<IEnumerable<int>> asyncSource = FromResult<IEnumerable<int>>(null);

            // Act & Assert
            await System.Threading.Tasks.Task.Yield();
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await asyncSource.TryLast();
            });
        }

        [Test]
        public async Task TryLast_Awaitable_FromAsyncSource_WithPredicate_ThrowsArgumentNullException_WhenPredicateIsNull()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3 };
            var asyncSource = FromResult<IEnumerable<int>>(list);

            // Act & Assert
            try
            {
                await asyncSource.TryLast((Func<int, bool>)null);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf<ArgumentNullException>(e, "null predicate 전달 시 ArgumentNullException이 발생해야 합니다.");
            }
        }

        #endregion
#endif
    }
}