using Tapanga.Plugin;

namespace Tapanga.Core;

public record ProfileDataEx: ProfileData
{
    public ProfileDataEx(GeneratorId generatorId, ProfileData profileData, Guid profileId)
        : base(profileData)
    {
        GeneratorId = generatorId;
        ProfileId = profileId;
    }

    public ProfileDataEx(GeneratorId generatorId, ProfileData profile)
        : base(profile)
    {
        GeneratorId = generatorId;
        ProfileId = profile.GetProfileId(GeneratorId);
    }

    public GeneratorId GeneratorId { get; }

    public Guid ProfileId { get; }
}
