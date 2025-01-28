using NUnit.Framework;
using Unity.PerformanceTesting;
using Optional;
using Optional.Unsafe; // if needed

namespace NOPE.Tests.PerformanceBenchmarks
{
    public enum OptTestError { General=1, TooSmall=2 }

    [TestFixture]
    public class Optional_CompositeTests
    {
        const int N = 100_000;

        [Test, Performance]
        public void SyncComposite_Optional()
        {
            Measure.Method(() =>
                {
                    for (int i = 0; i < N; i++)
                    {
                        // 1) Create success
                        var opt = Option.Some<int, OptTestError>(10);

                        // 2) Bind => if >5 => success, else fail
                        opt = opt.FlatMap(x => x > 5
                            ? Option.Some<int, OptTestError>(x + 100)
                            : Option.None<int, OptTestError>(OptTestError.TooSmall));

                        // 3) Map => multiply
                        opt = opt.Map(x => x * 2);

                        // 4) "Tap" => We can do .MatchSome:
                        opt.MatchSome(x =>
                        {
                            int dummy = x + 1;
                        });

                        // 5) Ensure => filter
                        opt = opt.Filter(x => x > 0, OptTestError.General);
                    }
                })
                .WarmupCount(10)
                .MeasurementCount(10)
                .IterationsPerMeasurement(10)
                .GC()
                .SampleGroup("Optional_SyncComposite")
                .Run();
        }
    }
}