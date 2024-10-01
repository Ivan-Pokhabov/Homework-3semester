namespace homework3.tests;

using homework3;

public class Tests
{
    private readonly int threadPoolSize = Environment.ProcessorCount;
    private MyThreadPool threadPool = null!;

    [SetUp]
    public void Initialization()
    {
        threadPool = new MyThreadPool(threadPoolSize);
    }

    [TearDown]
    public void Cleanup()
    {
        threadPool.ShutDown();
    }

    [Test]
    public void Submit_FunctionReturningValue_ExpectedValueReturned()
    {
        var expectedResult = 92;


        var task = threadPool.Submit(() => 
        { 
            Thread.Sleep(100);
            return 46 * 2;
        });


        Assert.That(task.Result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void MultipleContinueWith_ShouldHaveExpectedResult()
    {
        var expectedResult = 42;


        var myTask = threadPool.Submit(() => 2 * 0)
            .ContinueWith(x => x.ToString())
            .ContinueWith(x => x + "42")
            .ContinueWith(int.Parse!);


        Assert.That(myTask.Result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void SubmitAndContinueWith_FromMultipleThreads_ShouldPerformExpectedResult()
    {
        var threadsCount = 10;
        var expectedResult = 80 * threadPoolSize;   
        ManualResetEvent manualResetEvent = new(false);
        var threads = new Thread[threadsCount];
        var test = new IMyTask<int>[threadPoolSize * threadsCount];


        for (var i = 0; i < threadsCount; ++i)
        {
            var locali = i;
            threads[locali] = new Thread(() =>
                {
                    manualResetEvent.WaitOne();

                    for (var j = threadPoolSize * locali; j < threadPoolSize * (locali + 1); ++j)
                    {
                        test[j] = threadPool.Submit(() => 2 + 2).ContinueWith(r => r * 2);
                    }
                }
            );
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }
        
        Thread.Sleep(100);

        manualResetEvent.Set();

        foreach (var thread in threads)
        {
            thread.Join();
        }

        var result = test.Sum(x => x.Result);


        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void ContinueWith_ParentAndContinuationTask_ParentTaskCompletesFirst()
    {
        var flag = false;

        var parentTask = threadPool.Submit(() =>
        {
            Thread.Sleep(100);
            return 0;
        });


        parentTask.ContinueWith(result =>
        {
            if (parentTask.IsCompleted)
            {
                Volatile.Write(ref flag, true);
            }
            return 1;
        });

        Thread.Sleep(200);


        Assert.That(flag, Is.True);
    }

    [Test]
    public void Shutdown_TasksSubmitted_AllTasksComplete()
    {
        var flag = false;


        threadPool.Submit(() =>
        {
            Thread.Sleep(500);
            Volatile.Write(ref flag, true);
            return 0;
        });

        threadPool.ShutDown();


        Assert.That(flag, Is.True);
    }

    [Test]
    public async Task ConcurrentSubmitAndShutdown_ShouldPerformExpectedResult()
    {
        var expectedResult = 3628800;
        var factorialNumber = 10;
        var actualResult = 0;
        ManualResetEvent manualResetEvent = new(false);


        var submitTask = Task.Run(() =>
        {
            manualResetEvent.WaitOne();
            
            return threadPool.Submit(() => Enumerable.Range(1, factorialNumber).Aggregate(1, (a, b) => a * b));
        });
        var shutdownTak = Task.Run(() =>
        {
            manualResetEvent.WaitOne();
            
            threadPool.ShutDown();
        });

        try
        {
            manualResetEvent.Set(); 
            actualResult = (await submitTask).Result;
        }
        catch (OperationCanceledException)
        {
            Assert.Pass();
        }


        Assert.That(actualResult, Is.EqualTo(expectedResult));
    }

    [Test]
    public void ContinueWith_TasksAndMainThread_MainThreadNotBlocked()
    {
        var mainThreadContinueSignal = new AutoResetEvent(false);


        var parentTask = threadPool.Submit(() =>
        {
            Thread.Sleep(10000);
            return 0;
        });

        var continuationTask = parentTask.ContinueWith(result =>
        {
            mainThreadContinueSignal.Set();
            return 1;
        });

        bool flag = !mainThreadContinueSignal.WaitOne(1000);


        Assert.That(flag, Is.True);
    }

    [Test]
    public void Submit_AfterShutdown_ThrowsOperationCanceledException()
    {
        threadPool.ShutDown();


        Assert.Throws<OperationCanceledException>(() => threadPool.Submit(() => 0));
    }

    [Test]
    public void ContinueWith_AfterShutdown_ThrowsOperationCanceledExceptionn()
    {
        var task = threadPool.Submit(() => 0);
        threadPool.ShutDown();


        Assert.Throws<OperationCanceledException>(() => task.ContinueWith(result => result + 1));
    }
}