using NUnit.Framework;
using Unity.PerformanceTesting;
using static LanguageExt.Prelude;   // for Some(), None
using LanguageExt;                  // for Option<T>, etc.

namespace NOPE.Tests.PerformanceBenchmarks
{
    [TestFixture]
    public class LanguageExtPerformanceTests
    {
        const int N = 100_000;

        [Test, Performance]
        public void CreateSuccess_Failure_LangExt()
        {
            Measure.Method(() =>
            {
                for (int i = 0; i < N; i++)
                {
                    // success
                    Option<int> ok = Some( i );
                    // fail
                    Option<int> fail = None;
                }
            })
            .WarmupCount(5)
            .MeasurementCount(20)
            .IterationsPerMeasurement(2)
            .GC()
            .SampleGroup("LanguageExt_CreateResult")
            .Run();
        }

        [Test, Performance]
        public void Bind_LangExt()
        {
            Option<int> input = Some(10);

            Measure.Method(() =>
            {
                for (int i = 0; i < N; i++)
                {
                    var r = input.Bind(x =>
                        x > 5
                          ? Some<double>(x * 2.0)
                          : Option<double>.None
                    );
                }
            })
            .WarmupCount(5)
            .MeasurementCount(20)
            .IterationsPerMeasurement(2)
            .GC()
            .SampleGroup("LanguageExt_Bind")
            .Run();
        }

        [Test, Performance]
        public void Map_LangExt()
        {
            Option<int> input = Some(10);

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
            .SampleGroup("LanguageExt_Map")
            .Run();
        }

        [Test, Performance]
        public void Tap_LangExt()
        {
            Option<int> input = Some(10);

            Measure.Method(() =>
            {
                for(int i=0; i<N; i++)
                {
                    input.IfSome(x => { var y = x*2; });
                }
            })
            .WarmupCount(5)
            .MeasurementCount(20)
            .IterationsPerMeasurement(2)
            .GC()
            .SampleGroup("LanguageExt_Tap")
            .Run();
        }

        [Test, Performance]
        public void Ensure_LangExt()
        {
            Option<int> input = Some(10);

            Measure.Method(() =>
            {
                for(int i=0; i<N; i++)
                {
                    // "Ensure" by Filter:
                    var r = input.Filter(x => x > 5);
                }
            })
            .WarmupCount(5)
            .MeasurementCount(20)
            .IterationsPerMeasurement(2)
            .GC()
            .SampleGroup("LanguageExt_Ensure")
            .Run();
        }
    }
}
