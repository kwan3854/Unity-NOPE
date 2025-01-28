using NUnit.Framework;
using UnityEngine;
using System.Threading.Tasks;

#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif

#if NOPE_AWAITABLE
// Assume you've defined your custom Awaitable, etc. here or in another file:
#endif

using NOPE.Runtime.Core.Maybe;

namespace NOPE.Tests
{
    [TestFixture]
    public class MaybeAsyncExtensionsTests
    {
#if NOPE_UNITASK
        //========================================================
        // 1. UniTask-based tests
        //========================================================

        [Test]
        public async Task Map_UniTask_ShouldTransformValue_WhenMaybeHasValue()
        {
            // Arrange
            var maybe = UniTask.FromResult(Maybe<int>.From(5));

            // Act
            var result = await maybe.Map(x => x * 2);

            // Assert
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(10, result.Value);
        }

        [Test]
        public async Task Map_UniTask_ShouldReturnNone_WhenMaybeHasNoValue()
        {
            // Arrange
            var maybe = UniTask.FromResult(Maybe<int>.None);

            // Act
            var result = await maybe.Map(x => x * 2);

            // Assert
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public async Task Bind_UniTask_ShouldReturnTransformedMaybe_WhenMaybeHasValue()
        {
            // Arrange
            var maybe = UniTask.FromResult(Maybe<int>.From(5));

            // Act
            var result = await maybe.Bind(x =>
                UniTask.FromResult(Maybe<string>.From(x.ToString()))
            );

            // Assert
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual("5", result.Value);
        }

        [Test]
        public async Task Bind_UniTask_ShouldReturnNone_WhenMaybeHasNoValue()
        {
            // Arrange
            var maybe = UniTask.FromResult(Maybe<int>.None);

            // Act
            var result = await maybe.Bind(x =>
                UniTask.FromResult(Maybe<string>.From(x.ToString()))
            );

            // Assert
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public async Task Tap_UniTask_ShouldExecuteAction_WhenMaybeHasValue()
        {
            // Arrange
            var maybe = UniTask.FromResult(Maybe<int>.From(5));
            bool actionExecuted = false;

            // Act
            await maybe.Tap(async x =>
            {
                actionExecuted = true;
                await UniTask.CompletedTask;
            });

            // Assert
            Assert.IsTrue(actionExecuted);
        }

        [Test]
        public async Task Tap_UniTask_ShouldNotExecuteAction_WhenMaybeHasNoValue()
        {
            // Arrange
            var maybe = UniTask.FromResult(Maybe<int>.None);
            bool actionExecuted = false;

            // Act
            await maybe.Tap(async x =>
            {
                actionExecuted = true;
                await UniTask.CompletedTask;
            });

            // Assert
            Assert.IsFalse(actionExecuted);
        }

        [Test]
        public async Task Match_UniTask_ShouldReturnOnValueResult_WhenMaybeHasValue()
        {
            // Arrange
            var maybe = Maybe<int>.From(5);

            // Act
            // Use the "sync Maybe<T> -> async onValue/onNone => UniTask<TResult>" overload
            var result = await maybe.Match(
                async x =>
                {
                    // async method that returns UniTask<int>
                    // return await MultiplyByTwo(x);
                    await UniTask.DelayFrame(1);
                    return x * 2;
                },   // returns UniTask<int>, not Task<int>
                onNoneAsync: () => UniTask.FromResult(0)
            );

            // Assert
            Assert.AreEqual(10, result);
        }
        
        private async UniTask<int> MultiplyByTwo(int x)
        {
            await UniTask.CompletedTask;
            return x * 2;
        }

        [Test]
        public async Task Match_UniTask_ShouldReturnOnNoneResult_WhenMaybeHasNoValue()
        {
            // Arrange
            var maybe = Maybe<int>.None;

            // Act
            var result = await maybe.Match(
                onValueAsync: x => UniTask.FromResult(x * 2),
                onNoneAsync: () => UniTask.FromResult(0)
            );

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public async Task Match_UniTask_AsyncResult_SyncHandlers_WhenHasValue()
        {
            // Arrange
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(5));

            // Act
            var result = await asyncMaybe.Match(
                onValue: x => x + 10,    // sync
                onNone: () => -1        // sync
            );

            // Assert
            Assert.AreEqual(15, result);
        }

        [Test]
        public async Task Match_UniTask_AsyncResult_AsyncHandlers_WhenHasValue()
        {
            // Arrange
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(5));

            // Act
            var result = await asyncMaybe.Match(
                onValueAsync: x => UniTask.FromResult(x + 10),
                onNoneAsync: () => UniTask.FromResult(-1)
            );

            // Assert
            Assert.AreEqual(15, result);
        }

        [Test]
        public async Task Finally_UniTask_ShouldExecuteFinalAction_WhenMaybeHasValue()
        {
            // Arrange
            var maybe = UniTask.FromResult(Maybe<int>.From(5));
            bool finalActionExecuted = false;

            // Act
            var finalResult = await maybe.Finally(async m =>
            {
                finalActionExecuted = true;
                await UniTask.CompletedTask;
            });

            // Assert
            Assert.IsTrue(finalActionExecuted);
            Assert.IsTrue(finalResult.HasValue);
            Assert.AreEqual(5, finalResult.Value);
        }

        [Test]
        public async Task Finally_UniTask_ShouldExecuteFinalAction_WhenMaybeHasNoValue()
        {
            // Arrange
            var maybe = UniTask.FromResult(Maybe<int>.None);
            bool finalActionExecuted = false;

            // Act
            var finalResult = await maybe.Finally(async m =>
            {
                finalActionExecuted = true;
                await UniTask.CompletedTask;
            });

            // Assert
            Assert.IsTrue(finalActionExecuted);
            Assert.IsFalse(finalResult.HasValue);
        }
#endif // NOPE_UNITASK


#if NOPE_AWAITABLE
        //========================================================
        // 2. Awaitable-based tests
        //========================================================

        // (EXAMPLE) We assume you have some custom 'Awaitable<T>' and
        // 'AwaitableCompletionSource<T>' class for your system.

        private static AwaitableCompletionSource<Maybe<int>> CreateMaybe(int? value)
        {
            // Returns Maybe<int>.From(value) if non-null, else None
            var cts = new AwaitableCompletionSource<Maybe<int>>();
            if (value.HasValue)
                cts.SetResult(Maybe<int>.From(value.Value));
            else
                cts.SetResult(Maybe<int>.None);
            return cts;
        }

        [Test]
        public async System.Threading.Tasks.Task Map_Awaitable_ShouldTransformValue_WhenMaybeHasValue()
        {
            var cts = CreateMaybe(5);
            var maybe = cts.Awaitable;

            var result = await maybe.Map(x => x * 2);
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(10, result.Value);
        }

        [Test]
        public async System.Threading.Tasks.Task Map_Awaitable_ShouldReturnNone_WhenMaybeHasNoValue()
        {
            var cts = CreateMaybe(null);
            var maybe = cts.Awaitable;

            var result = await maybe.Map(x => x * 2);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public async System.Threading.Tasks.Task Bind_Awaitable_ShouldReturnNone_WhenMaybeHasNoValue()
        {
            var cts = CreateMaybe(null);
            var maybe = cts.Awaitable;

            var result = await maybe.Bind(x =>
            {
                var innerCts = new AwaitableCompletionSource<Maybe<string>>();
                innerCts.SetResult(Maybe<string>.From(x.ToString()));
                return innerCts.Awaitable;
            });

            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public async System.Threading.Tasks.Task Tap_Awaitable_ShouldExecuteAction_WhenMaybeHasValue()
        {
            var cts = CreateMaybe(5);
            var maybe = cts.Awaitable;

            bool sideEffect = false;
            var final = await maybe.Tap(async x =>
            {
                sideEffect = true;
                // simulate some awaited operation
                var cts2 = new AwaitableCompletionSource();
                cts2.SetResult();
                await cts2.Awaitable;
            });

            Assert.IsTrue(sideEffect);
            Assert.IsTrue(final.HasValue);
            Assert.AreEqual(5, final.Value);
        }

        [Test]
        public async System.Threading.Tasks.Task Tap_Awaitable_ShouldNotExecuteAction_WhenMaybeHasNoValue()
        {
            var cts = CreateMaybe(null);
            var maybe = cts.Awaitable;

            bool sideEffect = false;
            var final = await maybe.Tap(async x =>
            {
                sideEffect = true;
                var cts2 = new AwaitableCompletionSource();
                cts2.SetResult();
                await cts2.Awaitable;
            });

            Assert.IsFalse(sideEffect);
            Assert.IsFalse(final.HasValue);
        }

        [Test]
        public async Task Match_Awaitable_ShouldReturnOnValueResult_WhenMaybeHasValue()
        {
            // Arrange
            var cts = CreateMaybe(5);
            var maybe = cts.Awaitable;

            // Act
            var result = await maybe.Match(
                onValueAsync: x =>
                {
                    // Return an Awaitable<int> directly
                    var cts2 = new AwaitableCompletionSource<int>();
                    // (pseudo-async logic could go here)
                    cts2.SetResult(x * 2);
                    return cts2.Awaitable;
                },
                onNoneAsync: () =>
                {
                    var cts2 = new AwaitableCompletionSource<int>();
                    cts2.SetResult(0);
                    return cts2.Awaitable;
                }
            );

            // Assert
            Assert.AreEqual(10, result);
        }

        [Test]
        public async Task Match_Awaitable_ShouldReturnOnNoneResult_WhenMaybeHasNoValue()
        {
            // Arrange
            var cts = CreateMaybe(null);
            var maybe = cts.Awaitable;

            // Act
            var result = await maybe.Match(
                onValueAsync: x =>
                {
                    var cts2 = new AwaitableCompletionSource<int>();
                    cts2.SetResult(x * 2);
                    return cts2.Awaitable;
                },
                onNoneAsync: () =>
                {
                    var cts2 = new AwaitableCompletionSource<int>();
                    cts2.SetResult(0);
                    return cts2.Awaitable;
                }
            );

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public async System.Threading.Tasks.Task Finally_Awaitable_ShouldExecuteFinalAction_WhenMaybeHasValue()
        {
            var cts = CreateMaybe(10);
            var maybe = cts.Awaitable;

            bool finalAction = false;
            var finalResult = await maybe.Finally(async m =>
            {
                finalAction = true;
                var cts2 = new AwaitableCompletionSource();
                cts2.SetResult();
                await cts2.Awaitable;
            });

            Assert.IsTrue(finalAction);
            Assert.IsTrue(finalResult.HasValue);
            Assert.AreEqual(10, finalResult.Value);
        }

        [Test]
        public async System.Threading.Tasks.Task Finally_Awaitable_ShouldExecuteFinalAction_WhenMaybeHasNoValue()
        {
            var cts = CreateMaybe(null);
            var maybe = cts.Awaitable;

            bool finalAction = false;
            var finalResult = await maybe.Finally(async m =>
            {
                finalAction = true;
                var cts2 = new AwaitableCompletionSource();
                cts2.SetResult();
                await cts2.Awaitable;
            });

            Assert.IsTrue(finalAction);
            Assert.IsFalse(finalResult.HasValue);
        }
#endif
    }
}
