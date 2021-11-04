using libanvl;
using System.Text.Json;
using Tapanga.Core.Serialization;
using Tapanga.Plugin;

namespace Tapanga.Core;

public class ProfileManager
{
    private readonly IEnumerable<IProfileGeneratorAdapter> _generators;

    private Opt<ProfileDataExCollection> _cachedProfileData = Opt<ProfileDataExCollection>.None;

    private bool _isDirty;

    public ProfileManager(GeneratorManager generatorManager)
    {
        _generators = generatorManager.GetProfileGenerators();
    }

    public enum RemoveProfileResult
    {
        OK,
        NoMatchingProfile,
        MultipleProfiles,
        DataLoadError,
        Failed,
    }

    public bool Load()
    {
        if (_cachedProfileData.IsNone)
        {
            var data = new ProfileDataExCollection();

            foreach (var gen in _generators)
            {
                var fragmentPath = gen.GeneratorId.GetFragmentFilePath(Constants.FragmentsRootPath);
                if (!File.Exists(fragmentPath))
                {
                    continue;
                }

                var optFragmentRoot = JsonSerializer
                    .Deserialize<FragmentRoot>(File.ReadAllText(fragmentPath))
                    .WrapOpt();

                if (optFragmentRoot is Opt<FragmentRoot>.Some someFragmentRoot)
                {
                    data.AddRange(gen.GeneratorId, someFragmentRoot.Value.Profiles.Select(p => GetProfileData(p)));
                }
                else
                {
                    // TODO log no profiles
                }
            }

            _cachedProfileData = data.WrapOpt();
        }

        return true;
    }

    public IEnumerable<ProfileDataEx> GetProfiles()
    {
        Load();
        return _cachedProfileData.SomeOrEmpty();
    }

    public bool Write(bool force = false)
    {
        if (!_isDirty && !force)
        {
            return true;
        }

        if (_cachedProfileData is Opt<ProfileDataExCollection>.Some someCollection)
        {
            var collection = someCollection.Value;
            var generatorLookup = collection.ToLookup(pdx => pdx.GeneratorId);

            foreach (var profileGroup in generatorLookup)
            {
                // TODO remove empty generator fragment
                // TODO track dirty per generator
                Write(profileGroup.Key, profileGroup);
            }

            return true;
        }

        return false;
    }

    public bool AddProfileData(GeneratorId generatorId, ProfileDataCollection profiles)
    {
        if (Load())
        {
            if (_cachedProfileData is Opt<ProfileDataExCollection>.Some someCollection)
            {
                if (profiles.Any())
                {
                    _isDirty = true;
                    someCollection.Value.AddRange(generatorId, profiles);
                }
                return true;
            }
        }

        return false;
    }

    public RemoveProfileResult RemoveProfile(string shortId)
    {
        if (Load())
        {
            if (_cachedProfileData is Opt<ProfileDataExCollection>.Some someCollection)
            {
                var collection = someCollection.Value;
                var matchingProfiles = collection
                    .Where(pdx => pdx.ProfileId.ToString().StartsWith(shortId));

                if (!matchingProfiles.Any())
                {
                    return RemoveProfileResult.NoMatchingProfile;
                }

                if (matchingProfiles.SingleOrDefault() is ProfileDataEx pdx)
                {
                    _isDirty = true;
                    return collection.Remove(pdx)
                        ? RemoveProfileResult.OK
                        : RemoveProfileResult.Failed;
                }
                else
                {
                    return RemoveProfileResult.MultipleProfiles;
                }
            }
        }

        return RemoveProfileResult.DataLoadError;
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

    private void Write(GeneratorId key, IEnumerable<ProfileDataEx> profiles)
    {
        Write(key, GetFragmentRoot(GetFragmentProfiles(key, profiles)));
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

    private static void Overwrite(string filePath, string json)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        File.WriteAllText(filePath, json);
    }

    private static ProfileData GetProfileData(Profile profile)
    {
        return new ProfileData(
            profile.Name!,
            profile.Commandline!,
            profile.StartingDirectory.WrapOpt(x => new DirectoryInfo(x!)),
            profile.TabTitle.WrapOpt(),
            profile.Icon!.WrapOpt<string, Icon>(s => new PathIcon(Path.GetFileName(s), s)));
    }
}
