using NUnit.Framework;

#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
using NOPE.Runtime.Core;
#endif

using System;
using UnityEngine;
using System.Threading.Tasks;
using NOPE.Runtime.Core.Result;
using Result = NOPE.Runtime.Core.Result.Result;

namespace NOPE.Tests
{
    public class ResultCreationAsyncExtensionsTests
    {
#if NOPE_UNITASK
        [Test]
        public async Task SuccessIf_UniTask_True()
        {
            UniTask<bool> condition = UniTask.FromResult(true);
            var r = await Result.SuccessIf(condition, 123, "Error");
            Assert.IsTrue(r.IsSuccess);
            Assert.AreEqual(123, r.Value);
        }

        [Test]
        public async Task SuccessIf_UniTask_False()
        {
            UniTask<bool> condition = UniTask.FromResult(false);
            var r = await Result.SuccessIf(condition, 999, "Failed!");
            Assert.IsTrue(r.IsFailure);
            Assert.AreEqual("Failed!", r.Error);
        }

        [Test]
        public async Task FailureIf_UniTask_True()
        {
            UniTask<bool> condition = UniTask.FromResult(true);
            var r = await Result.FailureIf(condition,555, "Condition is true => fail");
            Assert.IsTrue(r.IsFailure);
            Assert.AreEqual("Condition is true => fail", r.Error);
        }

        [Test]
        public async Task Of_UniTask_NoException()
        {
            UniTask<int> asyncFunc = DoAsync(10);
            var r = await Result.Of(asyncFunc);
            Assert.IsTrue(r.IsSuccess);
            Assert.AreEqual(10, r.Value);
        }

        [Test]
        public async Task Of_UniTask_WithException()
        {
            UniTask<int> asyncFunc = ThrowExceptionAsync<int>("Boom!");
            var r = await Result.Of(asyncFunc,ex => $"Custom: {ex.Message}");
            Assert.IsTrue(r.IsFailure);
            Assert.AreEqual("Custom: Boom!", r.Error);
        }

        private async UniTask<T> DoAsync<T>(T value)
        {
            await UniTask.Delay(100);
            return value;
        }

        private async UniTask<T> ThrowExceptionAsync<T>(string message)
        {
            await UniTask.Delay(100);
            throw new Exception(message);
        }
#endif // NOPE_UNITASK


#if NOPE_AWAITABLE
        [Test]
        public async Task SuccessIf_Awaitable_True()
        {
            var cts = new AwaitableCompletionSource<bool>();
            cts.SetResult(true);

            var r = await Result.SuccessIf(cts.Awaitable, 999, "Error");
            Assert.IsTrue(r.IsSuccess);
            Assert.AreEqual(999, r.Value);
        }

        [Test]
        public async Task FailureIf_Awaitable_True()
        {
            var cts = new AwaitableCompletionSource<bool>();
            cts.SetResult(true);

            var r = await Result.FailureIf(cts.Awaitable, 555, "Failed!");
            Assert.IsTrue(r.IsFailure);
            Assert.AreEqual("Failed!", r.Error);
        }

        [Test]
        public async Task Of_Awaitable_NoException()
        {
            var cts = new AwaitableCompletionSource<int>();
            cts.SetResult(777);

            var r = await Result.Of(cts.Awaitable);
            Assert.IsTrue(r.IsSuccess);
            Assert.AreEqual(777, r.Value);
        }

        [Test]
        public async Task Of_Awaitable_WithException()
        {
            // Simulate an "async" operation that throws
            var cts = new AwaitableCompletionSource<int>();
            // The idea: we won't call cts.SetResult => in real usage we'd throw
            // But for a direct throw, we might do something like a custom Awaitable that throws
            // So let's do a minimal approach:
            // We'll demonstrate by forcibly throwing after "await"
            var throwingAwaitable = ForceThrow<int>("Fatal error");

            var r = await Result.Of(throwingAwaitable, ex => $"Converted: {ex.Message}");
            Assert.IsTrue(r.IsFailure);
            Assert.AreEqual("Converted: Fatal error", r.Error);
        }

        private async Awaitable<int> ForceThrow<T>(string message)
        {
            await Awaitable.NextFrameAsync(); // simulate wait
            throw new Exception(message);
        }
#endif // NOPE_AWAITABLE
    }
}