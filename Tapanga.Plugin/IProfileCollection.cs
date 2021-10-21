
namespace Tapanga.Plugin
{
    public interface IProfileCollection
    {
        int Count { get; }

        bool IsReadOnly { get; }

        void Add(Profile item);

        bool Contains(Profile item);

        IEnumerator<Profile> GetEnumerator();

        bool Remove(Profile item);
    }
}