using System.Reflection;
using System.Runtime.Loader;
using Tapanga.Plugin;

namespace Tapanga.Core;

using ProfileGenerators = IEnumerable<IProfileGeneratorAdapter>;

public class GeneratorManager
{
    private static Opt<ProfileGenerators> cachedGenerators = Opt.None<ProfileGenerators>();
    private readonly Action<string> _infoLog;
    private readonly Opt<IEnumerable<DirectoryInfo>> _pluginPaths;

    public GeneratorManager(Action<string> infoLog, Opt<IEnumerable<DirectoryInfo>> pluginPaths)
    {
        _infoLog = infoLog;
        _pluginPaths = pluginPaths;
    }

    public ProfileGenerators GetProfileGenerators()
    {
        if (cachedGenerators is Opt<ProfileGenerators>.None)
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

            cachedGenerators = Opt.Some(dllPaths.SelectMany(dll => CreateGenerators(LoadPlugin(dll))));
        }

        if (cachedGenerators is Opt<ProfileGenerators>.Some someGenerators)
        {
            return someGenerators.Value;
        }

        return Enumerable.Empty<ProfileGeneratorAdapter>();
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
                    yield return new ProfileGeneratorAdapter(generator);
                }
            }
        }

        if (count < 1)
        {
            throw new InvalidOperationException($"No plugins found in Tapanga plugin assembly: {assembly.GetName().Name}");
        }

        _infoLog($"Loaded {count} generators from: {assembly.Location}");
    }

    private class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath) => _resolver = new AssemblyDependencyResolver(pluginPath);

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            // ensure the plugin base assembly is only loaded by the entry assembly
            if (assemblyName.FullName == typeof(IProfileGenerator).Assembly.FullName)
            {
                return null;
            }

            try
            {
                string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
                if (assemblyPath is not null)
                {
                    return LoadFromAssemblyPath(assemblyPath);

                }
            }
            catch (BadImageFormatException)
            {
                return null;
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string? libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath is not null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}
