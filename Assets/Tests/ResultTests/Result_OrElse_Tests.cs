using NOPE.Runtime.Core.Result;
using NUnit.Framework;
using System;

#if NOPE_UNITASK
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
#endif

#if NOPE_AWAITABLE
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
#endif

namespace NOPE.Tests.ResultTests
{
    [TestFixture]
    public class Result_OrElse_Tests
    {
        #region Basic OrElse Tests

        [Test]
        public void OrElse_Success_ReturnsOriginalResult()
        {
            var result = Result<int, string>.Success(42);
            
            var final = result.OrElse(() => Result<int, string>.Success(100));
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        [Test]
        public void OrElse_Failure_CallsFallbackFunction()
        {
            var result = Result<int, string>.Failure("Error");
            
            var final = result.OrElse(() => Result<int, string>.Success(100));
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(100, final.Value);
        }

        [Test]
        public void OrElse_Failure_FallbackAlsoFailure_ReturnsFallbackFailure()
        {
            var result = Result<int, string>.Failure("First Error");
            
            var final = result.OrElse(() => Result<int, string>.Failure("Second Error"));
            
            Assert.IsTrue(final.IsFailure);
            Assert.AreEqual("Second Error", final.Error);
        }

        [Test]
        public void OrElse_Success_FallbackNotExecuted()
        {
            var result = Result<int, string>.Success(42);
            var fallbackExecuted = false;
            
            var final = result.OrElse(() =>
            {
                fallbackExecuted = true;
                return Result<int, string>.Success(100);
            });
            
            Assert.IsFalse(fallbackExecuted);
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        [Test]
        public void OrElse_Failure_FallbackExecuted()
        {
            var result = Result<int, string>.Failure("Error");
            var fallbackExecuted = false;
            
            var final = result.OrElse(() =>
            {
                fallbackExecuted = true;
                return Result<int, string>.Success(100);
            });
            
            Assert.IsTrue(fallbackExecuted);
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(100, final.Value);
        }

        [Test]
        public void OrElse_ComplexTypes_Success()
        {
            var result = Result<TestClass, Exception>.Success(new TestClass { Value = 42 });
            
            var final = result.OrElse(() => Result<TestClass, Exception>.Success(new TestClass { Value = 100 }));
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value.Value);
        }

        [Test]
        public void OrElse_ComplexTypes_Failure()
        {
            var result = Result<TestClass, Exception>.Failure(new Exception("First"));
            
            var final = result.OrElse(() => Result<TestClass, Exception>.Success(new TestClass { Value = 100 }));
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(100, final.Value.Value);
        }

        [Test]
        public void OrElse_ChainedOperations()
        {
            var result = Result<int, string>.Failure("Error1");
            
            var final = result
                .OrElse(() => Result<int, string>.Failure("Error2"))
                .OrElse(() => Result<int, string>.Success(42));
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        [Test]
        public void OrElse_FallbackBasedOnError()
        {
            var result = Result<int, string>.Failure("NotFound");
            
            var final = result.OrElse(() =>
            {
                if (result.Error == "NotFound")
                    return Result<int, string>.Success(0);
                return Result<int, string>.Failure("Unknown Error");
            });
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(0, final.Value);
        }

        [Test]
        public void OrElse_MultipleFallbacks_ExecutionCount()
        {
            var result = Result<int, string>.Failure("Error");
            var executionCount = 0;
            
            var final = result
                .OrElse(() =>
                {
                    executionCount++;
                    return Result<int, string>.Failure("Error2");
                })
                .OrElse(() =>
                {
                    executionCount++;
                    return Result<int, string>.Success(42);
                });
            
            Assert.AreEqual(2, executionCount);
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        #endregion

        #region UniTask Tests

#if NOPE_UNITASK
        [Test]
        public async Task OrElse_UniTask_Sync_Success_ReturnsOriginalResult()
        {
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(42));
            
            var final = await asyncResult.OrElse(() => Result<int, string>.Success(100));
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        [Test]
        public async Task OrElse_UniTask_Sync_Failure_CallsFallbackFunction()
        {
            var asyncResult = UniTask.FromResult(Result<int, string>.Failure("Error"));
            
            var final = await asyncResult.OrElse(() => Result<int, string>.Success(100));
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(100, final.Value);
        }

        [Test]
        public async Task OrElse_Sync_UniTask_Success_ReturnsOriginalResult()
        {
            var result = Result<int, string>.Success(42);
            
            var final = await result.OrElse(() => UniTask.FromResult(Result<int, string>.Success(100)));
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        [Test]
        public async Task OrElse_Sync_UniTask_Failure_CallsFallbackFunction()
        {
            var result = Result<int, string>.Failure("Error");
            
            var final = await result.OrElse(() => UniTask.FromResult(Result<int, string>.Success(100)));
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(100, final.Value);
        }

        [Test]
        public async Task OrElse_UniTask_UniTask_Success_ReturnsOriginalResult()
        {
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(42));
            
            var final = await asyncResult.OrElse(() => UniTask.FromResult(Result<int, string>.Success(100)));
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        [Test]
        public async Task OrElse_UniTask_UniTask_Failure_CallsFallbackFunction()
        {
            var asyncResult = UniTask.FromResult(Result<int, string>.Failure("Error"));
            
            var final = await asyncResult.OrElse(() => UniTask.FromResult(Result<int, string>.Success(100)));
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(100, final.Value);
        }

        [Test]
        public async Task OrElse_UniTask_FallbackNotExecutedWhenSuccess()
        {
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(42));
            var fallbackExecuted = false;
            
            var final = await asyncResult.OrElse(() =>
            {
                fallbackExecuted = true;
                return UniTask.FromResult(Result<int, string>.Success(100));
            });
            
            Assert.IsFalse(fallbackExecuted);
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        [Test]
        public async Task OrElse_Sync_UniTask_FallbackNotExecutedWhenSuccess()
        {
            var result = Result<int, string>.Success(42);
            var fallbackExecuted = false;
            
            var final = await result.OrElse(() =>
            {
                fallbackExecuted = true;
                return UniTask.FromResult(Result<int, string>.Success(100));
            });
            
            Assert.IsFalse(fallbackExecuted);
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        [Test]
        public async Task OrElse_UniTask_ChainedOperations()
        {
            var asyncResult = UniTask.FromResult(Result<int, string>.Failure("Error1"));
            
            var final = await asyncResult
                .OrElse(() => Result<int, string>.Failure("Error2"))
                .OrElse(() => UniTask.FromResult(Result<int, string>.Success(42)));
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        [Test]
        public async Task OrElse_UniTask_DelayedExecution()
        {
            var asyncResult = UniTask.Run(async () =>
            {
                await UniTask.Delay(10);
                return Result<int, string>.Failure("Error");
            });
            
            var final = await asyncResult.OrElse(() => Result<int, string>.Success(100));
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(100, final.Value);
        }

        [Test]
        public async Task OrElse_UniTask_AsyncFallbackDelayed()
        {
            var result = Result<int, string>.Failure("Error");
            
            var final = await result.OrElse(() => UniTask.Run(async () =>
            {
                await UniTask.Delay(10);
                return Result<int, string>.Success(100);
            }));
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(100, final.Value);
        }
#endif

        #endregion

        #region Awaitable Tests

#if NOPE_AWAITABLE
        [Test]
        public async Task OrElse_Awaitable_Sync_Success_ReturnsOriginalResult()
        {
            var asyncResult = AwaitableFromResult(Result<int, string>.Success(42));
            
            var final = await asyncResult.OrElse(() => Result<int, string>.Success(100));
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        [Test]
        public async Task OrElse_Awaitable_Sync_Failure_CallsFallbackFunction()
        {
            var asyncResult = AwaitableFromResult(Result<int, string>.Failure("Error"));
            
            var final = await asyncResult.OrElse(() => Result<int, string>.Success(100));
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(100, final.Value);
        }

        [Test]
        public async Task OrElse_Sync_Awaitable_Success_ReturnsOriginalResult()
        {
            var result = Result<int, string>.Success(42);
            
            var final = await result.OrElse(() => AwaitableFromResult(Result<int, string>.Success(100)));
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        [Test]
        public async Task OrElse_Sync_Awaitable_Failure_CallsFallbackFunction()
        {
            var result = Result<int, string>.Failure("Error");
            
            var final = await result.OrElse(() => AwaitableFromResult(Result<int, string>.Success(100)));
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(100, final.Value);
        }

        [Test]
        public async Task OrElse_Awaitable_Awaitable_Success_ReturnsOriginalResult()
        {
            var asyncResult = AwaitableFromResult(Result<int, string>.Success(42));
            
            var final = await asyncResult.OrElse(() => AwaitableFromResult(Result<int, string>.Success(100)));
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        [Test]
        public async Task OrElse_Awaitable_Awaitable_Failure_CallsFallbackFunction()
        {
            var asyncResult = AwaitableFromResult(Result<int, string>.Failure("Error"));
            
            var final = await asyncResult.OrElse(() => AwaitableFromResult(Result<int, string>.Success(100)));
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(100, final.Value);
        }

        [Test]
        public async Task OrElse_Awaitable_FallbackNotExecutedWhenSuccess()
        {
            var asyncResult = AwaitableFromResult(Result<int, string>.Success(42));
            var fallbackExecuted = false;
            
            var final = await asyncResult.OrElse(() =>
            {
                fallbackExecuted = true;
                return AwaitableFromResult(Result<int, string>.Success(100));
            });
            
            Assert.IsFalse(fallbackExecuted);
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        [Test]
        public async Task OrElse_Sync_Awaitable_FallbackNotExecutedWhenSuccess()
        {
            var result = Result<int, string>.Success(42);
            var fallbackExecuted = false;
            
            var final = await result.OrElse(() =>
            {
                fallbackExecuted = true;
                return AwaitableFromResult(Result<int, string>.Success(100));
            });
            
            Assert.IsFalse(fallbackExecuted);
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        [Test]
        public async Task OrElse_Awaitable_ChainedOperations()
        {
            var asyncResult = AwaitableFromResult(Result<int, string>.Failure("Error1"));
            
            var final = await (await asyncResult
                .OrElse(() => Result<int, string>.Failure("Error2")))
                .OrElse(() => AwaitableFromResult(Result<int, string>.Success(42)));
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        [Test]
        public async Task OrElse_Awaitable_DelayedExecution()
        {
            var asyncResult = DelayedAwaitableFromResult(Result<int, string>.Failure("Error"));
            
            var final = await asyncResult.OrElse(() => Result<int, string>.Success(100));
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(100, final.Value);
        }

        [Test]
        public async Task OrElse_Awaitable_AsyncFallbackDelayed()
        {
            var result = Result<int, string>.Failure("Error");
            
            var final = await result.OrElse(() => DelayedAwaitableFromResult(Result<int, string>.Success(100)));
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(100, final.Value);
        }

        private async Awaitable<T> AwaitableFromResult<T>(T value)
        {
            await UniTask.Yield();
            return value;
        }

        private async Awaitable<T> DelayedAwaitableFromResult<T>(T value)
        {
            await UniTask.WaitForSeconds(0.0f);
            return value;
        }
#endif

        #endregion

        #region Helper Classes

        private class TestClass
        {
            public int Value { get; set; }
        }

        #endregion
    }
}