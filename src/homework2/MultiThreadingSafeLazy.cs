namespace homework2;

public class MultiThreadingSafeLazy<T>(Func<T> supplier) : ILazy<T>
{
    private bool wasСalculate = false;
    private readonly Func<T> supplier = supplier;
    private Exception? supplierException;
    private T? value;
    private readonly object locker = new();

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