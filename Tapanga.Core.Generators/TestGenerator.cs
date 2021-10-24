using Tapanga.Plugin;

namespace Tapanga.Core.Generators;

public class TestGenerator : DelegateGenerator<TestGenerator.Arguments>
{
    public class Arguments : CommonArguments
    {
        public Arguments(string profileName, OptionalArgument<string> profileTitle, OptionalArgument<DirectoryInfo> starting, string greeting)
            : base(profileName, profileTitle, starting)
        {
            Greeting = greeting;
        }

        [UserArgument("How do you want to greet the user?", ShortName = "g")]
        [DefaultValueFactory(nameof(GetDefaultGreeting))]
        public string Greeting { get; set; }

        public static string GetDefaultGreeting()
        {
            return "Hey";
        }
    }

    public override GeneratorInfo GeneratorInfo { get; } = new GeneratorInfo(
        "test",
        "Test Profiles",
        new GeneratorDescription
        {
                "This is just a test generator.",
                "Using this is probably a bad idea.",
                string.Join("\n", typeof(TestGenerator).Assembly.GetManifestResourceNames()),
        });

    protected override int GeneratorCore(IProfileCollection profiles, Arguments args)
    {
        profiles.Add(new Profile(
            Name: args.ProfileName,
            CommandLine: $"echo {args.Greeting.FormatParameter()}",
            StartingDirectory: args.StartingDirectory.AsOpt(),
            TabTitle: args.ProfileTitle.AsOpt(),
            Icon: Opt.None<Icon>()
            ));

        return 0;
    }
}
