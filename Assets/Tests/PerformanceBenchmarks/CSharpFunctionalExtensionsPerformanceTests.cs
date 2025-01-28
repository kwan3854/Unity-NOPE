using CSharpFunctionalExtensions;
using NUnit.Framework;
using Unity.PerformanceTesting;
using System.Threading.Tasks; // if doing an async version

namespace NOPE.Tests.PerformanceBenchmarks
{
    [TestFixture]
    public class CSharpFunctionalExtensions_CompositeTests
    {
        private const int N = 100_000;

        [Test, Performance]
        public void SyncComposite_CFE()
        {
            Measure.Method(() =>
                {
                    for (int i = 0; i < N; i++)
                    {
                        // 1) Create success
                        var r = Result.Success(10);

                        // 2) Bind => if >5 => success, else fail
                        r = r.Bind(x => x > 5
                            ? Result.Success(x + 100)
                            : Result.Failure<int>("TooSmall"));

                        // 3) Map => multiply
                        r = r.Map(x => x * 2);

                        // 4) Tap => side effect
                        r = r.Tap(x =>
                        {
                            int dummy = x + 1;
                        });

                        // 5) Ensure => must be > 0
                        r = r.Ensure(x => x > 0, "GeneralError");
                    }
                })
                .WarmupCount(10)
                .MeasurementCount(10)
                .IterationsPerMeasurement(10)
                .GC()
                .SampleGroup("CFE_SyncComposite")
                .Run();
        }

        // If you want an async test for C#FE:
        //[Test, Performance]
        //public void AsyncComposite_CFE()
        //{
        //    // Similar pattern but with tasks
        //}
    }
}