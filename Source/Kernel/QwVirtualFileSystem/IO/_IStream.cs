using System;

namespace QwVirtualFileSystem.IO
{
    public interface IStream : IDisposable
    {
        int Postion { get; set; }
        int Length { get; }
        void Close();
    }

    public interface IStreamReader : IStream
    {
        int Read(int offset, int count, out byte[] buffer);
    }

#if VFS_WRITE
    public interface IStreamWriter : IStream
    {
        int Write(byte[] buffer, int offset, int count);
    }
#endif
}
