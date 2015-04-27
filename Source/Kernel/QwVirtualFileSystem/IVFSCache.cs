using QwVirtualFileSystem.IO;

namespace QwVirtualFileSystem
{
    public interface IVFSCache
    {
        void Update(string uri, IVFSItem item);
        void Update(string uri, IVFSItem item, bool append);

        IVFSItem Delete(string uri);

        IVFSItem Query(string uri);

        void Clear();
    }
}
