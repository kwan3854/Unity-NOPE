using NOPE.Runtime.Core.Result;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace NOPE.Tests.PerformanceBenchmarks
{
    [TestFixture]
    public class NOPEPerformanceTests_Composite
    {
        private const int N = 100_000;
        
        public enum TestError
        {
            None = 0,
            General = 1,
            TooSmall = 2
        }

        // ------------------ SYNC Composite Example ------------------
        [Test, Performance]
        public void SyncComposite_NOPE()
        {
            Measure.Method(() =>
                {
                    for (int i = 0; i < N; i++)
                    {
                        // 1) Create a success
                        var r = Result<int, TestError>.Success(10);

                        // 2) Bind => if >5 => success, else fail
                        r = r.Bind(x => x > 5
                            ? Result<int, TestError>.Success(x + 100)
                            : TestError.TooSmall);

                        // 3) Map => multiply
                        r = r.Map(x => x * 2);

                        // 4) Tap => side effect
                        r = r.Tap(x =>
                        {
                            int dummy = x + 1;
                        });

                        // 5) Ensure => must be > 0
                        r = r.Ensure(x => x > 0, TestError.General);
                    }
                })
                .WarmupCount(10)
                .DynamicMeasurementCount()
                .IterationsPerMeasurement(10)
                .GC()
                .SampleGroup("NOPE_SyncComposite")
                .Run();
        }
    }
}
