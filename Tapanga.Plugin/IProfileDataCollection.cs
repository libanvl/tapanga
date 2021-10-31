
namespace Tapanga.Plugin
{
    /// <summary>
    /// A collection of <see cref="ProfileData"/> records.
    /// </summary>
    public interface IProfileDataCollection
    {
        int Count { get; }

        /// <summary>
        /// Adds <paramref name="item"/> to the collection. If the collection contains an item with the same <see cref="ProfileData.Name,"/>
        /// a short random string will be appended to the name.
        /// </summary>
        /// <param name="item"></param>
        void Add(ProfileData item);

        bool Contains(ProfileData item);

        IEnumerator<ProfileData> GetEnumerator();
    }
}