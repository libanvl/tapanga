using System.Collections;
using Tapanga.Plugin;

namespace Tapanga.Core;

public class ProfileDataExCollection : ICollection<ProfileDataEx>
{
    private readonly List<ProfileDataEx> _inner = new();

    public static ProfileDataExCollection Empty { get; } = new ProfileDataExCollection();

    public bool IsReadOnly => ((ICollection<ProfileDataEx>)_inner).IsReadOnly;

    public int Count => ((ICollection<ProfileDataEx>)_inner).Count;

    public void Add(ProfileDataEx item) => _inner.Add(item);

    public void Add(GeneratorId generatorId, ProfileData profile)
    {
        if (_inner.Any(pd => pd.Name == profile.Name))
        {
            profile = profile with { Name = $"{profile.Name} {Utilities.GetShortRandomId(bytes: 3)}" };
        }

        _inner.Add(new ProfileDataEx(generatorId, profile));
    }

    public void AddRange(GeneratorId generatorId, IEnumerable<ProfileData> profiles)
    {
        foreach (var profile in profiles)
        {
            Add(generatorId, profile);
        }
    }

    public void Clear() => _inner.Clear();

    public bool Contains(ProfileDataEx item) => _inner.Contains(item);

    public void CopyTo(ProfileDataEx[] array, int arrayIndex)
    {
        ((ICollection<ProfileDataEx>)_inner).CopyTo(array, arrayIndex);
    }

    public IEnumerator<ProfileDataEx> GetEnumerator() => _inner.GetEnumerator();

    public bool Remove(ProfileDataEx item) => _inner.Remove(item);

    IEnumerator IEnumerable.GetEnumerator() => _inner.GetEnumerator();
}
