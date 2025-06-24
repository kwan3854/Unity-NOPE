using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace NOPE.Tests.PerformanceBenchmarks
{
    [TestFixture]
    public class CSharpFunctionalExtensions_CompositeTests
    {
        private const int N = 100_000;
        
        public enum TestError
        {
            None = 0,
            General = 1,
            TooSmall = 2
        }

        [Test, Performance]
        public async Task SyncComposite_CFE()
        {
            Measure.Method(() =>
                {
                    for (int i = 0; i < N; i++)
                    {
                        // 1) Create success
                        Result<int, TestError> r = Result.Success<int, TestError>(10);

                        // 2) Bind => if >5 => success, else fail
                        r = r.Bind(x => x > 5
                            ? Result.Success<int, TestError>(x + 100)
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