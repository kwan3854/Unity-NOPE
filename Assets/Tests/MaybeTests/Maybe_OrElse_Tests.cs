using NOPE.Runtime.Core.Maybe;
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

namespace NOPE.Tests.MaybeTests
{
    [TestFixture]
    public class Maybe_OrElse_Tests
    {
        #region OrElse<T> -> Maybe<T>

        [Test]
        public void OrElse_Maybe_WithValue_ReturnsOriginalMaybe()
        {
            var maybe = Maybe<int>.From(42);
            var fallback = Maybe<int>.From(100);
            
            var result = maybe.OrElse(() => fallback);
            
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public void OrElse_Maybe_WithoutValue_ReturnsFallbackMaybe()
        {
            var maybe = Maybe<int>.None;
            var fallback = Maybe<int>.From(100);
            
            var result = maybe.OrElse(() => fallback);
            
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(100, result.Value);
        }

        [Test]
        public void OrElse_Maybe_WithoutValue_FallbackIsNone_ReturnsNone()
        {
            var maybe = Maybe<int>.None;
            var fallback = Maybe<int>.None;
            
            var result = maybe.OrElse(() => fallback);
            
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void OrElse_Maybe_FallbackFunctionExecutedOnlyWhenNeeded()
        {
            var maybe = Maybe<int>.From(42);
            var executionCount = 0;
            
            var result = maybe.OrElse(() => 
            {
                executionCount++;
                return Maybe<int>.From(100);
            });
            
            Assert.AreEqual(0, executionCount);
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public void OrElse_Maybe_FallbackFunctionExecutedWhenNoValue()
        {
            var maybe = Maybe<int>.None;
            var executionCount = 0;
            
            var result = maybe.OrElse(() => 
            {
                executionCount++;
                return Maybe<int>.From(100);
            });
            
            Assert.AreEqual(1, executionCount);
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(100, result.Value);
        }

        #endregion

        #region OrElse<T, E> -> Result<T, E>

        [Test]
        public void OrElse_Result_WithValue_ReturnsSuccessResult()
        {
            var maybe = Maybe<int>.From(42);
            var error = "Error";
            
            var result = maybe.OrElse(() => Result<int, string>.Failure(error));
            
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public void OrElse_Result_WithoutValue_ReturnsFallbackResult()
        {
            var maybe = Maybe<int>.None;
            var error = "Error";
            
            var result = maybe.OrElse(() => Result<int, string>.Failure(error));
            
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(error, result.Error);
        }

        [Test]
        public void OrElse_Result_WithoutValue_FallbackIsSuccess_ReturnsSuccess()
        {
            var maybe = Maybe<int>.None;
            
            var result = maybe.OrElse(() => Result<int, string>.Success(100));
            
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(100, result.Value);
        }

        [Test]
        public void OrElse_Result_FallbackFunctionExecutedOnlyWhenNeeded()
        {
            var maybe = Maybe<int>.From(42);
            var executionCount = 0;
            
            var result = maybe.OrElse(() => 
            {
                executionCount++;
                return Result<int, string>.Failure("Error");
            });
            
            Assert.AreEqual(0, executionCount);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }

        #endregion

        #region UniTask Tests

#if NOPE_UNITASK
        [Test]
        public async Task OrElse_UniTask_Maybe_Sync_WithValue_ReturnsOriginalMaybe()
        {
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(42));
            var fallback = Maybe<int>.From(100);
            
            var result = await asyncMaybe.OrElse(() => fallback);
            
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public async Task OrElse_UniTask_Maybe_Sync_WithoutValue_ReturnsFallbackMaybe()
        {
            var asyncMaybe = UniTask.FromResult(Maybe<int>.None);
            var fallback = Maybe<int>.From(100);
            
            var result = await asyncMaybe.OrElse(() => fallback);
            
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(100, result.Value);
        }

        [Test]
        public async Task OrElse_UniTask_Maybe_Async_WithValue_ReturnsOriginalMaybe()
        {
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(42));
            var asyncFallback = UniTask.FromResult(Maybe<int>.From(100));
            
            var result = await asyncMaybe.OrElse(() => asyncFallback);
            
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public async Task OrElse_UniTask_Maybe_Async_WithoutValue_ReturnsFallbackMaybe()
        {
            var asyncMaybe = UniTask.FromResult(Maybe<int>.None);
            var asyncFallback = UniTask.FromResult(Maybe<int>.From(100));
            
            var result = await asyncMaybe.OrElse(() => asyncFallback);
            
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(100, result.Value);
        }

        [Test]
        public async Task OrElse_Maybe_UniTask_WithValue_ReturnsOriginalMaybe()
        {
            var maybe = Maybe<int>.From(42);
            var asyncFallback = UniTask.FromResult(Maybe<int>.From(100));
            
            var result = await maybe.OrElse(() => asyncFallback);
            
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public async Task OrElse_Maybe_UniTask_WithoutValue_ReturnsFallbackMaybe()
        {
            var maybe = Maybe<int>.None;
            var asyncFallback = UniTask.FromResult(Maybe<int>.From(100));
            
            var result = await maybe.OrElse(() => asyncFallback);
            
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(100, result.Value);
        }

        [Test]
        public async Task OrElse_UniTask_Result_Sync_WithValue_ReturnsSuccessResult()
        {
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(42));
            var error = "Error";
            
            var result = await asyncMaybe.OrElse(() => Result<int, string>.Failure(error));
            
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public async Task OrElse_UniTask_Result_Sync_WithoutValue_ReturnsFallbackResult()
        {
            var asyncMaybe = UniTask.FromResult(Maybe<int>.None);
            var error = "Error";
            
            var result = await asyncMaybe.OrElse(() => Result<int, string>.Failure(error));
            
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(error, result.Error);
        }

        [Test]
        public async Task OrElse_UniTask_Result_Async_WithValue_ReturnsSuccessResult()
        {
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(42));
            var asyncFallback = UniTask.FromResult(Result<int, string>.Failure("Error"));
            
            var result = await asyncMaybe.OrElse(() => asyncFallback);
            
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public async Task OrElse_UniTask_Result_Async_WithoutValue_ReturnsFallbackResult()
        {
            var asyncMaybe = UniTask.FromResult(Maybe<int>.None);
            var asyncFallback = UniTask.FromResult(Result<int, string>.Failure("Error"));
            
            var result = await asyncMaybe.OrElse(() => asyncFallback);
            
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual("Error", result.Error);
        }

        [Test]
        public async Task OrElse_Maybe_UniTask_Result_WithValue_ReturnsSuccessResult()
        {
            var maybe = Maybe<int>.From(42);
            var asyncFallback = UniTask.FromResult(Result<int, string>.Failure("Error"));
            
            var result = await maybe.OrElse(() => asyncFallback);
            
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public async Task OrElse_Maybe_UniTask_Result_WithoutValue_ReturnsFallbackResult()
        {
            var maybe = Maybe<int>.None;
            var asyncFallback = UniTask.FromResult(Result<int, string>.Failure("Error"));
            
            var result = await maybe.OrElse(() => asyncFallback);
            
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual("Error", result.Error);
        }

        [Test]
        public async Task OrElse_UniTask_FallbackFunctionExecutedOnlyWhenNeeded()
        {
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(42));
            var executionCount = 0;
            
            var result = await asyncMaybe.OrElse(() => 
            {
                executionCount++;
                return UniTask.FromResult(Maybe<int>.From(100));
            });
            
            Assert.AreEqual(0, executionCount);
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(42, result.Value);
        }
#endif

        #endregion

        #region Awaitable Tests

#if NOPE_AWAITABLE
        [Test]
        public async Task OrElse_Awaitable_Maybe_Sync_WithValue_ReturnsOriginalMaybe()
        {
            var asyncMaybe = AwaitableFromResult(Maybe<int>.From(42));
            var fallback = Maybe<int>.From(100);
            
            var result = await asyncMaybe.OrElse(() => fallback);
            
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public async Task OrElse_Awaitable_Maybe_Sync_WithoutValue_ReturnsFallbackMaybe()
        {
            var asyncMaybe = AwaitableFromResult(Maybe<int>.None);
            var fallback = Maybe<int>.From(100);
            
            var result = await asyncMaybe.OrElse(() => fallback);
            
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(100, result.Value);
        }

        [Test]
        public async Task OrElse_Awaitable_Maybe_Async_WithValue_ReturnsOriginalMaybe()
        {
            var asyncMaybe = AwaitableFromResult(Maybe<int>.From(42));
            var asyncFallback = AwaitableFromResult(Maybe<int>.From(100));
            
            var result = await asyncMaybe.OrElse(() => asyncFallback);
            
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public async Task OrElse_Awaitable_Maybe_Async_WithoutValue_ReturnsFallbackMaybe()
        {
            var asyncMaybe = AwaitableFromResult(Maybe<int>.None);
            var asyncFallback = AwaitableFromResult(Maybe<int>.From(100));
            
            var result = await asyncMaybe.OrElse(() => asyncFallback);
            
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(100, result.Value);
        }

        [Test]
        public async Task OrElse_Maybe_Awaitable_WithValue_ReturnsOriginalMaybe()
        {
            var maybe = Maybe<int>.From(42);
            var asyncFallback = AwaitableFromResult(Maybe<int>.From(100));
            
            var result = await maybe.OrElse(() => asyncFallback);
            
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public async Task OrElse_Maybe_Awaitable_WithoutValue_ReturnsFallbackMaybe()
        {
            var maybe = Maybe<int>.None;
            var asyncFallback = AwaitableFromResult(Maybe<int>.From(100));
            
            var result = await maybe.OrElse(() => asyncFallback);
            
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(100, result.Value);
        }

        [Test]
        public async Task OrElse_Awaitable_Result_Sync_WithValue_ReturnsSuccessResult()
        {
            var asyncMaybe = AwaitableFromResult(Maybe<int>.From(42));
            var error = "Error";
            
            var result = await asyncMaybe.OrElse(() => Result<int, string>.Failure(error));
            
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public async Task OrElse_Awaitable_Result_Sync_WithoutValue_ReturnsFallbackResult()
        {
            var asyncMaybe = AwaitableFromResult(Maybe<int>.None);
            var error = "Error";
            
            var result = await asyncMaybe.OrElse(() => Result<int, string>.Failure(error));
            
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(error, result.Error);
        }

        [Test]
        public async Task OrElse_Awaitable_Result_Async_WithValue_ReturnsSuccessResult()
        {
            var asyncMaybe = AwaitableFromResult(Maybe<int>.From(42));
            var asyncFallback = AwaitableFromResult(Result<int, string>.Failure("Error"));
            
            var result = await asyncMaybe.OrElse(() => asyncFallback);
            
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public async Task OrElse_Awaitable_Result_Async_WithoutValue_ReturnsFallbackResult()
        {
            var asyncMaybe = AwaitableFromResult(Maybe<int>.None);
            var asyncFallback = AwaitableFromResult(Result<int, string>.Failure("Error"));
            
            var result = await asyncMaybe.OrElse(() => asyncFallback);
            
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual("Error", result.Error);
        }

        [Test]
        public async Task OrElse_Maybe_Awaitable_Result_WithValue_ReturnsSuccessResult()
        {
            var maybe = Maybe<int>.From(42);
            var asyncFallback = AwaitableFromResult(Result<int, string>.Failure("Error"));
            
            var result = await maybe.OrElse(() => asyncFallback);
            
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public async Task OrElse_Maybe_Awaitable_Result_WithoutValue_ReturnsFallbackResult()
        {
            var maybe = Maybe<int>.None;
            var asyncFallback = AwaitableFromResult(Result<int, string>.Failure("Error"));
            
            var result = await maybe.OrElse(() => asyncFallback);
            
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual("Error", result.Error);
        }

        [Test]
        public async Task OrElse_Awaitable_FallbackFunctionExecutedOnlyWhenNeeded()
        {
            var asyncMaybe = AwaitableFromResult(Maybe<int>.From(42));
            var executionCount = 0;
            
            var result = await asyncMaybe.OrElse(() => 
            {
                executionCount++;
                return AwaitableFromResult(Maybe<int>.From(100));
            });
            
            Assert.AreEqual(0, executionCount);
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(42, result.Value);
        }

        private async Task<T> AwaitableFromResult<T>(T value)
        {
            await Awaitable.NextFrameAsync();
            return value;
        }
#endif

        #endregion
    }
}