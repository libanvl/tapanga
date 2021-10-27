namespace Tapanga.Plugin
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ProfileGeneratorAttribute : Attribute
    {
        public ProfileGeneratorAttribute(string key, string version)
        {
            if (key.Any(c => char.IsWhiteSpace(c)))
            {
                throw new ArgumentException(
                    "The Generator key must not include any whitespace characters.",
                    nameof(key));
            }

            Key = key;
            Version = Version.Parse(version);
        }

        public string Key { get; }

        public Version Version { get; }
    }
}
