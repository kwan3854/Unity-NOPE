using NUnit.Framework;
using Unity.PerformanceTesting;
using static LanguageExt.Prelude;
using LanguageExt;

namespace NOPE.Tests.PerformanceBenchmarks
{
    [TestFixture]
    public class LanguageExt_CompositeTests
    {
        private const int N = 100_000;
        
        public enum TestError
        {
            None = 0,
            General = 1,
            TooSmall = 2
        }

        [Test, Performance]
        public void SyncComposite_LangExt()
        {
            Measure.Method(() =>
                {
                    for (int i = 0; i < N; i++)
                    {
                        // 1) Create success
                        Either<TestError, int> r = Right<TestError, int>(10);

                        // 2) Bind => if >5 => success, else fail
                        r = r.Bind(x => x > 5
                            ? Right<TestError, int>(x + 100)
                            : Left<TestError, int>(TestError.TooSmall));

                        // 3) Map => multiply
                        r = r.Map(x => x * 2);

                        // 4) Tap => side effect
                        r.IfRight(x =>
                        {
                            int dummy = x + 1;
                        });

                        // 5) Ensure => must be > 0 (여기서는 Bind를 통해 필터링)
                        r = r.Bind(x => x > 0
                            ? Right<TestError, int>(x)
                            : Left<TestError, int>(TestError.General));
                    }
                })
                .WarmupCount(10)
                .DynamicMeasurementCount()
                .IterationsPerMeasurement(10)
                .GC()
                .SampleGroup("LangExt_SyncComposite")
                .Run();
        }
    }
}