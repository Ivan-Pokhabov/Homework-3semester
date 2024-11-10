namespace homework5.Attributes;

/// <summary>
/// Attribute for test method.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class TestAttribute : MyNUnitAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestAttribute"/> class.
    /// </summary>
    /// <param name="ignoreMessage">Reason why test should be ignored.</param>
    /// <param name="expected">Expected exception.</param>
    public TestAttribute(string? ignoreMessage = null, Type? expected = null)
    {
        IgnoreMessage = ignoreMessage;
        ExpectedException = expected;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestAttribute"/> class.
    /// </summary>
    /// <param name="expected">Expected Exception.</param>
    public TestAttribute(Type expected)
    {
        IgnoreMessage = null;
        ExpectedException = expected;
    }

    /// <summary>
    /// Gets expected exception.
    /// </summary>
    public Type? ExpectedException { get; private set; }

    /// <summary>
    /// Gets ignore message.
    /// </summary>
    public string? IgnoreMessage { get; private set; }
}