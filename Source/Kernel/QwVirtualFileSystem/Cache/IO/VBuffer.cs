using System;
using System.Collections.Generic;

namespace QwVirtualFileSystem.Cache.IO
{
    internal sealed class VBuffer : Dictionary<int, BufferSegment>
    {
        // 5M size buffer segment
        public const int DefautlSegmentSize = 50;//5 * 1024 * 1024;
        private int _nextSegmentIndex = 0;

        private int _segmentSize;

        public BufferSegment Last
        {
            get { return this[Count - 1]; }
        }

        public int TryRead(VBufferInfo bufferInfo, out byte[] buffer, int offset, int count, int postion = 0)
        {
            buffer = new byte[count];
            if (bufferInfo == null)
                return -1;

            // 检测是否在范围之内
            var len = buffer.Length - offset;
            var size = len >= count ? count : len;
            var trueTotal = size;

            // 剩下的数量
            var lastCount = bufferInfo.Count - postion;
            trueTotal = lastCount >= trueTotal ? trueTotal : lastCount;

            int currentSegmentOffset;
            var index = SearchIndex(bufferInfo, postion, out currentSegmentOffset);

            var segment = this[index];
            size = segment.TryRead(offset, trueTotal, bufferInfo.Offset, ref buffer);
            size = ReadNext(segment, buffer, trueTotal, size);

            return size;
        }

#if VFS_WRITE
        public int TryWrite(VBufferInfo bufferInfo, byte[] buffer, int offset, int count, int postion = 0)
        {
            // 检测是否在范围之内
            var len = buffer.Length - offset;
            var size = len >= count ? count : len;
            var tr = size;

            // 剩下的数量
            var lastCount = bufferInfo.Count - postion;
            tr = lastCount >= tr ? tr : lastCount;

            int currentSegmentOffset;
            var index = SearchIndex(bufferInfo, postion, out currentSegmentOffset);

            var segment = this[index];

            var writeSize = segment.TryWrite(buffer, offset, tr, currentSegmentOffset);
            int lastSize = -1;
            if (tr != writeSize)
            {
                lastCount = tr - writeSize;
                while (lastCount > 0 && segment.NeedNext)
                {
                    segment = this[segment.NextIndex];
                    lastSize = segment.TryWrite(buffer, 0, lastCount);
                    lastCount -= lastSize;
                }
            }
            // 将剩余的填入 0
            if (lastCount > 0 && lastSize != -1)
            {
                var zBuffer = new byte[lastCount];
                segment.TryWrite(zBuffer, 0, lastCount, lastSize);
            }
            return tr;
        }
#endif

        private int SearchIndex(VBufferInfo bufferInfo, int postion, out int currentSegmentOffset)
        {
            currentSegmentOffset = 0;
            var index = bufferInfo.SegmentIndex;
            if (postion != 0)
            {
                postion += bufferInfo.Offset;
                var segment = this[index];

                while (postion > segment.Length && segment.NeedNext)
                {
                    //剔除已经过去的
                    postion -= segment.Length;
                    segment = this[segment.NextIndex];
                }
                currentSegmentOffset = postion;
            }
            return index;
        }

        public byte[] ReadBytes(VBufferInfo bufferInfo)
        {
            if (bufferInfo == null)
                return null;

            var buffer = new byte[bufferInfo.Count];

            var segment = this[bufferInfo.SegmentIndex];
            var size = segment.TryRead(bufferInfo.Offset, buffer.Length, ref buffer);
            ReadNext(segment, buffer, buffer.Length, size);

            return buffer;
        }

        private int ReadNext(BufferSegment current, byte[] buffer, int count, int currentSize)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (!current.NeedNext)
                return currentSize;
            var segment = this[current.NextIndex];
            var size = segment.TryRead(0, count, currentSize, ref buffer);
            return ReadNext(segment, buffer, count, currentSize + size);
        }

        public VBufferInfo ApplyFor(byte[] buffer)
        {
            return ApplyFor(buffer, DefautlSegmentSize);
        }

        public VBufferInfo ApplyFor(byte[] buffer, int segmentSize)
        {
            _segmentSize = segmentSize;
            // 当前块索引
            var index = _nextSegmentIndex;

            var segment = QueryNext();
            int offset;
            var size = segment.TryWrite(buffer, out offset);
            FillBuffer(segment, buffer, size);
            return VBufferInfo.Create(index, offset, buffer.Length);
        }

        private void FillBuffer(BufferSegment parentSegment, byte[] buffer, int bufferPos)
        {
            if (buffer.Length == bufferPos)
                return;
            var currentIndex = _nextSegmentIndex;
            var segment = QueryNext();
            parentSegment.NextIndex = currentIndex;

            int offset;
            var size = segment.TryWrite(buffer, bufferPos, out offset);
            if (buffer.Length == bufferPos + size)
                return;
            FillBuffer(segment, buffer, bufferPos + size);
        }

        private BufferSegment QueryNext()
        {
            if (Count == 0)
            {
                return AppendNewSegment();
            }
            var lastSegmentNode = Last;
            if (lastSegmentNode.IsFull)
            {
                return AppendNewSegment();
            }
            return lastSegmentNode;
        }

        private BufferSegment AppendNewSegment()
        {
            var segment = CreateSegment();
            Add(segment);
            return segment;
        }

        private void Add(BufferSegment segment)
        {
            Add(_nextSegmentIndex, segment);
            _nextSegmentIndex++;
        }

        private BufferSegment CreateSegment()
        {
            return new BufferSegment(_segmentSize);
        }
    }
}
