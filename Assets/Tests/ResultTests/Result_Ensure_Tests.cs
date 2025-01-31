using System;
using System.Threading.Tasks;
using NOPE.Runtime.Core.Result;
using NUnit.Framework;
using UnityEngine;

#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace NOPE.Tests.ResultTests
{
    [TestFixture]
    public class Result_Ensure_Tests
    {
        [Test]
        public void Ensure_Sync_ShouldRemainSuccess_IfPredicateIsTrue()
        {
            // Arrange
            var result = Result<int, string>.Success(10);

            // Act
            var ensuredResult = result.Ensure(
                predicate: x => x > 0,
                error: "Value is not greater than 0"
            );

            // Assert
            Assert.IsTrue(ensuredResult.IsSuccess, "Result should remain Success");
            Assert.AreEqual(10, ensuredResult.Value, "Value should remain the same on success+true predicate");
        }

        [Test]
        public void Ensure_Sync_ShouldBecomeFailure_IfPredicateIsFalse()
        {
            // Arrange
            var result = Result<int, string>.Success(10);

            // Act
            var ensuredResult = result.Ensure(
                predicate: x => x < 0,  // false
                error: "Value is not less than 0"
            );

            // Assert
            Assert.IsTrue(ensuredResult.IsFailure, "Result should become Failure if predicate is false");
            Assert.AreEqual("Value is not less than 0", ensuredResult.Error, "Error should match the provided error");
        }

        [Test]
        public void Ensure_Sync_ShouldRemainFailure_IfOriginalResultIsFailure()
        {
            // Arrange
            var result = Result<int, string>.Failure("Original Error");

            // Act
            var ensuredResult = result.Ensure(
                predicate: x => x > 0, 
                error: "Should not matter"
            );

            // Assert
            Assert.IsTrue(ensuredResult.IsFailure, "Should remain failure if original is failure");
            Assert.AreEqual("Original Error", ensuredResult.Error, "Should keep original error message");
        }


#if NOPE_UNITASK
        [Test]
        public async Task Ensure_UniTask_SyncResult_AsyncPredicate_ShouldRemainSuccess_IfPredicateIsTrue()
        {
            // Arrange
            var result = Result<int, string>.Success(42);

            // Act
            var ensuredResult = await result.Ensure(
                asyncPredicate: async x =>
                {
                    await UniTask.DelayFrame(1);
                    return x == 42;  // true
                },
                error: "Value is not 42"
            );

            // Assert
            Assert.IsTrue(ensuredResult.IsSuccess);
            Assert.AreEqual(42, ensuredResult.Value);
        }

        [Test]
        public async Task Ensure_UniTask_SyncResult_AsyncPredicate_ShouldBecomeFailure_IfPredicateIsFalse()
        {
            // Arrange
            var result = Result<int, string>.Success(100);

            // Act
            var ensuredResult = await result.Ensure(
                asyncPredicate: async x =>
                {
                    await UniTask.DelayFrame(1);
                    return x < 0; // false
                },
                error: "Value is not negative"
            );

            // Assert
            Assert.IsTrue(ensuredResult.IsFailure);
            Assert.AreEqual("Value is not negative", ensuredResult.Error);
        }

        [Test]
        public async Task Ensure_UniTask_SyncResult_AsyncPredicate_ShouldRemainFailure_IfOriginalIsFailure()
        {
            // Arrange
            var result = Result<int, string>.Failure("Original failure");

            // Act
            var ensuredResult = await result.Ensure(
                asyncPredicate: async x =>
                {
                    await UniTask.DelayFrame(1);
                    return x > 0; // won't matter
                },
                error: "Should not appear"
            );

            // Assert
            Assert.IsTrue(ensuredResult.IsFailure);
            Assert.AreEqual("Original failure", ensuredResult.Error);
        }

        [Test]
        public async Task Ensure_UniTask_AsyncResult_SyncPredicate_ShouldRemainSuccess_IfPredicateIsTrue()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(10));

            // Act
            var ensuredResult = await asyncResult.Ensure(
                predicate: x => x == 10,
                error: "Value is not 10"
            );

            // Assert
            Assert.IsTrue(ensuredResult.IsSuccess);
            Assert.AreEqual(10, ensuredResult.Value);
        }

        [Test]
        public async Task Ensure_UniTask_AsyncResult_SyncPredicate_ShouldBecomeFailure_IfPredicateIsFalse()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(10));

            // Act
            var ensuredResult = await asyncResult.Ensure(
                predicate: x => x > 999,
                error: "Value is not greater than 999"
            );

            // Assert
            Assert.IsTrue(ensuredResult.IsFailure);
            Assert.AreEqual("Value is not greater than 999", ensuredResult.Error);
        }

        [Test]
        public async Task Ensure_UniTask_AsyncResult_SyncPredicate_ShouldRemainFailure_IfOriginalIsFailure()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Failure("Async original fail"));

            // Act
            var ensuredResult = await asyncResult.Ensure(
                predicate: x => x > 0,
                error: "Not used"
            );

            // Assert
            Assert.IsTrue(ensuredResult.IsFailure);
            Assert.AreEqual("Async original fail", ensuredResult.Error);
        }

        [Test]
        public async Task Ensure_UniTask_AsyncResult_AsyncPredicate_ShouldRemainSuccess_IfPredicateIsTrue()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(123));

            // Act
            var ensuredResult = await asyncResult.Ensure(
                predicateAsync: async x =>
                {
                    await UniTask.DelayFrame(1);
                    return x == 123; 
                },
                error: "Value is not 123"
            );

            // Assert
            Assert.IsTrue(ensuredResult.IsSuccess);
            Assert.AreEqual(123, ensuredResult.Value);
        }

        [Test]
        public async Task Ensure_UniTask_AsyncResult_AsyncPredicate_ShouldBecomeFailure_IfPredicateIsFalse()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(999));

            // Act
            var ensuredResult = await asyncResult.Ensure(
                predicateAsync: async x =>
                {
                    await UniTask.DelayFrame(1);
                    return x < 0; // false
                },
                error: "Value is not negative"
            );

            // Assert
            Assert.IsTrue(ensuredResult.IsFailure);
            Assert.AreEqual("Value is not negative", ensuredResult.Error);
        }

        [Test]
        public async Task Ensure_UniTask_AsyncResult_AsyncPredicate_ShouldRemainFailure_IfOriginalIsFailure()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Failure("Already failed"));

            // Act
            var ensuredResult = await asyncResult.Ensure(
                predicateAsync: async x =>
                {
                    await UniTask.DelayFrame(1);
                    return x > 0;
                },
                error: "Unreachable error"
            );

            // Assert
            Assert.IsTrue(ensuredResult.IsFailure);
            Assert.AreEqual("Already failed", ensuredResult.Error);
        }

#endif // NOPE_UNITASK


#if NOPE_AWAITABLE
        [Test]
        public async Task Ensure_Awaitable_SyncResult_AsyncPredicate_ShouldRemainSuccess_IfPredicateIsTrue()
        {
            // Arrange
            var result = Result<int, string>.Success(50);

            // Act
            var ensuredResult = await result.Ensure(
                asyncPredicate: async x =>
                {
                    await MyTestAwaitable.Delay(1);
                    return x == 50;
                },
                error: "Value is not 50"
            );

            // Assert
            Assert.IsTrue(ensuredResult.IsSuccess);
            Assert.AreEqual(50, ensuredResult.Value);
        }

        [Test]
        public async Task Ensure_Awaitable_SyncResult_AsyncPredicate_ShouldBecomeFailure_IfPredicateIsFalse()
        {
            // Arrange
            var result = Result<int, string>.Success(50);

            // Act
            var ensuredResult = await result.Ensure(
                asyncPredicate: async x =>
                {
                    await MyTestAwaitable.Delay(1);
                    return x < 10; // false
                },
                error: "Value is not less than 10"
            );

            // Assert
            Assert.IsTrue(ensuredResult.IsFailure);
            Assert.AreEqual("Value is not less than 10", ensuredResult.Error);
        }

        [Test]
        public async Task Ensure_Awaitable_SyncResult_AsyncPredicate_ShouldRemainFailure_IfOriginalIsFailure()
        {
            // Arrange
            var result = Result<int, string>.Failure("Awaitable original error");

            // Act
            var ensuredResult = await result.Ensure(
                asyncPredicate: async x =>
                {
                    await MyTestAwaitable.Delay(1);
                    return x > 0;
                },
                error: "ShouldNotHappen"
            );

            // Assert
            Assert.IsTrue(ensuredResult.IsFailure);
            Assert.AreEqual("Awaitable original error", ensuredResult.Error);
        }

        [Test]
        public async Task Ensure_Awaitable_AsyncResult_SyncPredicate_ShouldRemainSuccess_IfPredicateIsTrue()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Success(999));

            // Act
            var ensuredResult = await asyncResult.Ensure(
                predicate: x => x == 999,
                error: "Value is not 999"
            );

            // Assert
            Assert.IsTrue(ensuredResult.IsSuccess);
            Assert.AreEqual(999, ensuredResult.Value);
        }

        [Test]
        public async Task Ensure_Awaitable_AsyncResult_SyncPredicate_ShouldBecomeFailure_IfPredicateIsFalse()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Success(999));

            // Act
            var ensuredResult = await asyncResult.Ensure(
                predicate: x => x < 0,  // false
                error: "Value is not negative"
            );

            // Assert
            Assert.IsTrue(ensuredResult.IsFailure);
            Assert.AreEqual("Value is not negative", ensuredResult.Error);
        }

        [Test]
        public async Task Ensure_Awaitable_AsyncResult_SyncPredicate_ShouldRemainFailure_IfOriginalIsFailure()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Failure("Awaitable fail"));

            // Act
            var ensuredResult = await asyncResult.Ensure(
                predicate: x => x > 0,
                error: "Not used"
            );

            // Assert
            Assert.IsTrue(ensuredResult.IsFailure);
            Assert.AreEqual("Awaitable fail", ensuredResult.Error);
        }

        [Test]
        public async Task Ensure_Awaitable_AsyncResult_AsyncPredicate_ShouldRemainSuccess_IfPredicateIsTrue()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Success(1234));

            // Act
            var ensuredResult = await asyncResult.Ensure(
                predicateAwaitable: async x =>
                {
                    await MyTestAwaitable.Delay(1);
                    return x == 1234;
                },
                error: "Value is not 1234"
            );

            // Assert
            Assert.IsTrue(ensuredResult.IsSuccess);
            Assert.AreEqual(1234, ensuredResult.Value);
        }

        [Test]
        public async Task Ensure_Awaitable_AsyncResult_AsyncPredicate_ShouldBecomeFailure_IfPredicateIsFalse()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Success(1234));

            // Act
            var ensuredResult = await asyncResult.Ensure(
                predicateAwaitable: async x =>
                {
                    await MyTestAwaitable.Delay(1);
                    return x > 9999; // false
                },
                error: "Value is not > 9999"
            );

            // Assert
            Assert.IsTrue(ensuredResult.IsFailure);
            Assert.AreEqual("Value is not > 9999", ensuredResult.Error);
        }

        [Test]
        public async Task Ensure_Awaitable_AsyncResult_AsyncPredicate_ShouldRemainFailure_IfOriginalIsFailure()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Failure("Already fail"));

            // Act
            var ensuredResult = await asyncResult.Ensure(
                predicateAwaitable: async x =>
                {
                    await MyTestAwaitable.Delay(1);
                    return x == 777;
                },
                error: "Unused error"
            );

            // Assert
            Assert.IsTrue(ensuredResult.IsFailure);
            Assert.AreEqual("Already fail", ensuredResult.Error);
        }

        private static class MyTestAwaitable
        {
            public static async Awaitable Delay(int frames)
            {
                // 예시: Task.Delay() 사용
                await Task.Delay(10 * frames);
            }

            public static async Awaitable<Result<T, E>> FromResult<T, E>(Result<T, E> result)
            {
                await Task.Yield();
                return result;
            }
        }
#endif // NOPE_AWAITABLE
    }
}