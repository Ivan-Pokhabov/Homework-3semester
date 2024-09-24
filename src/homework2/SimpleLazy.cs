namespace homework2;

/// <summary>
/// Simple implementation of ILazy interface that not safe for multi-threading.
/// </summary>
/// <typeparam name="T">Supplier result type.</typeparam>
/// <param name="supplier">Function that we evaluate.</param>
public class SimpleLazy<T>(Func<T> supplier) : ILazy<T>
{
    private bool wasСalculate = false;
    private Func<T> supplier = supplier;
    private Exception? supplierException;
    private T? value;

    /// <inheritdoc/>
    public T? Get()
    {
        if (wasСalculate)
        {
            return value;
        }

        if (supplierException is not null)
        {
            throw supplierException;
        }

        if (supplier is null)
        {
            throw new InvalidDataException();
        }

        try
        {
            value = supplier();
        }
        catch (Exception e)
        {
            supplierException = e;
            throw supplierException;
        }

        wasСalculate = true;
        return value;
    }
}