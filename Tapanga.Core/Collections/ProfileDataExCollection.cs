using System.Collections;
using Tapanga.Plugin;

namespace Tapanga.Core;

internal class ProfileDataExCollection : ICollection<ProfileDataEx>
{
    private readonly List<ProfileDataEx> _inner = new();

    public int Count => _inner.Count;

    public bool IsReadOnly => ((ICollection<ProfileDataEx>)_inner).IsReadOnly;

    public void Add(ProfileDataEx item) => _inner.Add(item);

    public void Add(GeneratorId generatorId, ProfileData profile) => 
        _inner.Add(new ProfileDataEx(generatorId, profile));

    public void AddRange(GeneratorId generatorId, IEnumerable<ProfileData> profiles) =>
        _inner.AddRange(profiles.Select(pd => new ProfileDataEx(generatorId, pd)));

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
