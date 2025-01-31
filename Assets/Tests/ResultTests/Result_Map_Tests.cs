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
    public class Result_Map_Tests
    {
        //--------------------------------------------------------------------------
        // 1) 동기 버전 Map 테스트
        //--------------------------------------------------------------------------

        [Test]
        public void Map_Sync_ShouldReturnSuccess_IfOriginalIsSuccess()
        {
            // Arrange
            var original = Result<int, string>.Success(10);

            // Act
            var mapped = original.Map(x => x * 2);

            // Assert
            Assert.IsTrue(mapped.IsSuccess);
            Assert.AreEqual(20, mapped.Value);
        }

        [Test]
        public void Map_Sync_ShouldReturnFailure_IfOriginalIsFailure()
        {
            // Arrange
            var original = Result<int, string>.Failure("Original Failure");

            // Act
            var mapped = original.Map(x => x * 2);

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("Original Failure", mapped.Error);
        }

        [Test]
        public void Map_Sync_SelectorThrowsException_ShouldPropagateException()
        {
            // Arrange
            var original = Result<int, string>.Success(5);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = original.Map(_ =>
                {
                    throw new InvalidOperationException("Boom!");
                    
#pragma warning disable CS0162 // Unreachable code detected
                    return 0;
#pragma warning restore CS0162 // Unreachable code detected
                });
            });
        }

#if NOPE_UNITASK
        //--------------------------------------------------------------------------
        // 2) UniTask 버전 Map 테스트
        //--------------------------------------------------------------------------

        //====================== 동기 Result + 비동기 selector =====================

        [Test]
        public async Task Map_UniTask_SyncResult_AsyncSelector_ShouldReturnSuccess_IfOriginalSuccess()
        {
            // Arrange
            var original = Result<int, string>.Success(42);

            // Act
            var mapped = await original.Map(async x =>
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(10));
                return x * 2;
            });

            // Assert
            Assert.IsTrue(mapped.IsSuccess);
            Assert.AreEqual(84, mapped.Value);
        }

        [Test]
        public async Task Map_UniTask_SyncResult_AsyncSelector_ShouldReturnFailure_IfOriginalFailure()
        {
            // Arrange
            var original = Result<int, string>.Failure("Fail here");

            // Act
            var mapped = await original.Map(async x =>
            {
                // 이 코드는 호출되지 않아야 함
                await UniTask.Delay(TimeSpan.FromMilliseconds(10));
                return x * 100;
            });

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("Fail here", mapped.Error);
        }

        [Test]
        public void Map_UniTask_SyncResult_AsyncSelector_IfSelectorThrows_ShouldThrowException()
        {
            // Arrange
            var original = Result<int, string>.Success(999);

            // Act & Assert
            try
            {
                var _ = original.Map<int, int, string>(async _ =>
                {
                    await UniTask.Delay(TimeSpan.FromMilliseconds(10));
                    throw new InvalidOperationException("Boom!");
                });
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf<InvalidOperationException>(e);
                Assert.AreEqual("Boom!", e.Message);
            }
        }

        //====================== 비동기 Result + 동기 selector =====================

        [Test]
        public async Task Map_UniTask_AsyncResult_SyncSelector_ShouldReturnSuccess_IfOriginalSuccess()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(10));

            // Act
            var mapped = await asyncResult.Map(x => x + 5);

            // Assert
            Assert.IsTrue(mapped.IsSuccess);
            Assert.AreEqual(15, mapped.Value);
        }

        [Test]
        public async Task Map_UniTask_AsyncResult_SyncSelector_ShouldReturnFailure_IfOriginalFailure()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Failure("Async Fail"));

            // Act
            var mapped = await asyncResult.Map(x => x + 5);

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("Async Fail", mapped.Error);
        }

        [Test]
        public void Map_UniTask_AsyncResult_SyncSelector_IfSelectorThrows_ShouldThrowException()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(9));

            // Act & Assert
            try
            {
                var _ = asyncResult.Map(_ =>
                {
                    throw new InvalidOperationException("Boom!");
                    
#pragma warning disable CS0162 // Unreachable code detected
                    return 0;
#pragma warning restore CS0162 // Unreachable code detected
                });
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf<InvalidOperationException>(e);
                Assert.AreEqual("Boom!", e.Message);
            }
        }

        //====================== 비동기 Result + 비동기 selector =====================

        [Test]
        public async Task Map_UniTask_AsyncResult_AsyncSelector_ShouldReturnSuccess_IfOriginalSuccess()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(100));

            // Act
            var mapped = await asyncResult.Map(async x =>
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(10));
                return x / 2;
            });

            // Assert
            Assert.IsTrue(mapped.IsSuccess);
            Assert.AreEqual(50, mapped.Value);
        }

        [Test]
        public async Task Map_UniTask_AsyncResult_AsyncSelector_ShouldReturnFailure_IfOriginalFailure()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Failure("Async Fail v2"));

            // Act
            var mapped = await asyncResult.Map(async x =>
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(10));
                return x * 2;
            });

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("Async Fail v2", mapped.Error);
        }

        [Test]
        public void Map_UniTask_AsyncResult_AsyncSelector_IfSelectorThrows_ShouldThrowException()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(10));

            // Act & Assert
            try
            {
                var _ = asyncResult.Map<int, int, string>(async _ =>
                {
                    await UniTask.Delay(TimeSpan.FromMilliseconds(10));
                    throw new InvalidOperationException("Boom!");
                });
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf<InvalidOperationException>(e);
                Assert.AreEqual("Boom!", e.Message);
            }
        }
#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        //--------------------------------------------------------------------------
        // 3) Awaitable 버전 Map 테스트
        //--------------------------------------------------------------------------

        // 여기서는 'MyTestAwaitable'로 'Task' 기반 지연을 시뮬레이션합니다.
        // 실제 Awaitable 구현에 맞춰 조정하세요.

        [Test]
        public async Task Map_Awaitable_SyncResult_AsyncSelector_ShouldReturnSuccess_IfOriginalSuccess()
        {
            // Arrange
            var original = Result<int, string>.Success(50);

            // Act
            var mapped = await original.Map(async x =>
            {
                await MyTestAwaitable.Delay(1);
                return x * 3;
            });

            // Assert
            Assert.IsTrue(mapped.IsSuccess);
            Assert.AreEqual(150, mapped.Value);
        }

        [Test]
        public async Task Map_Awaitable_SyncResult_AsyncSelector_ShouldReturnFailure_IfOriginalFailure()
        {
            // Arrange
            var original = Result<int, string>.Failure("Awaitable fail");

            // Act
            var mapped = await original.Map(async x =>
            {
                await MyTestAwaitable.Delay(1);
                return x * 999;
            });

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("Awaitable fail", mapped.Error);
        }

        [Test]
        public void Map_Awaitable_SyncResult_AsyncSelector_IfSelectorThrows_ShouldThrowException()
        {
            // Arrange
            var original = Result<int, string>.Success(999);

            // Act & Assert
            try
            {
                var _ = original.Map<int, int, string>(async _ =>
                {
                    await MyTestAwaitable.Delay(1);
                    throw new InvalidOperationException("Boom!");
                });
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf<InvalidOperationException>(e);
                Assert.AreEqual("Boom!", e.Message);
            }
        }

        [Test]
        public async Task Map_Awaitable_AsyncResult_SyncSelector_ShouldReturnSuccess_IfOriginalSuccess()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Success(10));

            // Act
            var mapped = await asyncResult.Map(x => x + 7);

            // Assert
            Assert.IsTrue(mapped.IsSuccess);
            Assert.AreEqual(17, mapped.Value);
        }

        [Test]
        public async Task Map_Awaitable_AsyncResult_SyncSelector_ShouldReturnFailure_IfOriginalFailure()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Failure("Fail from awaitable"));

            // Act
            var mapped = await asyncResult.Map(x => x * 2);

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("Fail from awaitable", mapped.Error);
        }

        [Test]
        public void Map_Awaitable_AsyncResult_SyncSelector_IfSelectorThrows_ShouldThrowException()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Success(999));

            // Act & Assert
            try
            {
                var _ = asyncResult.Map(_ =>
                {
                    throw new InvalidOperationException("Boom!");
#pragma warning disable CS0162 // Unreachable code detected
                    return 0;
#pragma warning restore CS0162 // Unreachable code detected
                });
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf<InvalidOperationException>(e);
                Assert.AreEqual("Boom!", e.Message);
            }
        }

        [Test]
        public async Task Map_Awaitable_AsyncResult_AsyncSelector_ShouldReturnSuccess_IfOriginalSuccess()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Success(5));

            // Act
            var mapped = await asyncResult.Map(async x =>
            {
                await MyTestAwaitable.Delay(1);
                return x + 10;
            });

            // Assert
            Assert.IsTrue(mapped.IsSuccess);
            Assert.AreEqual(15, mapped.Value);
        }

        [Test]
        public async Task Map_Awaitable_AsyncResult_AsyncSelector_ShouldReturnFailure_IfOriginalFailure()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Failure("Fail x2"));

            // Act
            var mapped = await asyncResult.Map(async x =>
            {
                await MyTestAwaitable.Delay(1);
                return x * 2;
            });

            // Assert
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("Fail x2", mapped.Error);
        }

        [Test]
        public void Map_Awaitable_AsyncResult_AsyncSelector_IfSelectorThrows_ShouldThrowException()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Success(11));

            // Act & Assert
            try
            {
                var _ = asyncResult.Map<int, int, string>(async _ =>
                {
                    await MyTestAwaitable.Delay(1);
                    throw new InvalidOperationException("Boom!");
                });
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf<InvalidOperationException>(e);
                Assert.AreEqual("Boom!", e.Message);
            }
        }

        //--------------------------------------------------------------------------
        // MyTestAwaitable 예시 (실제 Awaitable 구현체에 맞춰 교체)
        //--------------------------------------------------------------------------
        private static class MyTestAwaitable
        {
            public static async Awaitable Delay(int frames)
            {
                // 단순 예시로 Task.Delay 사용
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