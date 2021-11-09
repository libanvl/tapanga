using libanvl;
using System.ComponentModel;
using Tapanga.Plugin;

namespace Tapanga.Core.Generators;

[ProfileGenerator("core.simple", "0.1", IsEnabled = false)]
[Description("Generates simple greeter profile.")]
public class SimpleGenerator : IProfileGenerator, IProvideUserArguments
{
    public GeneratorInfo GeneratorInfo => GeneratorInfo.Empty;

    public IReadOnlyList<UserArgument> GetUserArguments() => new List<UserArgument>
        {
            new UserArgument<string>("username", "u", "The user to greet", None.String, Required: true)
        };

    public Delegate GetGeneratorDelegate(IProfileDataCollection profiles)
    {
        return (string username) =>
        {
            profiles.Add(new ProfileData(
                "Simple Profile",
                $"cmd.exe /k \"echo hello {username} && pause\"",
                new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)),
                "Simple Profile Tab",
                Opt.None<Icon>()
                ));

            return 0;
        };
    }
}
