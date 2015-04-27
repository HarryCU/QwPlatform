namespace QwMicroKernel.Net
{
    public interface IBufferManager
    {
        int Count { get; }
        void WriteBuffer(byte[] buffer, int offset, int count);
        void WriteBuffer(byte[] buffer);
        void WriteShort(short value, bool convert);
        void WriteInt(int value, bool convert);
        void WriteLong(long value, bool convert);
        void WriteString(string value);
        void Clear();
        void Clear(int count);
    }
}
