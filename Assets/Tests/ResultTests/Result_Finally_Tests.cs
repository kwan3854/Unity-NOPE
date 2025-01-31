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
    public class Result_Finally_Tests
    {
        //--------------------------------------------------------------------------
        // 1) 동기 버전 Finally 테스트
        //--------------------------------------------------------------------------

        [Test]
        public void Finally_Sync_Success_ShouldInvokeFinalFuncAndReturnResult()
        {
            // Arrange
            var result = Result<int, string>.Success(42);

            // Act
            // 최종적으로 int -> string 변환(또는 어떤 다른 TOut)해서 반환하는 예시
            var finalValue = result.Finally(finalResult =>
            {
                Assert.IsTrue(finalResult.IsSuccess, "Should receive success result");
                Assert.AreEqual(42, finalResult.Value, "Should pass correct value");
                return $"Final: {finalResult.Value * 2}";
            });

            // Assert
            Assert.AreEqual("Final: 84", finalValue);
        }

        [Test]
        public void Finally_Sync_Failure_ShouldInvokeFinalFuncAndReturnResult()
        {
            // Arrange
            var result = Result<int, string>.Failure("Some Error");

            // Act
            var finalValue = result.Finally(finalResult =>
            {
                Assert.IsTrue(finalResult.IsFailure, "Should receive failure result");
                Assert.AreEqual("Some Error", finalResult.Error, "Should pass correct error");
                return "Handled in final func";
            });

            // Assert
            Assert.AreEqual("Handled in final func", finalValue);
        }

        [Test]
        public void Finally_Sync_FuncThrowsException_ShouldPropagateException()
        {
            // Arrange
            var result = Result<int, string>.Success(10);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
            {
                // 만약 finalFunc 내부가 throw하면, 현재 테스트 메서드로 예외가 propagate됨
                var _ = result.Finally(_ =>
                {
                    throw new InvalidOperationException("Boom!");
                    
                    // fake return value
                    return 0;
                });
            });
        }

#if NOPE_UNITASK
        //--------------------------------------------------------------------------
        // 2) UniTask 버전 Finally 테스트
        //--------------------------------------------------------------------------

        //====================== 동기 Result + 비동기 finalFunc =====================

        [Test]
        public async Task Finally_UniTask_SyncResult_AsyncFinalFunc_Success()
        {
            // Arrange
            var result = Result<int, string>.Success(100);

            // Act
            var finalValue = await result.Finally(async finalResult =>
            {
                await UniTask.DelayFrame(1);
                Assert.IsTrue(finalResult.IsSuccess);
                Assert.AreEqual(100, finalResult.Value);
                return $"AsyncFinal({finalResult.Value})";
            });

            // Assert
            Assert.AreEqual("AsyncFinal(100)", finalValue);
        }

        [Test]
        public async Task Finally_UniTask_SyncResult_AsyncFinalFunc_Failure()
        {
            // Arrange
            var result = Result<int, string>.Failure("Failure msg");

            // Act
            var finalValue = await result.Finally(async finalResult =>
            {
                await UniTask.DelayFrame(1);
                Assert.IsTrue(finalResult.IsFailure);
                Assert.AreEqual("Failure msg", finalResult.Error);
                return "Final from failure";
            });

            // Assert
            Assert.AreEqual("Final from failure", finalValue);
        }

        //====================== 비동기 Result + 동기 finalFunc =====================

        [Test]
        public async Task Finally_UniTask_AsyncResult_SyncFinalFunc_Success()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(999));

            // Act
            var finalValue = await asyncResult.Finally(finalResult =>
            {
                Assert.IsTrue(finalResult.IsSuccess);
                Assert.AreEqual(999, finalResult.Value);
                return "Sync final!";
            });

            // Assert
            Assert.AreEqual("Sync final!", finalValue);
        }

        [Test]
        public async Task Finally_UniTask_AsyncResult_SyncFinalFunc_Failure()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Failure("Async fail"));

            // Act
            var finalValue = await asyncResult.Finally(finalResult =>
            {
                Assert.IsTrue(finalResult.IsFailure);
                Assert.AreEqual("Async fail", finalResult.Error);
                return "Got failure in final";
            });

            // Assert
            Assert.AreEqual("Got failure in final", finalValue);
        }

        //====================== 비동기 Result + 비동기 finalFunc =====================

        [Test]
        public async Task Finally_UniTask_AsyncResult_AsyncFinalFunc_Success()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(50));

            // Act
            var finalValue = await asyncResult.Finally(async finalResult =>
            {
                await UniTask.DelayFrame(1);
                Assert.IsTrue(finalResult.IsSuccess);
                return $"Async final with {finalResult.Value}";
            });

            // Assert
            Assert.AreEqual("Async final with 50", finalValue);
        }

        [Test]
        public async Task Finally_UniTask_AsyncResult_AsyncFinalFunc_Failure()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Failure("UniTask failure"));

            // Act
            var finalValue = await asyncResult.Finally(async finalResult =>
            {
                await UniTask.DelayFrame(1);
                Assert.IsTrue(finalResult.IsFailure);
                return "Handled failure in final";
            });

            // Assert
            Assert.AreEqual("Handled failure in final", finalValue);
        }

        [Test]
        public async Task Finally_UniTask_finalFuncThrows_ShouldThrowException()
        {
            // Arrange
            var result = Result<int, string>.Success(1);
            var exception = new InvalidOperationException("Boom!");
            // Act & Assert

            try
            {
                var someValue = await result.Finally(async _ =>
                {
                    await UniTask.DelayFrame(1);
                    throw exception;

                    // fake return value
                    return 0;
                });
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf<InvalidOperationException>(e);
                Assert.AreEqual(exception, e);
            }
        }

#endif // NOPE_UNITASK


#if NOPE_AWAITABLE
        //--------------------------------------------------------------------------
        // 3) Awaitable 버전 Finally 테스트
        //--------------------------------------------------------------------------

        [Test]
        public async Task Finally_Awaitable_SyncResult_AsyncFinalFunc_Success()
        {
            // Arrange
            var result = Result<int, string>.Success(123);

            // Act
            var finalValue = await result.Finally(async finalResult =>
            {
                await MyTestAwaitable.Delay(1);
                Assert.IsTrue(finalResult.IsSuccess);
                return $"Awaitable final with {finalResult.Value}";
            });

            // Assert
            Assert.AreEqual("Awaitable final with 123", finalValue);
        }

        [Test]
        public async Task Finally_Awaitable_SyncResult_AsyncFinalFunc_Failure()
        {
            // Arrange
            var result = Result<int, string>.Failure("Awaitable error");

            // Act
            var finalValue = await result.Finally(async finalResult =>
            {
                await MyTestAwaitable.Delay(1);
                Assert.IsTrue(finalResult.IsFailure);
                return "Fail handled in final";
            });

            // Assert
            Assert.AreEqual("Fail handled in final", finalValue);
        }

        [Test]
        public async Task Finally_Awaitable_AsyncResult_SyncFinalFunc_Success()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Success(999));

            // Act
            var finalValue = await asyncResult.Finally(finalResult =>
            {
                Assert.IsTrue(finalResult.IsSuccess);
                Assert.AreEqual(999, finalResult.Value);
                return "Sync final via awaitable result";
            });

            // Assert
            Assert.AreEqual("Sync final via awaitable result", finalValue);
        }

        [Test]
        public async Task Finally_Awaitable_AsyncResult_SyncFinalFunc_Failure()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Failure("Async fail"));

            // Act
            var finalValue = await asyncResult.Finally(finalResult =>
            {
                Assert.IsTrue(finalResult.IsFailure);
                Assert.AreEqual("Async fail", finalResult.Error);
                return "Handled fail in sync final";
            });

            // Assert
            Assert.AreEqual("Handled fail in sync final", finalValue);
        }

        [Test]
        public async Task Finally_Awaitable_AsyncResult_AsyncFinalFunc_Success()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Success(777));

            // Act
            var finalValue = await asyncResult.Finally(async finalResult =>
            {
                await MyTestAwaitable.Delay(1);
                Assert.IsTrue(finalResult.IsSuccess);
                return $"awaitable final with {finalResult.Value}";
            });

            // Assert
            Assert.AreEqual("awaitable final with 777", finalValue);
        }

        [Test]
        public async Task Finally_Awaitable_AsyncResult_AsyncFinalFunc_Failure()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Failure("Fail 777"));

            // Act
            var finalValue = await asyncResult.Finally(async finalResult =>
            {
                await MyTestAwaitable.Delay(1);
                Assert.IsTrue(finalResult.IsFailure);
                return "fail handled in final func";
            });

            // Assert
            Assert.AreEqual("fail handled in final func", finalValue);
        }

        [Test]
        public void Finally_Awaitable_finalFuncThrows_ShouldThrowException()
        {
            // Arrange
            var result = Result<int, string>.Success(999);
            var exception = new InvalidOperationException("Boom!");
            // Act & Assert
            try
            {
                var someValue = result.Finally(async _ =>
                {
                    await MyTestAwaitable.Delay(1);
                    throw exception;

                    // fake return value
                    return 0;
                });
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf<InvalidOperationException>(e);
                Assert.AreEqual(exception, e);
            }
        }
        
        private static class MyTestAwaitable
        {
            public static async Awaitable Delay(int frames)
            {
                // 예시로 Task.Delay() 사용
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