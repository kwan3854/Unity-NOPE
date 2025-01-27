using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using NOPE.Runtime.Core;
using NOPE.Runtime.Core.Maybe;
using UnityEngine;

namespace NOPE.Tests
{
    [TestFixture]
    public class MaybeAsyncExtensionsTests
    {
#if NOPE_UNITASK
        [Test]
        public async Task Map_UniTask_ShouldTransformValue_WhenMaybeHasValue()
        {
            var maybe = UniTask.FromResult(Maybe<int>.From(5));
            var result = await maybe.Map(x => x * 2);
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(10, result.Value);
        }

        [Test]
        public async Task Map_UniTask_ShouldReturnNone_WhenMaybeHasNoValue()
        {
            var maybe = UniTask.FromResult(Maybe<int>.None);
            var result = await maybe.Map(x => x * 2);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public async Task Bind_UniTask_ShouldReturnTransformedMaybe_WhenMaybeHasValue()
        {
            var maybe = UniTask.FromResult(Maybe<int>.From(5));
            var result = await maybe.Bind(x => UniTask.FromResult(Maybe<string>.From(x.ToString())));
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual("5", result.Value);
        }

        [Test]
        public async Task Bind_UniTask_ShouldReturnNone_WhenMaybeHasNoValue()
        {
            var maybe = UniTask.FromResult(Maybe<int>.None);
            var result = await maybe.Bind(x => UniTask.FromResult(Maybe<string>.From(x.ToString())));
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public async Task Tap_UniTask_ShouldExecuteAction_WhenMaybeHasValue()
        {
            var maybe = UniTask.FromResult(Maybe<int>.From(5));
            bool actionExecuted = false;
            await maybe.Tap(async x => { actionExecuted = true; await UniTask.CompletedTask; });
            Assert.IsTrue(actionExecuted);
        }

        [Test]
        public async Task Tap_UniTask_ShouldNotExecuteAction_WhenMaybeHasNoValue()
        {
            var maybe = UniTask.FromResult(Maybe<int>.None);
            bool actionExecuted = false;
            await maybe.Tap(async x => { actionExecuted = true; await UniTask.CompletedTask; });
            Assert.IsFalse(actionExecuted);
        }

        [Test]
        public async Task Match_UniTask_ShouldReturnOnValueResult_WhenMaybeHasValue()
        {
            var maybe = UniTask.FromResult(Maybe<int>.From(5));
            var result = await maybe.Match(
                async x => { await UniTask.CompletedTask; return x * 2; },
                async () => { await UniTask.CompletedTask; return 0; });
            Assert.AreEqual(10, result);
        }

        [Test]
        public async Task Match_UniTask_ShouldReturnOnNoneResult_WhenMaybeHasNoValue()
        {
            var maybe = UniTask.FromResult(Maybe<int>.None);
            var result = await maybe.Match(
                async x => { await UniTask.CompletedTask; return x * 2; },
                async () => { await UniTask.CompletedTask; return 0; });
            Assert.AreEqual(0, result);
        }

        [Test]
        public async Task Finally_UniTask_ShouldExecuteFinalAction_WhenMaybeHasValue()
        {
            var maybe = UniTask.FromResult(Maybe<int>.From(5));
            bool finalActionExecuted = false;
            await maybe.Finally(async x => { finalActionExecuted = true; await UniTask.CompletedTask; });
            Assert.IsTrue(finalActionExecuted);
        }

        [Test]
        public async Task Finally_UniTask_ShouldExecuteFinalAction_WhenMaybeHasNoValue()
        {
            var maybe = UniTask.FromResult(Maybe<int>.None);
            bool finalActionExecuted = false;
            await maybe.Finally(async x => { finalActionExecuted = true; await UniTask.CompletedTask; });
            Assert.IsTrue(finalActionExecuted);
        }
#endif

#if NOPE_AWAITABLE
        [Test]
        public async Task Map_Awaitable_ShouldTransformValue_WhenMaybeHasValue()
        {
            var cts = new AwaitableCompletionSource<Maybe<int>>();
            cts.SetResult(Maybe<int>.From(5));
            
            var maybe = cts.Awaitable;
            var result = await maybe.Map(x => x * 2);
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(10, result.Value);
        }

        [Test]
        public async Task Map_Awaitable_ShouldReturnNone_WhenMaybeHasNoValue()
        {
            var cts = new AwaitableCompletionSource<Maybe<int>>();
            cts.SetResult(Maybe<int>.None);
            
            var maybe = cts.Awaitable;
            var result = await maybe.Map(x => x * 2);
            Assert.IsFalse(result.HasValue);
        }
        
        [Test]
        public async Task Bind_Awaitable_ShouldReturnNone_WhenMaybeHasNoValue()
        {
            var cts = new AwaitableCompletionSource<Maybe<int>>();
            cts.SetResult(Maybe<int>.None);
            
            var maybe = cts.Awaitable;
            var bindCts = new AwaitableCompletionSource<Maybe<string>>();
            var result = await maybe.Bind(x => bindCts.Awaitable);
            Assert.IsFalse(result.HasValue);
        }
        
        [Test]
        public async Task Tap_Awaitable_ShouldExecuteAction_WhenMaybeHasValue()
        {
            var cts = new AwaitableCompletionSource<Maybe<int>>();
            cts.SetResult(Maybe<int>.From(5));
            
            var maybe = cts.Awaitable;
            bool actionExecuted = false;
            var taskCts = new AwaitableCompletionSource();
            await maybe.Tap(async x => { actionExecuted = true; await taskCts.Awaitable; taskCts.SetResult(); });
            Assert.IsTrue(actionExecuted);
        }
        
        [Test]
        public async Task Tap_Awaitable_ShouldNotExecuteAction_WhenMaybeHasNoValue()
        {
            var cts = new AwaitableCompletionSource<Maybe<int>>();
            cts.SetResult(Maybe<int>.None);
            
            var maybe = cts.Awaitable;
            bool actionExecuted = false;
            var taskCts = new AwaitableCompletionSource();
            await maybe.Tap(async x => { actionExecuted = true; await taskCts.Awaitable; taskCts.SetResult(); });
            Assert.IsFalse(actionExecuted);
        }
        
        [Test]
        public async Task Match_Awaitable_ShouldReturnOnValueResult_WhenMaybeHasValue()
        {
            var cts = new AwaitableCompletionSource<Maybe<int>>();
            cts.SetResult(Maybe<int>.From(5));
            
            var maybe = cts.Awaitable;
            var taskCts = new AwaitableCompletionSource();
            var result = await maybe.Match(
                async x => { await taskCts.Awaitable; taskCts.SetResult(); return x * 2; },
                async () => { await taskCts.Awaitable; taskCts.SetResult(); return 0; });
            Assert.AreEqual(10, result);
        }
        
        [Test]
        public async Task Match_Awaitable_ShouldReturnOnNoneResult_WhenMaybeHasNoValue()
        {
            var cts = new AwaitableCompletionSource<Maybe<int>>();
            cts.SetResult(Maybe<int>.None);
            
            var maybe = cts.Awaitable;
            var taskCts = new AwaitableCompletionSource();
            var result = await maybe.Match(
                async x => { await taskCts.Awaitable; taskCts.SetResult(); return x * 2; },
                async () => { await taskCts.Awaitable; taskCts.SetResult(); return 0; });
            Assert.AreEqual(0, result);
        }
        
        [Test]
        public async Task Finally_Awaitable_ShouldExecuteFinalAction_WhenMaybeHasValue()
        {
            var cts = new AwaitableCompletionSource<Maybe<int>>();
            cts.SetResult(Maybe<int>.From(5));
            
            var maybe = cts.Awaitable;
            bool finalActionExecuted = false;
            var taskCts = new AwaitableCompletionSource();
            await maybe.Finally(async x => { finalActionExecuted = true; await taskCts.Awaitable; taskCts.SetResult(); });
            Assert.IsTrue(finalActionExecuted);
        }
        
        [Test]
        public async Task Finally_Awaitable_ShouldExecuteFinalAction_WhenMaybeHasNoValue()
        {
            var cts = new AwaitableCompletionSource<Maybe<int>>();
            cts.SetResult(Maybe<int>.None);
            
            var maybe = cts.Awaitable;
            bool finalActionExecuted = false;
            var taskCts = new AwaitableCompletionSource();
            await maybe.Finally(async x => { finalActionExecuted = true; await taskCts.Awaitable; taskCts.SetResult(); });
            Assert.IsTrue(finalActionExecuted);
        }
#endif
    }
}