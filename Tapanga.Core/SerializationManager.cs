using libanvl;
using Microsoft;
using Tapanga.Core.Serialization;
using Tapanga.Plugin;

namespace Tapanga.Core;

public class SerializationManager
{
    private readonly IEnumerable<IProfileGeneratorAdapter> _generators;
    private readonly Dictionary<GeneratorId, bool> _isDirtyMap;

    private Opt<ProfileDataExCollection> _cachedProfileData = Opt<ProfileDataExCollection>.None;

    public SerializationManager(GeneratorManager generatorManager)
    {
        _generators = generatorManager.GetProfileGenerators();
        _isDirtyMap = _generators
            .ToDictionary(pga => pga.GeneratorId, _ => false);
    }

    public enum RemoveProfileResult
    {
        OK,
        NoMatchingProfile,
        MultipleProfiles,
        DataLoadError,
        Failed,
    }

    public bool TryLoad(out ProfileDataExCollection profiles)
    {
        if (_cachedProfileData.IsNone)
        {
            try
            {
                var data = new ProfileDataExCollection();

                foreach (var gen in _generators)
                {
                    var optFragmentRoot = GeneratorReaderWriter
                        .Factory(gen.GeneratorId)
                        .Read();

                    if (optFragmentRoot is Opt<FragmentRoot>.Some someFragmentRoot)
                    {
                        data.AddRange(
                            gen.GeneratorId,
                            someFragmentRoot.Value.Profiles.Select(p => GetProfileData(p)));
                    }
                    else
                    {
                        // TODO log no profiles
                    }
                }

                _cachedProfileData = data.WrapOpt();
            }
            catch
            {
                profiles = ProfileDataExCollection.Empty;
                return false;
            }
        }

        profiles = _cachedProfileData.Unwrap();
        return true;
    }

    public bool Write(bool force = false)
    {
        if (!_isDirtyMap.Values.Any(isDirty => isDirty) && !force)
        {
            return true;
        }

        if (_cachedProfileData is Opt<ProfileDataExCollection>.Some someCollection)
        {
            var collection = someCollection.Value;
            var generatorLookup = collection.ToLookup(pdx => pdx.GeneratorId);

            var dirtyGeneratorIds = _isDirtyMap
                .Where(pair => pair.Value)
                .Select(pair => pair.Key);

            foreach (var generatorId in dirtyGeneratorIds)
            {
                GeneratorReaderWriter
                    .Factory(generatorId)
                    .Write(generatorLookup[generatorId]);
            }

            return true;
        }

        return false;
    }

    public bool AddProfileData(ProfileDataCollectionMap profilesMap)
    {
        var results = new List<bool>();
        foreach (var (generatorId, profileCollection) in profilesMap)
        {
            results.Add(AddProfileData(generatorId, profileCollection));
        }

        return results.All(r => r);
    }

    public bool AddProfileData(GeneratorId generatorId, ProfileDataCollection profiles)
    {
        if (TryLoad(out var result))
        {
            if (profiles.Any())
            {
                Assumes.True(_isDirtyMap.ContainsKey(generatorId));

                _isDirtyMap[generatorId] = true;
                result.AddRange(generatorId, profiles);
            }

            return true;
        }

        return false;
    }

    public RemoveProfileResult RemoveProfile(string shortId)
    {
        if (TryLoad(out var collection))
        {
            var matchingProfiles = collection
                .Where(pdx => pdx.ProfileId.ToString().StartsWith(shortId));

            if (!matchingProfiles.Any())
            {
                return RemoveProfileResult.NoMatchingProfile;
            }

            if (matchingProfiles.SingleOrDefault() is ProfileDataEx pdx)
            {
                Assumes.True(_isDirtyMap.ContainsKey(pdx.GeneratorId));
                _isDirtyMap[pdx.GeneratorId] = true;

                return collection.Remove(pdx)
                    ? RemoveProfileResult.OK
                    : RemoveProfileResult.Failed;
            }
            else
            {
                return RemoveProfileResult.MultipleProfiles;
            }
        }

        return RemoveProfileResult.DataLoadError;
    }

    public IEnumerable<RemoveProfileResult> RemoveGeneratorProfiles(string generatorKey)
    {
        if (TryLoad(out var collection))
        {
            var matchingProfiles = collection
                .Where(pdx => pdx.GeneratorId.Key.Equals(generatorKey, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (!matchingProfiles.Any())
            {
                return new[] { RemoveProfileResult.NoMatchingProfile };
            }


            var results = new List<RemoveProfileResult>();
            foreach (var profile in matchingProfiles)
            {
                Assumes.True(_isDirtyMap.ContainsKey(profile.GeneratorId));
                _isDirtyMap[profile.GeneratorId] = true;

                results.Add(collection.Remove(profile)
                    ? RemoveProfileResult.OK
                    : RemoveProfileResult.Failed);
            }

            return results;
        }

        return new[] { RemoveProfileResult.DataLoadError };
    }

    private static ProfileData GetProfileData(Profile profile)
    {
        return new ProfileData(
            profile.Name!,
            profile.Commandline!,
            profile.StartingDirectory.WrapOpt(x => new DirectoryInfo(x!)),
            profile.TabTitle.WrapOpt(),
            profile.Icon.WrapOpt<string, Icon>(s => new PathIcon(Path.GetFileName(s), s)));
    }
}
