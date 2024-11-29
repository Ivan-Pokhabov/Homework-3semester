namespace homework5.Assert.AssertionException;

/// <summary>
/// Class of exception taht throws when assertion in test was failed.
/// </summary>
public class AssertionFailedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssertionFailedException"/> class.
    /// </summary>
    public AssertionFailedException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertionFailedException"/> class.
    /// </summary>
    /// <param name="message">Message of assertion.</param>
    public AssertionFailedException(string message)
        : base(message)
    {
    }
}