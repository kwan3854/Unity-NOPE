using NOPE.Runtime;
using NOPE.Runtime.Async;
using NUnit.Framework;
using UnityEngine;

#if NOPE_UNITASK
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
#endif

namespace NOPE.Tests
{
    public class ResultAsyncExtensionsTests
    {
#if NOPE_UNITASK
        [Test]
        public async Task Bind_Success_ResultShouldBeBound_UniTask()
        {
            // Arrange
            UniTask<Result<int>> asyncResult = UniTask.FromResult(Result<int>.Success(10));

            // Act
            var bound = await asyncResult.Bind(x => UniTask.FromResult(Result<string>.Success((x * 2).ToString())));

            // Assert
            Assert.IsTrue(bound.IsSuccess);
            Assert.AreEqual("20", bound.Value);
        }

        [Test]
        public async Task Bind_Failure_ShouldKeepError_UniTask()
        {
            UniTask<Result<int>> asyncResult = UniTask.FromResult(Result<int>.Failure("fail!"));
            var bound = await asyncResult.Bind(x => UniTask.FromResult(Result<string>.Success((x * 2).ToString())));
            Assert.IsTrue(bound.IsFailure);
            Assert.AreEqual("fail!", bound.Error);
        }

        [Test]
        public async Task Tap_Success_ShouldExecuteSideEffect_UniTask()
        {
            UniTask<Result<int>> asyncResult = UniTask.FromResult(Result<int>.Success(10));
            bool sideEffectExecuted = false;
            await asyncResult.Tap(x =>
            {
                sideEffectExecuted = true;
                return UniTask.CompletedTask;
            });
            Assert.IsTrue(sideEffectExecuted);
        }

        [Test]
        public async Task Tap_Failure_ShouldNotExecuteSideEffect_UniTask()
        {
            UniTask<Result<int>> asyncResult = UniTask.FromResult(Result<int>.Failure("fail!"));
            bool sideEffectExecuted = false;
            await asyncResult.Tap(x =>
            {
                sideEffectExecuted = true;
                return UniTask.CompletedTask;
            });
            Assert.IsFalse(sideEffectExecuted);
        }

        [Test]
        public async Task Ensure_Success_ShouldReturnOriginalIfPredicateTrue_UniTask()
        {
            UniTask<Result<int>> asyncResult = UniTask.FromResult(Result<int>.Success(10));
            var ensured = await asyncResult.Ensure(x => UniTask.FromResult(x > 5), "Value is too small");
            Assert.IsTrue(ensured.IsSuccess);
            Assert.AreEqual(10, ensured.Value);
        }

        [Test]
        public async Task Ensure_Success_ShouldReturnFailureIfPredicateFalse_UniTask()
        {
            UniTask<Result<int>> asyncResult = UniTask.FromResult(Result<int>.Success(10));
            var ensured = await asyncResult.Ensure(x => UniTask.FromResult(x > 15), "Value is too small");
            Assert.IsTrue(ensured.IsFailure);
            Assert.AreEqual("Value is too small", ensured.Error);
        }

        [Test]
        public async Task Ensure_Failure_ShouldKeepError_UniTask()
        {
            UniTask<Result<int>> asyncResult = UniTask.FromResult(Result<int>.Failure("fail!"));
            var ensured = await asyncResult.Ensure(x => UniTask.FromResult(x > 5), "Value is too small");
            Assert.IsTrue(ensured.IsFailure);
            Assert.AreEqual("fail!", ensured.Error);
        }

        [Test]
        public async Task Match_Success_ShouldExecuteOnSuccess_UniTask()
        {
            UniTask<Result<int>> asyncResult = UniTask.FromResult(Result<int>.Success(10));
            var result = await asyncResult.Match(
                onSuccessAsync: x => UniTask.FromResult(x * 2),
                onFailureAsync: err => UniTask.FromResult(-1));
            Assert.AreEqual(20, result);
        }

        [Test]
        public async Task Match_Failure_ShouldExecuteOnFailure_UniTask()
        {
            UniTask<Result<int>> asyncResult = UniTask.FromResult(Result<int>.Failure("fail!"));
            var result = await asyncResult.Match(
                onSuccessAsync: x => UniTask.FromResult(x * 2),
                onFailureAsync: err => UniTask.FromResult(-1));
            Assert.AreEqual(-1, result);
        }
#endif

#if NOPE_AWAITABLE
        private static Awaitable<Result<int>> FakeAwaitableSuccess(int value)
        {
            var cts = new AwaitableCompletionSource<Result<int>>();
            cts.SetResult(Result<int>.Success(value));
            return cts.Awaitable;
        }

        private static Awaitable<Result<int>> FakeAwaitableFailure(string error)
        {
            var cts = new AwaitableCompletionSource<Result<int>>();
            cts.SetResult(Result<int>.Failure(error));
            return cts.Awaitable;
        }

        [Test]
        public async Task Bind_Success_ResultShouldBeBound_Awaitable()
        {
            var asyncRes = FakeAwaitableSuccess(10);
            var bound = await asyncRes.Bind(x => FakeAwaitableSuccess(x * 2));
            Assert.IsTrue(bound.IsSuccess);
            Assert.AreEqual(20, bound.Value);
        }

        [Test]
        public async Task Bind_Failure_ShouldKeepError_Awaitable()
        {
            var asyncRes = FakeAwaitableFailure("fail!");
            var bound = await asyncRes.Bind(x => FakeAwaitableSuccess(x * 2));
            Assert.IsTrue(bound.IsFailure);
            Assert.AreEqual("fail!", bound.Error);
        }

        [Test]
        public async Task Tap_Success_ShouldExecuteSideEffect_Awaitable()
        {
            var asyncRes = FakeAwaitableSuccess(10);
            bool sideEffectExecuted = false;
            await asyncRes.Tap(x =>
            {
                sideEffectExecuted = true;
                var cts = new AwaitableCompletionSource();
                cts.SetResult();
                return cts.Awaitable;
            });
            Assert.IsTrue(sideEffectExecuted);
        }

        [Test]
        public async Task Tap_Failure_ShouldNotExecuteSideEffect_Awaitable()
        {
            var asyncRes = FakeAwaitableFailure("fail!");
            bool sideEffectExecuted = false;
            await asyncRes.Tap(x =>
            {
                sideEffectExecuted = true;
                var cts = new AwaitableCompletionSource();
                cts.SetResult();
                return cts.Awaitable;
            });
            Assert.IsFalse(sideEffectExecuted);
        }

        private static Awaitable<T> CreateCompletedAwaitable<T>(T result)
        {
            var cts = new AwaitableCompletionSource<T>();
            cts.SetResult(result);
            return cts.Awaitable;
        }

        [Test]
        public async Task Ensure_Success_ShouldReturnOriginalIfPredicateTrue_Awaitable()
        {
            var asyncRes = FakeAwaitableSuccess(10);
            var ensured = await asyncRes.Ensure(x => CreateCompletedAwaitable(x > 5), "Value is too small");
            Assert.IsTrue(ensured.IsSuccess);
            Assert.AreEqual(10, ensured.Value);
        }

        [Test]
        public async Task Ensure_Success_ShouldReturnFailureIfPredicateFalse_Awaitable()
        {
            var asyncRes = FakeAwaitableSuccess(10);
            var ensured = await asyncRes.Ensure(x => CreateCompletedAwaitable(x > 15), "Value is too small");
            Assert.IsTrue(ensured.IsFailure);
            Assert.AreEqual("Value is too small", ensured.Error);
        }

        [Test]
        public async Task Ensure_Failure_ShouldKeepError_Awaitable()
        {
            var asyncRes = FakeAwaitableFailure("fail!");
            var ensured = await asyncRes.Ensure(x => CreateCompletedAwaitable(x > 5), "Value is too small");
            Assert.IsTrue(ensured.IsFailure);
            Assert.AreEqual("fail!", ensured.Error);
        }

        [Test]
        public async Task Match_Success_ShouldExecuteOnSuccess_Awaitable()
        {
            var asyncRes = FakeAwaitableSuccess(10);
            var result = await asyncRes.Match(
                onSuccessAwaitable: x => CreateCompletedAwaitable(x * 2),
                onFailureAwaitable: err => CreateCompletedAwaitable(-1));
            Assert.AreEqual(20, result);
        }

        [Test]
        public async Task Match_Failure_ShouldExecuteOnFailure_Awaitable()
        {
            var asyncRes = FakeAwaitableFailure("fail!");
            var result = await asyncRes.Match(
                onSuccessAwaitable: x => CreateCompletedAwaitable(x * 2),
                onFailureAwaitable: err => CreateCompletedAwaitable(-1));
            Assert.AreEqual(-1, result);
        }
#endif
    }
}