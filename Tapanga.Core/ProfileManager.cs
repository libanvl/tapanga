using Microsoft;
using System.Text.Json;
using Tapanga.Core.Serialization;
using Tapanga.Plugin;

namespace Tapanga.Core;

public class ProfileManager
{
    private const string profilesJsonFile = "tapanga.json";

    // C:\Users\< user >\AppData\Local\Microsoft\Windows Terminal\Fragments\{ext}\{ file - name}.json
    private static readonly string fragmentsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Microsoft",
            "Windows Terminal",
            "Fragments",
            "Tapanga");

    public IDictionary<GeneratorId, ProfileDataCollection> LoadProfiles()
    {
        var root = new FragmentRoot();

        string filepath = Path.Combine(fragmentsPath, profilesJsonFile);
        if (File.Exists(filepath))
        {
            using (var fileStream = File.OpenRead(filepath))
            {
                root = JsonSerializer.Deserialize<FragmentRoot>(fileStream)!;
                // better null handling
                Assumes.NotNull(root);
            }

            // check tapanga versions are compatible
        }

        var result = new Dictionary<GeneratorId, ProfileDataCollection>();

        foreach (var profile in root.Profiles)
        {
            if (!result.TryGetValue(profile.TapangaMetadata.GeneratorId!, out var dataCollection))
            {
                dataCollection = new ProfileDataCollection();
                result[profile.TapangaMetadata.GeneratorId!] = dataCollection;
            }

            dataCollection.Add(profile.AsProfileData());
        }

        return result;
    }

    public void WriteProfiles(IDictionary<GeneratorId, ProfileDataCollection> profilesMap)
    {
        if (profilesMap.Count < 1)
        {
            return;
        }

        var fragmentProfiles = new List<Profile>();
        foreach (var (generatorId, profiles) in profilesMap)
        {
            foreach (var pro in profiles)
            {
                var metadata = new TapangaMetadata
                {
                    ProfileId = pro.GetProfileId(),
                    GeneratorId = generatorId
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
        }

        var profilePathInfo = Directory.CreateDirectory(fragmentsPath);
        var root = new FragmentRoot();
        string filepath = Path.Combine(profilePathInfo.FullName, profilesJsonFile);

        root.TapangaVersion = new TapangaVersion(
            typeof(IProfileGenerator).Assembly.GetName().Version ?? new Version(0, 0),
            GetType().Assembly.GetName().Version ?? new Version(0, 0));

        root.Profiles.AddRange(fragmentProfiles);

        var data = JsonSerializer.Serialize(root, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(filepath, data);
    }

    private Opt<PathIcon> SerializeIcon(GeneratorId generatorId, Opt<Icon> optIcon)
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
                    Path.Combine(fragmentsPath, "Icons", $"{generatorId.AssemblyName}.{generatorId.AssemblyVersion}"));

                string iconPath = Path.Combine(iconsDirectory.FullName, streamIcon.Name);
                if (File.Exists(iconPath))
                {
                    using var fs = new FileStream(iconPath, FileMode.Create);
                    streamIcon.Stream.CopyTo(fs);
                }

                return new PathIcon(streamIcon.Name, iconPath).WrapOpt();
            }
        }

        return Opt.None<PathIcon>();
    }
}
