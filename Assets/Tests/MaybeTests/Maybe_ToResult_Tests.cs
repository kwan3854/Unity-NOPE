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
    public class Maybe_ToResult_Tests
    {
        #region Basic ToResult Tests

        [Test]
        public void ToResult_WithValue_ReturnsSuccessResult()
        {
            var maybe = Maybe<int>.From(42);
            var error = "No value";
            
            var result = maybe.ToResult(error);
            
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public void ToResult_WithoutValue_ReturnsFailureResult()
        {
            var maybe = Maybe<int>.None;
            var error = "No value";
            
            var result = maybe.ToResult(error);
            
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(error, result.Error);
        }

        [Test]
        public void ToResult_WithValue_DifferentTypes()
        {
            var maybe = Maybe<string>.From("Hello");
            var error = 404;
            
            var result = maybe.ToResult(error);
            
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Hello", result.Value);
        }

        [Test]
        public void ToResult_WithoutValue_DifferentTypes()
        {
            var maybe = Maybe<string>.None;
            var error = 404;
            
            var result = maybe.ToResult(error);
            
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(404, result.Error);
        }

        [Test]
        public void ToResult_WithValue_ComplexErrorType()
        {
            var maybe = Maybe<int>.From(42);
            var error = new Exception("No value");
            
            var result = maybe.ToResult(error);
            
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public void ToResult_WithoutValue_ComplexErrorType()
        {
            var maybe = Maybe<int>.None;
            var error = new Exception("No value");
            
            var result = maybe.ToResult(error);
            
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(error, result.Error);
            Assert.AreEqual("No value", result.Error.Message);
        }

        [Test]
        public void ToResult_WithValue_NullableValueType()
        {
            var maybe = Maybe<int?>.From(42);
            var error = "No value";
            
            var result = maybe.ToResult(error);
            
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public void ToResult_WithValue_NullableValueTypeWithNull()
        {
            var maybe = Maybe<int?>.From(null);
            var error = "No value";
            
            var result = maybe.ToResult(error);
            
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(error, result.Error);
        }

        [Test]
        public void ToResult_WithoutValue_NullableValueType()
        {
            var maybe = Maybe<int?>.None;
            var error = "No value";
            
            var result = maybe.ToResult(error);
            
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(error, result.Error);
        }

        [Test]
        public void ToResult_WithValue_ReferenceType()
        {
            var obj = new TestClass { Value = 42 };
            var maybe = Maybe<TestClass>.From(obj);
            var error = "No value";
            
            var result = maybe.ToResult(error);
            
            Assert.IsTrue(result.IsSuccess);
            Assert.AreSame(obj, result.Value);
            Assert.AreEqual(42, result.Value.Value);
        }

        [Test]
        public void ToResult_WithoutValue_ReferenceType()
        {
            var maybe = Maybe<TestClass>.None;
            var error = "No value";
            
            var result = maybe.ToResult(error);
            
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(error, result.Error);
        }

        #endregion

        #region UniTask Tests

#if NOPE_UNITASK
        [Test]
        public async Task ToResult_UniTask_WithValue_ReturnsSuccessResult()
        {
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(42));
            var error = "No value";
            
            var result = await asyncMaybe.ToResult(error);
            
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public async Task ToResult_UniTask_WithoutValue_ReturnsFailureResult()
        {
            var asyncMaybe = UniTask.FromResult(Maybe<int>.None);
            var error = "No value";
            
            var result = await asyncMaybe.ToResult(error);
            
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(error, result.Error);
        }

        [Test]
        public async Task ToResult_UniTask_WithValue_DifferentTypes()
        {
            var asyncMaybe = UniTask.FromResult(Maybe<string>.From("Hello"));
            var error = 404;
            
            var result = await asyncMaybe.ToResult(error);
            
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Hello", result.Value);
        }

        [Test]
        public async Task ToResult_UniTask_WithoutValue_DifferentTypes()
        {
            var asyncMaybe = UniTask.FromResult(Maybe<string>.None);
            var error = 404;
            
            var result = await asyncMaybe.ToResult(error);
            
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(404, result.Error);
        }

        [Test]
        public async Task ToResult_UniTask_WithValue_ComplexErrorType()
        {
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(42));
            var error = new Exception("No value");
            
            var result = await asyncMaybe.ToResult(error);
            
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public async Task ToResult_UniTask_WithoutValue_ComplexErrorType()
        {
            var asyncMaybe = UniTask.FromResult(Maybe<int>.None);
            var error = new Exception("No value");
            
            var result = await asyncMaybe.ToResult(error);
            
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(error, result.Error);
            Assert.AreEqual("No value", result.Error.Message);
        }

        [Test]
        public async Task ToResult_UniTask_ChainedOperations()
        {
            var asyncMaybe = UniTask.FromResult(Maybe<int>.From(42));
            
            var result = await asyncMaybe
                .ToResult("Initial error");
            
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public async Task ToResult_UniTask_DelayedExecution()
        {
            var asyncMaybe = UniTask.Create(async () =>
            {
                await UniTask.Delay(10);
                return Maybe<int>.From(42);
            });
            
            var result = await asyncMaybe.ToResult("No value");
            
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }
#endif

        #endregion

        #region Awaitable Tests

#if NOPE_AWAITABLE
        [Test]
        public async Task ToResult_Awaitable_WithValue_ReturnsSuccessResult()
        {
            var asyncMaybe = AwaitableFromResult(Maybe<int>.From(42));
            var error = "No value";
            
            var result = await asyncMaybe.ToResult(error);
            
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public async Task ToResult_Awaitable_WithoutValue_ReturnsFailureResult()
        {
            var asyncMaybe = AwaitableFromResult(Maybe<int>.None);
            var error = "No value";
            
            var result = await asyncMaybe.ToResult(error);
            
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(error, result.Error);
        }

        [Test]
        public async Task ToResult_Awaitable_WithValue_DifferentTypes()
        {
            var asyncMaybe = AwaitableFromResult(Maybe<string>.From("Hello"));
            var error = 404;
            
            var result = await asyncMaybe.ToResult(error);
            
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Hello", result.Value);
        }

        [Test]
        public async Task ToResult_Awaitable_WithoutValue_DifferentTypes()
        {
            var asyncMaybe = AwaitableFromResult(Maybe<string>.None);
            var error = 404;
            
            var result = await asyncMaybe.ToResult(error);
            
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(404, result.Error);
        }

        [Test]
        public async Task ToResult_Awaitable_WithValue_ComplexErrorType()
        {
            var asyncMaybe = AwaitableFromResult(Maybe<int>.From(42));
            var error = new Exception("No value");
            
            var result = await asyncMaybe.ToResult(error);
            
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public async Task ToResult_Awaitable_WithoutValue_ComplexErrorType()
        {
            var asyncMaybe = AwaitableFromResult(Maybe<int>.None);
            var error = new Exception("No value");
            
            var result = await asyncMaybe.ToResult(error);
            
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(error, result.Error);
            Assert.AreEqual("No value", result.Error.Message);
        }

        [Test]
        public async Task ToResult_Awaitable_ChainedOperations()
        {
            var asyncMaybe = AwaitableFromResult(Maybe<int>.From(42));
            
            var result = await asyncMaybe
                .ToResult("Initial error");
            
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public async Task ToResult_Awaitable_DelayedExecution()
        {
            var asyncMaybe = DelayedAwaitableFromResult(Maybe<int>.From(42));
            
            var result = await asyncMaybe.ToResult("No value");
            
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
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