using libanvl;
using System.Text.Json;
using Tapanga.Core.Serialization;
using Tapanga.Plugin;

namespace Tapanga.Core;

internal class GeneratorReaderWriter
{
    private readonly GeneratorId _genereratorId;

    private GeneratorReaderWriter(GeneratorId generatorId)
    {
        _genereratorId = generatorId;
    }

    public static Func<GeneratorId, GeneratorReaderWriter> Factory => FactoryImpl.Create;

    public Opt<FragmentRoot> Read()
    {
        var fragmentPath = _genereratorId.GetFragmentFilePath(Constants.FragmentsRootPath);
        if (!File.Exists(fragmentPath))
        {
            return Opt<FragmentRoot>.None;
        }

        return JsonSerializer
            .Deserialize<FragmentRoot>(File.ReadAllText(fragmentPath))
            .WrapOpt();
    }

    public void WriteContent(string name, StringCollection content)
    {
        string pluginPath = _genereratorId.GetPluginSerializationDirectoryPath(Constants.FragmentsRootPath);
        Directory.CreateDirectory(pluginPath);

        Overwrite(Path.Combine(pluginPath, name), content);
    }

    public void Write(IEnumerable<ProfileDataEx> profiles)
    {
        Write(_genereratorId, GetFragmentRoot(GetFragmentProfiles(_genereratorId, profiles)));
    }

    private static void Write(GeneratorId key, FragmentRoot fragmentRoot)
    {
        string pluginPath = key.GetPluginSerializationDirectoryPath(Constants.FragmentsRootPath);
        Directory.CreateDirectory(pluginPath);

        string profilesFilePath = key.GetFragmentFilePath(Constants.FragmentsRootPath);

        Overwrite(
            profilesFilePath,
            JsonSerializer.Serialize(fragmentRoot, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
    }

    private static void Overwrite(string filePath, string content)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        File.WriteAllText(filePath, content);
    }

    private static IEnumerable<Profile> GetFragmentProfiles(GeneratorId generatorId, IEnumerable<ProfileData> profiles)
    {
        return profiles.Select(pd => GetFragmentProfile(generatorId, pd));
    }

    private static Profile GetFragmentProfile(GeneratorId generatorId, ProfileData pro)
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

        if (pro.StartingDirectory.Select(di => di.FullName) is Opt<string>.Some someDirectory)
        {
            fragmentProfile.StartingDirectory = someDirectory.Value;
        }

        if (pro.TabTitle is Opt<string>.Some someTabTitle)
        {
            fragmentProfile.TabTitle = someTabTitle.Value;
        }

        if (pro.Icon is Opt<Icon>.Some someIcon)
        {
            if (SerializeIcon(generatorId, someIcon).Select(i => i.Path) is Opt<string>.Some someIconPath)
            {
                fragmentProfile.Icon = someIconPath.Value;
            }
        }

        return fragmentProfile;
    }

    private static Opt<PathIcon> SerializeIcon(GeneratorId generatorId, Opt<Icon> optIcon)
    {
        if (optIcon is Opt<Icon>.Some someIcon)
        {
            if (someIcon.Value is PathIcon pathIcon)
            {
                return pathIcon;
            }

            if (someIcon.Value is StreamIcon streamIcon)
            {
                var iconsDirectory = Directory.CreateDirectory(
                    Path.Combine(generatorId.GetPluginSerializationDirectoryPath(Constants.FragmentsRootPath), "Icons"));

                string iconPath = Path.Combine(iconsDirectory.FullName, streamIcon.Name);
                if (!File.Exists(iconPath))
                {
                    using var fs = new FileStream(iconPath, FileMode.Create);
                    streamIcon.Stream.CopyTo(fs);
                }

                return new PathIcon(streamIcon.Name, iconPath);
            }
        }

        return Opt<PathIcon>.None;
    }

    private FragmentRoot GetFragmentRoot(IEnumerable<Profile> profiles)
    {
        var root = new FragmentRoot
        {
            TapangaVersion = new TapangaVersion(
                typeof(IProfileGenerator).Assembly.GetName().Version ?? new Version(0, 0),
                GetType().Assembly.GetName().Version ?? new Version(0, 0))
        };

        root.Profiles.AddRange(profiles);
        return root;
    }

    private static class FactoryImpl
    {
        private static readonly Dictionary<GeneratorId, GeneratorReaderWriter> _cache = new();

        public static GeneratorReaderWriter Create(GeneratorId generatorId)
        {
            if (!_cache.TryGetValue(generatorId, out var result))
            {
                result = new GeneratorReaderWriter(generatorId);
                _cache.Add(generatorId, result);
            }

            return result;
        }
    }
}
