using NOPE.Runtime.Core.Maybe;
using NUnit.Framework;

namespace NOPE.Tests.MaybeTests
{
    [TestFixture]
    public class Maybe_TryGet_Tests
    {
        [Test]
        public void TryGetValue_HasValue_ReturnsTrueAndValue()
        {
            var maybe = Maybe<int>.From(42);
            var result = maybe.TryGetValue(out var value);
            Assert.IsTrue(result);
            Assert.AreEqual(42, value);
        }
        
        [Test]
        public void TryGetValue_NoValue_ReturnsFalseAndDefault()
        {
            var maybe = Maybe<int>.None;
            var result = maybe.TryGetValue(out var value);
            Assert.IsFalse(result);
            Assert.AreEqual(default(int), value);
        }
        
        [Test]
        public void TryGetValue_NullValue_ReturnsFalseAndDefault()
        {
            var maybe = Maybe<object>.From(null);
            var result = maybe.TryGetValue(out var value);
            Assert.IsFalse(result);
            Assert.AreEqual(default(object), value);
        }
        
        [Test]
        public void TryGetValue_NoValue_OutValueDefaulted()
        {
            var maybe = Maybe<int>.None;
            maybe.TryGetValue(out var value);
            
            Assert.AreEqual(default(int), value);
            
            var maybe2 = Maybe<TestClass>.None;
            maybe2.TryGetValue(out var value2);
            
            Assert.AreEqual(default(TestClass), value2);
        }
        
        private class TestClass
        {
            // ReSharper disable once UnusedMember.Local
            public int Value { get; set; }
        }
    }
}
