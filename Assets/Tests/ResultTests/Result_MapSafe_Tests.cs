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
    public class Result_MapSafe_Tests
    {
        //--------------------------------------------------------------------------
        // 1) 동기 버전 MapSafe 테스트
        //--------------------------------------------------------------------------

        [Test]
        public void MapSafe_Sync_Success_ShouldMapValue()
        {
            // Arrange
            var original = Result<int, string>.Success(10);

            // Act
            var mapped = original.MapSafe(
                x => x * 2,
                ex => $"Error from exception: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mapped.IsSuccess);
            Assert.AreEqual(20, mapped.Value);
        }

        [Test]
        public void MapSafe_Sync_Success_IfSelectorThrowsException_ShouldReturnFailureWithErrorHandler()
        {
            // Arrange
            var original = Result<int, string>.Success(999);

            // Act
            var mapped = original.MapSafe(
                x =>
                {
                    throw new InvalidOperationException("TestException");
#pragma warning disable CS0162 // Unreachable code detected
                    return x + 1;
#pragma warning restore CS0162 // Unreachable code detected
                },
                ex => $"Caught: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("Caught: TestException", mapped.Error);
        }

        [Test]
        public void MapSafe_Sync_Failure_ShouldReturnOriginalFailureWithoutCallingSelector()
        {
            // Arrange
            var original = Result<int, string>.Failure("Original Error");
            bool selectorCalled = false;

            // Act
            var mapped = original.MapSafe(
                x =>
                {
                    selectorCalled = true;
                    return x + 1;
                },
                ex => $"Never used: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("Original Error", mapped.Error);
            Assert.IsFalse(selectorCalled, "Selector should not be called if original is failure");
        }


#if NOPE_UNITASK
        //--------------------------------------------------------------------------
        // 2) UniTask 버전 MapSafe 테스트
        //--------------------------------------------------------------------------

        //====================== 동기 Result + 비동기 Selector =====================

        [Test]
        public async Task MapSafe_UniTask_SyncResult_AsyncSelector_ShouldMapValueOnSuccess()
        {
            // Arrange
            var original = Result<int, string>.Success(42);

            // Act
            var mapped = await original.MapSafe(
                async x =>
                {
                    await UniTask.Delay(1);
                    return x + 8;
                },
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mapped.IsSuccess);
            Assert.AreEqual(50, mapped.Value);
        }

        [Test]
        public async Task MapSafe_UniTask_SyncResult_AsyncSelector_IfSelectorThrows_ShouldReturnFailure()
        {
            // Arrange
            var original = Result<int, string>.Success(999);

            // Act
            var mapped = await original.MapSafe(
                async x =>
                {
                    await UniTask.Delay(1);
                    throw new InvalidOperationException("Boom!");
                    
#pragma warning disable CS0162 // Unreachable code detected
                    return x + 1;
#pragma warning restore CS0162 // Unreachable code detected
                },
                ex => $"SelectorFail: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("SelectorFail: Boom!", mapped.Error);
        }

        [Test]
        public async Task MapSafe_UniTask_SyncResult_AsyncSelector_IfOriginalIsFailure_ShouldSkipSelector()
        {
            // Arrange
            var original = Result<int, string>.Failure("UniTask fail");
            bool selectorCalled = false;

            // Act
            var mapped = await original.MapSafe(
                async x =>
                {
                    selectorCalled = true;
                    await UniTask.Delay(1);
                    return x + 1;
                },
                ex => $"ShouldNotUse: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("UniTask fail", mapped.Error);
            Assert.IsFalse(selectorCalled);
        }

        //====================== 비동기 Result + 동기 Selector =====================

        [Test]
        public async Task MapSafe_UniTask_AsyncResult_SyncSelector_ShouldMapValueOnSuccess()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(10));

            // Act
            var mapped = await asyncResult.MapSafe(
                x => x * 3,
                ex => $"Error: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mapped.IsSuccess);
            Assert.AreEqual(30, mapped.Value);
        }

        [Test]
        public async Task MapSafe_UniTask_AsyncResult_SyncSelector_IfSelectorThrows_ShouldReturnFailure()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(999));

            // Act
            var mapped = await asyncResult.MapSafe(
                x =>
                {
                    throw new InvalidOperationException("Selector broke!");
                    
#pragma warning disable CS0162 // Unreachable code detected
                    return x + 1;
#pragma warning restore CS0162 // Unreachable code detected
                },
                ex => $"CaughtEx: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("CaughtEx: Selector broke!", mapped.Error);
        }

        [Test]
        public async Task MapSafe_UniTask_AsyncResult_SyncSelector_IfOriginalIsFailure_ShouldSkipSelector()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Failure("Error in AsyncResult"));
            bool wasCalled = false;

            // Act
            var mapped = await asyncResult.MapSafe(
                x =>
                {
                    wasCalled = true;
                    return x + 2;
                },
                ex => $"Won't Happen: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("Error in AsyncResult", mapped.Error);
            Assert.IsFalse(wasCalled);
        }

        //====================== 비동기 Result + 비동기 Selector =====================

        [Test]
        public async Task MapSafe_UniTask_AsyncResult_AsyncSelector_ShouldMapValueOnSuccess()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(50));

            // Act
            var mapped = await asyncResult.MapSafe(
                async x =>
                {
                    await UniTask.Delay(1);
                    return x + 10;
                },
                ex => $"Ex: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mapped.IsSuccess);
            Assert.AreEqual(60, mapped.Value);
        }

        [Test]
        public async Task MapSafe_UniTask_AsyncResult_AsyncSelector_IfSelectorThrows_ShouldReturnFailure()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(8));

            // Act
            var mapped = await asyncResult.MapSafe(
                async x =>
                {
                    await UniTask.Delay(1);
                    throw new InvalidOperationException("Map fail!");
                    
#pragma warning disable CS0162 // Unreachable code detected
                    return x + 1;
#pragma warning restore CS0162 // Unreachable code detected
                },
                ex => $"Handled: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("Handled: Map fail!", mapped.Error);
        }

        [Test]
        public async Task MapSafe_UniTask_AsyncResult_AsyncSelector_IfOriginalIsFailure_ShouldSkipSelector()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Failure("Async fail v2"));
            bool wasCalled = false;

            // Act
            var mapped = await asyncResult.MapSafe(
                async x =>
                {
                    wasCalled = true;
                    await UniTask.Delay(1);
                    return x + 999;
                },
                ex => $"ShouldNever: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("Async fail v2", mapped.Error);
            Assert.IsFalse(wasCalled);
        }

#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        //--------------------------------------------------------------------------
        // 3) Awaitable 버전 MapSafe 테스트
        //--------------------------------------------------------------------------

        [Test]
        public async Task MapSafe_Awaitable_SyncResult_AsyncSelector_ShouldMapValueOnSuccess()
        {
            // Arrange
            var original = Result<int, string>.Success(10);

            // Act
            var mapped = await original.MapSafe(
                async x =>
                {
                    await MyTestAwaitable.Delay(1);
                    return x + 5;
                },
                ex => $"AwErr: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mapped.IsSuccess);
            Assert.AreEqual(15, mapped.Value);
        }

        [Test]
        public async Task MapSafe_Awaitable_SyncResult_AsyncSelector_IfSelectorThrows_ShouldReturnFailure()
        {
            // Arrange
            var original = Result<int, string>.Success(999);

            // Act
            var mapped = await original.MapSafe(
                async x =>
                {
                    await MyTestAwaitable.Delay(1);
                    throw new InvalidOperationException("Awaitable thrown!");
                    
#pragma warning disable CS0162 // Unreachable code detected
                    return x + 1;
#pragma warning restore CS0162 // Unreachable code detected
                },
                ex => $"MappedErr: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("MappedErr: Awaitable thrown!", mapped.Error);
        }

        [Test]
        public async Task MapSafe_Awaitable_SyncResult_AsyncSelector_IfOriginalIsFailure_ShouldSkipSelector()
        {
            // Arrange
            var original = Result<int, string>.Failure("Awt fail!");
            bool selectorCalled = false;

            // Act
            var mapped = await original.MapSafe(
                async x =>
                {
                    selectorCalled = true;
                    await MyTestAwaitable.Delay(1);
                    return x + 9999;
                },
                ex => $"Unused: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("Awt fail!", mapped.Error);
            Assert.IsFalse(selectorCalled);
        }

        [Test]
        public async Task MapSafe_Awaitable_AsyncResult_SyncSelector_ShouldMapValueOnSuccess()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Success(123));

            // Act
            var mapped = await asyncResult.MapSafe(
                x => x * 2,
                ex => $"Err: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mapped.IsSuccess);
            Assert.AreEqual(246, mapped.Value);
        }

        [Test]
        public async Task MapSafe_Awaitable_AsyncResult_SyncSelector_IfSelectorThrows_ShouldReturnFailure()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Success(50));

            // Act
            var mapped = await asyncResult.MapSafe(
                x =>
                {
                    throw new InvalidOperationException("Sync throw in Map!");
                    
#pragma warning disable CS0162 // Unreachable code detected
                    return x + 1;
#pragma warning restore CS0162 // Unreachable code detected
                },
                ex => $"AwHandled: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("AwHandled: Sync throw in Map!", mapped.Error);
        }

        [Test]
        public async Task MapSafe_Awaitable_AsyncResult_SyncSelector_IfOriginalIsFailure_ShouldSkipSelector()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Failure("Awaitable fail #2"));
            bool wasCalled = false;

            // Act
            var mapped = await asyncResult.MapSafe(
                x =>
                {
                    wasCalled = true;
                    return x + 1;
                },
                ex => $"Unreachable: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("Awaitable fail #2", mapped.Error);
            Assert.IsFalse(wasCalled);
        }

        [Test]
        public async Task MapSafe_Awaitable_AsyncResult_AsyncSelector_ShouldMapValueOnSuccess()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Success(777));

            // Act
            var mapped = await asyncResult.MapSafe(
                async x =>
                {
                    await MyTestAwaitable.Delay(1);
                    return x + 1;
                },
                ex => $"AwEx: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mapped.IsSuccess);
            Assert.AreEqual(778, mapped.Value);
        }

        [Test]
        public async Task MapSafe_Awaitable_AsyncResult_AsyncSelector_IfSelectorThrows_ShouldReturnFailure()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Success(1000));

            // Act
            var mapped = await asyncResult.MapSafe(
                // ReSharper disable once UnusedParameter.Local
                async x =>
                {
                    await MyTestAwaitable.Delay(1);
                    throw new Exception("Something broke");
                    
#pragma warning disable CS0162 // Unreachable code detected
                    return x + 1;
#pragma warning restore CS0162 // Unreachable code detected
                },
                ex => $"Transformed: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("Transformed: Something broke", mapped.Error);
        }

        [Test]
        public async Task MapSafe_Awaitable_AsyncResult_AsyncSelector_IfOriginalIsFailure_ShouldSkipSelector()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Failure("Awt fail #3"));
            bool wasCalled = false;

            // Act
            var mapped = await asyncResult.MapSafe(
                async x =>
                {
                    wasCalled = true;
                    await MyTestAwaitable.Delay(1);
                    return x + 9999;
                },
                ex => $"Unused2: {ex.Message}"
            );

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("Awt fail #3", mapped.Error);
            Assert.IsFalse(wasCalled);
        }

        //--------------------------------------------------------------------------
        // MyTestAwaitable 예시 (실제 Awaitable 구현에 맞춰 조정)
        //--------------------------------------------------------------------------
        private static class MyTestAwaitable
        {
            public static async Awaitable Delay(int frames)
            {
                await Task.Delay(frames * 10);
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