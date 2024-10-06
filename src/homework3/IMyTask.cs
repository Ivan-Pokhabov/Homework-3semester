namespace homework3;

/// <summary>
/// Interface that represents a single operation and that executes async in different threads.
/// </summary>
/// <typeparam name="TResult">Operation result type.</typeparam>
public interface IMyTask<TResult>
{
    /// <summary>
    /// Gets a value indicating whether gets true if task completed.
    /// </summary>
    public bool IsCompleted { get; }

    /// <summary>
    /// Gets result of task.
    /// </summary>
    public TResult? Result { get; }

    /// <summary>
    /// Apply method to the result of this task.
    /// </summary>
    /// <param name="func">method to be applied to the result of current task.</param>
    /// <typeparam name="TNewResult">Result type of <see cref="func"/>.</typeparam>
    /// <returns>result of method <see cref="func"/> with current task result in argument.</returns>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult> func);
}
