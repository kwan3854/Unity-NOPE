using NOPE.Runtime.Core.Result;
using NUnit.Framework;
using System;

#if NOPE_UNITASK
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
#endif

#if NOPE_AWAITABLE
using UnityEngine;
#endif

namespace NOPE.Tests.ResultTests
{
    [TestFixture]
    public class Result_Or_Tests
    {
        #region Basic Or Tests

        [Test]
        public void Or_Success_ReturnsOriginalResult()
        {
            var result = Result<int, string>.Success(42);
            var fallback = Result<int, string>.Success(100);
            
            var final = result.Or(fallback);
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        [Test]
        public void Or_Failure_ReturnsFallbackResult()
        {
            var result = Result<int, string>.Failure("Error");
            var fallback = Result<int, string>.Success(100);
            
            var final = result.Or(fallback);
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(100, final.Value);
        }

        [Test]
        public void Or_Failure_FallbackAlsoFailure_ReturnsFallbackFailure()
        {
            var result = Result<int, string>.Failure("First Error");
            var fallback = Result<int, string>.Failure("Second Error");
            
            var final = result.Or(fallback);
            
            Assert.IsTrue(final.IsFailure);
            Assert.AreEqual("Second Error", final.Error);
        }

        [Test]
        public void Or_Success_FallbackIsFailure_ReturnsOriginalSuccess()
        {
            var result = Result<int, string>.Success(42);
            var fallback = Result<int, string>.Failure("Error");
            
            var final = result.Or(fallback);
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        [Test]
        public void Or_ComplexTypes_Success()
        {
            var result = Result<TestClass, Exception>.Success(new TestClass { Value = 42 });
            var fallback = Result<TestClass, Exception>.Success(new TestClass { Value = 100 });
            
            var final = result.Or(fallback);
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value.Value);
        }

        [Test]
        public void Or_ComplexTypes_Failure()
        {
            var result = Result<TestClass, Exception>.Failure(new Exception("First"));
            var fallback = Result<TestClass, Exception>.Success(new TestClass { Value = 100 });
            
            var final = result.Or(fallback);
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(100, final.Value.Value);
        }

        [Test]
        public void Or_ChainedOperations()
        {
            var result = Result<int, string>.Failure("Error1");
            var fallback1 = Result<int, string>.Failure("Error2");
            var fallback2 = Result<int, string>.Success(42);
            
            var final = result.Or(fallback1).Or(fallback2);
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        #endregion

        #region UniTask Tests

#if NOPE_UNITASK
        [Test]
        public async Task Or_UniTask_Sync_Success_ReturnsOriginalResult()
        {
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(42));
            var fallback = Result<int, string>.Success(100);
            
            var final = await asyncResult.Or(fallback);
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        [Test]
        public async Task Or_UniTask_Sync_Failure_ReturnsFallbackResult()
        {
            var asyncResult = UniTask.FromResult(Result<int, string>.Failure("Error"));
            var fallback = Result<int, string>.Success(100);
            
            var final = await asyncResult.Or(fallback);
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(100, final.Value);
        }

        [Test]
        public async Task Or_Sync_UniTask_Success_ReturnsOriginalResult()
        {
            var result = Result<int, string>.Success(42);
            var asyncFallback = UniTask.FromResult(Result<int, string>.Success(100));
            
            var final = await result.Or(asyncFallback);
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        [Test]
        public async Task Or_Sync_UniTask_Failure_ReturnsFallbackResult()
        {
            var result = Result<int, string>.Failure("Error");
            var asyncFallback = UniTask.FromResult(Result<int, string>.Success(100));
            
            var final = await result.Or(asyncFallback);
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(100, final.Value);
        }

        [Test]
        public async Task Or_UniTask_UniTask_Success_ReturnsOriginalResult()
        {
            var asyncResult = UniTask.FromResult(Result<int, string>.Success(42));
            var asyncFallback = UniTask.FromResult(Result<int, string>.Success(100));
            
            var final = await asyncResult.Or(asyncFallback);
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        [Test]
        public async Task Or_UniTask_UniTask_Failure_ReturnsFallbackResult()
        {
            var asyncResult = UniTask.FromResult(Result<int, string>.Failure("Error"));
            var asyncFallback = UniTask.FromResult(Result<int, string>.Success(100));
            
            var final = await asyncResult.Or(asyncFallback);
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(100, final.Value);
        }

        [Test]
        public async Task Or_UniTask_BothFailure_ReturnsFallbackFailure()
        {
            var asyncResult = UniTask.FromResult(Result<int, string>.Failure("First Error"));
            var asyncFallback = UniTask.FromResult(Result<int, string>.Failure("Second Error"));
            
            var final = await asyncResult.Or(asyncFallback);
            
            Assert.IsTrue(final.IsFailure);
            Assert.AreEqual("Second Error", final.Error);
        }

        [Test]
        public async Task Or_UniTask_FallbackNotEvaluatedWhenSuccess()
        {
            var result = Result<int, string>.Success(42);
            var evaluatedFallback = false;
            var asyncFallback = UniTask.Run(() =>
            {
                evaluatedFallback = true;
                return Result<int, string>.Success(100);
            });
            
            var final = await result.Or(asyncFallback);
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
            Assert.IsFalse(evaluatedFallback);
        }

        [Test]
        public async Task Or_UniTask_ChainedOperations()
        {
            var asyncResult = UniTask.FromResult(Result<int, string>.Failure("Error1"));
            var asyncFallback1 = UniTask.FromResult(Result<int, string>.Failure("Error2"));
            var fallback2 = Result<int, string>.Success(42);
            
            var final =  (await asyncResult.Or(asyncFallback1)).Or(fallback2);
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        [Test]
        public async Task Or_UniTask_DelayedExecution()
        {
            var asyncResult = UniTask.Run(async () =>
            {
                await UniTask.Delay(10);
                return Result<int, string>.Failure("Error");
            });
            var fallback = Result<int, string>.Success(100);
            
            var final = await asyncResult.Or(fallback);
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(100, final.Value);
        }
#endif

        #endregion

        #region Awaitable Tests

#if NOPE_AWAITABLE
        [Test]
        public async Task Or_Awaitable_Sync_Success_ReturnsOriginalResult()
        {
            var asyncResult = AwaitableFromResult(Result<int, string>.Success(42));
            var fallback = Result<int, string>.Success(100);
            
            var final = await asyncResult.Or(fallback);
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        [Test]
        public async Task Or_Awaitable_Sync_Failure_ReturnsFallbackResult()
        {
            var asyncResult = AwaitableFromResult(Result<int, string>.Failure("Error"));
            var fallback = Result<int, string>.Success(100);
            
            var final = await asyncResult.Or(fallback);
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(100, final.Value);
        }

        [Test]
        public async Task Or_Sync_Awaitable_Success_ReturnsOriginalResult()
        {
            var result = Result<int, string>.Success(42);
            var asyncFallback = AwaitableFromResult(Result<int, string>.Success(100));
            
            var final = await result.Or(asyncFallback);
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        [Test]
        public async Task Or_Sync_Awaitable_Failure_ReturnsFallbackResult()
        {
            var result = Result<int, string>.Failure("Error");
            var asyncFallback = AwaitableFromResult(Result<int, string>.Success(100));
            
            var final = await result.Or(asyncFallback);
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(100, final.Value);
        }

        [Test]
        public async Task Or_Awaitable_Awaitable_Success_ReturnsOriginalResult()
        {
            var asyncResult = AwaitableFromResult(Result<int, string>.Success(42));
            var asyncFallback = AwaitableFromResult(Result<int, string>.Success(100));
            
            var final = await asyncResult.Or(asyncFallback);
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        [Test]
        public async Task Or_Awaitable_Awaitable_Failure_ReturnsFallbackResult()
        {
            var asyncResult = AwaitableFromResult(Result<int, string>.Failure("Error"));
            var asyncFallback = AwaitableFromResult(Result<int, string>.Success(100));
            
            var final = await asyncResult.Or(asyncFallback);
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(100, final.Value);
        }

        [Test]
        public async Task Or_Awaitable_BothFailure_ReturnsFallbackFailure()
        {
            var asyncResult = AwaitableFromResult(Result<int, string>.Failure("First Error"));
            var asyncFallback = AwaitableFromResult(Result<int, string>.Failure("Second Error"));
            
            var final = await asyncResult.Or(asyncFallback);
            
            Assert.IsTrue(final.IsFailure);
            Assert.AreEqual("Second Error", final.Error);
        }

        [Test]
        public async Task Or_Awaitable_FallbackNotEvaluatedWhenSuccess()
        {
            var result = Result<int, string>.Success(42);
            var evaluatedFallback = false;
            var asyncFallback = EvaluatingAwaitableFromResult(() =>
            {
                evaluatedFallback = true;
                return Result<int, string>.Success(100);
            });
            
            var final = await result.Or(asyncFallback);
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
            Assert.IsFalse(evaluatedFallback);
        }

        [Test]
        public async Task Or_Awaitable_ChainedOperations()
        {
            var asyncResult = AwaitableFromResult(Result<int, string>.Failure("Error1"));
            var asyncFallback1 = AwaitableFromResult(Result<int, string>.Failure("Error2"));
            var fallback2 = Result<int, string>.Success(42);
            
            var final = await (await asyncResult.Or(asyncFallback1)).Or(fallback2);
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(42, final.Value);
        }

        [Test]
        public async Task Or_Awaitable_DelayedExecution()
        {
            var asyncResult = DelayedAwaitableFromResult(Result<int, string>.Failure("Error"));
            var fallback = Result<int, string>.Success(100);
            
            var final = await asyncResult.Or(fallback);
            
            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual(100, final.Value);
        }

        private async Task<T> AwaitableFromResult<T>(T value)
        {
            await Awaitable.NextFrameAsync();
            return value;
        }

        private async Task<T> DelayedAwaitableFromResult<T>(T value)
        {
            await Awaitable.WaitForSecondsAsync(0.01f);
            return value;
        }

        private async Task<T> EvaluatingAwaitableFromResult<T>(Func<T> valueFactory)
        {
            await Awaitable.NextFrameAsync();
            return valueFactory();
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