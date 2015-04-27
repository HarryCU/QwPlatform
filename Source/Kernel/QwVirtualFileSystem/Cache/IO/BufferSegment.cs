using System;

namespace QwVirtualFileSystem.Cache.IO
{
    internal class BufferSegment
    {
        private int _postions;
        private int _nextIndex = -1;
        private readonly byte[] _buffer;

        public int Length
        {
            get { return _buffer.Length; }
        }

        public bool IsFull
        {
            get { return _postions + 1 >= Length; }
        }

        public bool NeedNext
        {
            get { return NextIndex != -1; }
        }

        public int NextIndex
        {
            get { return _nextIndex; }
            set { _nextIndex = value; }
        }

        public BufferSegment(int bufferLength)
        {
            _postions = 0;
            _buffer = new byte[bufferLength];
        }

        public int TryRead(int offset, int count, ref byte[] buffer)
        {
            return TryRead(offset, count, 0, ref buffer);
        }

        public int TryRead(int offset, int count, int bufferOffset, ref byte[] buffer)
        {
            var bufferSize = count - bufferOffset;

            var lastPos = offset + bufferSize;
            var size = lastPos > Length ? Length - offset : bufferSize;
            //buffer = new byte[size];
            Buffer.BlockCopy(_buffer, offset, buffer, bufferOffset, size);
            return size;
        }

        public int TryWrite(byte[] buffer, out int postion)
        {
            return TryWrite(buffer, 0, out postion);
        }

        public int TryWrite(byte[] buffer, int offset, out int postion)
        {
            postion = -1;
            var count = buffer.Length - offset;

            var lastPos = _postions + count;
            var size = lastPos > Length ? Length - _postions : count;
            if (size <= 0)
                return -1;

            Buffer.BlockCopy(buffer, offset, _buffer, _postions, size);

            postion = _postions;
            _postions += size - 1;
            return size;
        }

        public int TryWrite(byte[] buffer, int offset, int count, int postion = 0)
        {
            var size = count - offset;
            size = postion + size >= Length ? Length - postion : size;

            Buffer.BlockCopy(buffer, offset, _buffer, postion, size);
            return size;
        }

        public override string ToString()
        {
            return string.Format("{{ Offset: {0}, Count: {1} }}", _postions, Length);
        }
    }
}
