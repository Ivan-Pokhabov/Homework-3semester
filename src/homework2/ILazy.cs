namespace homework2;

/// <summary>
/// Interface for lazy function evaluation.
/// </summary>
/// <typeparam name="T">Function result type.</typeparam>
public interface ILazy<T>
{
    /// <summary>
    /// Returns result of function evaluation.
    /// </summary>
    /// <returns>Function result.</returns>
    T? Get();
}