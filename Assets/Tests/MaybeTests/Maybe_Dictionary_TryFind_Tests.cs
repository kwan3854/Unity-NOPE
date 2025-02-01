using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NOPE.Runtime.Core.Maybe;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
#if NOPE_AWAITABLE
// Awaitable 관련 네임스페이스(예: NOPE.Awaitable)가 있다면 추가하세요.
#endif

namespace NOPE.Tests.MaybeTests
{
    /// <summary>
    /// MaybeDictionaryExtensions의 TryFind 확장 메서드에 대한 유닛 테스트입니다.
    /// </summary>
    public class Maybe_Dictionary_TryFind_Tests
    {
        #region 동기 테스트

        [Test]
        public void TryFind_ReturnsMaybeWithValue_WhenKeyExists()
        {
            // Arrange
            var dict = new Dictionary<string, int>
            {
                { "one", 1 }
            };

            // Act
            var maybe = dict.TryFind("one");

            // Assert
            Assert.IsTrue(maybe.HasValue, "존재하는 키에 대해 Maybe는 값을 보유해야 합니다.");
            Assert.AreEqual(1, maybe.Value, "키에 해당하는 값이 올바르게 반환되어야 합니다.");
        }

        [Test]
        public void TryFind_ReturnsMaybeNone_WhenKeyDoesNotExist()
        {
            // Arrange
            var dict = new Dictionary<string, int>
            {
                { "one", 1 }
            };

            // Act
            var maybe = dict.TryFind("two");

            // Assert
            Assert.IsFalse(maybe.HasValue, "존재하지 않는 키에 대해 Maybe는 값을 보유하지 않아야 합니다.");
            Assert.Throws<InvalidOperationException>(() => { var _ = maybe.Value; },
                "값이 없는 Maybe에서 Value 접근 시 예외가 발생해야 합니다.");
        }

        [Test]
        public void TryFind_ThrowsArgumentNullException_WhenDictionaryIsNull()
        {
            // Arrange
            Dictionary<string, int> dict = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => dict.TryFind("key"),
                "null 딕셔너리를 전달하면 ArgumentNullException이 발생해야 합니다.");
        }

        #endregion

#if NOPE_UNITASK
        #region UniTask 기반 비동기 테스트

        [UnityTest]
        public IEnumerator TryFindAsync_WithUniTask_ReturnsMaybeWithValue() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var dict = new Dictionary<string, int>
            {
                { "one", 1 }
            };
            var asyncDict = UniTask.FromResult<IDictionary<string, int>>(dict);

            // Act
            var maybe = await asyncDict.TryFind("one");

            // Assert
            Assert.IsTrue(maybe.HasValue, "비동기 딕셔너리에서 존재하는 키에 대해 Maybe는 값을 보유해야 합니다.");
            Assert.AreEqual(1, maybe.Value, "비동기 딕셔너리에서 키에 해당하는 값이 올바르게 반환되어야 합니다.");
        });

        [UnityTest]
        public IEnumerator TryFindAsync_WithUniTask_ReturnsMaybeNone_WhenKeyDoesNotExist() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var dict = new Dictionary<string, int>
            {
                { "one", 1 }
            };
            var asyncDict = UniTask.FromResult<IDictionary<string, int>>(dict);

            // Act
            var maybe = await asyncDict.TryFind("two");

            // Assert
            Assert.IsFalse(maybe.HasValue, "비동기 딕셔너리에서 존재하지 않는 키에 대해 Maybe는 값을 보유하지 않아야 합니다.");
            Assert.Throws<InvalidOperationException>(() => { var _ = maybe.Value; },
                "값이 없는 Maybe에서 Value 접근 시 예외가 발생해야 합니다.");
        });

        [UnityTest]
        public IEnumerator TryFindAsync_WithUniTask_ThrowsArgumentNullException_WhenDictionaryIsNull() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            UniTask<IDictionary<string, int>> asyncDict = UniTask.FromResult<IDictionary<string, int>>(null);

            // Act & Assert
            try
            {
                var maybe = await asyncDict.TryFind("key");
                Assert.Fail("null 딕셔너리 전달 시 ArgumentNullException이 발생해야 합니다.");
            }
            catch (ArgumentNullException)
            {
                // 성공 케이스
            }
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

        // Awaitable 타입이 UniTask와 유사하게 FromResult 메서드를 제공한다고 가정합니다.

        [Test]
        public async System.Threading.Tasks.Task TryFindAsync_Awaitable_ReturnsMaybeWithValue()
        {
            // Arrange
            var dict = new Dictionary<string, int>
            {
                { "one", 1 }
            };
            var asyncDict = FromResult<IDictionary<string, int>>(dict);

            // Act
            var maybe = await asyncDict.TryFind("one");

            // Assert
            Assert.IsTrue(maybe.HasValue, "Awaitable 딕셔너리에서 존재하는 키에 대해 Maybe는 값을 보유해야 합니다.");
            Assert.AreEqual(1, maybe.Value, "Awaitable 딕셔너리에서 키에 해당하는 값이 올바르게 반환되어야 합니다.");
        }

        [Test]
        public async System.Threading.Tasks.Task TryFindAsync_Awaitable_ReturnsMaybeNone_WhenKeyDoesNotExist()
        {
            // Arrange
            var dict = new Dictionary<string, int>
            {
                { "one", 1 }
            };
            var asyncDict = FromResult<IDictionary<string, int>>(dict);

            // Act
            var maybe = await asyncDict.TryFind("two");

            // Assert
            Assert.IsFalse(maybe.HasValue, "Awaitable 딕셔너리에서 존재하지 않는 키에 대해 Maybe는 값을 보유하지 않아야 합니다.");
            Assert.Throws<InvalidOperationException>(() => { var _ = maybe.Value; },
                "값이 없는 Maybe에서 Value 접근 시 예외가 발생해야 합니다.");
        }

        [Test]
#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
        public async System.Threading.Tasks.Task TryFindAsync_Awaitable_ThrowsArgumentNullException_WhenDictionaryIsNull()
#pragma warning restore CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
        {
            // Arrange
            Awaitable<IDictionary<string, int>> asyncDict = FromResult<IDictionary<string, int>>(null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await asyncDict.TryFind("key");
            });
        }

        #endregion
#endif
    }
}
