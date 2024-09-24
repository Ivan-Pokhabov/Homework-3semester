namespace homework2;

public class SimpleLazy<T>(Func<T> supplier) : ILazy<T>
{
    private bool wasСalculate = false;
    private Func<T> supplier = supplier;
    private Exception? supplierException;
    private T? value;

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