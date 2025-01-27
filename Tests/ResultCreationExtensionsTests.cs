using System;
using NOPE.Runtime;
using NOPE.Runtime.Core;
using NUnit.Framework;

namespace NOPE.Tests
{
    public class ResultCreationExtensionsTests
    {
        [Test]
        public void SuccessIf_True_ReturnsSuccess()
        {
            var r = Result.SuccessIf(true, 100, "Not used");
            Assert.IsTrue(r.IsSuccess);
            Assert.AreEqual(100, r.Value);
        }

        [Test]
        public void SuccessIf_False_ReturnsFailure()
        {
            var r = Result.SuccessIf(false, 100, "Error!");
            Assert.IsTrue(r.IsFailure);
            Assert.AreEqual("Error!", r.Error);
        }

        [Test]
        public void SuccessIf_Func_True()
        {
            var r = Result.SuccessIf(() => 1 + 1 == 2, 999, "Math error");
            Assert.IsTrue(r.IsSuccess);
            Assert.AreEqual(999, r.Value);
        }

        [Test]
        public void FailureIf_True_ReturnsFailure()
        {
            var r = Result.FailureIf(true, 777, "Oops!");
            Assert.IsTrue(r.IsFailure);
            Assert.AreEqual("Oops!", r.Error);
        }

        [Test]
        public void FailureIf_False_ReturnsSuccess()
        {
            var r = Result.FailureIf(false, 777, "Will not use");
            Assert.IsTrue(r.IsSuccess);
            Assert.AreEqual(777, r.Value);
        }

        [Test]
        public void Of_NoException_ShouldReturnSuccess()
        {
            var r = Result.Of(() => 10);
            Assert.IsTrue(r.IsSuccess);
            Assert.AreEqual(10, r.Value);
        }

        [Test]
        public void Of_ThrowsException_ShouldReturnFailure()
        {
            var r = Result.Of<int>(() =>
            {
                throw new InvalidOperationException("Something went wrong");
            });
            Assert.IsTrue(r.IsFailure);
            Assert.AreEqual("Something went wrong", r.Error);
        }

        [Test]
        public void Of_CustomErrorConverter()
        {
            var r = Result.Of<int>(
                () => throw new Exception("original message"),
                ex => $"converted: {ex.Message}"
            );
            Assert.IsTrue(r.IsFailure);
            Assert.AreEqual("converted: original message", r.Error);
        }
    }
}