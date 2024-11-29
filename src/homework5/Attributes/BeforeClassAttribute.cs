namespace homework5.Attributes;

/// <summary>
/// Method attribute that runs one time before any tests was ran.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class BeforeClassAttribute : MyNUnitAttribute
{
}