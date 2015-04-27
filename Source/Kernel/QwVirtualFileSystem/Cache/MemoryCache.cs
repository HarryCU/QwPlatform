using QwMicroKernel.Collections;
using QwVirtualFileSystem.IO;

namespace QwVirtualFileSystem.Cache
{
    public class MemoryCache : IVFSCache
    {
        private readonly QwConcurrent<string, IVFSItem> _data = new QwConcurrent<string, IVFSItem>();

        protected ICache<string, IVFSItem> Data
        {
            get { return _data; }
        }

        public void Update(string uri, IVFSItem item)
        {
            Update(uri, item, true);
        }

        public void Update(string uri, IVFSItem item, bool append)
        {
            if (Data.Has(uri) && !append)
                throw new VFSRuntimeException("已存在相同的Key({0}).", uri);

            Data[uri] = item;
        }

        public IVFSItem Delete(string uri)
        {
            var item = Data[uri];
            if (item != null)
            {
                Data.Remove(uri);
            }
            return item;
        }

        public IVFSItem Query(string uri)
        {
            return Data[uri];
        }

        public void Clear()
        {
            Data.Clear();
        }
    }
}
