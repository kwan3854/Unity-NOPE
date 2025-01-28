using NUnit.Framework;
using Unity.PerformanceTesting;
using CSharpFunctionalExtensions;
namespace NOPE.Tests.PerformanceBenchmarks
{
    [TestFixture]
    public class CSharpFunctionalExtensionsPerformanceTests
    {
        const int N = 100_000;

        [Test, Performance]
        public void CreateSuccess_Failure_CSharpFunctionalExtensions()
        {
            Measure.Method(() =>
                {
                    for (int i = 0; i < N; i++)
                    {
                        var rOk = Result.Success(i);
                        var rFail = Result.Failure<int>("General error");
                    }
                })
                .WarmupCount(5)
                .MeasurementCount(20)
                .IterationsPerMeasurement(2)
                .GC()
                .SampleGroup("CSharpFunctionalExtensions_CreateResult")
                .Run();
        }

        [Test, Performance]
        public void Bind_CSharpFunctionalExtensions()
        {
            var input = Result.Success(10);

            Measure.Method(() =>
                {
                    for (int i = 0; i < N; i++)
                    {
                        var r = input.Bind(x => x > 5
                            ? Result.Success(x * 2.0)
                            : Result.Failure<double>("Too small"));
                    }
                })
                .WarmupCount(5)
                .MeasurementCount(20)
                .IterationsPerMeasurement(2)
                .GC()
                .SampleGroup("CSharpFunctionalExtensions_Bind")
                .Run();
        }

        [Test, Performance]
        public void Map_CSharpFunctionalExtensions()
        {
            var input = Result.Success(10);

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
                .SampleGroup("CSharpFunctionalExtensions_Map")
                .Run();
        }

        [Test, Performance]
        public void Tap_CSharpFunctionalExtensions()
        {
            var input = Result.Success(10);

            Measure.Method(() =>
                {
                    for (int i = 0; i < N; i++)
                    {
                        input.Tap(x => { var y = x * 2; });
                    }
                })
                .WarmupCount(5)
                .MeasurementCount(20)
                .IterationsPerMeasurement(2)
                .GC()
                .SampleGroup("CSharpFunctionalExtensions_Tap")
                .Run();
        }

        [Test, Performance]
        public void Ensure_CSharpFunctionalExtensions()
        {
            var input = Result.Success(10);

            Measure.Method(() =>
                {
                    for (int i = 0; i < N; i++)
                    {
                        var r = input.Ensure(x => x > 5, "Too small");
                    }
                })
                .WarmupCount(5)
                .MeasurementCount(20)
                .IterationsPerMeasurement(2)
                .GC()
                .SampleGroup("CSharpFunctionalExtensions_Ensure")
                .Run();
        }
    }
}