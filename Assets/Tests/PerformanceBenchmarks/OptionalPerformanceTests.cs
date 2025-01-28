using NUnit.Framework;
using Unity.PerformanceTesting;
using Optional;

namespace NOPE.Tests.PerformanceBenchmarks
{
    public enum OptTestError { General=1, TooSmall=2 }

    [TestFixture]
    public class OptionalPerformanceTests
    {
        const int N = 100_000;

        [Test, Performance]
        public void CreateSuccess_Failure_Optional()
        {
            Measure.Method(() =>
            {
                for (int i = 0; i < N; i++)
                {
                    var ok = Option.Some<int, OptTestError>(i);
                    var fail = Option.None<int, OptTestError>(OptTestError.General);
                }
            })
            .WarmupCount(5)
            .MeasurementCount(20)
            .IterationsPerMeasurement(2)
            .GC()
            .SampleGroup("Optional_CreateResult")
            .Run();
        }

        [Test, Performance]
        public void Bind_Optional()
        {
            // success input
            var input = Option.Some<int, OptTestError>(10);

            Measure.Method(() =>
            {
                for (int i = 0; i < N; i++)
                {
                    var r = input.FlatMap(x =>
                        x > 5
                          ? Option.Some<double, OptTestError>(x * 2.0)
                          : Option.None<double, OptTestError>(OptTestError.TooSmall)
                    );
                }
            })
            .WarmupCount(5)
            .MeasurementCount(20)
            .IterationsPerMeasurement(2)
            .GC()
            .SampleGroup("Optional_Bind")
            .Run();
        }

        [Test, Performance]
        public void Map_Optional()
        {
            var input = Option.Some<int, OptTestError>(10);

            Measure.Method(() =>
            {
                for (int i=0; i<N; i++)
                {
                    var r = input.Map(x => x * 2);
                }
            })
            .WarmupCount(5)
            .MeasurementCount(20)
            .IterationsPerMeasurement(2)
            .GC()
            .SampleGroup("Optional_Map")
            .Run();
        }

        [Test, Performance]
        public void Tap_Optional()
        {
            var input = Option.Some<int, OptTestError>(10);

            Measure.Method(() =>
            {
                for (int i=0; i<N; i++)
                {
                    // No direct "Tap," but we can do a side effect in MatchSome
                    input.MatchSome(x => { var y = x * 2; });
                }
            })
            .WarmupCount(5)
            .MeasurementCount(20)
            .IterationsPerMeasurement(2)
            .GC()
            .SampleGroup("Optional_Tap")
            .Run();
        }

        [Test, Performance]
        public void Ensure_Optional()
        {
            var input = Option.Some<int, OptTestError>(10);

            Measure.Method(() =>
            {
                for (int i=0; i<N; i++)
                {
                    // "Ensure" via Filter(predicate, exceptionVal)
                    var r = input.Filter(x => x > 5, OptTestError.TooSmall);
                }
            })
            .WarmupCount(5)
            .MeasurementCount(20)
            .IterationsPerMeasurement(2)
            .GC()
            .SampleGroup("Optional_Ensure")
            .Run();
        }
    }
}

