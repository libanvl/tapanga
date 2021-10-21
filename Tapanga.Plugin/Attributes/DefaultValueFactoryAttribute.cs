namespace Tapanga.Plugin;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class DefaultValueFactoryAttribute : Attribute
{
    public DefaultValueFactoryAttribute(string methodName)
    {
        MethodName = methodName;
    }

    public string MethodName { get; }
}
