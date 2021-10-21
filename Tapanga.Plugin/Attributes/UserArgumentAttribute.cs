namespace Tapanga.Plugin;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class UserArgumentAttribute : Attribute
{
    public UserArgumentAttribute(string description)
    {
        Description = description;
    }

    public string Description { get; }

    public string ShortName { get; set; } = string.Empty;

    public int Sort { get; set; } = 0;
}
