using System.ComponentModel;
using System.Reflection;
using Tapanga.Plugin;

namespace Tapanga.Core;

internal class ProfileGeneratorAdapter : IProvideUserArguments, IProfileGenerator, IProfileGeneratorAdapter
{
    private readonly IProfileGenerator _inner;

    public ProfileGeneratorAdapter(IProfileGenerator profileGenerator)
    {
        _inner = profileGenerator;
        GeneratorId = _inner.GetGeneratorId();

        if (profileGenerator.GetType().GetCustomAttribute<DescriptionAttribute>() is DescriptionAttribute attr)
        {
            Description = attr.Description;
        }
    }

    public StringCollection GeneratorInfo => _inner.GeneratorInfo;

    public GeneratorId GeneratorId { get; }

    public string Description { get; } = string.Empty;

    public Delegate GetGeneratorDelegate(IProfileDataCollection profiles) => _inner.GetGeneratorDelegate(profiles);

    public virtual IReadOnlyList<UserArgument> GetUserArguments()
    {
        if (_inner is IProvideUserArguments provider)
        {
            return provider.GetUserArguments();
        }

        return Enumerable.Empty<UserArgument>().ToList();
    }
}
