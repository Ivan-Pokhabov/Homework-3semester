namespace homework2;

/// <summary>
/// Implementation of ILazy interface that safe for multi-threading.
/// </summary>
/// <typeparam name="T">Supplier result type.</typeparam>
/// <param name="supplier">Function that we evaluate.</param>
public class MultiThreadingSafeLazy<T>(Func<T> supplier) : ILazy<T>
{
    private readonly Func<T> supplier = supplier;
    private readonly object locker = new ();
    private bool wasСalculate = false;
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

        lock (locker)
        {
            if (wasСalculate)
            {
                return value;
            }

            if (supplierException is not null)
            {
                throw supplierException;
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
}