namespace QwVirtualFileSystem.Cache.IO
{
    internal sealed class VBufferInfo
    {
        public readonly int SegmentIndex;
        public readonly int Offset;
        public readonly int Count;

        private VBufferInfo(int segmentIndex, int offset, int count)
        {
            SegmentIndex = segmentIndex;
            Offset = offset;
            Count = count;
        }

        public static VBufferInfo Create(int segmentIndex, int offset, int count)
        {
            return new VBufferInfo(segmentIndex, offset, count);
        }

        public override string ToString()
        {
            return string.Format("{{Index: {0}, Offset: {1}, Count: {2}}}", SegmentIndex, Offset, Count);
        }
    }
}
