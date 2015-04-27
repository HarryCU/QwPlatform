using QwVirtualFileSystem.Cache.IO;

namespace QwVirtualFileSystem.IO
{
    internal class File : AbstractVFSItem, IFile
    {
        private readonly VBufferInfo _bufferInfo;
        private readonly Folder _folder;

        public Folder Owner
        {
            get { return _folder; }
        }

        public VBufferInfo BufferInfo
        {
            get
            {
                return _bufferInfo;
            }
        }

        public File(VBufferInfo bufferInfo, string fullName, Folder folder)
            : base(fullName)
        {
            _bufferInfo = bufferInfo;
            _folder = folder;
        }

        public int Length
        {
            get { return _bufferInfo.Count; }
        }

        public IStreamReader OpenRead()
        {
            return new StreamReader(this);
        }

#if VFS_WRITE
        public IStreamWriter OpenWrite()
        {
            return new StreamWriter(this);
        }
#endif
    }
}
