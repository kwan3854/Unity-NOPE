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
    public class Result_MapError_Tests
    {
        //--------------------------------------------------------------------------
        // 1) 동기 버전 MapError 테스트
        //--------------------------------------------------------------------------

        [Test]
        public void MapError_Sync_ShouldKeepSuccess_IfOriginalIsSuccess()
        {
            // Arrange
            var original = Result<int, string>.Success(10);

            // Act
            // string -> int 변환 예시
            var mapped = original.MapError(err => err.Length);

            // Assert
            Assert.IsTrue(mapped.IsSuccess);
            Assert.AreEqual(10, mapped.Value);
        }

        [Test]
        public void MapError_Sync_ShouldTransformFailure_IfOriginalIsFailure()
        {
            // Arrange
            var original = Result<int, string>.Failure("FailError");

            // Act
            var mapped = original.MapError(err => err.Length); // E1=string => E2=int

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("FailError".Length, mapped.Error);
        }

        [Test]
        public void MapError_Sync_ErrorSelectorThrows_ShouldPropagateException()
        {
            // Arrange
            var original = Result<int, string>.Failure("Fail again");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = original.MapError(err =>
                {
                    throw new InvalidOperationException("Boom!");
                    
                    // fake return
#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.
                    return 0;
#pragma warning restore CS0162 // 접근할 수 없는 코드가 있습니다.
                });
            });
        }

        [Test]
        public void MapError_Sync_SuccessDoesNotCallErrorSelector()
        {
            // Arrange
            var original = Result<int, string>.Success(999);
            bool wasCalled = false;

            // Act
            var mapped = original.MapError(err =>
            {
                wasCalled = true;
                return err.Length;
            });

            // Assert
            Assert.IsTrue(mapped.IsSuccess);
            Assert.AreEqual(999, mapped.Value);
            Assert.IsFalse(wasCalled, "Should not call error selector if original was success");
        }


#if NOPE_UNITASK
        //--------------------------------------------------------------------------
        // 2) UniTask 버전 MapError 테스트
        //--------------------------------------------------------------------------

        //===================== 동기 Result + 비동기 errorSelectorAsync =====================

        [Test]
        public async Task MapError_UniTask_SyncResult_AsyncErrorSelector_SuccessRemainsSuccess()
        {
            // Arrange
            var original = Result<int, string>.Success(42);

            // Act
            var mapped = await original.MapError(async err =>
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(5));
                return err.Length; // E2 = int
            });

            // Assert
            Assert.IsTrue(mapped.IsSuccess);
            Assert.AreEqual(42, mapped.Value);
        }

        [Test]
        public async Task MapError_UniTask_SyncResult_AsyncErrorSelector_FailureTransformsError()
        {
            // Arrange
            var original = Result<int, string>.Failure("Fail me");

            // Act
            var mapped = await original.MapError(async err =>
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(5));
                return err.Length;
            });

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("Fail me".Length, mapped.Error);
        }

        [Test]
        public async Task MapError_UniTask_SyncResult_AsyncErrorSelector_IfSelectorThrowsShouldBubbleUp()
        {
            // Arrange
            var original = Result<int, string>.Failure("Fail!!");

            // Act & Assert
            try
            {
                await original.MapError(async err =>
                {
                    await UniTask.Delay(TimeSpan.FromMilliseconds(5));
                    throw new InvalidOperationException("Boom1");
                    
                    // fake return
#pragma warning disable CS0162 // Unreachable code detected
                    return 0;
#pragma warning restore CS0162 // Unreachable code detected
                });
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf<InvalidOperationException>(e);
                Assert.AreEqual("Boom1", e.Message);
            }
        }

        //===================== 비동기 Result + 동기 errorSelector =====================

        [Test]
        public async Task MapError_UniTask_AsyncResult_SyncErrorSelector_SuccessRemainsSuccess()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(10));

            // Act
            var mapped = await asyncResult.MapError(err => err.Length);

            // Assert
            Assert.IsTrue(mapped.IsSuccess);
            Assert.AreEqual(10, mapped.Value);
        }

        [Test]
        public async Task MapError_UniTask_AsyncResult_SyncErrorSelector_FailureTransformsError()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Failure("Error!!"));

            // Act
            var mapped = await asyncResult.MapError(err => err.Length);

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("Error!!".Length, mapped.Error);
        }

        [Test]
        public async Task MapError_UniTask_AsyncResult_SyncErrorSelector_Throws()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Failure("??"));

            // Act & Assert
            try
            {
                await asyncResult.MapError(err =>
                {
                    throw new FormatException("Boom2");
                    
                    // fake return
#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.
                    return 0;
#pragma warning restore CS0162 // 접근할 수 없는 코드가 있습니다.
                });
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf<FormatException>(e);
                Assert.AreEqual("Boom2", e.Message);
            }
        }

        //===================== 비동기 Result + 비동기 errorSelectorAsync =====================

        [Test]
        public async Task MapError_UniTask_AsyncResult_AsyncErrorSelector_SuccessRemainsSuccess()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(999));

            // Act
            var mapped = await asyncResult.MapError(async err =>
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(5));
                return err.Length;
            });

            // Assert
            Assert.IsTrue(mapped.IsSuccess);
            Assert.AreEqual(999, mapped.Value);
        }

        [Test]
        public async Task MapError_UniTask_AsyncResult_AsyncErrorSelector_FailureTransformsError()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Failure("OtherFailure"));

            // Act
            var mapped = await asyncResult.MapError(async err =>
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(5));
                return err.Length;
            });

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("OtherFailure".Length, mapped.Error);
        }

        [Test]
        public async Task MapError_UniTask_AsyncResult_AsyncErrorSelector_Throws()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Failure("X"));

            // Act & Assert
            try
            {
                await asyncResult.MapError(async err =>
                {
                    await UniTask.Delay(TimeSpan.FromMilliseconds(5));
                    throw new InvalidOperationException("Boom3");
                    
                    // fake return
#pragma warning disable CS0162 // Unreachable code detected
                    return 0;
#pragma warning restore CS0162 // Unreachable code detected
                });
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf<InvalidOperationException>(e);
                Assert.AreEqual("Boom3", e.Message);
            }
        }
#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        //--------------------------------------------------------------------------
        // 3) Awaitable 버전 MapError 테스트
        //--------------------------------------------------------------------------
        // 간단한 MyTestAwaitable로 'Task' 지연 시뮬레이션

        [Test]
        public async Task MapError_Awaitable_SyncResult_AsyncErrorSelector_SuccessRemainsSuccess()
        {
            // Arrange
            var original = Result<int, string>.Success(100);

            // Act
            var mapped = await original.MapError(async err =>
            {
                await MyTestAwaitable.Delay(1);
                return err.Length;
            });

            // Assert
            Assert.IsTrue(mapped.IsSuccess);
            Assert.AreEqual(100, mapped.Value);
        }

        [Test]
        public async Task MapError_Awaitable_SyncResult_AsyncErrorSelector_FailureTransformsError()
        {
            // Arrange
            var original = Result<int, string>.Failure("Failure text");

            // Act
            var mapped = await original.MapError(async err =>
            {
                await MyTestAwaitable.Delay(1);
                return err.Length;
            });

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("Failure text".Length, mapped.Error);
        }

        [Test]
        public async Task MapError_Awaitable_SyncResult_AsyncErrorSelector_Throws()
        {
            // Arrange
            var original = Result<int, string>.Failure("RRR");

            // Act & Assert
            try
            {
                await original.MapError(async err =>
                {
                    await MyTestAwaitable.Delay(1);
                    throw new InvalidOperationException("Boom4");
                    
                    // fake return
                    return 0;
                });
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf<InvalidOperationException>(e);
                Assert.AreEqual("Boom4", e.Message);
            }
        }

        [Test]
        public async Task MapError_Awaitable_AsyncResult_SyncErrorSelector_SuccessRemainsSuccess()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Success(999));

            // Act
            var mapped = await asyncResult.MapError(err => err.Length);

            // Assert
            Assert.IsTrue(mapped.IsSuccess);
            Assert.AreEqual(999, mapped.Value);
        }

        [Test]
        public async Task MapError_Awaitable_AsyncResult_SyncErrorSelector_FailureTransformsError()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Failure("AsyncFail"));

            // Act
            var mapped = await asyncResult.MapError(err => err.Length);

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("AsyncFail".Length, mapped.Error);
        }

        [Test]
        public async Task MapError_Awaitable_AsyncResult_SyncErrorSelector_Throws()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Failure("??"));

            // Act & Assert
            try
            {
                await asyncResult.MapError(err =>
                {
                    throw new InvalidOperationException("Boom5");
                    
                    // fake return
#pragma warning disable CS0162 // Unreachable code detected
                    return 0;
#pragma warning restore CS0162 // Unreachable code detected
                });
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf<InvalidOperationException>(e);
                Assert.AreEqual("Boom5", e.Message);
            }
        }

        [Test]
        public async Task MapError_Awaitable_AsyncResult_AsyncErrorSelector_SuccessRemainsSuccess()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Success(123));

            // Act
            var mapped = await asyncResult.MapError(async err =>
            {
                await MyTestAwaitable.Delay(1);
                return err.Length;
            });

            // Assert
            Assert.IsTrue(mapped.IsSuccess);
            Assert.AreEqual(123, mapped.Value);
        }

        [Test]
        public async Task MapError_Awaitable_AsyncResult_AsyncErrorSelector_FailureTransformsError()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Failure("Hello??"));

            // Act
            var mapped = await asyncResult.MapError(async err =>
            {
                await MyTestAwaitable.Delay(1);
                return err.Length;
            });

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("Hello??".Length, mapped.Error);
        }

        [Test]
        public async Task MapError_Awaitable_AsyncResult_AsyncErrorSelector_Throws()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Failure("Throw error test"));

            // Act & Assert
            try
            {
                await asyncResult.MapError(async err =>
                {
                    await MyTestAwaitable.Delay(1);
                    throw new InvalidOperationException("Boom6");
                    
                    // fake return
#pragma warning disable CS0162 // Unreachable code detected
                    return 0;
#pragma warning restore CS0162 // Unreachable code detected
                });
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf<InvalidOperationException>(e);
                Assert.AreEqual("Boom6", e.Message);
            }
        }

        //--------------------------------------------------------------------------
        // MyTestAwaitable (간단한 awaitable 시뮬레이션)
        //--------------------------------------------------------------------------
        private static class MyTestAwaitable
        {
            public static async Awaitable Delay(int frames)
            {
                await Task.Delay(frames * 10);
            }

            public static async Awaitable<Result<T,E>> FromResult<T,E>(Result<T,E> result)
            {
                await Task.Yield();
                return result;
            }
        }
#endif // NOPE_AWAITABLE
    }
}