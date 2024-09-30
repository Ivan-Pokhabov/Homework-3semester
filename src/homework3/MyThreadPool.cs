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
    private readonly AutoResetEvent accessToTasksEvent;
    private readonly AutoResetEvent threadWakeUpBeforeCancelEvent;
    private readonly ManualResetEvent threadWakeUpAfterCancelEvent;
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
        accessToTasksEvent = new (true);
        threadWakeUpBeforeCancelEvent = new (false);
        threadWakeUpAfterCancelEvent = new (false);
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
    /// <exception cref="InvalidOperationException">Thread pool was shut down.</exception>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> func)
    {
        if (cts.Token.IsCancellationRequested)
        {
            throw new InvalidOperationException("ThreadPool was shut down");
        }

        var task = new MyTask<TResult>(this, func);

        accessToTasksEvent.WaitOne();

        tasks.Enqueue(task.Compute);
        threadWakeUpBeforeCancelEvent.Set();

        accessToTasksEvent.Set();

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
        threadWakeUpAfterCancelEvent.Set();

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
        accessToTasksEvent.Dispose();
        threadWakeUpBeforeCancelEvent.Dispose();
        threadWakeUpAfterCancelEvent.Dispose();
    }

    private void Worker(CancellationToken token)
    {
        WaitHandle.WaitAny([threadWakeUpAfterCancelEvent, threadWakeUpBeforeCancelEvent]);
        while (true)
        {
            accessToTasksEvent.WaitOne();
            if (tasks.TryDequeue(out var task))
            {
                accessToTasksEvent.Set();
                task();
            }
            else if (token.IsCancellationRequested)
            {
                accessToTasksEvent.Set();
                break;
            }
            else
            {
                accessToTasksEvent.Set();
                WaitHandle.WaitAny([threadWakeUpAfterCancelEvent, threadWakeUpBeforeCancelEvent]);
            }
        }

        Interlocked.Increment(ref threadsDone);
    }

    private void SubmitContinuation(Action func)
    {
        accessToTasksEvent.WaitOne();

        tasks.Enqueue(func);
        threadWakeUpBeforeCancelEvent.Set();

        accessToTasksEvent.Set();
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
                throw new InvalidOperationException("Thread pool was shut down");
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