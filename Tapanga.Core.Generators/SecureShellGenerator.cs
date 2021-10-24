using System.ComponentModel;
using System.Diagnostics;
using Tapanga.Plugin;

namespace Tapanga.Core.Generators;

public class SecureShellGenerator : DelegateGenerator<SecureShellGenerator.Arguments>
{
    public class Arguments : CommonArguments
    {
        public Arguments(
            string profileName,
            OptionalArgument<string> profileTitle,
            OptionalArgument<DirectoryInfo> startingDirectory,
            FileInfo sshExe,
            OptionalArgument<bool> verbose,
            OptionalArgument<string> opts,
            Uri destination,
            OptionalArgument<string> command)
            : base(profileName, profileTitle, startingDirectory)
        {
            SshExe = sshExe;
            Verbose = verbose;
            Opts = opts;
            Destination = destination;
            Command = command;
        }

        [UserArgument("Path to the SSH client", ShortName = "s")]
        [DefaultValueFactory(nameof(GetSshExeDefault))]
        public FileInfo SshExe { get; }

        [UserArgument("Enable verbose output for SSH client", ShortName = "v")]
        [DefaultValue(false)]
        public OptionalArgument<bool> Verbose { get; }

        [UserArgument("Additional options for the SSH client", ShortName = "o")]
        public OptionalArgument<string> Opts { get; }

        [UserArgument("SSH destination", ShortName = "d", Sort = -1)]
        public Uri Destination { get; }

        [UserArgument("Command to execute on the SSH destination", ShortName = "c")]
        public OptionalArgument<string> Command { get; }

        public static FileInfo GetSshExeDefault()
        {
            var sshInfo = new ProcessStartInfo
            {
                FileName = "ssh.exe",
                UseShellExecute = true
            };

            string sshProcessPath = "ssh.exe";

            var sshProcess = Process.Start(sshInfo);
            if (sshProcess is not null)
            {
                using (sshProcess)
                {
                    sshProcessPath = sshProcess.MainModule?.FileName ?? sshProcessPath;
                    sshProcess.Kill(entireProcessTree: true);
                }
            }

            return new FileInfo(sshProcessPath);
        }
    }

    public override GeneratorInfo GeneratorInfo { get; } = new GeneratorInfo(
        "ssh",
        "SSH Connection Profiles",
        Opt.None<GeneratorDescription>());

    protected override int GeneratorCore(IProfileCollection profiles, Arguments args)
    {
        const string sshResourceName = "Tapanga.Core.Generators.Resources.ssh.png";

        var assembly = typeof(SecureShellGenerator).Assembly;
        var resource = assembly.GetManifestResourceStream(sshResourceName);

        Opt<Icon> icon = resource is null
            ? Opt.None<Icon>()
            : Opt.Some<Icon>(new Icon(sshResourceName, resource));

        Profile profile = new(
            args.ProfileName,
            $"{args.SshExe}{args.Verbose.FormatParameter("-v")}{args.Opts.FormatParameter()}{args.Destination.ToString().FormatParameter()}{args.Command.FormatParameter()}",
            args.StartingDirectory,
            args.ProfileTitle,
            icon);

        profiles.Add(profile);

        return 0;
    }
}
