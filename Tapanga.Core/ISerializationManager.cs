namespace Tapanga.Core;

public interface ISerializationManager
{
    bool AddProfileData(ProfileDataCollectionMap profilesMap);

    IEnumerable<RemoveProfileResult> RemoveGeneratorProfiles(string generatorKey);

    RemoveProfileResult RemoveProfile(GeneratorId generatorId, string shortId);

    bool TryLoad(out ProfileDataExCollection profiles);

    bool Write(bool force = false);
}