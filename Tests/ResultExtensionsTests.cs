using NOPE.Runtime;
using NOPE.Runtime.Core;
using NOPE.Runtime.Core.Result;
using NUnit.Framework;

namespace NOPE.Tests
{
    public class ResultExtensionsTests
    {
        [Test]
        public void Map_Success_TransformsValue()
        {
            var r = Result<int>.Success(10);
            var mapped = r.Map(x => x * 3);
            Assert.IsTrue(mapped.IsSuccess);
            Assert.AreEqual(30, mapped.Value);
        }

        [Test]
        public void Map_Failure_RetainsError()
        {
            var r = Result<int>.Failure("Oops");
            var mapped = r.Map(x => x * 3);
            Assert.IsTrue(mapped.IsFailure);
            Assert.AreEqual("Oops", mapped.Error);
        }

        [Test]
        public void Bind_Success_TransformsToNewResult()
        {
            var r = Result<int>.Success(10);
            var bound = r.Bind(x => Result<string>.Success((x * 3).ToString()));
            Assert.IsTrue(bound.IsSuccess);
            Assert.AreEqual("30", bound.Value);
        }

        [Test]
        public void Bind_Failure_RetainsError()
        {
            var r = Result<int>.Failure("Oops");
            var bound = r.Bind(x => Result<string>.Success((x * 3).ToString()));
            Assert.IsTrue(bound.IsFailure);
            Assert.AreEqual("Oops", bound.Error);
        }

        [Test]
        public void Tap_Success_ExecutesAction()
        {
            var r = Result<int>.Success(10);
            var tapped = false;
            r.Tap(x => tapped = true);
            Assert.IsTrue(tapped);
        }

        [Test]
        public void Tap_Failure_DoesNotExecuteAction()
        {
            var r = Result<int>.Failure("Oops");
            var tapped = false;
            r.Tap(x => tapped = true);
            Assert.IsFalse(tapped);
        }

        [Test]
        public void Ensure_Success_PredicateTrue_ReturnsOriginal()
        {
            var r = Result<int>.Success(10);
            var ensured = r.Ensure(x => x > 5, "Value is too small");
            Assert.IsTrue(ensured.IsSuccess);
            Assert.AreEqual(10, ensured.Value);
        }

        [Test]
        public void Ensure_Success_PredicateFalse_ReturnsFailure()
        {
            var r = Result<int>.Success(10);
            var ensured = r.Ensure(x => x > 15, "Value is too small");
            Assert.IsTrue(ensured.IsFailure);
            Assert.AreEqual("Value is too small", ensured.Error);
        }

        [Test]
        public void Ensure_Failure_RetainsError()
        {
            var r = Result<int>.Failure("Oops");
            var ensured = r.Ensure(x => x > 5, "Value is too small");
            Assert.IsTrue(ensured.IsFailure);
            Assert.AreEqual("Oops", ensured.Error);
        }

        [Test]
        public void Match_Success_ExecutesOnSuccess()
        {
            var r = Result<int>.Success(10);
            var result = r.Match(
                onSuccess: x => x * 2,
                onFailure: err => -1);
            Assert.AreEqual(20, result);
        }

        [Test]
        public void Match_Failure_ExecutesOnFailure()
        {
            var r = Result<int>.Failure("Oops");
            var result = r.Match(
                onSuccess: x => x * 2,
                onFailure: err => -1);
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void MapError_Failure_TransformsError()
        {
            var r = Result<int>.Failure("Oops");
            var mappedError = r.MapError(err => err.ToUpper());
            Assert.IsTrue(mappedError.IsFailure);
            Assert.AreEqual("OOPS", mappedError.Error);
        }

        [Test]
        public void MapError_Success_RetainsValue()
        {
            var r = Result<int>.Success(10);
            var mappedError = r.MapError(err => err.ToUpper());
            Assert.IsTrue(mappedError.IsSuccess);
            Assert.AreEqual(10, mappedError.Value);
        }
    }
}