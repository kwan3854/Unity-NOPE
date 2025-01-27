using NOPE.Runtime.AdvancedExtensions;
using NOPE.Runtime.Core;
using NUnit.Framework;

namespace NOPE.Tests
{
    public class MaybeLinqTests
    {
        [Test]
        public void LinqSyntax_SimpleQuery_ReturnsExpectedResult()
        {
            var maybe = Maybe<int>.From(5);

            var result = from x in maybe
                where x > 0
                select x * 2;

            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(10, result.Value);
        }

        [Test]
        public void LinqSyntax_WhereClause_ReturnsNoneIfConditionNotMet()
        {
            var maybe = Maybe<int>.From(-5);

            var result = from x in maybe
                where x > 0
                select x * 2;

            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void LinqSyntax_SelectMany_ReturnsExpectedResult()
        {
            var maybe1 = Maybe<int>.From(3);
            var maybe2 = Maybe<int>.From(4);

            var result = from x in maybe1
                from y in maybe2
                select x + y;

            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(7, result.Value);
        }

        [Test]
        public void LinqSyntax_SelectMany_ReturnsNoneIfAnyNone()
        {
            var maybe1 = Maybe<int>.From(3);
            var maybe2 = Maybe<int>.None;

            var result = from x in maybe1
                from y in maybe2
                select x + y;

            Assert.IsFalse(result.HasValue);
        }
    }
}