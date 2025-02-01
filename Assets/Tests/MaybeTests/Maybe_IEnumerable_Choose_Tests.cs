using System;
using System.Collections.Generic;
using System.Linq;
using NOPE.Runtime.Core.Maybe;
using NUnit.Framework;
#if NOPE_UNITASK
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
#endif

#if NOPE_AWAITABLE
using UnityEngine;
#endif

namespace NOPE.Tests.MaybeTests
{
    /// <summary>
    /// MaybeEnumerableExtensions의 Choose 확장 메서드에 대한 유닛 테스트입니다.
    /// </summary>
    public class Maybe_Enumerable_Choose_Tests
    {
        #region 동기 테스트

        [Test]
        public void Choose_WithoutSelector_FiltersOnlyValues()
        {
            // Arrange: 일부는 값이 있고 일부는 None인 시퀀스 준비
            var maybes = new List<Maybe<int>>
            {
                1,            // 암시적 변환으로 Maybe<int>.From(1)
                Maybe<int>.None,
                3,
                Maybe<int>.None,
                5
            };

            // Act
            IEnumerable<int> result = maybes.Choose();

            // Assert
            CollectionAssert.AreEqual(new[] { 1, 3, 5 }, result.ToArray(), "값이 있는 항목만 선택되어야 합니다.");
        }

        [Test]
        public void Choose_WithSelector_FiltersAndTransformsValues()
        {
            // Arrange
            var maybes = new List<Maybe<int>>
            {
                2,
                Maybe<int>.None,
                4,
                6,
                Maybe<int>.None
            };

            // Act: 각 값을 10배하는 변환 함수 적용
            IEnumerable<int> result = maybes.Choose(x => x * 10);

            // Assert
            CollectionAssert.AreEqual(new[] { 20, 40, 60 }, result.ToArray(), "값이 변환되어 선택되어야 합니다.");
        }

        [Test]
        public void Choose_WithoutSelector_ThrowsArgumentNullException_WhenSourceIsNull()
        {
            // Arrange
            List<Maybe<int>> source = null;

            // Act & Assert
            // Assert.Throws<ArgumentNullException>(() => source.Choose(), "null source 전달 시 ArgumentNullException이 발생해야 합니다.");
            
            // ========================================================================================================
            // 수정: iterator block(즉, yield return을 사용하는 메서드)는 실행이 지연(deferred execution)되므로
            // 확장 메서드인 Choose 메서드들은 호출 시점에 바로 내부 코드가 실행되지 않고,
            // 반환된 IEnumerable의 GetEnumerator() 혹은 MoveNext()가 호출될 때 내부 코드가 실행됩니다.
            // 따라서, null 체크와 관련된 예외 발생도 실제로 열거(iteration)를 시작할 때 일어나게 됩니다.
            // 이러한 특성 때문에 null 체크 예외는 테스트 코드에서 직접 확인할 수 없으므로,
            // 아래와 같이 null 체크 예외가 발생하는 코드를 실행하고, 예외가 발생하는지 확인합니다.
            // ========================================================================================================
            
            // Act
            var ex = Assert.Catch<ArgumentNullException>(() =>
            {
                var result = source.Choose();
                foreach (var _ in result)
                {
                    // do nothing
                }
            });
            
            // Assert
            Assert.IsNotNull(ex, "null source 전달 시 ArgumentNullException이 발생해야 합니다.");
        }

        [Test]
        public void Choose_WithSelector_ThrowsArgumentNullException_WhenSourceIsNull()
        {
            // Arrange
            List<Maybe<int>> source = null;

            // Act & Assert
            var ex = Assert.Catch<ArgumentNullException>(() =>
            {
                var result = source.Choose(x => x * 2);
                foreach (var _ in result)
                {
                    // do nothing
                }
            });
            
            // Assert
            Assert.IsNotNull(ex, "null source 전달 시 ArgumentNullException이 발생해야 합니다.");
        }

        [Test]
        public void Choose_WithSelector_ThrowsArgumentNullException_WhenSelectorIsNull()
        {
            // Arrange
            var maybes = new List<Maybe<int>> { 1, 2, 3 };

            // Act & Assert
            var ex = Assert.Catch<ArgumentNullException>(() =>
            {
                Func<int, int> selector = null;
                var result = maybes.Choose(selector);
                foreach (var _ in result)
                {
                    // do nothing
                }
            });
            
            // Assert
            Assert.IsNotNull(ex, "null selector 전달 시 ArgumentNullException이 발생해야 합니다.");
        }

        #endregion

#if NOPE_UNITASK
        #region UniTask 기반 비동기 테스트

        [UnityTest]
        public IEnumerator Choose_WithAsyncSelector_FiltersAndTransformsValues_UniTask() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybes = new List<Maybe<int>>
            {
                1,
                Maybe<int>.None,
                3,
                5,
                Maybe<int>.None
            };

            // Act: 비동기 selector (예: 값에 100을 더함)
            IEnumerable<int> result = await maybes.Choose(async x =>
            {
                await UniTask.Yield();
                return x + 100;
            });

            // Assert
            CollectionAssert.AreEqual(new[] { 101, 103, 105 }, result.ToArray(), "비동기 selector 적용 후 올바른 결과가 나와야 합니다.");
        });

        [UnityTest]
        public IEnumerator Choose_FromAsyncSource_FiltersOnlyValues_UniTask() => UniTask.ToCoroutine(async () =>
        {
            // Arrange: UniTask<IEnumerable<Maybe<int>>> 생성
            var maybes = new List<Maybe<int>>
            {
                10,
                Maybe<int>.None,
                20,
                30,
                Maybe<int>.None
            };
            var asyncSource = UniTask.FromResult<IEnumerable<Maybe<int>>>(maybes);

            // Act
            IEnumerable<int> result = await asyncSource.Choose();

            // Assert
            CollectionAssert.AreEqual(new[] { 10, 20, 30 }, result.ToArray(), "UniTask async source에서 값이 올바르게 선택되어야 합니다.");
        });

        [UnityTest]
        public IEnumerator Choose_WithSelector_FromAsyncSource_FiltersAndTransformsValues_UniTask() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybes = new List<Maybe<int>>
            {
                5,
                Maybe<int>.None,
                15
            };
            var asyncSource = UniTask.FromResult<IEnumerable<Maybe<int>>>(maybes);

            // Act: selector는 값을 문자열로 변환
            IEnumerable<string> result = await asyncSource.Choose(x => $"Value: {x}");

            // Assert
            CollectionAssert.AreEqual(new[] { "Value: 5", "Value: 15" }, result.ToArray(), "비동기 source와 selector가 올바르게 동작해야 합니다.");
        });

        [UnityTest]
        public IEnumerator Choose_WithAsyncSelector_FromAsyncSource_FiltersAndTransformsValues_UniTask() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var maybes = new List<Maybe<int>>
            {
                7,
                Maybe<int>.None,
                14
            };
            var asyncSource = UniTask.FromResult<IEnumerable<Maybe<int>>>(maybes);

            // Act: 비동기 selector (예: 값을 제곱)
            IEnumerable<int> result = await asyncSource.Choose(async x =>
            {
                await UniTask.Yield();
                return x * x;
            });

            // Assert
            CollectionAssert.AreEqual(new[] { 49, 196 }, result.ToArray(), "비동기 selector를 통한 값 변환이 올바르게 수행되어야 합니다.");
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
        public async System.Threading.Tasks.Task Choose_FromAsyncSource_Awaitable_FiltersOnlyValues()
        {
            // Arrange
            var maybes = new List<Maybe<int>>
            {
                100,
                Maybe<int>.None,
                200
            };
            var asyncSource = FromResult<IEnumerable<Maybe<int>>>(maybes);

            // Act
            IEnumerable<int> result = await asyncSource.Choose();

            // Assert
            CollectionAssert.AreEqual(new[] { 100, 200 }, result.ToArray(), "Awaitable async source에서 값이 올바르게 선택되어야 합니다.");
        }

        [Test]
        public async System.Threading.Tasks.Task Choose_WithSelector_FromAsyncSource_Awaitable_FiltersAndTransformsValues()
        {
            // Arrange
            var maybes = new List<Maybe<int>>
            {
                3,
                Maybe<int>.None,
                6
            };
            var asyncSource = FromResult<IEnumerable<Maybe<int>>>(maybes);

            // Act: selector는 값을 문자열로 변환
            IEnumerable<string> result = await asyncSource.Choose(x => $"Num: {x}");

            // Assert
            CollectionAssert.AreEqual(new[] { "Num: 3", "Num: 6" }, result.ToArray(), "Awaitable async source와 selector가 올바르게 동작해야 합니다.");
        }

        [Test]
        public async System.Threading.Tasks.Task Choose_WithAwaitableSelector_FromAsyncSource_Awaitable_FiltersAndTransformsValues()
        {
            // Arrange
            var maybes = new List<Maybe<int>>
            {
                4,
                Maybe<int>.None,
                8
            };
            var asyncSource = FromResult<IEnumerable<Maybe<int>>>(maybes);

            // Act: selectorAwaitable는 값을 3배하여 문자열로 변환
            IEnumerable<string> result = await asyncSource.Choose(async x =>
            {
                // 가짜 Awaitable: await할 수 있도록 FromResult 사용
                return await FromResult($"Triple: {x * 3}");
            });

            // Assert
            CollectionAssert.AreEqual(new[] { "Triple: 12", "Triple: 24" }, result.ToArray(), "Awaitable selector를 통한 값 변환이 올바르게 수행되어야 합니다.");
        }

        #endregion
#endif
    }
}
