namespace Tapanga.Plugin;

public interface IProfileGenerator
{
    public GeneratorInfo GeneratorInfo { get; }

    public Delegate GetGeneratorDelegate(IProfileCollection profiles);
}

public interface IProvideUserArguments
{
    public IReadOnlyList<UserArgument> GetUserArguments();
}

public abstract record UserArgument(string LongName, Opt<string> ShortName, string Description, bool Required, int Sort);

public record UserArgument<T>(string LongName, Opt<string> ShortName, string Description, Opt<T> Default, bool Required, int Sort = 0)
    : UserArgument(LongName, ShortName, Description, Required, Sort);

public record GeneratorInfo(string ShortName, string LongName, Opt<GeneratorDescription> Description);

public record Profile(string Name, string CommandLine, Opt<DirectoryInfo> StartingDirectory, Opt<string> TabTitle, Opt<Icon> Icon);

public record Icon(string Name, Stream Stream);
