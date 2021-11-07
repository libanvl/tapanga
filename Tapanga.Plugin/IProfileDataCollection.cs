
namespace Tapanga.Plugin;

/// <summary>
/// A collection of <see cref="ProfileData"/> records.
/// </summary>
public interface IProfileDataCollection
{
    /// <summary>
    /// The number of items in the collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Adds <paramref name="item"/> to the collection.
    /// </summary>
    /// <param name="item"></param>
    void Add(ProfileData item);

    /// <summary>
    /// Gets a value indicating whether <paramref name="item"/> is in the collection.
    /// </summary>
    /// <param name="item"></param>
    bool Contains(ProfileData item);

    /// <summary>
    /// Gets an enumerator to iterate over the collection.
    /// </summary>
    IEnumerator<ProfileData> GetEnumerator();
}
