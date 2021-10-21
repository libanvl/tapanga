namespace Tapanga.Plugin;

public interface IDelegateProfileGenerator : IProfileGenerator
{
    bool IsAsyncDelegate { get; }

    Type ParameterType { get; }
}
