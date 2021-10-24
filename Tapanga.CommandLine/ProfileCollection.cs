using System.Collections;
using Tapanga.Plugin;

namespace Tapanga.CommandLine;

public class ProfileCollection : ICollection<Profile>, IProfileCollection
{
    private readonly List<Profile> inner = new();

    public int Count => ((ICollection<Profile>)inner).Count;

    public bool IsReadOnly => ((ICollection<Profile>)inner).IsReadOnly;

    public void Add(Profile item)
    {
        ((ICollection<Profile>)inner).Add(item);
    }

    public void Clear()
    {
        ((ICollection<Profile>)inner).Clear();
    }

    public bool Contains(Profile item)
    {
        return ((ICollection<Profile>)inner).Contains(item);
    }

    public void CopyTo(Profile[] array, int arrayIndex)
    {
        ((ICollection<Profile>)inner).CopyTo(array, arrayIndex);
    }

    public IEnumerator<Profile> GetEnumerator()
    {
        return ((IEnumerable<Profile>)inner).GetEnumerator();
    }

    public bool Remove(Profile item)
    {
        return ((ICollection<Profile>)inner).Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)inner).GetEnumerator();
    }
}
