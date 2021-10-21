using System.Reflection;
using Tapanga.Plugin;

namespace Tapanga.Core;

public class GeneratorManager
{
    public IEnumerable<IProfileGenerator> GetProfileGenerators()
    {
        string? pluginsPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly()?.Location);
        string absolutePath = Path.Combine(pluginsPath!, "Tapanga.Core.Generators.dll");
        var core = LoadPlugin(absolutePath);
        return CreateGenerators(core);
    }

    static Assembly LoadPlugin(string absolutePath)
    {
        Console.WriteLine($"Loading profile generators from: {absolutePath}");
        PluginLoadContext loadContext = new(absolutePath);
        return loadContext.LoadFromAssemblyPath(absolutePath);
    }

    static IEnumerable<IProfileGenerator> CreateGenerators(Assembly assembly)
    {
        int count = 0;

        foreach (Type type in assembly.GetTypes())
        {
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

        if (count == 0)
        {
            string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
            throw new ApplicationException(
                $"Can't find any type which implements IProfileGenerator in {assembly} from {assembly.Location}.\n" +
                $"Available types: {availableTypes}");
        }
    }
}
