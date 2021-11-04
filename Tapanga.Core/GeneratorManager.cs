using libanvl;
using Microsoft;
using System.Reflection;
using System.Runtime.Loader;
using Tapanga.Plugin;

namespace Tapanga.Core;

using ProfileGenerators = List<IProfileGeneratorAdapter>;

public class GeneratorManager
{
    private static Opt<ProfileGenerators> cachedGenerators = Opt.None<ProfileGenerators>();
    private readonly Opt<IEnumerable<DirectoryInfo>> _pluginPaths;

    public GeneratorManager(Opt<IEnumerable<DirectoryInfo>> pluginPaths)
    {
        _pluginPaths = pluginPaths;
    }

    public ProfileGenerators GetProfileGenerators()
    {
        if (cachedGenerators.IsNone)
        {
            IEnumerable<string> pluginsPaths = _pluginPaths
                .SomeOrEmpty()
                .Select(di => di.FullName);

            var dllPaths = pluginsPaths.SelectMany(pp => Directory.EnumerateFiles(pp, "*.dll", new EnumerationOptions
            {
                MatchCasing = MatchCasing.CaseInsensitive,
            }));

            cachedGenerators = Opt.Some(
                dllPaths
                .SelectMany(dll => CreateGenerators(LoadPlugin(dll)))
                .ToList());
        }

        if (cachedGenerators is Opt<ProfileGenerators>.Some someGenerators)
        {
            return someGenerators.Value;
        }

        return Assumes.NotReachable<ProfileGenerators>();
    }

    private static Assembly LoadPlugin(string absolutePath)
    {
        PluginLoadContext loadContext = new(absolutePath);
        return loadContext.LoadFromAssemblyPath(absolutePath);
    }

    private IEnumerable<IProfileGeneratorAdapter> CreateGenerators(Assembly assembly)
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
    }

    private class PluginLoadContext : AssemblyLoadContext
    {
        private static readonly string[] sharedAssemblies = new[]
        {
            typeof(IProfileGenerator).Assembly.FullName!,
            typeof(OptBase).Assembly.FullName!,
        };

        private readonly AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath) => _resolver = new AssemblyDependencyResolver(pluginPath);

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            // ensure the shared assemblies are not loaded in this context
            if (sharedAssemblies.Contains(assemblyName.FullName))
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
