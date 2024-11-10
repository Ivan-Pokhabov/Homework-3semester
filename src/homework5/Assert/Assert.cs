namespace homework5.Assert;

using homework5.Assert.AssertionException;

/// <summary>
/// Class for check test result.
/// </summary>
public static class Assert
{
    /// <summary>
    /// Method that check condition.
    /// </summary>
    /// <param name="condition">Bool.</param>
    /// <exception cref="AssertionFailedException">Condition should be true for test passing.</exception>
    public static void That(bool condition)
    {
        if (!condition)
        {
            throw new AssertionFailedException();
        }
    }
}