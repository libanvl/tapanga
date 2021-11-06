using libanvl;
using System.ComponentModel;
using Tapanga.Plugin;

namespace Tapanga.Core.Generators;

[ProfileGenerator("core.demo", "0.1")]
[Description("A generator used for demonstrating delegate generator features.")]
public class TestGenerator : DelegateGenerator<TestGenerator.Arguments>
{
    public class Arguments : CommonArguments
    {
        [UserArgument("How do you want to greet the user?", ShortName = "g", IsRequired = true)]
        [DefaultValue("Hey!")]
        public string? Greeting { get; set; }
    }

    public override GeneratorInfo GeneratorInfo { get; } = new GeneratorInfo
    {
        "This is just a test generator.",
        "Using this is probably a bad idea.",
        string.Join("\n", typeof(TestGenerator).Assembly.GetManifestResourceNames()),
    };

    protected override int GeneratorCore(IProfileDataCollection profiles, Arguments args)
    {
        profiles.Add(new ProfileData(
            Name: NotNullOrThrow(args.ProfileName),
            CommandLine: $"echo {args.Greeting.Format()}",
            StartingDirectory: args.StartingDirectory.WrapOpt(),
            TabTitle: args.ProfileTitle.WrapOpt(whitespaceIsNone: true),
            Icon: Opt<Icon>.None));

        return 0;
    }
}
