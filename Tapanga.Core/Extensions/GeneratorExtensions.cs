namespace Tapanga.Core
{
    internal static class GeneratorExtensions
    {
        public static string GetPluginSerializationName(this GeneratorId generatorId)
        {
            return $"{Constants.ExtensionName}${generatorId.AssemblyName}@{generatorId.AssemblyVersion}";
        }

        public static string GetGeneratorSerializationName(this GeneratorId generatorId)
        {
            return $"{generatorId.Key}@{generatorId.Version}";
        }

        public static string GetNamespace(this GeneratorId generatorId)
        {
            return $"{generatorId.GetPluginSerializationName()}${generatorId.GetGeneratorSerializationName()}";
        }

        public static string GetPluginSerializationPath(this GeneratorId generatorId, string rootPath)
        {
            return Path.Combine(
                rootPath,
                generatorId.GetPluginSerializationName());
        }

        public static string GetFragmentFilePath(this GeneratorId generatorId, string rootPath)
        {
            return Path.Combine(
                generatorId.GetPluginSerializationPath(rootPath),
                $"{generatorId.GetGeneratorSerializationName()}.json");
        }
    }
}
