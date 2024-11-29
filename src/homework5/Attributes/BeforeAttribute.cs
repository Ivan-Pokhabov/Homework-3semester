namespace homework5.Attributes;

/// <summary>
/// Method attribute that runs before every test.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class BeforeAttribute : MyNUnitAttribute
{
}