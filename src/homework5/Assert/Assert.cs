namespace homework5.Assert;

using homework5.Assert.AssertionException;

public static class Assert
{
    public static void That(bool condition)
    {
        if (!condition)
        {
            throw new AssertionFailedException();
        }
    }
}