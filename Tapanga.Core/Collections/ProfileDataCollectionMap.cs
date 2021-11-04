using System.Collections;

namespace Tapanga.Core;

public class ProfileDataCollectionMap : IEnumerable<KeyValuePair<GeneratorId, ProfileDataCollection>>
{
    private readonly Dictionary<GeneratorId, ProfileDataCollection> _inner = new();

    public ProfileDataCollection this[GeneratorId key]
    {
        get
        {
            if (!_inner.TryGetValue(key, out var dataCollection))
            {
                dataCollection = new ProfileDataCollection();
                _inner.Add(key, dataCollection);
            }

            return dataCollection;
        }
    }

    public IEnumerator<KeyValuePair<GeneratorId, ProfileDataCollection>> GetEnumerator()
    {
        return _inner.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_inner).GetEnumerator();
    }
}
