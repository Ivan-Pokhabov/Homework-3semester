namespace homework5.Assert.AssertionException;

public class AssertionFailedException : Exception
{
    public AssertionFailedException()
    {

    }

    public AssertionFailedException(string message) : base(message)
    {

    }
}