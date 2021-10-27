namespace Tapanga.Plugin;

public interface IGeneratorArguments
{
    string? ProfileName { get; set; }

    string? ProfileTitle { get; set; }

    DirectoryInfo? StartingDirectory { get; set; }
}
