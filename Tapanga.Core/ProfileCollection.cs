using System.Collections;
using Tapanga.Plugin;

namespace Tapanga.Core;

public class ProfileCollection : ICollection<ProfileData>, IProfileCollection
{
    private readonly List<ProfileData> inner = new();

    public int Count => ((ICollection<ProfileData>)inner).Count;

    public bool IsReadOnly => ((ICollection<ProfileData>)inner).IsReadOnly;

    public void Add(ProfileData item)
    {
        ((ICollection<ProfileData>)inner).Add(item);
    }

    public void Clear()
    {
        ((ICollection<ProfileData>)inner).Clear();
    }

    public bool Contains(ProfileData item)
    {
        return ((ICollection<ProfileData>)inner).Contains(item);
    }

    public void CopyTo(ProfileData[] array, int arrayIndex)
    {
        ((ICollection<ProfileData>)inner).CopyTo(array, arrayIndex);
    }

    public IEnumerator<ProfileData> GetEnumerator()
    {
        return ((IEnumerable<ProfileData>)inner).GetEnumerator();
    }

    public bool Remove(ProfileData item)
    {
        return ((ICollection<ProfileData>)inner).Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)inner).GetEnumerator();
    }
}
