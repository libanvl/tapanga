namespace Tapanga.Plugin;

/// <summary>
/// Describes a property that is a generator user argument.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class UserArgumentAttribute : Attribute
{
    /// <param name="description">The description of the argument.</param>
    public UserArgumentAttribute(string description)
    {
        Description = description;
    }

    /// <summary>
    /// The description of the argument.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// An optional short name for the argument.
    /// </summary>
    public string ShortName { get; set; } = string.Empty;

    /// <summary>
    /// Whether the argument is required.
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// The display sort order of the argument.
    /// </summary>
    public int Sort { get; set; } = 0;
}
