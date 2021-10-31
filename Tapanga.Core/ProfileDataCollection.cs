using System.Collections;
using Tapanga.Plugin;

namespace Tapanga.Core;

public class ProfileDataCollection : ICollection<ProfileData>, IProfileDataCollection
{
    private readonly List<ProfileData> inner = new();

    public int Count => inner.Count;

    public bool IsReadOnly => ((ICollection<ProfileData>)inner).IsReadOnly;

    public void Add(ProfileData item)
    {
        if (inner.Any(pd => pd.Name == item.Name))
        {
            item = item with { Name = $"{item.Name} {Utilities.GetShortRandomId(bytes: 3)}" };
        }

        inner.Add(item);
    }

    public void Clear() => inner.Clear();

    public bool Contains(ProfileData item) => inner.Contains(item);

    public void CopyTo(ProfileData[] array, int arrayIndex) => inner.CopyTo(array, arrayIndex);

    public IEnumerator<ProfileData> GetEnumerator() => inner.GetEnumerator();

    public bool Remove(ProfileData item) => inner.Remove(item);

    IEnumerator IEnumerable.GetEnumerator() => inner.GetEnumerator();
}
