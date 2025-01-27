using NOPE.Runtime.Core.Result;
using NOPE.Runtime.Core.Result.Async;
using NUnit.Framework;
using UnityEngine;
using System.Threading.Tasks;

#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace NOPE.Tests
{
    /// <summary>
    /// Comprehensive test suite for Result async extensions, covering both
    /// UniTask and custom Awaitable (depending on define symbols).
    /// Includes "Finally" in the CFE style (end-of-chain) plus other extension methods.
    /// </summary>
    public class ResultAsyncExtensionsTests
    {
#if NOPE_UNITASK
        //===============================================================
        // 1. Basic Tests (Bind, Tap, Ensure, Map) for UniTask
        //===============================================================

        [Test]
        public async Task Bind_Success_ResultShouldBeBound_UniTask()
        {
            // Arrange
            var asyncResult = UniTask.FromResult(Result<int>.Success(10));

            // Act
            var bound = await asyncResult.Bind(x =>
                UniTask.FromResult(Result<string>.Success((x * 2).ToString())));

            // Assert
            Assert.IsTrue(bound.IsSuccess);
            Assert.AreEqual("20", bound.Value);
        }

        [Test]
        public async Task Bind_Failure_ShouldKeepError_UniTask()
        {
            var asyncResult = UniTask.FromResult(Result<int>.Failure("fail!"));
            var bound = await asyncResult.Bind(x =>
                UniTask.FromResult(Result<string>.Success((x * 2).ToString())));

            Assert.IsTrue(bound.IsFailure);
            Assert.AreEqual("fail!", bound.Error);
        }

        [Test]
        public async Task Tap_Success_ShouldExecuteSideEffect_UniTask()
        {
            var asyncResult = UniTask.FromResult(Result<int>.Success(10));
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
            var asyncResult = UniTask.FromResult(Result<int>.Failure("fail!"));
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
            var asyncResult = UniTask.FromResult(Result<int>.Success(10));
            var ensured = await asyncResult.Ensure(x => UniTask.FromResult(x > 5), "Value is too small");

            Assert.IsTrue(ensured.IsSuccess);
            Assert.AreEqual(10, ensured.Value);
        }

        [Test]
        public async Task Ensure_Success_ShouldReturnFailureIfPredicateFalse_UniTask()
        {
            var asyncResult = UniTask.FromResult(Result<int>.Success(10));
            var ensured = await asyncResult.Ensure(x => UniTask.FromResult(x > 15), "Value is too small");

            Assert.IsTrue(ensured.IsFailure);
            Assert.AreEqual("Value is too small", ensured.Error);
        }

        [Test]
        public async Task Ensure_Failure_ShouldKeepError_UniTask()
        {
            var asyncResult = UniTask.FromResult(Result<int>.Failure("fail!"));
            var ensured = await asyncResult.Ensure(x => UniTask.FromResult(x > 5), "Value is too small");

            Assert.IsTrue(ensured.IsFailure);
            Assert.AreEqual("fail!", ensured.Error);
        }

        [Test]
        public async Task Map_SyncToAsync_Success_UniTask()
        {
            var result = Result<int>.Success(5);
            var mapped = await result.Map(x => UniTask.FromResult(x * 2));

            Assert.IsTrue(mapped.IsSuccess);
            Assert.AreEqual(10, mapped.Value);
        }

        [Test]
        public async Task Map_AsyncToAsync_Success_UniTask()
        {
            var asyncResult = UniTask.FromResult(Result<int>.Success(5));
            var mapped = await asyncResult.Map(x => UniTask.FromResult(x * 3));

            Assert.IsTrue(mapped.IsSuccess);
            Assert.AreEqual(15, mapped.Value);
        }

        //===============================================================
        // 1-b. Finally (CFE style) Tests
        //===============================================================

        [Test]
        public void Finally_SyncResult_SyncFinalFunc_ShouldReturnInt()
        {
            // (1) Sync result => Sync finalFunc => returns int
            var result = Result<int>.Success(10);

            int finalValue = result.Finally<int, int>(r =>
            {
                return r.IsSuccess ? r.Value : -1;
            });
            Assert.AreEqual(10, finalValue);
        }

        [Test]
        public async Task Finally_SyncResult_AsyncFinalFunc_ShouldReturnInt()
        {
            // (2) Sync result => Async finalFunc => returns UniTask<int>
            var result = Result<int>.Success(10);

            int finalValue = await result.Finally<int, int>(async r =>
            {
                await UniTask.Yield();
                return r.IsSuccess ? r.Value : -1;
            });
            Assert.AreEqual(10, finalValue);
        }

        [Test]
        public async Task Finally_AsyncResult_SyncFinalFunc_ShouldReturnInt()
        {
            // (3) Async UniTask<Result<int>> => Sync finalFunc => returns UniTask<int>
            var asyncResult = UniTask.FromResult(Result<int>.Success(10));

            int finalValue = await asyncResult.Finally<int, int>(r =>
            {
                return r.IsSuccess ? r.Value : -1;
            });
            Assert.AreEqual(10, finalValue);
        }

        [Test]
        public async Task Finally_AsyncResult_AsyncFinalFunc_ShouldReturnInt()
        {
            // (4) Async UniTask<Result<int>> => Async finalFunc => returns UniTask<int>
            var asyncResult = UniTask.FromResult(Result<int>.Success(10));

            int finalValue = await asyncResult.Finally<int, int>(async r =>
            {
                await UniTask.Yield();
                return r.IsSuccess ? r.Value : -1;
            });
            Assert.AreEqual(10, finalValue);
        }

        [Test]
        public async Task Finally_AsyncResult_Failure_ShouldReturnFallback()
        {
            var asyncResult = UniTask.FromResult(Result<int>.Failure("Something went wrong"));

            int finalValue = await asyncResult.Finally<int, int>(async r =>
            {
                await UniTask.Yield();
                return r.IsSuccess ? r.Value : -999;
            });
            Assert.AreEqual(-999, finalValue);
        }

        //===============================================================
        // 2. Match Tests (8 Overloads) - UniTask
        //===============================================================

        [Test]
        public void Match_1a_SyncResult_SyncOnSuccess_SyncOnFailure_UniTask()
        {
            var success = Result<int>.Success(5);
            var sVal = success.Match(
                onSuccess: x => x + 10,
                onFailure: err => -999
            );
            Assert.AreEqual(15, sVal);

            var failure = Result<int>.Failure("fail!");
            var fVal = failure.Match(
                onSuccess: x => x + 10,
                onFailure: err => -999
            );
            Assert.AreEqual(-999, fVal);
        }

        [Test]
        public async Task Match_1b_SyncResult_SyncOnSuccess_AsyncOnFailure_UniTask()
        {
            var success = Result<int>.Success(5);
            var sVal = await success.Match(
                onSuccess: x => x + 10,
                onFailureAsync: err => UniTask.FromResult(-999)
            );
            Assert.AreEqual(15, sVal);

            var failure = Result<int>.Failure("fail!");
            var fVal = await failure.Match(
                onSuccess: x => x + 10,
                onFailureAsync: err => UniTask.FromResult(-999)
            );
            Assert.AreEqual(-999, fVal);
        }

        [Test]
        public async Task Match_1c_SyncResult_AsyncOnSuccess_SyncOnFailure_UniTask()
        {
            var success = Result<int>.Success(7);
            var sVal = await success.Match(
                onSuccessAsync: x => UniTask.FromResult(x * 2),
                onFailure: err => -1
            );
            Assert.AreEqual(14, sVal);

            var failure = Result<int>.Failure("fail!");
            var fVal = await failure.Match(
                onSuccessAsync: x => UniTask.FromResult(x * 2),
                onFailure: err => -1
            );
            Assert.AreEqual(-1, fVal);
        }

        [Test]
        public async Task Match_1d_SyncResult_AsyncOnSuccess_AsyncOnFailure_UniTask()
        {
            var success = Result<int>.Success(10);
            var sVal = await success.Match(
                onSuccessAsync: x => UniTask.FromResult(x + 1),
                onFailureAsync: err => UniTask.FromResult(-1)
            );
            Assert.AreEqual(11, sVal);

            var failure = Result<int>.Failure("fail!");
            var fVal = await failure.Match(
                onSuccessAsync: x => UniTask.FromResult(x + 1),
                onFailureAsync: err => UniTask.FromResult(-1)
            );
            Assert.AreEqual(-1, fVal);
        }

        [Test]
        public async Task Match_2a_AsyncResult_SyncOnSuccess_SyncOnFailure_UniTask()
        {
            var success = UniTask.FromResult(Result<int>.Success(3));
            var sVal = await success.Match(
                onSuccess: x => x + 10,
                onFailure: err => -999
            );
            Assert.AreEqual(13, sVal);

            var failure = UniTask.FromResult(Result<int>.Failure("fail!"));
            var fVal = await failure.Match(
                onSuccess: x => x + 10,
                onFailure: err => -999
            );
            Assert.AreEqual(-999, fVal);
        }

        [Test]
        public async Task Match_2b_AsyncResult_SyncOnSuccess_AsyncOnFailure_UniTask()
        {
            var success = UniTask.FromResult(Result<int>.Success(4));
            var sVal = await success.Match(
                onSuccess: x => x + 10,
                onFailureAsync: err => UniTask.FromResult(-999)
            );
            Assert.AreEqual(14, sVal);

            var failure = UniTask.FromResult(Result<int>.Failure("fail!"));
            var fVal = await failure.Match(
                onSuccess: x => x + 10,
                onFailureAsync: err => UniTask.FromResult(-999)
            );
            Assert.AreEqual(-999, fVal);
        }

        [Test]
        public async Task Match_2c_AsyncResult_AsyncOnSuccess_SyncOnFailure_UniTask()
        {
            var success = UniTask.FromResult(Result<int>.Success(8));
            var sVal = await success.Match(
                onSuccessAsync: x => UniTask.FromResult(x * 2),
                onFailure: err => -1
            );
            Assert.AreEqual(16, sVal);

            var failure = UniTask.FromResult(Result<int>.Failure("fail!"));
            var fVal = await failure.Match(
                onSuccessAsync: x => UniTask.FromResult(x * 2),
                onFailure: err => -1
            );
            Assert.AreEqual(-1, fVal);
        }

        [Test]
        public async Task Match_2d_AsyncResult_AsyncOnSuccess_AsyncOnFailure_UniTask()
        {
            var success = UniTask.FromResult(Result<int>.Success(10));
            var sVal = await success.Match(
                onSuccessAsync: x => UniTask.FromResult(x + 1),
                onFailureAsync: err => UniTask.FromResult(-999)
            );
            Assert.AreEqual(11, sVal);

            var failure = UniTask.FromResult(Result<int>.Failure("fail!"));
            var fVal = await failure.Match(
                onSuccessAsync: x => UniTask.FromResult(x + 1),
                onFailureAsync: err => UniTask.FromResult(-999)
            );
            Assert.AreEqual(-999, fVal);
        }

        //===============================================================
        // 3. Practical Chaining Examples (Sync + Async mixed)
        //===============================================================

        [Test]
        public async Task Chain_SyncAndAsync_MultipleSteps_UniTask_Success()
        {
            // Start with a synchronous success result
            var initial = Result<int>.Success(2);

            // Build a chain of operations (mixing sync & async)
            // Note: "Match(...)" returns an int, so the chain becomes int,
            // then we call .Finally(...) which also returns an int, ending the chain.

            var final = await initial
                // 1) Sync Bind
                .Bind(x => Result<int>.Success(x + 3))
                // 2) Async Tap
                .Tap(x => UniTask.Create(async () =>
                {
                    await UniTask.Yield();
                    Debug.Log("Tap side effect with value: " + x);
                }))
                // 3) Async Ensure
                .Ensure(x => UniTask.FromResult(x < 10), "Value is too large!")
                // 4) Async Map
                .Map(x => UniTask.FromResult($"Value is {x}"))
                // 5) Async Match to get an integer
                .Match(
                    onSuccessAsync: async str =>
                    {
                        await UniTask.Yield();
                        return Result<int>.Success(str.Length);
                    },
                    onFailureAsync: async err =>
                    {
                        await UniTask.Yield();
                        return Result<int>.Failure(err);
                    }
                )
                // 6) We can do a sync final or an async final. Let's do sync for demonstration.
                .Finally(r =>
                {
                    Debug.Log($"Final integer result: {r}");
                    return r.IsSuccess ? r.Value : -1;
                });

            // "Value is 5" => length = 10
            Assert.AreEqual(10, final);
        }

        [Test]
        public async Task Chain_SyncAndAsync_MultipleSteps_UniTask_Failure()
        {
            // Start with a synchronous failure result
            var initial = Result<int>.Failure("Initial error");

            var final = await initial
                .Bind(x => Result<int>.Success(x + 3))   // won't run
                .Tap(x => UniTask.Create(async () =>
                {
                    await UniTask.Yield();
                    
                    Debug.Log("Tap side effect: " + x);
                })) // won't run
                .Ensure(x => UniTask.FromResult(x < 10), "Value is too large!")  // won't run
                .Map(x => UniTask.FromResult($"Value is {x}")) // won't run
                .Match(
                    onSuccessAsync: async str =>
                    {
                        await UniTask.Yield();
                        return Result<int>.Success(str.Length);
                    },
                    onFailureAsync: async err =>
                    {
                        await UniTask.Yield();
                        return Result<int>.Failure(err);
                    }
                )
                .Finally(async r =>
                {
                    await UniTask.Yield();
                    Debug.Log($"Final integer result: {r}");
                    return r.IsSuccess ? r.Value : -1;
                });

            Assert.AreEqual(-1, final);
        }
#endif // NOPE_UNITASK


#if NOPE_AWAITABLE
        //===============================================================
        // 1. Basic Tests (Bind, Tap, Ensure, Map) - Awaitable
        //===============================================================

        private static AwaitableCompletionSource<Result<int>> MakeCompletion(Result<int> r)
        {
            var cts = new AwaitableCompletionSource<Result<int>>();
            cts.SetResult(r);
            return cts;
        }

        private static AwaitableCompletionSource<T> MakeCompletion<T>(T value)
        {
            var cts = new AwaitableCompletionSource<T>();
            cts.SetResult(value);
            return cts;
        }

        [Test]
        public async Task Bind_Success_ResultShouldBeBound_Awaitable()
        {
            var asyncRes = MakeCompletion(Result<int>.Success(10)).Awaitable;
            var bound = await asyncRes.Bind(x =>
                MakeCompletion(Result<string>.Success((x * 2).ToString())).Awaitable
            );

            Assert.IsTrue(bound.IsSuccess);
            Assert.AreEqual("20", bound.Value);
        }

        [Test]
        public async Task Bind_Failure_ShouldKeepError_Awaitable()
        {
            var asyncRes = MakeCompletion(Result<int>.Failure("fail!")).Awaitable;
            var bound = await asyncRes.Bind(x =>
                MakeCompletion(Result<string>.Success((x * 2).ToString())).Awaitable
            );

            Assert.IsTrue(bound.IsFailure);
            Assert.AreEqual("fail!", bound.Error);
        }

        // ... (Tap, Ensure, Map tests in a similar pattern) ...

        //===============================================================
        // 2. FinallyAwaitable (CFE style) - example
        //===============================================================

        [Test]
        public async Task FinallyAwaitable_SyncResult_AsyncFinalFunc()
        {
            var result = Result<int>.Success(10);

            // we have "FinallyAwaitable<T, TOut>" to produce "Awaitable<TOut>"
            var finalAwaitable = result.FinallyAwaitable<int, int>(r =>
            {
                // return an Awaitable<int>
                var cts = new AwaitableCompletionSource<int>();
                cts.SetResult(r.IsSuccess ? r.Value : -1);
                return cts.Awaitable;
            });

            var finalValue = await finalAwaitable; 
            Assert.AreEqual(10, finalValue);
        }

        //===============================================================
        // 3. (Optional) Match usage with Awaitable
        //===============================================================
        // ... (Similar to the UniTask-based Match tests, 
        // but referencing your custom .Match(...) with 
        // onSuccessAwaitable / onFailureAwaitable ...
#endif // NOPE_AWAITABLE
    }
}
