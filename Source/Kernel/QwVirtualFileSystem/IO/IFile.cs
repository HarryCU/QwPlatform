namespace QwVirtualFileSystem.IO
{
    public interface IFile : IVFSItem
    {
        int Length { get; }

        IStreamReader OpenRead();
 #if VFS_WRITE
       IStreamWriter OpenWrite();
#endif
        }
}
