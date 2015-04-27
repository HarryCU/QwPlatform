using System;
using QwMicroKernel;
using QwVirtualFileSystem.Cache.IO;

namespace QwVirtualFileSystem.IO
{
    internal abstract class Stream : Disposer, IStream
    {
        private int _postion;
        private readonly File _file;
        private bool _closed;

        protected VBufferInfo BufferInfo
        {
            get { return File.BufferInfo; }
        }

        protected VBuffer Buffer
        {
            get { return File.Owner.Buffer; }
        }

        public int Postion
        {
            get { return _postion; }
            set
            {
                if (_postion > Length - 1)
                    throw new IndexOutOfRangeException();
                _postion = value;
            }
        }

        public int Length
        {
            get { return _file.Length; }
        }

        protected bool IsClosed
        {
            get { return _closed; }
        }

        public File File
        {
            get { return _file; }
        }

        protected Stream(File file)
        {
            if (file == null)
                throw new ArgumentNullException("file");
            _file = file;
            Postion = 0;
            _closed = false;
        }

        public void Close()
        {
            _closed = true;
        }

        protected override void Release()
        {
            Close();
        }
    }

    internal sealed class StreamReader : Stream, IStreamReader
    {
        public StreamReader(File file)
            : base(file)
        {
        }

        public int Read(int offset, int count, out byte[] buffer)
        {
            return Buffer.TryRead(BufferInfo, out buffer, offset, count, Postion);
        }
    }

#if VFS_WRITE
    internal sealed class StreamWriter : Stream, IStreamWriter
    {
        public StreamWriter(File file)
            : base(file)
        {
        }

        public int Write(byte[] buffer, int offset, int count)
        {
            return Buffer.TryWrite(BufferInfo, buffer, offset, count, Postion);
        }
    }
#endif
}
