using NOPE.Runtime.Core.Result;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace NOPE.Tests.PerformanceBenchmarks
{
    public enum TestError
    {
        None = 0,
        General = 1,
        TooSmall = 2
    }

    [TestFixture]
    public class NOPEPerformanceTests_ValueTypeError
    {
        const int N = 100_000;

        [Test, Performance]
        public void CreateSuccess_Failure_NOPE_EnumError()
        {
            Measure.Method(() =>
                {
                    for (int i = 0; i < N; i++)
                    {
                        var rOk = Result<int, TestError>.Success(i);
                        var rFail = Result<int, TestError>.Failure(TestError.General);
                    }
                })
                .WarmupCount(5)
                .MeasurementCount(20)
                .IterationsPerMeasurement(2)
                .GC()
                .SampleGroup("NOPE_CreateResult_EnumError")
                .Run();
        }

        [Test, Performance]
        public void Bind_NOPE_EnumError()
        {
            var input = Result<int, TestError>.Success(10);

            Measure.Method(() =>
                {
                    for (int i = 0; i < N; i++)
                    {
                        var r = input.Bind(x => x > 5
                            ? Result<double, TestError>.Success(x * 2.0)
                            : Result<double, TestError>.Failure(TestError.TooSmall));
                    }
                })
                .WarmupCount(5)
                .MeasurementCount(20)
                .IterationsPerMeasurement(2)
                .GC()
                .SampleGroup("NOPE_Bind_EnumError")
                .Run();
        }

        [Test, Performance]
        public void Map_NOPE_EnumError()
        {
            var input = Result<int, TestError>.Success(10);

            Measure.Method(() =>
                {
                    for (int i = 0; i < N; i++)
                    {
                        var r = input.Map(x => x * 2);
                    }
                })
                .WarmupCount(5)
                .MeasurementCount(20)
                .IterationsPerMeasurement(2)
                .GC()
                .SampleGroup("NOPE_Map_EnumError")
                .Run();
        }

        [Test, Performance]
        public void Tap_NOPE_EnumError()
        {
            var input = Result<int, TestError>.Success(10);

            Measure.Method(() =>
                {
                    for (int i = 0; i < N; i++)
                    {
                        input.Tap(x =>
                        {
                            var y = x * 2;
                        });
                    }
                })
                .WarmupCount(5)
                .MeasurementCount(20)
                .IterationsPerMeasurement(2)
                .GC()
                .SampleGroup("NOPE_Tap_EnumError")
                .Run();
        }

        [Test, Performance]
        public void Ensure_NOPE_EnumError()
        {
            var input = Result<int, TestError>.Success(10);

            Measure.Method(() =>
                {
                    for (int i = 0; i < N; i++)
                    {
                        var r = input.Ensure(x => x > 5, TestError.TooSmall);
                    }
                })
                .WarmupCount(5)
                .MeasurementCount(20)
                .IterationsPerMeasurement(2)
                .GC()
                .SampleGroup("NOPE_Ensure_EnumError")
                .Run();
        }
    }
}