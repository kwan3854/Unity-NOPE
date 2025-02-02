using NOPE.Runtime.Core.Maybe;
using NUnit.Framework;

namespace NOPE.Tests.MaybeTests
{
    /// <summary>
    /// MaybeExtensions의 TryGetValue 메서드에 대한 유닛 테스트입니다.
    /// </summary>
    public class Maybe_TryGetValue_Tests
    {
        [Test]
        public void TryGetValue_WithValue_ReturnsTrueAndSetsOutValue()
        {
            // Arrange: 값이 있는 Maybe<int> 생성 (예: 42)
            var maybe = Maybe<int>.From(42);

            // Act: TryGetValue 호출
            bool result = maybe.TryGetValue(out int value);

            // Assert: 반환값은 true이고, out value는 42여야 함
            Assert.IsTrue(result, "값이 있는 경우 TryGetValue는 true를 반환해야 합니다.");
            Assert.AreEqual(42, value, "TryGetValue에 의해 반환된 값이 원래 값과 동일해야 합니다.");
        }

        [Test]
        public void TryGetValue_WithNone_ReturnsFalseAndSetsOutValueToDefault()
        {
            // Arrange: None인 Maybe<int> 생성
            var maybe = Maybe<int>.None;

            // Act: TryGetValue 호출
            bool result = maybe.TryGetValue(out int value);

            // Assert: 반환값은 false이고, out value는 default(int)여야 함
            Assert.IsFalse(result, "None인 경우 TryGetValue는 false를 반환해야 합니다.");
            Assert.AreEqual(default(int), value, "None인 경우 out 매개변수는 기본값이어야 합니다.");
        }
    }
}