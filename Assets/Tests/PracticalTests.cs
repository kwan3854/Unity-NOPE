using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NOPE.Runtime.Core;
using NUnit.Framework;

#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif

#if NOPE_AWAITABLE
using UnityEngine;
#endif

using NOPE.Runtime.Core.Maybe;
using NOPE.Runtime.Core.Result;

namespace NOPE.Tests
{
    [TestFixture]
    public class NOPEPracticalTests
    {
        // 1) Example: Combine & CombineValues (Sync)
        [Test]
        public void CombineAndCombineValues_Sync()
        {
            // "Result<int>" is effectively "Result<int,string>"
            Result<int, string> r1 = 10; 
            Result<int, string> r2 = Result<int, string>.Failure("Err2");

            // Combine => yields "Result<Unit,string>"
            var combined = Result.Combine(r1, r2);
            Assert.IsTrue(combined.IsFailure);
            Assert.AreEqual("Err2", combined.Error);

            // CombineValues => produce tuple => "Result<(int,int),string>"
            var r3 = Result<int, string>.Success(20);
            var tupleResult = Result.CombineValues(r1, r3);
            Assert.IsTrue(tupleResult.IsSuccess);
            Assert.AreEqual((10,20), tupleResult.Value);
        }

        // 2) Example: SuccessIf, FailureIf, Of
        [Test]
        public void SuccessIf_And_FailureIf()
        {
            var r = Result.SuccessIf(condition: () => DateTime.Now.Year > 2000,
                                     resultValue: 999,
                                     error: "Year <= 2000?");
            Assert.IsTrue(r.IsSuccess); 

            var r2 = Result.FailureIf(true, 123, "Condition triggered fail");
            Assert.IsTrue(r2.IsFailure);
            Assert.AreEqual("Condition triggered fail", r2.Error);
        }

        [Test]
        public void Of_WrapsExceptions()
        {
            var res = Result.Of<int,string>(
                () => throw new InvalidOperationException("IOEx"),
                ex => $"Ex: {ex.Message}");
            Assert.IsTrue(res.IsFailure);
            Assert.AreEqual("Ex: IOEx", res.Error);
        }

        // 3) Maybe + Collection
        [Test]
        public void Maybe_Collection_Utils()
        {
            var dict = new Dictionary<string,int>() {
                ["apple"] = 10,
                ["banana"] = 5
            };

            var maybeApple = dict.TryFind("apple");
            Assert.IsTrue(maybeApple.HasValue);
            Assert.AreEqual(10, maybeApple.Value);

            var maybeKiwi = dict.TryFind("kiwi");
            Assert.IsFalse(maybeKiwi.HasValue);

            var arr = new [] {
                Maybe<int>.From(1),
                Maybe<int>.None,
                Maybe<int>.From(3)
            };
            // Choose => filter out None
            var chosen = arr.Choose();
            Assert.AreEqual(2, chosen.Count());

            // TryFirst => "3"
            var first = chosen.TryFirst(x => x == 3);
            Assert.IsTrue(first.HasValue);
            Assert.AreEqual(3, first.Value);
        }

        // 4) Maybe: LINQ style
        [Test]
        public void Maybe_Linq_Syntax()
        {
            Maybe<int> m = 10;
            var query =
                from x in m
                where x > 5
                select x+100;

            Assert.IsTrue(query.HasValue);
            Assert.AreEqual(110, query.Value);
        }
        
        [Test]
        public void Result_Implicit_Conversion()
        {
            Result<int, string> r = 10;
            Assert.IsTrue(r.IsSuccess);
            Assert.AreEqual(10, r.Value);
            
            Result<int, string> r3 = "Error";
            Assert.IsTrue(r3.IsFailure);
            Assert.AreEqual("Error", r3.Error);
            
            Result<int, string> r4 = 100;
            Assert.IsTrue(r4.IsSuccess);
            Assert.AreEqual(100, r4.Value);

            var a = 100;
            var b = 200;
            Result<int, string> r2 = b == 0 ?
                "Divide by zero"
                : 100;
            Assert.IsTrue(r2.IsSuccess);
            Assert.AreEqual(100, r2.Value);
        }

        [Test]
        public void Result_ValueLess_Combine()
        {
            var r1 = Result<int, string>.Success(2);
            var r2 = Result<int, string>.Success(3);
            var combined = Result.Combine(r1, r2);
            
            Assert.IsTrue(combined.IsSuccess);
            Assert.AreEqual(Unit.Value, combined.Value);
            
            var r3 = Result<int, string>.Failure("Fail");
            var combined2 = Result.Combine(r1, r3);
            Assert.IsTrue(combined2.IsFailure);
            Assert.AreEqual("Fail", combined2.Error);
        }

        [Test]
        public void CombineValues_Example()
        {
            var r1 = Result<int, string>.Success(2);
            var r2 = Result<int, string>.Success(3);
            var r3 = Result<int, string>.Failure("Fail");

            // Combine two results into a tuple
            var combinedTuple = Result.CombineValues(r1, r2);
            Assert.IsTrue(combinedTuple.IsSuccess);
            Assert.AreEqual((2, 3), combinedTuple.Value);

            // Combine three results into an array
            var combinedArray = Result.CombineValues(r1, r2, r3);
            Assert.IsTrue(combinedArray.IsFailure);
            Assert.AreEqual("Fail", combinedArray.Error);
            

            var r4 = Result<int, string>.Success(10);
            var r5 = r4.Bind(x => Result<string, string>.Success($"Value is {x}"));
    
            Assert.IsTrue(r5.IsSuccess);
            Assert.AreEqual("Value is 10", r5.Value);
    
            var r6 = Result<int, string>.Failure("Initial failure");
            var r7 = r6.Bind(x => Result<string, string>.Success($"Value is {x}"));
    
            Assert.IsTrue(r7.IsFailure);
            Assert.AreEqual("Initial failure", r7.Error);
        }

        [Test]
        public void Readme_TestBed()
        {
            // Initial Result with int value
            var r1 = Result<int, string>.Success(10);
    
            // Map to change the value type from int to string
            var r2 = r1.Map(x => $"Value is {x}");

            // Verify the new Result has a string value
            Assert.IsTrue(r2.IsSuccess);
            Assert.AreEqual("Value is 10", r2.Value);

            // Initial Result with failure
            var r3 = Result<int, string>.Failure("Initial failure");
    
            // Map should not change the failure state
            var r4 = r3.Map(x => $"Value is {x}");

            // Verify the failure state is preserved
            Assert.IsTrue(r4.IsFailure);
            Assert.AreEqual("Initial failure", r4.Error);
        }
        
#if NOPE_UNITASK
        // 5) Async usage (UniTask)
        [Test]
        public async Task Async_Chain_Result()
        {
            var r1 = await SlowSuccess(10);
            var final = await r1.Bind(async val =>
            {
                await UniTask.Delay(1);
                return (val > 5)
                    ? Result<string, string>.Success("Val>5")
                    : Result<string, string>.Failure("Val<=5");
            });

            Assert.IsTrue(final.IsSuccess);
            Assert.AreEqual("Val>5", final.Value);
        }

        private async UniTask<Result<int, string>> SlowSuccess(int val)
        {
            await UniTask.Delay(1);
            return val; // => success
        }

        [Test]
        public async Task Maybe_Async_Example()
        {
            var source = new[] {
                Maybe<string>.From("apple"),
                Maybe<string>.None,
                Maybe<string>.From("banana")
            };

            var processed = await ProcessMaybeListAsync(source);
            Assert.AreEqual(2, processed.Count);
            Assert.AreEqual("APPLE", processed[0]);
        }

        private async UniTask<List<string>> ProcessMaybeListAsync(IEnumerable<Maybe<string>> items)
        {
            var result = new List<string>();
            foreach (var m in items)
            {
                if (m.HasValue)
                {
                    await UniTask.Delay(1);
                    result.Add(m.Value.ToUpper());
                }
            }
            return result;
        }
#endif

#if NOPE_AWAITABLE
        // 6) If using Awaitable with Unity6+
        [Test]
        public async Task CombineValues_Async_Awaitable()
        {
            var r1 = SlowSuccess(10);
            var r2 = SlowSuccess(20);
            var combined = await Result.CombineValues(r1, r2);
            Assert.IsTrue(combined.IsSuccess);
            Assert.AreEqual((10,20), combined.Value);
        }
        
        private async Awaitable<Result<int, string>> SlowSuccess(int val)
        {
            await Task.Delay(1);
            return val; // => success
        }
        
        private async Awaitable<Result<int, string>> SlowFailure()
        {
            await Task.Delay(1);
            return "Fail";
        }
#endif

        // 7) Mixed scenario (Maybe + Result)
        [Test]
        public void MixedScenario_Sync()
        {
            Maybe<int> parseResult = ParseInt("1234"); 
            var final = parseResult
                .Match(
                    onValue: val => (Result<int, string>) val,
                    onNone: () => (Result<int, string>) "Not a number"
                )
                .Ensure(x => x < 2000, "Too big!")
                .Map(x => x+10)
                .Tap(x => Console.WriteLine($"Value after addition: {x}"))
                .Match(
                    onSuccess: x => $"Success => {x}",
                    onFailure: err => $"Fail => {err}"
                );

            Assert.AreEqual("Success => 1244", final);
        }

        private Maybe<int> ParseInt(string s)
        {
            if (int.TryParse(s, out int val)) return val;
            return Maybe<int>.None;
        }
    }
}
