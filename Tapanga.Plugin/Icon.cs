namespace Tapanga.Plugin;

/// <summary>
/// Describes a profile icon.
/// </summary>
/// <param name="Name">The display name of the icon</param>
public abstract record Icon(string Name);

/// <summary>
/// Describes an icon held in a <see cref="Stream"/>.
/// </summary>
/// <param name="Name">The display name of the icon</param>
/// <param name="Stream">The stream holding the icon data</param>
public record StreamIcon(string Name, Stream Stream) : Icon(Name);

/// <summary>
/// Describes an icon on disk.
/// </summary>
/// <param name="Name">The display name of the icon</param>
/// <param name="Path">The path on disk of the icon data</param>
public record PathIcon(string Name, string Path) : Icon(Name);
