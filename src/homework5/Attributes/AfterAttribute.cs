namespace homework5.Attributes;

/// <summary>
/// Method attribute that runs after every test.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class AfterAttribute : MyNUnitAttribute
{
}