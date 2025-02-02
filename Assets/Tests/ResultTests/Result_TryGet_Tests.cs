using NOPE.Runtime.Core.Result;
using NUnit.Framework;

namespace NOPE.Tests.ResultTests
{
    /// <summary>
    /// TryGetValue 및 TryGetError 확장 메서드에 대한 유닛 테스트입니다.
    /// </summary>
    public class Result_TryGet_Tests
    {
        [Test]
        public void TryGetValue_WithSuccess_ReturnsTrueAndValue()
        {
            // Arrange: 성공 Result 생성
            int expectedValue = 42;
            var result = Result<int, string>.Success(expectedValue);

            // Act: 값 가져오기 시도
            bool success = result.TryGetValue(out int value);

            // Assert: 성공 시 true와 올바른 값이 반환되어야 함
            Assert.IsTrue(success, "성공한 Result에서 TryGetValue는 true를 반환해야 합니다.");
            Assert.AreEqual(expectedValue, value, "Result의 값이 올바르게 반환되어야 합니다.");
        }

        [Test]
        public void TryGetValue_WithFailure_ReturnsFalseAndDefault()
        {
            // Arrange: 실패 Result 생성
            var result = Result<int, string>.Failure("error");

            // Act: 값 가져오기 시도
            bool success = result.TryGetValue(out int value);

            // Assert: 실패 시 false와 기본값(default)이 반환되어야 함
            Assert.IsFalse(success, "실패한 Result에서 TryGetValue는 false를 반환해야 합니다.");
            Assert.AreEqual(default(int), value, "실패한 Result의 값은 기본값이어야 합니다.");
        }

        [Test]
        public void TryGetError_WithFailure_ReturnsTrueAndError()
        {
            // Arrange: 실패 Result 생성
            string expectedError = "error occurred";
            var result = Result<int, string>.Failure(expectedError);

            // Act: 에러 가져오기 시도
            bool hasError = result.TryGetError(out string error);

            // Assert: 실패 시 true와 올바른 에러가 반환되어야 함
            Assert.IsTrue(hasError, "실패한 Result에서 TryGetError는 true를 반환해야 합니다.");
            Assert.AreEqual(expectedError, error, "Result의 에러 메시지가 올바르게 반환되어야 합니다.");
        }

        [Test]
        public void TryGetError_WithSuccess_ReturnsFalseAndDefault()
        {
            // Arrange: 성공 Result 생성
            var result = Result<int, string>.Success(123);

            // Act: 에러 가져오기 시도
            bool hasError = result.TryGetError(out string error);

            // Assert: 성공 시 false와 기본값(default)이 반환되어야 함
            Assert.IsFalse(hasError, "성공한 Result에서 TryGetError는 false를 반환해야 합니다.");
            Assert.AreEqual(default(string), error, "성공한 Result의 에러는 기본값이어야 합니다.");
        }
    }
}
