namespace homework3;

using System.Collections.Concurrent;

/// <summary>
/// Represents a thread pool for executing tasks concurrently.
/// </summary>
public class MyThreadPool : IDisposable
{
    private readonly Thread[] threads;
    private readonly ConcurrentQueue<Action> tasks;
    private readonly CancellationTokenSource cts;
    private readonly AutoResetEvent access;
    private readonly AutoResetEvent wakeUpEvent;
    private readonly ManualResetEvent cancelEvent;
    private int threadsDone;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPool"/> class.
    /// </summary>
    /// <param name="threadsNumber">Number of threads in treeadpool.</param>
    public MyThreadPool(int threadsNumber)
    {
        if (threadsNumber <= 0)
        {
            throw new ArgumentException(null, nameof(threadsNumber));
        }

        threadsDone = 0;
        access = new (true);
        wakeUpEvent = new (false);
        cancelEvent = new (false);
        tasks = new ();
        cts = new ();
        tasks = new ();
        threads = Enumerable.Range(0, threadsNumber).Select(_ => new Thread(() => Worker(cts.Token)) { IsBackground = true }).ToArray();

        for (var i = 0; i < threadsNumber; ++i)
        {
            threads[i].Start();
        }
    }

    /// <summary>
    /// Add task to thread pool.
    /// </summary>
    /// <typeparam name="TResult">Type of task's result.</typeparam>
    /// <param name="func">Task.</param>
    /// <returns>Instance of <see cref="IMytask"/> interface.</returns>
    /// <exception cref="OperationCanceledException">Thread pool was shut down.</exception>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> func)
    {
        if (cts.Token.IsCancellationRequested)
        {
            throw new OperationCanceledException("ThreadPool was shut down");
        }

        var task = new MyTask<TResult>(this, func);

        access.WaitOne();

        tasks.Enqueue(task.Compute);
        wakeUpEvent.Set();

        access.Set();

        return task;
    }

    /// <summary>
    /// Shuts down the thread pool and waits for all threads to complete execution.
    /// </summary>
    public void ShutDown()
    {
        if (cts.Token.IsCancellationRequested)
        {
            return;
        }

        cts.Cancel();
        cancelEvent.Set();

        while (true)
        {
            if (threadsDone == threads.Length)
            {
                break;
            }
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        ShutDown();
        access.Dispose();
        wakeUpEvent.Dispose();
        cancelEvent.Dispose();
    }

    private void Worker(CancellationToken token)
    {
        WaitHandle.WaitAny([cancelEvent, wakeUpEvent]);
        while (true)
        {
            access.WaitOne();
            if (tasks.TryDequeue(out var task))
            {
                access.Set();
                task();
            }
            else if (token.IsCancellationRequested)
            {
                access.Set();
                break;
            }
            else
            {
                access.Set();
                WaitHandle.WaitAny([cancelEvent, wakeUpEvent]);
            }
        }

        Interlocked.Increment(ref threadsDone);
    }

    private void SubmitContinuation(Action func)
    {
        access.WaitOne();

        tasks.Enqueue(func);
        wakeUpEvent.Set();

        access.Set();
    }

    private class MyTask<TResult>(MyThreadPool threadPool, Func<TResult> function) : IMyTask<TResult>
    {
        private readonly MyThreadPool threadPool = threadPool;
        private readonly ManualResetEvent resultEvent = new (false);
        private readonly List<Action> continuations = [];
        private Func<TResult>? action = function;
        private Exception? actionException;
        private TResult? result;

        private volatile bool isCompleted = false;

        public bool IsCompleted => isCompleted;

        public TResult? Result
        {
            get
            {
                resultEvent.WaitOne();
                if (actionException != null)
                {
                    throw new AggregateException(actionException);
                }

                return result;
            }
        }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult> func)
        {
            if (threadPool.cts.Token.IsCancellationRequested)
            {
                throw new OperationCanceledException("Thread pool was shut down");
            }

            if (isCompleted)
            {
                return threadPool.Submit(() => func(Result));
            }

            var delayedContinuation = new MyTask<TNewResult>(threadPool, () => func(Result));
            continuations.Add(delayedContinuation.Compute);

            return delayedContinuation;
        }

        internal void Compute()
        {
            if (action == null)
            {
                actionException = new Exception("Function can not be null. ");
                return;
            }

            try
            {
                result = action();
            }
            catch (AggregateException e)
            {
                actionException = new Exception("Parent task thrown exception", e);
            }
            catch (Exception e)
            {
                actionException = e;
            }
            finally
            {
                action = null;
                isCompleted = true;

                resultEvent.Set();
                SubmitDelayedContinuations();
            }
        }

        private void SubmitDelayedContinuations()
            => continuations.ForEach(threadPool.SubmitContinuation);
    }
}