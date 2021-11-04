using System.Collections;
using Tapanga.Plugin;

namespace Tapanga.Core;

public class ProfileDataCollection : IReadOnlyList<ProfileData>, IProfileDataCollection
{
    private readonly List<ProfileData> inner = new();

    public ProfileData this[int index] => ((IReadOnlyList<ProfileData>)inner)[index];

    public int Count => inner.Count;

    public void Add(ProfileData item)
    {
        if (inner.Any(pd => pd.Name == item.Name))
        {
            item = item with { Name = $"{item.Name} {Utilities.GetShortRandomId(bytes: 3)}" };
        }

        inner.Add(item);
    }

    public bool Contains(ProfileData item) => inner.Contains(item);

    public IEnumerator<ProfileData> GetEnumerator() => inner.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => inner.GetEnumerator();
}
