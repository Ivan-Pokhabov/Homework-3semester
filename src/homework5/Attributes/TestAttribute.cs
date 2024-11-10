namespace homework5.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class TestAttribute: MyNUnitAttribute
{
    public TestAttribute(string? ignoreMessage = null, Type? expected = null)
    {
        IgnoreMessage = ignoreMessage;
        ExpectedException = expected;    
    }
    
    public TestAttribute(Type expected)
    {
        IgnoreMessage = null;
        ExpectedException = expected;    
    }
    
    public Type? ExpectedException { get; private set; }
    
    public string? IgnoreMessage { get; private set; }
}