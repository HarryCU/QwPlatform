using QwVirtualFileSystem.IO;

namespace QwVirtualFileSystem
{
    public interface ISchemeProvider
    {
        string Scheme { get; }

        IFolder CreateFolder(string uri);
    }
}
