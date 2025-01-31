using NOPE.Runtime.Core.Result;
using NUnit.Framework;
using System.Threading.Tasks;
using UnityEngine;
#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace NOPE.Tests.ResultTests
{
    [TestFixture]
    public class Result_SuccessIf_Tests
    {
        [Test]
        public void Result_SuccessIf_ShouldReturnSuccess_WhenConditionIsTrue()
        {
            // Act
            var result = Result.SuccessIf(true, 42, "Error message");

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }
        
        [Test]
        public void Result_SuccessIf_ShouldReturnFailure_WhenConditionIsFalse()
        {
            // Act
            var result = Result.SuccessIf(false, 42, "Error message");

            // Assert
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual("Error message", result.Error);
        }
        
        [Test]
        public void Result_SuccessIf_ShouldReturnSuccess_WhenConditionIsTrue_Lazy()
        {
            // Act
            var result = Result.SuccessIf(() => true, 42, "Error message");

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }
        
        [Test]
        public void Result_SuccessIf_ShouldReturnFailure_WhenConditionIsFalse_Lazy()
        {
            // Act
            var result = Result.SuccessIf(() => false, 42, "Error message");

            // Assert
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual("Error message", result.Error);
        }

#if NOPE_UNITASK
        [Test]
        public async Task Result_SuccessIf_ShouldReturnSuccess_WhenConditionIsTrue_Async()
        {
            // Act
            var result = await Result.SuccessIf(async () =>
            {
                await UniTask.Delay(1);
                return true;
            }, 42, "Error message");
            
            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }
        
        [Test]
        public async Task Result_SuccessIf_ShouldReturnFailure_WhenConditionIsFalse_Async()
        {
            // Act
            var result = await Result.SuccessIf(async () =>
            {
                await UniTask.Delay(1);
                return false;
            }, 42, "Error message");
            
            // Assert
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual("Error message", result.Error);
        }
#endif

#if NOPE_AWAITABLE
        [Test]
        public async Task Result_SuccessIf_ShouldReturnSuccess_WhenConditionIsTrue_Async()
        {
            // Act
            var result = await Result.SuccessIf(async () =>
            {
                await Task.Delay(1);
                return true;
            }, 42, "Error message");
            
            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(42, result.Value);
        }
        
        [Test]
        public async Task Result_SuccessIf_ShouldReturnFailure_WhenConditionIsFalse_Async()
        {
            // Act
            var result = await Result.SuccessIf(async () =>
            {
                await Task.Delay(1);
                return false;
            }, 42, "Error message");
            
            // Assert
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual("Error message", result.Error);
        }
#endif
    }
}