
namespace Tapanga.Plugin
{
    public interface IProfileCollection
    {
        int Count { get; }

        void Add(ProfileData item);

        bool Contains(ProfileData item);

        IEnumerator<ProfileData> GetEnumerator();
    }
}