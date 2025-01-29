using NUnit.Framework;
using Unity.PerformanceTesting;
using OneOf;

namespace NOPE.Tests.PerformanceBenchmarks
{
    // We'll define a small extension for "Map" / "Bind"
    public static class OneOfExtensions
    {
        // Emulate "Map":  if we have T -> U, but no new error creation
        public static OneOf<U, E> Map<T, U, E>(this OneOf<T, E> one, System.Func<T, U> mapFunc)
        {
            return one.Match<OneOf<U, E>>(
                tVal => mapFunc(tVal), // success -> transform
                eVal => eVal // failure -> pass through
            );
        }

        // Emulate "Bind": if we have T -> OneOf<U,E>
        public static OneOf<U, E> Bind<T, U, E>(this OneOf<T, E> one, System.Func<T, OneOf<U, E>> bindFunc)
        {
            return one.Match(
                tVal => bindFunc(tVal),
                eVal => OneOf<U, E>.FromT1(eVal) // keep the same error
            );
        }

        // "Ensure": if condition fails -> new error
        public static OneOf<T, E> Ensure<T, E>(this OneOf<T, E> one, System.Func<T, bool> predicate, E failVal)
        {
            return one.Match(
                tVal => predicate(tVal)
                    ? one
                    : OneOf<T, E>.FromT1(failVal),
                eVal => eVal
            );
        }

        // "Tap": do a side effect if success
        public static OneOf<T, E> Tap<T, E>(this OneOf<T, E> one, System.Action<T> sideEffect)
        {
            one.Switch(
                tVal => sideEffect(tVal),
                eVal => { } // do nothing on error
            );
            return one;
        }
    }


    [TestFixture]
    public class OneOf_CompositeTests
    {
        private const int N = 100_000;
        
        public enum TestError
        {
            None = 0,
            General = 1,
            TooSmall = 2
        }

        [Test, Performance]
        public void SyncComposite_OneOf()
        {
            Measure.Method(() =>
                {
                    for (int i = 0; i < N; i++)
                    {
                        // 1) Create success
                        OneOf<int, TestError> val = 10;

                        // 2) Bind => if >5 => success, else fail
                        val = val.Bind(x => x > 5
                            ? (OneOf<int, TestError>)(x + 100)
                            : TestError.TooSmall);

                        // 3) Map => multiply
                        val = val.Map(x => x * 2);

                        // 4) Tap => side effect
                        val.Tap(x =>
                        {
                            var dummy = x + 1;
                        });

                        // 5) Ensure => must be > 0
                        val = val.Ensure(x => x > 0, TestError.General);
                    }
                })
                .WarmupCount(10)
                .DynamicMeasurementCount()
                .IterationsPerMeasurement(10)
                .GC()
                .SampleGroup("OneOf_SyncComposite")
                .Run();
        }

        // no built-in 'async' bridging in OneOf by default, 
        // but you can illustrate a parallel if you want to do tasks + OneOf
    }
}