using NUnit.Framework;
using Unity.PerformanceTesting;
using OneOf;

namespace NOPE.Tests.PerformanceBenchmarks
{
    // Our "error" type
    public enum OneOfError { General=1, TooSmall=2 }

    // We'll define a small extension for "Map" / "Bind"
    public static class OneOfExtensions
    {
        // Emulate "Map":  if we have T -> U, but no new error creation
        public static OneOf<U, E> Map<T, U, E>(this OneOf<T, E> one, System.Func<T, U> mapFunc)
        {
            return one.Match<OneOf<U,E>>(
                tVal => mapFunc(tVal),  // success -> transform
                eVal => eVal            // failure -> pass through
            );
        }

        // Emulate "Bind": if we have T -> OneOf<U,E>
        public static OneOf<U,E> Bind<T,U,E>(this OneOf<T,E> one, System.Func<T, OneOf<U,E>> bindFunc)
        {
            return one.Match(
                tVal => bindFunc(tVal),
                eVal => OneOf<U,E>.FromT1(eVal) // keep the same error
            );
        }

        // "Ensure": if condition fails -> new error
        public static OneOf<T, E> Ensure<T,E>(this OneOf<T,E> one, System.Func<T,bool> predicate, E failVal)
        {
            return one.Match(
                tVal => predicate(tVal)
                    ? one
                    : OneOf<T,E>.FromT1(failVal),
                eVal => eVal
            );
        }

        // "Tap": do a side effect if success
        public static OneOf<T,E> Tap<T,E>(this OneOf<T,E> one, System.Action<T> sideEffect)
        {
            one.Switch(
                tVal => sideEffect(tVal),
                eVal => {} // do nothing on error
            );
            return one;
        }
    }

    [TestFixture]
    public class OneOfPerformanceTests
    {
        const int N = 100_000;

        [Test, Performance]
        public void CreateSuccess_Failure_OneOf()
        {
            Measure.Method(() =>
            {
                for (int i=0; i<N; i++)
                {
                    // success
                    OneOf<int, OneOfError> ok = 123;
                    // fail
                    OneOf<int, OneOfError> fail = OneOfError.General;
                }
            })
            .WarmupCount(5)
            .MeasurementCount(20)
            .IterationsPerMeasurement(2)
            .GC()
            .SampleGroup("OneOf_CreateResult")
            .Run();
        }

        [Test, Performance]
        public void Bind_OneOf()
        {
            OneOf<int, OneOfError> input = 10;

            Measure.Method(() =>
            {
                for(int i=0; i<N; i++)
                {
                    var r = input.Bind(x =>
                        x>5 ? (OneOf<double,OneOfError>) (x*2.0)
                             : OneOfError.TooSmall
                    );
                }
            })
            .WarmupCount(5)
            .MeasurementCount(20)
            .IterationsPerMeasurement(2)
            .GC()
            .SampleGroup("OneOf_Bind")
            .Run();
        }

        [Test, Performance]
        public void Map_OneOf()
        {
            OneOf<int,OneOfError> input = 10;

            Measure.Method(() =>
            {
                for(int i=0; i<N; i++)
                {
                    var r = input.Map(x => x*2);
                }
            })
            .WarmupCount(5)
            .MeasurementCount(20)
            .IterationsPerMeasurement(2)
            .GC()
            .SampleGroup("OneOf_Map")
            .Run();
        }

        [Test, Performance]
        public void Tap_OneOf()
        {
            OneOf<int,OneOfError> input = 10;

            Measure.Method(() =>
            {
                for(int i=0; i<N; i++)
                {
                    input.Tap(x => { var y = x*2; });
                }
            })
            .WarmupCount(5)
            .MeasurementCount(20)
            .IterationsPerMeasurement(2)
            .GC()
            .SampleGroup("OneOf_Tap")
            .Run();
        }

        [Test, Performance]
        public void Ensure_OneOf()
        {
            OneOf<int,OneOfError> input = 10;

            Measure.Method(() =>
            {
                for(int i=0; i<N; i++)
                {
                    var r = input.Ensure(x => x>5, OneOfError.TooSmall);
                }
            })
            .WarmupCount(5)
            .MeasurementCount(20)
            .IterationsPerMeasurement(2)
            .GC()
            .SampleGroup("OneOf_Ensure")
            .Run();
        }
    }
}
