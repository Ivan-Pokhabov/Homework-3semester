namespace homework2.tests;

using homework2;

public class LazyTests
{
    private static readonly Random Rand = new();
    public static IEnumerable<TestCaseData> LazyCorrectFunctionsWithRand
    {
        get
        {
            yield return new TestCaseData(new SimpleLazy<int>(() => Rand.Next()));
            yield return new TestCaseData(new MultiThreadingSafeLazy<int>(() => Rand.Next()));
        }
    }

    public static IEnumerable<TestCaseData> LazyCorrectFunctionsReturnsNull
    {
        get
        {
            yield return new TestCaseData(new SimpleLazy<object?>(() => null));
            yield return new TestCaseData(new MultiThreadingSafeLazy<object?>(() => null));
        }
    }

    public static IEnumerable<TestCaseData> LazyExceptionsFunction
    {
        get
        {
            yield return new TestCaseData(new SimpleLazy<int>(() => throw new ArgumentException()));
            yield return new TestCaseData(new MultiThreadingSafeLazy<int>(() => throw new FileLoadException()));
        }
    }

    [TestCaseSource(nameof(LazyCorrectFunctionsWithRand))]
    public void LazyGet_WithCorrectFunctions_ShouldReturnSameResults(ILazy<int> lazy)
    {
        var firstResult = lazy.Get();


        Assert.That(firstResult, Is.EqualTo(lazy.Get()));
    }

    [TestCaseSource(nameof(LazyCorrectFunctionsReturnsNull))]
    public void LazyGet_WithCorrectFunctionReturnsNull_ShouldReturnSameResults(ILazy<object> lazy)
    {
        var firstResult = lazy.Get();


        Assert.That(firstResult, Is.EqualTo(lazy.Get()));
        Assert.That(firstResult, Is.Null);
    }

    [TestCaseSource(nameof(LazyExceptionsFunction))]
    public void LazyGet_WithFunctionException_ShouldThrowsSameException(ILazy<int> lazy)
    {
        try
        {
            lazy.Get();
        }
        catch (Exception first)
        {
            try
            {
                lazy.Get();
            }
            catch (Exception second)
            {
                Assert.That(first, Is.EqualTo(second));
            }
        }
    }

    [Test]
    public void Lazy_WithNullSupplier_ShouldThrowsInvalidDataException()
    {
        Assert.Throws<InvalidDataException>(() => new SimpleLazy<double>(null!).Get());
        Assert.Throws<InvalidDataException>(() => new MultiThreadingSafeLazy<double>(null!).Get());
    }

    [Test]
    public void Lazy_ShouldUseSupplierOnce()
    {
        var x = 0;
        int function() => x + 1;
        const int result = 1;

        var multiThreadLazy = new MultiThreadingSafeLazy<int>(function);
        var access = new ManualResetEvent(false);

        var processorsCount = Environment.ProcessorCount;
        var threads = new Thread[processorsCount];
        var resultsOfLazyGet = new int[processorsCount];


        for (var _ = 0; _ < 10000; ++_)
        {
            for (var i = 0; i < processorsCount; i++)
            {
                var local = i;
                threads[i] = new Thread(() =>
                {
                    access.WaitOne();
                    resultsOfLazyGet[local] = multiThreadLazy.Get();
                });
                threads[i].Start();
            }

            access.Set();

            foreach (var thread in threads)
            {
                thread.Join();
            }


            for (var i = 0; i < processorsCount; i++)
            {
                Assert.That(resultsOfLazyGet[i], Is.EqualTo(result));
            }

            access.Reset();
            x = 0;
        }

    }

    [Test]
    public void MultiThreadingSafeLazy_WithCorrectFunction_ShouldWorkCorrectlyInParallel()
    {
        static int function() => 1 + 2 * 3;
        var multiThreadLazy = new MultiThreadingSafeLazy<int>(function);
        var access = new ManualResetEvent(false);

        var processorsCount = Environment.ProcessorCount;
        var threads = new Thread[processorsCount];
        var resultsOfLazyGet = new int[processorsCount];


        for (var _ = 0; _ < 10000; ++_)
        {
            for (var i = 0; i < processorsCount; i++)
            {
                var local = i;
                threads[i] = new Thread(() =>
                {
                    access.WaitOne();
                    resultsOfLazyGet[local] = multiThreadLazy.Get();
                });
                threads[i].Start();
            }

            access.Set();

            foreach (var thread in threads)
            {
                thread.Join();
            }


            for (var i = 0; i < processorsCount; i++)
            {
                Assert.That(resultsOfLazyGet[i], Is.EqualTo(function()));
            }

            access.Reset();
        }
    }
}