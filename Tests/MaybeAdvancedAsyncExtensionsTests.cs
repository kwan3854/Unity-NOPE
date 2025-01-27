using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NOPE.Runtime.AdvancedExtensions;
using NOPE.Runtime.Core;
using NUnit.Framework;
using UnityEngine;

namespace NOPE.Tests
{
#if NOPE_UNITASK
    [TestFixture]
    public class MaybeAdvancedAsyncExtensionsUniTaskTests
    {
        [Test]
        public async Task GetValueOrThrow_HasValue_ReturnsValue()
        {
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(42));
            var result = await asyncMaybe.GetValueOrThrow();
            Assert.AreEqual(42, result);
        }

        [Test]
        public void GetValueOrThrow_NoValue_ThrowsInvalidOperationException()
        {
            var asyncMaybe = UniTask.FromResult(Maybe<int>.None);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await asyncMaybe.GetValueOrThrow());
        }

        [Test]
        public void GetValueOrThrow_CustomException_ThrowsCustom()
        {
            var asyncMaybe = UniTask.FromResult(Maybe<int>.None);
            var customEx = new ArgumentOutOfRangeException("test");
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await asyncMaybe.GetValueOrThrow(customEx));
        }

        [Test]
        public async Task GetValueOrDefault_HasValue_ReturnsValue()
        {
            var asyncMaybe = UniTask.FromResult(Maybe<string>.From("OK"));
            var result = await asyncMaybe.GetValueOrDefault("Default");
            Assert.AreEqual("OK", result);
        }

        [Test]
        public async Task GetValueOrDefault_NoValue_ReturnsDefault()
        {
            var asyncMaybe = UniTask.FromResult(Maybe<string>.None);
            var result = await asyncMaybe.GetValueOrDefault("Default");
            Assert.AreEqual("Default", result);
        }

        [Test]
        public async Task Where_AsyncPredicate_Valid()
        {
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(5));
            var result = await asyncMaybe.Where(async x =>
            {
                await UniTask.Delay(1);
                return x > 0;
            });
            Assert.IsTrue(result.HasValue);
        }

        [Test]
        public async Task Where_AsyncPredicate_Invalid()
        {
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(-1));
            var result = await asyncMaybe.Where(async x =>
            {
                await UniTask.Delay(1);
                return x > 0;
            });
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public async Task Where_Predicate_Valid()
        {
            var asyncMaybe = UniTask.FromResult(Maybe<string>.From("text"));
            var result = await asyncMaybe.Where(s => s == "text");
            Assert.IsTrue(result.HasValue);
        }

        [Test]
        public async Task Where_Predicate_Invalid()
        {
            var asyncMaybe = UniTask.FromResult(Maybe<string>.From("other"));
            var result = await asyncMaybe.Where(s => s == "text");
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public async Task Or_HasValue_ReturnsOriginal()
        {
            var original = Maybe<int>.From(10);
            var fallback = Maybe<int>.From(99);
            var asyncMaybe = UniTask.FromResult(original);
            var result = await asyncMaybe.Or(fallback);
            Assert.AreEqual(10, result.Value);
        }

        [Test]
        public async Task Or_NoValue_ReturnsFallback()
        {
            var fallback = Maybe<int>.From(99);
            var asyncMaybe = UniTask.FromResult(Maybe<int>.None);
            var result = await asyncMaybe.Or(fallback);
            Assert.AreEqual(99, result.Value);
        }

        [Test]
        public async Task Or_ValueOverload_HasValue_ReturnsOriginal()
        {
            var maybe = UniTask.FromResult(Maybe<int>.From(7));
            var result = await maybe.Or(999);
            Assert.AreEqual(7, result.Value);
        }

        [Test]
        public async Task Or_ValueOverload_NoValue_ReturnsNew()
        {
            var maybe = UniTask.FromResult(Maybe<int>.None);
            var result = await maybe.Or(999);
            Assert.AreEqual(999, result.Value);
        }

        [Test]
        public async Task Select_HasValue_TransformsValue()
        {
            var maybe = UniTask.FromResult(Maybe<int>.From(3));
            var result = await maybe.Select(x => x * 2);
            Assert.AreEqual(6, result.Value);
        }

        [Test]
        public async Task Select_NoValue_ReturnsNone()
        {
            var maybe = UniTask.FromResult(Maybe<int>.None);
            var result = await maybe.Select(x => x * 2);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public async Task SelectMany_HasValue_BindsValue()
        {
            var maybe = UniTask.FromResult(Maybe<int>.From(5));
            var result = await maybe.SelectMany(x => UniTask.FromResult(Maybe<string>.From($"Value:{x}")));
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual("Value:5", result.Value);
        }

        [Test]
        public async Task SelectMany_NoValue_ReturnsNone()
        {
            var maybe = UniTask.FromResult(Maybe<int>.None);
            var result = await maybe.SelectMany(x => UniTask.FromResult(Maybe<string>.From($"Value:{x}")));
            Assert.IsFalse(result.HasValue);
        }
    }
#endif

#if NOPE_AWAITABLE
    // For illustration, these tests assume you have an Awaitable<T> type
    // and a suitable test harness. The approach is similar to the UniTask tests.
    // Adjust as necessary for your environment.
    [TestFixture]
    public class MaybeAdvancedAsyncExtensionsAwaitableTests
    {
        private static Awaitable<T> CreateAwaitable<T>(T result)
        {
            var cts = new AwaitableCompletionSource<T>();
            cts.SetResult(result);
            return cts.Awaitable;
        }

        [Test]
        public void GetValueOrThrow_HasValue_ReturnsValue()
        {
            var asyncMaybe = CreateAwaitable(Maybe<int>.From(42));
            Assert.DoesNotThrowAsync(async () => await asyncMaybe.GetValueOrThrow());
        }

        [Test]
        public void GetValueOrThrow_NoValue_ThrowsInvalidOp()
        {
            var asyncMaybe = CreateAwaitable(Maybe<int>.None);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await asyncMaybe.GetValueOrThrow());
        }

        [Test]
        public async Task GetValueOrDefault_HasValue_ReturnsValue()
        {
            var asyncMaybe = CreateAwaitable(Maybe<int>.From(10));
            var result = await asyncMaybe.GetValueOrDefault(-1);
            Assert.AreEqual(10, result);
        }

        [Test]
        public async Task GetValueOrDefault_NoValue_ReturnsDefault()
        {
            var asyncMaybe = CreateAwaitable(Maybe<int>.None);
            var result = await asyncMaybe.GetValueOrDefault(-1);
            Assert.AreEqual(-1, result);
        }

        [Test]
        public async Task Where_Predicate_Valid()
        {
            var asyncMaybe = CreateAwaitable(Maybe<int>.From(5));
            var result = await asyncMaybe.Where(x => x == 5);
            Assert.IsTrue(result.HasValue);
        }

        [Test]
        public async Task Where_Predicate_Invalid()
        {
            var asyncMaybe = CreateAwaitable(Maybe<int>.From(5));
            var result = await asyncMaybe.Where(x => x > 10);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public async Task Or_HasValue_ReturnsOriginal()
        {
            var original = Maybe<int>.From(7);
            var fallback = Maybe<int>.From(99);
            var asyncMaybe = CreateAwaitable(original);
            var result = await asyncMaybe.Or(fallback);
            Assert.AreEqual(7, result.Value);
        }

        [Test]
        public async Task Or_NoValue_ReturnsFallback()
        {
            var fallback = Maybe<int>.From(99);
            var asyncMaybe = CreateAwaitable(Maybe<int>.None);
            var result = await asyncMaybe.Or(fallback);
            Assert.AreEqual(99, result.Value);
        }

        [Test]
        public async Task Select_HasValue_TransformsValue()
        {
            var asyncMaybe = CreateAwaitable(Maybe<int>.From(2));
            var result = await asyncMaybe.Select(x => x * 3);
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(6, result.Value);
        }

        [Test]
        public async Task Select_NoValue_ReturnsNone()
        {
            var asyncMaybe = CreateAwaitable(Maybe<int>.None);
            var result = await asyncMaybe.Select(x => x * 3);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public async Task SelectMany_HasValue_BindsValue()
        {
            var asyncMaybe = CreateAwaitable(Maybe<int>.From(9));
            var result = await asyncMaybe.SelectMany(x => CreateAwaitable(Maybe<string>.From($"Val:{x}")));
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual("Val:9", result.Value);
        }

        [Test]
        public async Task SelectMany_NoValue_ReturnsNone()
        {
            var asyncMaybe = CreateAwaitable(Maybe<int>.None);
            var result = await asyncMaybe.SelectMany(x => CreateAwaitable(Maybe<string>.From($"Val:{x}")));
            Assert.IsFalse(result.HasValue);
        }
    }
#endif
}