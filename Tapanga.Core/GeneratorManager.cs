using System.Reflection;
using Tapanga.Plugin;

namespace Tapanga.Core;

using ProfileGenerators = IEnumerable<IProfileGenerator>;

public class GeneratorManager
{
    private static Opt<ProfileGenerators> _cachedGenerators = Opt.None<ProfileGenerators>();
    private readonly Action<string> _infoLog;
    private Opt<IEnumerable<DirectoryInfo>> _pluginPaths;

    public GeneratorManager(Action<string> infoLog, Opt<IEnumerable<DirectoryInfo>> pluginPaths)
    {
        _infoLog = infoLog;
        _pluginPaths = pluginPaths;
    }

    public ProfileGenerators GetProfileGenerators()
    {
        if (_cachedGenerators is Opt<ProfileGenerators>.None)
        {
            IEnumerable<string> pluginsPaths = _pluginPaths switch
            {
                Opt<IEnumerable<DirectoryInfo>>.Some some => some.Value.Select(di => di.FullName),
                _ => Enumerable.Empty<string>()
            };

            var dllPaths = pluginsPaths.SelectMany(pp => Directory.EnumerateFiles(pp, "*.dll", new EnumerationOptions
            {
                MatchCasing = MatchCasing.CaseInsensitive,
            }));

            _cachedGenerators = Opt.Some(dllPaths.SelectMany(dll => CreateGenerators(LoadPlugin(dll))));
        }

        if (_cachedGenerators is Opt<ProfileGenerators>.Some someGenerators)
        {
            return someGenerators.Value;
        }

        return Enumerable.Empty<IProfileGenerator>();
    }

    private static Assembly LoadPlugin(string absolutePath)
    {
        PluginLoadContext loadContext = new(absolutePath);
        return loadContext.LoadFromAssemblyPath(absolutePath);
    }

    private ProfileGenerators CreateGenerators(Assembly assembly)
    {
        if (assembly.GetCustomAttribute<PluginAssemblyAttribute>() is null)
        {
            yield break;
        }

        int count = 0;

        foreach (Type type in assembly.GetExportedTypes())
        {
            if (typeof(IDelegateProfileGenerator).IsAssignableFrom(type))
            {
                var result = Activator.CreateInstance(type) as IDelegateProfileGenerator;
                if (result is IDelegateProfileGenerator generator)
                {
                    count++;
                    yield return new DelegateGeneratorAdapter(generator);
                    continue;
                }
            }

            if (typeof(IProfileGenerator).IsAssignableFrom(type))
            {
                var result = Activator.CreateInstance(type) as IProfileGenerator;
                if (result is IProfileGenerator generator)
                {
                    count++;
                    yield return generator;
                }
            }
        }

        if (count < 1)
        {
            throw new InvalidOperationException($"No plugins found in Tapanga plugin assembly: {assembly.GetName().Name}");
        }

        _infoLog($"Loaded {count} generators from: {assembly.Location}");
    }
}
