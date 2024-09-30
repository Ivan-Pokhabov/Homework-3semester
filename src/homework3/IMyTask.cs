namespace homework3;

public interface IMyTask<TResult>
{
    public bool IsCompleted { get; }

    public TResult? Result { get; }

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult> func);
}
