namespace homework5.Attributes;

/// <summary>
/// Method attribute that runs one time after all tests was complited.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class AfterClassAttribute : MyNUnitAttribute
{
}