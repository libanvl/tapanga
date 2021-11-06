using System.Collections;
using Tapanga.Plugin;

namespace Tapanga.Core;

public class ProfileDataCollection : IReadOnlyList<ProfileData>, IProfileDataCollection
{
    private readonly List<ProfileData> _inner = new();

    public ProfileData this[int index] => ((IReadOnlyList<ProfileData>)_inner)[index];

    public int Count => _inner.Count;

    public void Add(ProfileData item) => _inner.Add(item);

    public bool Contains(ProfileData item) => _inner.Contains(item);

    public IEnumerator<ProfileData> GetEnumerator() => _inner.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _inner.GetEnumerator();
}
