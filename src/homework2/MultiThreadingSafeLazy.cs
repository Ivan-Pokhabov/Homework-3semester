namespace homework2;

/// <summary>
/// Implementation of ILazy interface that safe for multi-threading.
/// </summary>
/// <typeparam name="T">Supplier result type.</typeparam>
/// <param name="supplier">Function that we evaluate.</param>
public class MultiThreadingSafeLazy<T>(Func<T> supplier) : ILazy<T>
{
    private readonly object locker = new ();
    private volatile Func<T>? supplier = supplier;
    private volatile bool wasСalculate = false;
    private volatile Exception? supplierException;
    private volatile object? value;

    /// <inheritdoc/>
    public T? Get()
    {
        if (wasСalculate)
        {
            return value is null ? default : (T)value;
        }

        if (supplierException is not null)
        {
            throw supplierException;
        }

        lock (locker)
        {
            if (wasСalculate)
            {
                return value is null ? default : (T)value;
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

            supplier = null;
            wasСalculate = true;
            return value is null ? default : (T)value;
        }
    }
}