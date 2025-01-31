using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NOPE.Runtime.Core.Result;
using NUnit.Framework;

namespace NOPE.Tests.ResultTests
{
    [TestFixture]
    public class Result_FailureIf_Tests
    {
        [Test]
        public void Result_FailureIf_ShouldReturnFailure_WhenConditionIsTrue()
        {
            // Act
            var result = Result.FailureIf(true, 42, "Error message");

            // Assert
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual("Error message", result.Error);
        }

        [Test]
        public void Result_FailureIf_ShouldReturnSuccess_WhenConditionIsFalse()
        {
            // Act
            var result = Result.FailureIf(false, 42, "Error message");

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public void Result_FailureIf_ShouldReturnFailure_WhenConditionIsTrue_Func()
        {
            // Act
            var result = Result.FailureIf(() => true, 42, "Error message");

            // Assert
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual("Error message", result.Error);
        }

        [Test]
        public void Result_FailureIf_ShouldReturnSuccess_WhenConditionIsFalse_Func()
        {
            // Act
            var result = Result.FailureIf(() => false, 42, "Error message");

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }
#if NOPE_UNITASK
        [Test]
        public async Task Result_FailureIf_ShouldReturnFailure_WhenConditionIsTrue_Func_Async()
        {
            // Act
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            var result = await Result.FailureIf(async () => true, 42, "Error message");
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

            // Assert
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual("Error message", result.Error);
        }
        
        [Test]
        public async Task Result_FailureIf_ShouldReturnSuccess_WhenConditionIsFalse_Func_Async()
        {
            // Act
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            var result = await Result.FailureIf(async () => false, 42, "Error message");
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }
#endif

#if NOPE_AWAITABLE
        [Test]
        public async Task Result_FailureIf_ShouldReturnFailure_WhenConditionIsTrue_Func_Async()
        {
            // Act
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            var result = await Result.FailureIf(async () => true, 42, "Error message");
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

            // Assert
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual("Error message", result.Error);
        }
        
        [Test]
        public async Task Result_FailureIf_ShouldReturnSuccess_WhenConditionIsFalse_Func_Async()
        {
            // Act
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            var result = await Result.FailureIf(async () => false, 42, "Error message");
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }
#endif
    }
}