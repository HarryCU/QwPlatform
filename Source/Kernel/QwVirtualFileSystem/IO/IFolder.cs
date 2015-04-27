using QwVirtualFileSystem.Collections;

namespace QwVirtualFileSystem.IO
{
    public interface IFolder : IVFSItem
    {
        IFolderArray SubFolders { get; }
        IFileArray Files { get; }

        IFile SearchFile(string fileName);
        IStreamReader OpenRead(string fileName);
    }
}
