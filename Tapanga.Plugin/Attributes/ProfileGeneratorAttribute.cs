namespace Tapanga.Plugin;

/// <summary>
/// Describes a class that is a profile generator.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class ProfileGeneratorAttribute : Attribute
{
    /// <param name="key">A short key for the generator. Should be in the format "[plugin].[generator]"</param>
    /// <param name="version">The generator version.</param>
    /// <exception cref="ArgumentException"></exception>
    public ProfileGeneratorAttribute(string key, string version)
    {
        if (key.Any(c => char.IsWhiteSpace(c)))
        {
            throw new ArgumentException(
                "The Generator key must not include any whitespace characters.",
                nameof(key));
        }

        Key = key;
        Version = Version.Parse(version);
    }

    /// <summary>
    /// A short key for the generator.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// The version of the generator.
    /// </summary>
    public Version Version { get; }

    /// <summary>
    /// Whether the generator is enabled for use.
    /// </summary>
    public bool IsEnabled { get; set; } = true;
}
