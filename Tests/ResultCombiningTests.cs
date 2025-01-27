using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NOPE.Runtime.Core;
using NUnit.Framework;
using UnityEngine;
using System.Linq;

namespace NOPE.Tests
{
    public class ResultCombiningTests
    {
        [Test]
        public void Combine_SuccessfulResults_ReturnsSuccess()
        {
            var result1 = Result<int>.Success(1);
            var result2 = Result<string>.Success("test");
            var result3 = Result<bool>.Success(true);

            var combinedResult = Result.CombineWith(result1, result2, result3);

            Assert.IsTrue(combinedResult.IsSuccess);
        }

        [Test]
        public void Combine_FailedResult_ReturnsFirstFailure()
        {
            var result1 = Result<int>.Success(1);
            var result2 = Result<string>.Failure("error");
            var result3 = Result<bool>.Success(true);

            var combinedResult = Result.CombineWith(result1, result2, result3);

            Assert.IsTrue(combinedResult.IsFailure);
            Assert.AreEqual("error", combinedResult.Error);
        }

        [Test]
        public void Combine_MultipleFailedResults_ReturnsFirstFailure()
        {
            var result1 = Result<int>.Failure("error1");
            var result2 = Result<string>.Failure("error2");
            var result3 = Result<bool>.Failure("error3");

            var combinedResult = Result.CombineWith(result1, result2, result3);

            Assert.IsTrue(combinedResult.IsFailure);
            Assert.AreEqual("error1", combinedResult.Error);
        }

        [Test]
        public void Combine_MoreThanThreeResults_ReturnsSuccess()
        {
            var result1 = Result<int>.Success(1);
            var result2 = Result<string>.Success("test");
            var result3 = Result<bool>.Success(true);
            var result4 = Result<double>.Success(1.0);
            var result5 = Result<char>.Success('c');

            var combinedResult = Result.CombineWith(result1, result2, result3, result4, result5);

            Assert.IsTrue(combinedResult.IsSuccess);
        }

        [Test]
        public async Task CombineAsync_SuccessfulResults_ReturnsSuccess()
        {
            var result1 = UniTask.FromResult(Result<int>.Success(1));
            var result2 = UniTask.FromResult(Result<string>.Success("test"));
            var result3 = UniTask.FromResult(Result<bool>.Success(true));

            var combinedResult = await Result.CombineWith(result1, result2, result3);

            Assert.IsTrue(combinedResult.IsSuccess);
        }

        [Test]
        public async Task CombineAsync_FailedResult_ReturnsFirstFailure()
        {
            var result1 = UniTask.FromResult(Result<int>.Success(1));
            var result2 = UniTask.FromResult(Result<string>.Failure("error"));
            var result3 = UniTask.FromResult(Result<bool>.Success(true));

            var combinedResult = await Result.CombineWith(result1, result2, result3);

            Assert.IsTrue(combinedResult.IsFailure);
            Assert.AreEqual("error", combinedResult.Error);
        }

        [Test]
        public async Task CombineAsync_MultipleFailedResults_ReturnsFirstFailure()
        {
            var result1 = UniTask.FromResult(Result<int>.Failure("error1"));
            var result2 = UniTask.FromResult(Result<string>.Failure("error2"));
            var result3 = UniTask.FromResult(Result<bool>.Failure("error3"));

            var combinedResult = await Result.CombineWith(result1, result2, result3);

            Assert.IsTrue(combinedResult.IsFailure);
            Assert.AreEqual("error1", combinedResult.Error);
        }

        [Test]
        public async Task CombineAsync_MoreThanThreeResults_ReturnsSuccess()
        {
            var result1 = UniTask.FromResult(Result<int>.Success(1));
            var result2 = UniTask.FromResult(Result<string>.Success("test"));
            var result3 = UniTask.FromResult(Result<bool>.Success(true));
            var result4 = UniTask.FromResult(Result<double>.Success(1.0));
            var result5 = UniTask.FromResult(Result<char>.Success('c'));

            var combinedResult = await Result.CombineWith(result1, result2, result3, result4, result5);

            Assert.IsTrue(combinedResult.IsSuccess);
        }

#if NOPE_AWAITABLE
        [Test]
        public async Task CombineAwaitable_SuccessfulResults_ReturnsSuccess()
        {
            var cts1 = new AwaitableCompletionSource<Result<int>>();
            var cts2 = new AwaitableCompletionSource<Result<string>>();
            var cts3 = new AwaitableCompletionSource<Result<bool>>();

            cts1.SetResult(Result<int>.Success(1));
            cts2.SetResult(Result<string>.Success("test"));
            cts3.SetResult(Result<bool>.Success(true));

            var combinedResult = await Result.CombineWith(cts1.Awaitable, cts2.Awaitable, cts3.Awaitable);

            Assert.IsTrue(combinedResult.IsSuccess);
        }

        [Test]
        public async Task CombineAwaitable_FailedResult_ReturnsFirstFailure()
        {
            var cts1 = new AwaitableCompletionSource<Result<int>>();
            var cts2 = new AwaitableCompletionSource<Result<string>>();
            var cts3 = new AwaitableCompletionSource<Result<bool>>();

            cts1.SetResult(Result<int>.Success(1));
            cts2.SetResult(Result<string>.Failure("error"));
            cts3.SetResult(Result<bool>.Success(true));

            var combinedResult = await Result.CombineWith(cts1.Awaitable, cts2.Awaitable, cts3.Awaitable);

            Assert.IsTrue(combinedResult.IsFailure);
            Assert.AreEqual("error", combinedResult.Error);
        }

        [Test]
        public async Task CombineAwaitable_MultipleFailedResults_ReturnsFirstFailure()
        {
            var cts1 = new AwaitableCompletionSource<Result<int>>();
            var cts2 = new AwaitableCompletionSource<Result<string>>();
            var cts3 = new AwaitableCompletionSource<Result<bool>>();

            cts1.SetResult(Result<int>.Failure("error1"));
            cts2.SetResult(Result<string>.Failure("error2"));
            cts3.SetResult(Result<bool>.Failure("error3"));

            var combinedResult = await Result.CombineWith(cts1.Awaitable, cts2.Awaitable, cts3.Awaitable);

            Assert.IsTrue(combinedResult.IsFailure);
            Assert.AreEqual("error1", combinedResult.Error);
        }

        [Test]
        public async Task CombineAwaitable_MoreThanThreeResults_ReturnsSuccess()
        {
            var cts1 = new AwaitableCompletionSource<Result<int>>();
            var cts2 = new AwaitableCompletionSource<Result<string>>();
            var cts3 = new AwaitableCompletionSource<Result<bool>>();
            var cts4 = new AwaitableCompletionSource<Result<double>>();
            var cts5 = new AwaitableCompletionSource<Result<char>>();

            cts1.SetResult(Result<int>.Success(1));
            cts2.SetResult(Result<string>.Success("test"));
            cts3.SetResult(Result<bool>.Success(true));
            cts4.SetResult(Result<double>.Success(1.0));
            cts5.SetResult(Result<char>.Success('c'));

            var combinedResult = await Result.CombineWith(
                cts1.Awaitable, cts2.Awaitable, cts3.Awaitable, cts4.Awaitable, cts5.Awaitable);

            Assert.IsTrue(combinedResult.IsSuccess);
        }
#endif

        [Test]
        public void Combine_TwoResults_Success()
        {
            var r1 = Result<int>.Success(1);
            var r2 = Result<string>.Success("test");

            var combined = Result.CombineWith(r1, r2);

            Assert.IsTrue(combined.IsSuccess);
            Assert.AreEqual(1, combined.Value.Item1);
            Assert.AreEqual("test", combined.Value.Item2);
        }

        [Test]
        public void Combine_TwoResults_FirstFailure()
        {
            var r1 = Result<int>.Failure("error1");
            var r2 = Result<string>.Success("test");

            var combined = Result.CombineWith(r1, r2);

            Assert.IsTrue(combined.IsFailure);
            Assert.AreEqual("error1", combined.Error);
        }

        [Test]
        public void Combine_ThreeResults_Success()
        {
            var r1 = Result<int>.Success(1);
            var r2 = Result<string>.Success("test");
            var r3 = Result<bool>.Success(true);

            var combined = Result.CombineWith(r1, r2, r3);

            Assert.IsTrue(combined.IsSuccess);
            Assert.AreEqual(1, combined.Value.Item1);
            Assert.AreEqual("test", combined.Value.Item2);
            Assert.AreEqual(true, combined.Value.Item3);
        }

        [Test]
        public void Combine_FourResults_Success()
        {
            var r1 = Result<int>.Success(1);
            var r2 = Result<string>.Success("test");
            var r3 = Result<bool>.Success(true);
            var r4 = Result<double>.Success(1.5);

            var combined = Result.CombineWith(r1, r2, r3, r4);

            Assert.IsTrue(combined.IsSuccess);
            Assert.AreEqual(1, combined.Value.Item1);
            Assert.AreEqual("test", combined.Value.Item2);
            Assert.AreEqual(true, combined.Value.Item3);
            Assert.AreEqual(1.5, combined.Value.Item4);
        }

        [Test]
        public void Combine_FiveResults_Success()
        {
            var r1 = Result<int>.Success(1);
            var r2 = Result<string>.Success("test");
            var r3 = Result<bool>.Success(true);
            var r4 = Result<double>.Success(1.5);
            var r5 = Result<char>.Success('a');

            var combined = Result.CombineWith(r1, r2, r3, r4, r5);

            Assert.IsTrue(combined.IsSuccess);
            Assert.AreEqual(1, combined.Value.Item1);
            Assert.AreEqual("test", combined.Value.Item2);
            Assert.AreEqual(true, combined.Value.Item3);
            Assert.AreEqual(1.5, combined.Value.Item4);
            Assert.AreEqual('a', combined.Value.Item5);
        }

        [Test]
        public void Combine_ManyResults_Success()
        {
            var results = Enumerable.Range(0, 10)
                .Select(i => Result<int>.Success(i))
                .ToArray();

            var combined = Result.CombineWith(results);

            Assert.IsTrue(combined.IsSuccess);
            Assert.AreEqual(10, combined.Value.Count());
            Assert.AreEqual(45, combined.Value.Sum());
        }

        [Test]
        public void Combine_ManyResults_OneFailure()
        {
            var results = Enumerable.Range(0, 10)
                .Select(i => i == 5 ? Result<int>.Failure("Failed at 5") : Result<int>.Success(i))
                .ToArray();

            var combined = Result.CombineWith(results);

            Assert.IsTrue(combined.IsFailure);
            Assert.AreEqual("Failed at 5", combined.Error);
        }

        // UniTask versions
        [Test]
        public async Task Combine_TwoResultsAsync_Success()
        {
            var r1 = UniTask.FromResult(Result<int>.Success(1));
            var r2 = UniTask.FromResult(Result<string>.Success("test"));

            var combined = await Result.CombineWith(r1, r2);

            Assert.IsTrue(combined.IsSuccess);
            Assert.AreEqual(1, combined.Value.Item1);
            Assert.AreEqual("test", combined.Value.Item2);
        }

        // ... Similar async tests for 3,4,5 results ...

        [Test]
        public async Task Combine_ManyResultsAsync_Success()
        {
            var results = Enumerable.Range(0, 10)
                .Select(i => UniTask.FromResult(Result<int>.Success(i)))
                .ToArray();

            var combined = await Result.CombineWith(results);

            Assert.IsTrue(combined.IsSuccess);
            Assert.AreEqual(10, combined.Value.Count());
            Assert.AreEqual(45, combined.Value.Sum());
        }

#if NOPE_AWAITABLE
        // Awaitable versions
        [Test]
        public async Task Combine_TwoResultsAwaitable_Success()
        {
            var cts1 = new AwaitableCompletionSource<Result<int>>();
            var cts2 = new AwaitableCompletionSource<Result<string>>();

            cts1.SetResult(Result<int>.Success(1));
            cts2.SetResult(Result<string>.Success("test"));

            var combined = await Result.CombineWith(cts1.Awaitable, cts2.Awaitable);

            Assert.IsTrue(combined.IsSuccess);
            Assert.AreEqual(1, combined.Value.Item1);
            Assert.AreEqual("test", combined.Value.Item2);
        }

        // ... Similar Awaitable tests for 3,4,5 results ...

        [Test]
        public async Task Combine_ManyResultsAwaitable_Success()
        {
            var sources = Enumerable.Range(0, 10)
                .Select(_ => new AwaitableCompletionSource<Result<int>>())
                .ToArray();

            foreach (var (source, i) in sources.Select((s, i) => (s, i)))
            {
                source.SetResult(Result<int>.Success(i));
            }

            var combined = await Result.CombineWith(sources.Select(s => s.Awaitable).ToArray());

            Assert.IsTrue(combined.IsSuccess);
            Assert.AreEqual(10, combined.Value.Count());
            Assert.AreEqual(45, combined.Value.Sum());
        }
#endif

        // Edge cases
        [Test]
        public void Combine_EmptyArray_ReturnsSuccess()
        {
            var combined = Result.CombineWith(Array.Empty<Result<int>>());
            Assert.IsTrue(combined.IsSuccess);
            Assert.IsEmpty(combined.Value);
        }

        [Test]
        public void Combine_NullArray_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Result.CombineWith((Result<int>[])null));
        }

        [Test]
        public void Combine_ArrayWithNullElement_ThrowsException()
        {
            var results = new[] { Result<int>.Success(1), null, Result<int>.Success(3) };
            Assert.Throws<NullReferenceException>(() => Result.CombineWith(results));
        }
    }
}