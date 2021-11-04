using libanvl;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using Tapanga.Plugin;

namespace Tapanga.Core.Generators;

[ProfileGenerator("core.ssh", "0.1")]
[Description("Generates SSH connection profiles.")]
public class SecureShellGenerator : DelegateGenerator<SecureShellGenerator.Arguments>
{
    public class Arguments : CommonArguments
    {
        [UserArgument("Path to the SSH client", ShortName = "s", IsRequired = true)]
        [DefaultValueFactory(nameof(GetSshExeDefault))]
        public FileInfo? SshExe { get; set; }

        [UserArgument("Enable verbose output for SSH client", ShortName = "v")]
        [DefaultValue(false)]
        public bool Verbose { get; set; }

        [UserArgument("Additional options for the SSH client", ShortName = "o")]
        public string? Opts { get; set; }

        [UserArgument("SSH destination", ShortName = "d", IsRequired = true, Sort = -1)]
        public Uri? Destination { get; set; }

        [UserArgument("Command to execute on the SSH destination", ShortName = "c")]
        public string? Command { get; set; }

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

    public override GeneratorInfo GeneratorInfo => GeneratorInfo.Empty;

    protected override int GeneratorCore(IProfileDataCollection profiles, Arguments args)
    {
        const string sshResourceName = "Tapanga.Core.Generators.Resources.ssh.png";

        Opt<Icon> icon = Assembly.GetExecutingAssembly().GetOptIcon(sshResourceName);

        ProfileData profile = new(
            NotNullOrThrow(args.ProfileName),
            $"{args.SshExe}{args.Verbose.FormatParameter("-v")}{args.Opts.FormatParameter()}{NotNullOrThrow(args.Destination).ToString().FormatParameter()}{args.Command.FormatParameter()}",
            args.StartingDirectory.WrapOpt(),
            args.ProfileTitle.WrapOpt(whitespaceIsNone: true),
            icon);

        profiles.Add(profile);

        return 0;
    }
}
