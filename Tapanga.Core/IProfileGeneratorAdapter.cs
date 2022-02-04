using Tapanga.Plugin;

namespace Tapanga.Core;

public interface IProfileGeneratorAdapter
{
    string Description { get; }

    GeneratorId GeneratorId { get; }

    StringCollection GeneratorInfo { get; }

    Delegate GetGeneratorDelegate(IProfileDataCollection profiles);

    IReadOnlyList<UserArgument> GetUserArguments();
}
