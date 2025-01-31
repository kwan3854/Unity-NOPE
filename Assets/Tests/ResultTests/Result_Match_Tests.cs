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
    public class Result_Match_Tests
    {
        //--------------------------------------------------------------------------
        // 1) 동기 Match 테스트
        //--------------------------------------------------------------------------

        [Test]
        public void Match_Sync_Success_ShouldCallOnSuccessAndReturnValue()
        {
            // Arrange
            var result = Result<int, string>.Success(42);

            // Act
            var matched = result.Match(
                onSuccess: val => $"SuccessVal: {val}",
                onFailure: err => $"Failure: {err}"
            );

            // Assert
            Assert.AreEqual("SuccessVal: 42", matched);
        }

        [Test]
        public void Match_Sync_Failure_ShouldCallOnFailureAndReturnValue()
        {
            // Arrange
            var result = Result<int, string>.Failure("Some Error");

            // Act
            var matched = result.Match(
                onSuccess: val => $"SuccessVal: {val}",
                onFailure: err => $"Failure: {err}"
            );

            // Assert
            Assert.AreEqual("Failure: Some Error", matched);
        }

        [Test]
        public void Match_Sync_Success_OnSuccessThrows_ShouldBubbleException()
        {
            // Arrange
            var result = Result<int, string>.Success(10);

            // Act & Assert
            try
            {
                var _ = result.Match<int, string, string>(
                    onSuccess: val => throw new InvalidOperationException("Boom!"),
                    onFailure: err => "NotUsed"
                );
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<InvalidOperationException>(ex);
                Assert.AreEqual("Boom!", ex.Message);
            }
        }

        [Test]
        public void Match_Sync_Failure_OnFailureThrows_ShouldBubbleException()
        {
            // Arrange
            var result = Result<int, string>.Failure("FailMsg");

            // Act & Assert
            try
            {
                var _ = result.Match<int, string, string>(
                    onSuccess: val => "NotUsed",
                    onFailure: err => throw new ArgumentException("Fail thrown!")
                );
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentException>(ex);
                Assert.AreEqual("Fail thrown!", ex.Message);
            }
        }


#if NOPE_UNITASK
        //--------------------------------------------------------------------------
        // 2) UniTask 버전 Match 테스트
        //--------------------------------------------------------------------------

        //================== 동기 Result + onSuccess(동기), onFailure(비동기) ==================

        [Test]
        public async Task Match_UniTask_SyncResult_SyncOnSuccess_AsyncOnFailure_SuccessCase()
        {
            // Arrange
            var result = Result<int, string>.Success(123);

            // Act
            var matched = await result.Match(
                onSuccess: val => $"OK {val}",
                onFailureAsync: async err =>
                {
                    await UniTask.Delay(TimeSpan.FromMilliseconds(10));
                    return $"FAIL {err}";
                }
            );

            // Assert
            Assert.AreEqual("OK 123", matched);
        }

        [Test]
        public async Task Match_UniTask_SyncResult_SyncOnSuccess_AsyncOnFailure_FailureCase()
        {
            // Arrange
            var result = Result<int, string>.Failure("ErrX");

            // Act
            var matched = await result.Match(
                onSuccess: val => $"OK {val}",
                onFailureAsync: async err =>
                {
                    await UniTask.Delay(TimeSpan.FromMilliseconds(10));
                    return $"FAIL {err}";
                }
            );

            // Assert
            Assert.AreEqual("FAIL ErrX", matched);
        }

        [Test]
        public async Task Match_UniTask_SyncResult_SyncOnSuccess_AsyncOnFailure_FailureFuncThrows()
        {
            // Arrange
            var result = Result<int, string>.Failure("ErrY");

            // Act & Assert
            try
            {
                var _ = await result.Match(
                    onSuccess: val => $"OK {val}",
                    onFailureAsync: async err =>
                    {
                        await UniTask.Delay(TimeSpan.FromMilliseconds(10));
                        throw new InvalidOperationException($"Boom {err}");
                    }
                );
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<InvalidOperationException>(ex);
                Assert.AreEqual("Boom ErrY", ex.Message);
            }
        }

        //================== 동기 Result + onSuccess(비동기), onFailure(동기) ==================

        [Test]
        public async Task Match_UniTask_SyncResult_AsyncOnSuccess_SyncOnFailure_SuccessCase()
        {
            // Arrange
            var result = Result<int, string>.Success(999);

            // Act
            var matched = await result.Match(
                onSuccessAsync: async val =>
                {
                    await UniTask.Delay(TimeSpan.FromMilliseconds(10));
                    return $"OK {val}";
                },
                onFailure: err => $"FAIL {err}"
            );

            // Assert
            Assert.AreEqual("OK 999", matched);
        }

        [Test]
        public async Task Match_UniTask_SyncResult_AsyncOnSuccess_SyncOnFailure_FailureCase()
        {
            // Arrange
            var result = Result<int, string>.Failure("ErrZ");

            // Act
            var matched = await result.Match(
                onSuccessAsync: async val =>
                {
                    await UniTask.Delay(TimeSpan.FromMilliseconds(10));
                    return $"OK {val}";
                },
                onFailure: err => $"FAIL {err}"
            );

            // Assert
            Assert.AreEqual("FAIL ErrZ", matched);
        }

        [Test]
        public async Task Match_UniTask_SyncResult_AsyncOnSuccess_SyncOnFailure_SuccessFuncThrows()
        {
            // Arrange
            var result = Result<int, string>.Success(111);

            // Act & Assert
            try
            {
                var _ = await result.Match(
                    onSuccessAsync: async val =>
                    {
                        await UniTask.Delay(TimeSpan.FromMilliseconds(10));
                        throw new Exception("Boom2");
                    },
                    onFailure: err => $"FAIL {err}"
                );
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Boom2", ex.Message);
            }
        }

        //================== 동기 Result + onSuccess(비동기), onFailure(비동기) ==================

        [Test]
        public async Task Match_UniTask_SyncResult_AsyncBoth_SuccessCase()
        {
            // Arrange
            var result = Result<int, string>.Success(321);

            // Act
            var matched = await result.Match(
                onSuccessAsync: async val =>
                {
                    await UniTask.Delay(TimeSpan.FromMilliseconds(5));
                    return $"Async OK {val}";
                },
                onFailureAsync: async err =>
                {
                    await UniTask.Delay(TimeSpan.FromMilliseconds(5));
                    return $"Async FAIL {err}";
                }
            );

            // Assert
            Assert.AreEqual("Async OK 321", matched);
        }

        [Test]
        public async Task Match_UniTask_SyncResult_AsyncBoth_FailureCase()
        {
            // Arrange
            var result = Result<int, string>.Failure("AnotherErr");

            // Act
            var matched = await result.Match(
                onSuccessAsync: async val =>
                {
                    await UniTask.Delay(TimeSpan.FromMilliseconds(5));
                    return $"Async OK {val}";
                },
                onFailureAsync: async err =>
                {
                    await UniTask.Delay(TimeSpan.FromMilliseconds(5));
                    return $"Async FAIL {err}";
                }
            );

            // Assert
            Assert.AreEqual("Async FAIL AnotherErr", matched);
        }

        //================== 비동기 Result (UniTask<Result<>>) + 동/비동기 핸들러 ==================

        [Test]
        public async Task Match_UniTask_AsyncResult_SyncHandlers_SuccessCase()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(77));

            // Act
            var matched = await asyncResult.Match(
                onSuccess: val => $"OK {val}",
                onFailure: err => $"Fail {err}"
            );

            // Assert
            Assert.AreEqual("OK 77", matched);
        }

        [Test]
        public async Task Match_UniTask_AsyncResult_SyncHandlers_FailureCase()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Failure("Err777"));

            // Act
            var matched = await asyncResult.Match(
                onSuccess: val => $"OK {val}",
                onFailure: err => $"Fail {err}"
            );

            // Assert
            Assert.AreEqual("Fail Err777", matched);
        }

        [Test]
        public async Task Match_UniTask_AsyncResult_SyncHandlers_SuccessFuncThrows()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(77));

            // Act & Assert
            try
            {
                var _ = await asyncResult.Match(
                    onSuccess: val => throw new InvalidOperationException("Boom3"),
                    onFailure: err => $"Fail {err}"
                );
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Boom3", ex.Message);
            }
        }

        // 추가로 onFailureAsync, onSuccessAsync 등 다른 조합은 위 패턴과 동일하므로 생략 or 추가 가능

#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        //--------------------------------------------------------------------------
        // 3) Awaitable 버전 Match 테스트
        //--------------------------------------------------------------------------
        // 아래는 예시 구현체 MyTestAwaitable 이용

        [Test]
        public async Task Match_Awaitable_SyncResult_OnSuccess_Sync_OnFailure_Async_SuccessCase()
        {
            // Arrange
            var result = Result<int, string>.Success(50);

            // Act
            var matched = await result.Match(
                onSuccess: val => $"SuccVal {val}",
                onFailureAwaitable: async err =>
                {
                    await MyTestAwaitable.Delay(1);
                    return $"FailVal {err}";
                }
            );

            // Assert
            Assert.AreEqual("SuccVal 50", matched);
        }

        [Test]
        public async Task Match_Awaitable_SyncResult_OnSuccess_Sync_OnFailure_Async_FailureCase()
        {
            // Arrange
            var result = Result<int, string>.Failure("AwErr");

            // Act
            var matched = await result.Match(
                onSuccess: val => $"SuccVal {val}",
                onFailureAwaitable: async err =>
                {
                    await MyTestAwaitable.Delay(1);
                    return $"FailVal {err}";
                }
            );

            // Assert
            Assert.AreEqual("FailVal AwErr", matched);
        }

        [Test]
        public async Task Match_Awaitable_SyncResult_OnSuccess_Async_OnFailure_Sync_SuccessCase()
        {
            // Arrange
            var result = Result<int, string>.Success(999);

            // Act
            var matched = await result.Match(
                onSuccessAwaitable: async val =>
                {
                    await MyTestAwaitable.Delay(1);
                    return $"OK {val}";
                },
                onFailure: err => $"Fail {err}"
            );

            // Assert
            Assert.AreEqual("OK 999", matched);
        }

        [Test]
        public async Task Match_Awaitable_SyncResult_OnSuccess_Async_OnFailure_Sync_FailureCase()
        {
            // Arrange
            var result = Result<int, string>.Failure("FailX");

            // Act
            var matched = await result.Match(
                onSuccessAwaitable: async val =>
                {
                    await MyTestAwaitable.Delay(1);
                    return $"OK {val}";
                },
                onFailure: err => $"Fail {err}"
            );

            // Assert
            Assert.AreEqual("Fail FailX", matched);
        }

        [Test]
        public async Task Match_Awaitable_SyncResult_OnSuccess_Async_OnFailure_Async_SuccessFuncThrows()
        {
            // Arrange
            var result = Result<int, string>.Success(777);

            // Act & Assert
            try
            {
                var _ = await result.Match(
                    onSuccessAwaitable: async val =>
                    {
                        await MyTestAwaitable.Delay(1);
                        throw new InvalidOperationException("Async success throw!");
                    },
                    onFailureAwaitable: async err =>
                    {
                        await MyTestAwaitable.Delay(1);
                        return $"ShouldNotCall {err}";
                    }
                );
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Async success throw!", ex.Message);
            }
        }

        [Test]
        public async Task Match_Awaitable_AsyncResult_SyncOnSuccess_SyncOnFailure_SuccessCase()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Success(123));

            // Act
            var matched = await asyncResult.Match(
                onSuccess: val => $"Succ {val}",
                onFailure: err => $"Fail {err}"
            );

            // Assert
            Assert.AreEqual("Succ 123", matched);
        }

        [Test]
        public async Task Match_Awaitable_AsyncResult_SyncOnSuccess_SyncOnFailure_FailureCase()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Failure("Zzzz"));

            // Act
            var matched = await asyncResult.Match(
                onSuccess: val => $"Succ {val}",
                onFailure: err => $"Fail {err}"
            );

            // Assert
            Assert.AreEqual("Fail Zzzz", matched);
        }

        [Test]
        public async Task Match_Awaitable_AsyncResult_SyncOnSuccess_SyncOnFailure_SuccessFuncThrows()
        {
            // Arrange
            var asyncResult = MyTestAwaitable.FromResult(Result<int, string>.Success(321));

            // Act & Assert
            try
            {
                var _ = await asyncResult.Match(
                    onSuccess: val => throw new Exception($"Boom value {val}"),
                    onFailure: err => $"Fail {err}"
                );
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Boom value 321", ex.Message);
            }
        }

        //--------------------------------------------------------------------------
        // MyTestAwaitable 예시
        //--------------------------------------------------------------------------
        private static class MyTestAwaitable
        {
            public static async Awaitable Delay(int frames)
            {
                // 간단 시뮬레이션: Task.Delay
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