namespace Tapanga.Plugin;

public interface IGeneratorArguments
{
    string ProfileName { get; }

    OptionalArgument<string> ProfileTitle { get; }

    OptionalArgument<DirectoryInfo> StartingDirectory { get; }
}
