using System;
using NOPE.Runtime.Core.Maybe;
using NUnit.Framework;

namespace NOPE.Tests.MaybeTests
{
    /// <summary>
    /// Maybe 구조체에 대한 유닛 테스트입니다.
    /// </summary>
    public class MaybeTests
    {
        [Test]
        public void From_WithNonNullReference_ReturnsMaybeWithValue()
        {
            // Arrange
            string input = "test value";

            // Act
            var maybe = Maybe<string>.From(input);

            // Assert
            Assert.IsTrue(maybe.HasValue, "값이 존재할 때 HasValue는 true여야 합니다.");
            Assert.IsFalse(maybe.HasNoValue, "값이 존재할 때 HasNoValue는 false여야 합니다.");
            Assert.AreEqual(input, maybe.Value, "Maybe에 저장된 값이 입력값과 같아야 합니다.");
        }

        [Test]
        public void From_WithNullReference_ReturnsMaybeWithoutValue()
        {
            // Arrange
            string input = null;

            // Act
            var maybe = Maybe<string>.From(input);

            // Assert
            Assert.IsFalse(maybe.HasValue, "null인 경우 HasValue는 false여야 합니다.");
            Assert.IsTrue(maybe.HasNoValue, "null인 경우 HasNoValue는 true여야 합니다.");
            Assert.Throws<InvalidOperationException>(() => { var _ = maybe.Value; },
                "값이 없을 때 Value 접근 시 InvalidOperationException이 발생해야 합니다.");
        }

        [Test]
        public void None_ReturnsMaybeWithoutValue()
        {
            // Act
            var maybe = Maybe<int>.None;

            // Assert
            Assert.IsFalse(maybe.HasValue, "None은 HasValue가 false여야 합니다.");
            Assert.IsTrue(maybe.HasNoValue, "None은 HasNoValue가 true여야 합니다.");
            Assert.Throws<InvalidOperationException>(() => { var _ = maybe.Value; },
                "None에서 Value 접근 시 예외가 발생해야 합니다.");
        }

        [Test]
        public void ImplicitConversionOperator_WithNonNullValue_ReturnsMaybeWithValue()
        {
            // Arrange
            int input = 10;

            // Act
            Maybe<int> maybe = input; // 암시적 변환 사용

            // Assert
            Assert.IsTrue(maybe.HasValue, "암시적 변환 후 HasValue는 true여야 합니다.");
            Assert.AreEqual(input, maybe.Value, "암시적 변환된 값이 입력값과 같아야 합니다.");
        }

        [Test]
        public void ImplicitConversionOperator_WithNullReference_ReturnsMaybeWithoutValue()
        {
            // Arrange
            string input = null;

            // Act
            Maybe<string> maybe = input; // 암시적 변환 사용

            // Assert
            Assert.IsFalse(maybe.HasValue, "null 값을 암시적 변환하면 HasValue는 false여야 합니다.");
            Assert.Throws<InvalidOperationException>(() => { var _ = maybe.Value; },
                "암시적 변환된 null에서는 Value 접근 시 예외가 발생해야 합니다.");
        }

        [Test]
        public void Value_Property_ThrowsException_WhenNoValue()
        {
            // Arrange
            var maybe = Maybe<double>.None;

            // Assert
            Assert.Throws<InvalidOperationException>(() => { var _ = maybe.Value; },
                "값이 없는 경우 Value 프로퍼티에 접근하면 예외가 발생해야 합니다.");
        }
    }
}
