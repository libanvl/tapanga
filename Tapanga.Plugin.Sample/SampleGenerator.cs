using libanvl;
using System.ComponentModel;

namespace Tapanga.Plugin.Sample;

[ProfileGenerator("tapanga.sample", "0.1")]
[Description("Test platform for tapanga features")]
public class SampleDelegateGenerator : DelegateGenerator<CommonArguments>
{
    private readonly GeneratorContext _context;

    public SampleDelegateGenerator(GeneratorContext context)
    {
        _context = context;
    }

    public override StringCollection GeneratorInfo { get; } = StringCollection.Empty;

    protected override int GeneratorCore(IProfileDataCollection profiles, CommonArguments args)
    {
        var script = new StringCollection
        {
            @"set SAMPLE_VAR=""X:\HELLO"""
        };

        _context.ContextFileWriter(this, "myscript.cmd", script);

        profiles.Add(new ProfileData(
            NotNullOrThrow(args.ProfileName),
            $@"cmd.exe /k @{_context.ContextPathCombine(this, "myscript.cmd").Format()}",
            args.StartingDirectory.WrapOpt(),
            args.ProfileTitle.WrapOpt(),
            Opt<Icon>.None));

        return 0;
    }
}
