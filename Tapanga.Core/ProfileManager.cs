using System.Text.Json;
using Tapanga.Core.Serialization;
using Tapanga.Plugin;

namespace Tapanga.Core;

public class ProfileManager
{
    // C:\Users\< user >\AppData\Local\Microsoft\Windows Terminal\Fragments\{ext}\{ file - name}.json
    private static readonly string fragmentsRootPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Microsoft",
            "Windows Terminal",
            "Fragments");

    private readonly IEnumerable<IProfileGeneratorAdapter> _generators;

    public ProfileManager(GeneratorManager generatorManager)
    {
        _generators = generatorManager.GetProfileGenerators();
    }

    public ProfileCollectionMap LoadProfiles()
    {
        var result = new ProfileCollectionMap();

        foreach (var generator in _generators)
        {
            string pluginFile = generator.GeneratorId.GetFragmentFilePath(fragmentsRootPath);

            if (File.Exists(pluginFile))
            {
                FragmentRoot root;
                using (var fileStream = File.OpenRead(pluginFile))
                {
                    root = JsonSerializer.Deserialize<FragmentRoot>(fileStream)!;
                    if (root is null || !root.Profiles.Any())
                    {
                        // TODO log info: no profiles
                        continue;
                    }
                }

                // TODO check tapanga versions are compatible

                foreach (var profile in root.Profiles)
                {
                    result[generator.GeneratorId].Add(GetProfileData(profile));
                }
            }
        }

        return result;
    }

    public void WriteProfiles(ProfileCollectionMap profilesMap)
    {
        if (!profilesMap.Any())
        {
            return;
        }

        foreach (var (generatorId, profiles) in profilesMap)
        {
            if (!profiles.Any())
            {
                continue;
            }

            string pluginPath = generatorId.GetPluginSerializationPath(fragmentsRootPath);
            Directory.CreateDirectory(pluginPath);

            string profilesFilePath = generatorId.GetFragmentFilePath(fragmentsRootPath);
            File.WriteAllText(profilesFilePath, GetFragmentProfilesJson(generatorId, profiles));
        }
    }

    private string GetFragmentProfilesJson(GeneratorId generatorId, ProfileDataCollection profiles)
    {
        List<Profile> fragmentProfiles = GetFragmentProfiles(generatorId, profiles);

        var root = new FragmentRoot
        {
            TapangaVersion = new TapangaVersion(
                typeof(IProfileGenerator).Assembly.GetName().Version ?? new Version(0, 0),
                GetType().Assembly.GetName().Version ?? new Version(0, 0))
        };

        root.Profiles.AddRange(fragmentProfiles);

        return JsonSerializer.Serialize(root, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    private static List<Profile> GetFragmentProfiles(GeneratorId generatorId, ProfileDataCollection profiles)
    {
        var fragmentProfiles = new List<Profile>();

        foreach (var pro in profiles)
        {
            var metadata = new TapangaMetadata
            {
                ProfileId = pro.GetProfileId(generatorId)
            };

            var fragmentProfile = new Profile
            {
                Commandline = pro.CommandLine,
                Name = pro.Name,
                TapangaMetadata = metadata,
            };

            if (pro.StartingDirectory is Opt<DirectoryInfo>.Some someDirectoryInfo)
            {
                fragmentProfile.StartingDirectory = someDirectoryInfo.Value.FullName;
            }

            if (pro.TabTitle is Opt<string>.Some someTabTitle)
            {
                fragmentProfile.TabTitle = someTabTitle.Value;
            }

            if (pro.Icon is Opt<Icon>.Some someIcon)
            {
                if (SerializeIcon(generatorId, someIcon) is Opt<PathIcon>.Some somePathIcon)
                {
                    fragmentProfile.Icon = somePathIcon.Value.Path;
                }
            }

            fragmentProfiles.Add(fragmentProfile);
        }

        return fragmentProfiles;
    }

    private static Opt<PathIcon> SerializeIcon(GeneratorId generatorId, Opt<Icon> optIcon)
    {
        if (optIcon is Opt<Icon>.Some someIcon)
        {
            if (someIcon.Value is PathIcon pathIcon)
            {
                return pathIcon.WrapOpt();
            }

            if (someIcon.Value is StreamIcon streamIcon)
            {
                var iconsDirectory = Directory.CreateDirectory(
                    Path.Combine(generatorId.GetPluginSerializationPath(fragmentsRootPath), "Icons"));

                string iconPath = Path.Combine(iconsDirectory.FullName, streamIcon.Name);
                if (!File.Exists(iconPath))
                {
                    using var fs = new FileStream(iconPath, FileMode.Create);
                    streamIcon.Stream.CopyTo(fs);
                }

                return new PathIcon(streamIcon.Name, iconPath).WrapOpt();
            }
        }

        return Opt.None<PathIcon>();
    }

    private static ProfileData GetProfileData(Profile profile)
    {
        return new ProfileData(
            profile.Name!,
            profile.Commandline!,
            string.IsNullOrWhiteSpace(profile.StartingDirectory) ? Opt.None<DirectoryInfo>() : new DirectoryInfo(profile.StartingDirectory),
            profile.TabTitle!,
            string.IsNullOrWhiteSpace(profile.Icon) ? Opt.None<Icon>() : new PathIcon(Path.GetFileName(profile.Icon), profile.Icon)
        );
    }
}
