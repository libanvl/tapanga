namespace Tapanga.Plugin;

public abstract record Icon(string Name);

public record StreamIcon(string Name, Stream Stream) : Icon(Name);

public record PathIcon(string Name, string Path) : Icon(Name);
