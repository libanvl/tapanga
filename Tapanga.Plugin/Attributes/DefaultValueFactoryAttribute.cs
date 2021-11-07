namespace Tapanga.Plugin;

/// <summary>
/// Describes a factory method for a user argument's default value.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class DefaultValueFactoryAttribute : Attribute
{
    /// <param name="methodName">The name of a static method in the same class that will provide the default value.</param>
    public DefaultValueFactoryAttribute(string methodName)
    {
        MethodName = methodName;
    }

    /// <summary>
    /// The default value factory method name.
    /// </summary>
    public string MethodName { get; }
}
