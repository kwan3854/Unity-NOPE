// NUnit
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NOPE.Runtime.Core.Result;
using NUnit.Framework;
using UnityEngine;

#if NOPE_AWAITABLE
// using NOPE.Runtime.Core.Awaitable; // 가정
#endif

namespace NOPE.Tests
{
    // =======================================================
    // 1) BASIC TESTS
    // =======================================================
    [TestFixture]
    public class ResultTests_Basic
    {
        [Test]
        public void ResultT_Success_BasicProperties()
        {
            // Arrange
            var r = NOPE.Runtime.Core.Result.Result<int, string>.Success(123);

            // Assert
            Assert.IsTrue(r.IsSuccess);
            Assert.IsFalse(r.IsFailure);
            Assert.AreEqual(123, r.Value);

            // Accessing Error on success => should throw
            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = r.Error;
            });
        }

        [Test]
        public void ResultT_Failure_BasicProperties()
        {
            // Arrange
            var r = NOPE.Runtime.Core.Result.Result<int, string>.Failure("SomeError");

            // Assert
            Assert.IsTrue(r.IsFailure);
            Assert.IsFalse(r.IsSuccess);
            Assert.AreEqual("SomeError", r.Error);

            // Accessing Value on failure => should throw
            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = r.Value;
            });
        }

        [Test]
        public void ResultT_ImplicitSuccess()
        {
            NOPE.Runtime.Core.Result.Result<int, string> r = 999; // implicit => Success(999)

            Assert.IsTrue(r.IsSuccess);
            Assert.AreEqual(999, r.Value);
        }

        [Test]
        public void ResultT_ImplicitFailure()
        {
            NOPE.Runtime.Core.Result.Result<int, string> r = "ERR"; // implicit => Failure("ERR")

            Assert.IsTrue(r.IsFailure);
            Assert.AreEqual("ERR", r.Error);
        }

        [Test]
        public void Result_NoValue_Success()
        {
            var r = NOPE.Runtime.Core.Result.Result<string>.Success();

            Assert.IsTrue(r.IsSuccess);
            Assert.IsFalse(r.IsFailure);

            // Accessing Error => throw
            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = r.Error;
            });
        }

        [Test]
        public void Result_NoValue_Failure()
        {
            var r = NOPE.Runtime.Core.Result.Result<string>.Failure("FAIL");

            Assert.IsTrue(r.IsFailure);
            Assert.AreEqual("FAIL", r.Error);
        }
    }

    // =======================================================
    // 2) SYNC EXTENSIONS TESTS
    // =======================================================
    [TestFixture]
    public class ResultTests_SyncExtensions
    {
        [Test]
        public void Map_WhenSuccess_MapsValue()
        {
            var r = NOPE.Runtime.Core.Result.Result<int, string>.Success(10)
                .Map(v => v * 2);

            Assert.IsTrue(r.IsSuccess);
            Assert.AreEqual(20, r.Value);
        }

        [Test]
        public void Map_WhenFailure_DoesNotCallSelector()
        {
            var r = NOPE.Runtime.Core.Result.Result<int, string>.Failure("ERR")
                .Map(_ =>
                {
                    throw new Exception("ShouldNotBeCalled");
                    
                    return 0; // unreachable
                });

            Assert.IsTrue(r.IsFailure);
            Assert.AreEqual("ERR", r.Error);
        }

        [Test]
        public void Bind_WhenSuccess_BindsToNewResult()
        {
            var r = NOPE.Runtime.Core.Result.Result<int, string>.Success(5)
                .Bind(v =>
                {
                    if (v > 0) 
                        return NOPE.Runtime.Core.Result.Result<string, string>.Success("POS");
                    return NOPE.Runtime.Core.Result.Result<string, string>.Failure("NEG");
                });

            Assert.IsTrue(r.IsSuccess);
            Assert.AreEqual("POS", r.Value);
        }

        [Test]
        public void Bind_WhenFailure_SkipsBinder()
        {
            var r = NOPE.Runtime.Core.Result.Result<int, string>.Failure("ERR")
                .Bind(_ => NOPE.Runtime.Core.Result.Result<string, string>.Success("ShouldNotCall"));

            Assert.IsTrue(r.IsFailure);
            Assert.AreEqual("ERR", r.Error);
        }

        [Test]
        public void Tap_WhenSuccess_ExecutesAction()
        {
            bool sideEffect = false;
            var r = NOPE.Runtime.Core.Result.Result<int, string>.Success(10)
                .Tap(v => sideEffect = (v == 10));

            Assert.IsTrue(sideEffect);
            Assert.IsTrue(r.IsSuccess);
        }

        [Test]
        public void Tap_WhenFailure_SkipsAction()
        {
            bool sideEffect = false;
            var r = NOPE.Runtime.Core.Result.Result<int, string>.Failure("ERR")
                .Tap(_ => sideEffect = true);

            Assert.IsFalse(sideEffect);
            Assert.IsTrue(r.IsFailure);
        }

        [Test]
        public void Ensure_WhenSuccess_AndPredicateFails_ThenFailure()
        {
            var r = NOPE.Runtime.Core.Result.Result<int, string>.Success(5)
                .Ensure(v => v > 10, "NOT_BIG");

            Assert.IsTrue(r.IsFailure);
            Assert.AreEqual("NOT_BIG", r.Error);
        }

        [Test]
        public void Ensure_WhenFailure_IgnorePredicate()
        {
            var r = NOPE.Runtime.Core.Result.Result<int, string>.Failure("InitialErr")
                .Ensure(_ => true, "NewErr"); 
            // remains failure with original error
            Assert.IsTrue(r.IsFailure);
            Assert.AreEqual("InitialErr", r.Error);
        }

        [Test]
        public void MapError_WhenFailure_TransformsError()
        {
            var r = NOPE.Runtime.Core.Result.Result<int, string>.Failure("InitialERR")
                .MapError(e =>
                { 
                    return e.ToUpper();
                });

            Assert.IsTrue(r.IsFailure);
            Assert.AreEqual("INITIALERR", r.Error);
        }

        [Test]
        public void MapError_WhenSuccess_NoChange()
        {
            var r = NOPE.Runtime.Core.Result.Result<int, string>.Success(9)
                .MapError(e => e + "???");

            Assert.IsTrue(r.IsSuccess);
            Assert.AreEqual(9, r.Value);
        }

        [Test]
        public void MapError_E1_To_E2_Test()
        {
            var r1 = NOPE.Runtime.Core.Result.Result<int, int>.Failure(404);

            // E1=int -> E2=string
            var r2 = r1.MapError<int, int, string>(code => $"Error_{code}");
            Assert.IsTrue(r2.IsFailure);
            Assert.AreEqual("Error_404", r2.Error);
        }

        [Test]
        public void Match_ReturnsSuccessValueOrFailureValue()
        {
            var success = NOPE.Runtime.Core.Result.Result<int, string>.Success(123)
                .Match(
                    val => $"Val={val}",
                    err => $"Err={err}"
                );
            Assert.AreEqual("Val=123", success);

            var failure = NOPE.Runtime.Core.Result.Result<int, string>.Failure("Oops")
                .Match(
                    val => $"Val={val}",
                    err => $"Err={err}"
                );
            Assert.AreEqual("Err=Oops", failure);
        }

        [Test]
        public void Finally_AlwaysCalled()
        {
            var succ = NOPE.Runtime.Core.Result.Result<int, string>.Success(10)
                .Finally(r =>
                {
                    if (r.IsSuccess) return $"AllGood_{r.Value}";
                    return $"Error_{r.Error}";
                });
            Assert.AreEqual("AllGood_10", succ);

            var fail = NOPE.Runtime.Core.Result.Result<int, string>.Failure("X")
                .Finally(r => r.IsFailure ? "Failed" : "Success");
            Assert.AreEqual("Failed", fail);
        }
    }

    // =======================================================
    // 3) ASYNC EXTENSIONS TESTS
    // =======================================================
    [TestFixture]
    public class ResultTests_AsyncExtensions
    {
#if NOPE_UNITASK
        [Test]
        public async Task Bind_AsyncResult_SyncBinder_Success()
        {
            // asyncResult => Success(7)
            UniTask<NOPE.Runtime.Core.Result.Result<int, string>> asyncResult =
                UniTask.FromResult(NOPE.Runtime.Core.Result.Result<int, string>.Success(7));

            var r2 = await asyncResult.Bind(val =>
            {
                if (val >= 0) return NOPE.Runtime.Core.Result.Result<double, string>.Success(val * 1.1);
                return NOPE.Runtime.Core.Result.Result<double, string>.Failure("NEG");
            });

            Assert.IsTrue(r2.IsSuccess);
            Assert.AreEqual(7.7, r2.Value, 0.0001);
        }

        [Test]
        public async Task Bind_SyncResult_AsyncBinder_Failure()
        {
            // Sync result => Failure("Err1")
            var syncFail = NOPE.Runtime.Core.Result.Result<int, string>.Failure("Err1");
            var r2 = await syncFail.Bind(async _ =>
            {
                await UniTask.Delay(1);
                return NOPE.Runtime.Core.Result.Result<string, string>.Success("???");
            });
            Assert.IsTrue(r2.IsFailure);
            Assert.AreEqual("Err1", r2.Error);
        }

        [Test]
        public async Task MapError_AsyncOverloads()
        {
            // 동기 결과 + 비동기 변환
            var fail = NOPE.Runtime.Core.Result.Result<int, string>.Failure("Initial");
            var res1 = await fail.MapError(e => UniTask.FromResult(e + " + appended"));
            Assert.IsTrue(res1.IsFailure);
            Assert.AreEqual("Initial + appended", res1.Error);

            // 비동기 결과 + 동기 변환
            var asyncFail = UniTask.FromResult(NOPE.Runtime.Core.Result.Result<int,string>.Failure("ASFail"));
            var res2 = await asyncFail.MapError(e => e.ToLower());
            Assert.IsTrue(res2.IsFailure);
            Assert.AreEqual("asfail", res2.Error);
        }

        [Test]
        public async Task Match_AsyncResult_Async_onFailure()
        {
            var asyncFail = UniTask.FromResult(NOPE.Runtime.Core.Result.Result<int,string>.Failure("ERR999"));
            var final = await asyncFail.Match(
                onSuccessAsync: val => UniTask.FromResult($"Val_{val}"),
                onFailureAsync: err => UniTask.FromResult($"Err_{err}")
            );
            Assert.AreEqual("Err_ERR999", final);
        }
#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        [Test]
        public async Task Bind_AsyncResult_SyncBinder_Awaitable()
        {
            // 가정: Awaitable<Result<int,string>> 의 mock
            var asyncResult = GetFakeAwaitable(NOPE.Runtime.Core.Result.Result<int,string>.Success(5));

            var r2 = await asyncResult.Bind(val =>
            {
                if (val > 0) return NOPE.Runtime.Core.Result.Result<double,string>.Success(val * 2.0);
                return NOPE.Runtime.Core.Result.Result<double,string>.Failure("NEG");
            });

            Assert.IsTrue(r2.IsSuccess);
            Assert.AreEqual(10.0, r2.Value);
        }

        [Test]
        public async Task MapError_AsyncResult_AsyncTransform()
        {
            var asyncFail = GetFakeAwaitable(NOPE.Runtime.Core.Result.Result<int,string>.Failure("E1"));

            var changed = await asyncFail.MapError(e => GetFakeAwaitable(e + "_Extended"));
            Assert.IsTrue(changed.IsFailure);
            Assert.AreEqual("E1_Extended", changed.Error);
        }

        // 모의 Awaitable<T> 구현 예시
        private static Awaitable<ResultT> GetFakeAwaitable<ResultT>(ResultT value)
        {
            return new FakeAwaitable<ResultT>(value);
        }

        private class FakeAwaitable<T> : Awaitable<T>
        {
            private readonly T _value;
            public FakeAwaitable(T value) { _value = value; }
            public override Awaiter<T> GetAwaiter() => new FakeAwaiter<T>(_value);
        }

        private class FakeAwaiter<T> : Awaiter<T>
        {
            private readonly T _value;
            public FakeAwaiter(T value) { _value = value; }
            public override bool IsCompleted => true;
            public override T GetResult() => _value;
            public override void OnCompleted(Action continuation) => continuation?.Invoke();
        }
#endif // NOPE_AWAITABLE
    }

    // =======================================================
    // 4) COMBINE TESTS
    // =======================================================
    [TestFixture]
    public class ResultTests_Combine
    {
        [Test]
        public void CombineWith_2_Success()
        {
            var r1 = NOPE.Runtime.Core.Result.Result<int,string>.Success(100);
            var r2 = NOPE.Runtime.Core.Result.Result<double,string>.Success(3.14);

            var combined = NOPE.Runtime.Core.Result.Result.CombineWith(r1, r2); 
            Assert.IsTrue(combined.IsSuccess);
            Assert.AreEqual((100,3.14), combined.Value);
        }

        [Test]
        public void CombineWith_2_Failure()
        {
            var r1 = NOPE.Runtime.Core.Result.Result<int,string>.Success(100);
            var r2 = NOPE.Runtime.Core.Result.Result<double,string>.Failure("PI_ERR");

            var combined = NOPE.Runtime.Core.Result.Result.CombineWith(r1, r2);
            Assert.IsTrue(combined.IsFailure);
            Assert.AreEqual("PI_ERR", combined.Error);
        }

        [Test]
        public void Combine_Array()
        {
            var arr = new[]
            {
                NOPE.Runtime.Core.Result.Result<int,string>.Success(1),
                NOPE.Runtime.Core.Result.Result<int,string>.Success(2),
                NOPE.Runtime.Core.Result.Result<int,string>.Failure("X"),
                NOPE.Runtime.Core.Result.Result<int,string>.Success(4)
            };
            var combined = NOPE.Runtime.Core.Result.Result.CombineWith(arr);
            Assert.IsTrue(combined.IsFailure);
            Assert.AreEqual("X", combined.Error);
        }

#if NOPE_UNITASK
        [Test]
        public async Task CombineWith_2_Async_Success()
        {
            var async1 = UniTask.FromResult(NOPE.Runtime.Core.Result.Result<int,string>.Success(1));
            var async2 = UniTask.FromResult(NOPE.Runtime.Core.Result.Result<int,string>.Success(2));

            var combined = await NOPE.Runtime.Core.Result.Result.CombineWith(async1, async2);
            Assert.IsTrue(combined.IsSuccess);
            Assert.AreEqual((1,2), combined.Value);
        }

        [Test]
        public async Task CombineWith_Array_Async_Failure()
        {
            var tasks = new UniTask<NOPE.Runtime.Core.Result.Result<int,string>>[]
            {
                UniTask.FromResult(NOPE.Runtime.Core.Result.Result<int,string>.Success(10)),
                UniTask.FromResult(NOPE.Runtime.Core.Result.Result<int,string>.Failure("FailX")),
                UniTask.FromResult(NOPE.Runtime.Core.Result.Result<int,string>.Success(30))
            };
            var combined = await NOPE.Runtime.Core.Result.Result.CombineWith(tasks);
            Assert.IsTrue(combined.IsFailure);
            Assert.AreEqual("FailX", combined.Error);
        }
#endif
    }

    // =======================================================
    // 5) CHAINING SCENARIO TESTS
    // =======================================================
    [TestFixture]
    public class ResultTests_ChainingScenario
    {
        [Test]
        public void Scenario_Chain_Sync()
        {
            // 예: "string -> int" 변환 -> Ensure > 0 -> Map => "Ok" -> Match => 최종 string
            var r = ParseInt("42")
                .Ensure(v => v > 0, "NotPositive")
                .Map(v => $"Ok count={v}")
                .Match(
                    onSuccess: msg => $"SUCCESS: {msg}",
                    onFailure: err => $"FAILURE: {err}"
                );

            Assert.AreEqual("SUCCESS: Ok count=42", r);

            // parse 실패
            var r2 = ParseInt("NotNumber")
                .Ensure(v => v > 0, "NotPositive")
                .Map(v => $"Ok count={v}")
                .Match(
                    onSuccess: msg => $"SUCCESS: {msg}",
                    onFailure: err => $"FAILURE: {err}"
                );
            Assert.AreEqual("FAILURE: InvalidNumber", r2);

            NOPE.Runtime.Core.Result.Result<int,string> ParseInt(string input)
            {
                if (int.TryParse(input, out int val))
                    return val; // => Success
                return "InvalidNumber"; // => Failure
            }
        }

#if NOPE_UNITASK
        [Test]
        public async Task Scenario_Chain_Async()
        {
            UniTask<NOPE.Runtime.Core.Result.Result<int,string>> getScoreTask = MockFetchScoreAsync();

            var final = await getScoreTask
                .Ensure(score => score >= 0, "ScoreNeg")       // sync predicate
                .Map(score => UniTask.FromResult(score * 10)) // async map
                .Bind(async multi =>
                {
                    await UniTask.Delay(1);
                    return (multi > 100)
                        ? NOPE.Runtime.Core.Result.Result<string,string>.Success("HighScore")
                        : NOPE.Runtime.Core.Result.Result<string,string>.Failure("LowScore");
                })
                .Match(
                    onSuccessAsync: s => UniTask.FromResult($"OK: {s}"),
                    onFailureAsync: err => UniTask.FromResult($"FAIL: {err}")
                );

            // MockFetchScoreAsync() 가 15 => 15 >= 0 => map => 150 => bind => "HighScore"
            Assert.AreEqual("OK: HighScore", final);
        }

        private async UniTask<NOPE.Runtime.Core.Result.Result<int,string>> MockFetchScoreAsync()
        {
            await UniTask.Delay(1);
            return 15; // => Success(15)
        }
#endif
    }
}
