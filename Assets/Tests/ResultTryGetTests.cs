using System.Collections;
using NOPE.Runtime.Core.Result;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace NOPE.Tests
{
    public class ResultTryGetTests
    {
        [Test]
        public void TryGetValue_HasValue_ReturnsTrueAndValue()
        {
            var result = Result<int, string>.Success(42);
            var tryGetResult = result.TryGetValue(out var value);
            Assert.IsTrue(tryGetResult);
            Assert.AreEqual(42, value);
        }
        
        [Test]
        public void TryGetValue_NoValue_ReturnsFalseAndDefault()
        {
            var result = Result<int, string>.Failure("Error");
            var tryGetResult = result.TryGetValue(out var value);
            Assert.IsFalse(tryGetResult);
            Assert.AreEqual(default(int), value);
        }
        
        [Test]
        public void TryGetValue_NullValue_ReturnsTrueAndNull()
        {
            var result = Result<object, string>.Success(null);
            var tryGetResult = result.TryGetValue(out var value);
            Assert.IsTrue(tryGetResult);
            Assert.AreEqual(null, value);
        }
        
        [Test]
        public void TryGetValue_NoValue_OutValueDefaulted()
        {
            var result = Result<int, string>.Failure("Error");
            result.TryGetValue(out var value);
            
            Assert.AreEqual(default(int), value);
            
            var result2 = Result<TestClass, string>.Failure("Error");
            result2.TryGetValue(out var value2);
            
            Assert.AreEqual(default(TestClass), value2);
        }
        
        private class TestClass
        {
            public int Value { get; set; }
        }
        
        [Test]
        public void TryGetError_HasError_ReturnsTrueAndError()
        {
            var result = Result<int, string>.Failure("Error");
            var tryGetResult = result.TryGetError(out var error);
            Assert.IsTrue(tryGetResult);
            Assert.AreEqual("Error", error);
        }
        
        [Test]
        public void TryGetError_NoError_ReturnsFalseAndDefault()
        {
            var result = Result<int, string>.Success(42);
            var tryGetResult = result.TryGetError(out var error);
            Assert.IsFalse(tryGetResult);
            Assert.AreEqual(default(string), error);
        }
        
        [Test]
        public void TryGetError_NullError_ReturnsTrueAndNull()
        {
            var result = Result<int, object>.Failure(null);
            var tryGetResult = result.TryGetError(out var error);
            Assert.IsTrue(tryGetResult);
            Assert.AreEqual(null, error);
        }
        
        [Test]
        public void TryGetError_NoError_OutErrorDefaulted()
        {
            var result = Result<int, string>.Success(42);
            result.TryGetError(out var error);
            
            Assert.AreEqual(default(string), error);
            
            var result2 = Result<int, TestClass>.Success(42);
            result2.TryGetError(out var error2);
            
            Assert.AreEqual(default(TestClass), error2);
        }
    }
}
