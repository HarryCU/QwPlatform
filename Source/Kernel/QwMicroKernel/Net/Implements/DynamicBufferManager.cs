using System;
using System.Net;
using System.Text;

namespace QwMicroKernel.Net.Implements
{
    internal sealed class DynamicBufferManager : IBufferManager
    {
        //存放内存的数组
        public byte[] Buffer
        {
            get;
            private set;
        }
        //写入数据大小
        public int Count
        {
            get;
            private set;
        }

        public int ReserveCount
        {
            get { return Buffer.Length - Count; }
        }

        public DynamicBufferManager(int bufferSize)
        {
            Count = 0;
            Buffer = new byte[bufferSize];
        }

        public void Clear()
        {
            Count = 0;
        }

        //清理指定大小的数据
        public void Clear(int count)
        {
            if (count >= Count) //如果需要清理的数据大于现有数据大小，则全部清理
            {
                Count = 0;
            }
            else
            {
                for (int i = 0; i < Count - count; i++) //否则后面的数据往前移
                {
                    Buffer[i] = Buffer[count + i];
                }
                Count = Count - count;
            }
        }

        //设置缓存大小
        public void SetBufferSize(int size)
        {
            if (Buffer.Length < size)
            {
                byte[] tmpBuffer = new byte[size];
                Array.Copy(Buffer, 0, tmpBuffer, 0, Count); //复制以前的数据
                Buffer = tmpBuffer; //替换
            }
        }

        public void WriteBuffer(byte[] buffer, int offset, int count)
        {
            if (ReserveCount >= count) //缓冲区空间够，不需要申请
            {
                Array.Copy(buffer, offset, Buffer, Count, count);
                Count = Count + count;
            }
            else //缓冲区空间不够，需要申请更大的内存，并进行移位
            {
                int totalSize = Buffer.Length + count - ReserveCount; //总大小-空余大小
                byte[] tmpBuffer = new byte[totalSize];
                Array.Copy(Buffer, 0, tmpBuffer, 0, Count); //复制以前的数据
                Array.Copy(buffer, offset, tmpBuffer, Count, count); //复制新写入的数据
                Count = Count + count;
                Buffer = tmpBuffer; //替换
            }
        }

        public void WriteBuffer(byte[] buffer)
        {
            WriteBuffer(buffer, 0, buffer.Length);
        }

        public void WriteShort(short value, bool convert)
        {
            if (convert)
            {
                value = IPAddress.HostToNetworkOrder(value); //NET是小头结构，网络字节是大头结构，需要客户端和服务器约定好
            }
            byte[] tmpBuffer = BitConverter.GetBytes(value);
            WriteBuffer(tmpBuffer);
        }

        public void WriteInt(int value, bool convert)
        {
            if (convert)
            {
                value = IPAddress.HostToNetworkOrder(value); //NET是小头结构，网络字节是大头结构，需要客户端和服务器约定好
            }
            byte[] tmpBuffer = BitConverter.GetBytes(value);
            WriteBuffer(tmpBuffer);
        }

        public void WriteLong(long value, bool convert)
        {
            if (convert)
            {
                value = IPAddress.HostToNetworkOrder(value); //NET是小头结构，网络字节是大头结构，需要客户端和服务器约定好
            }
            byte[] tmpBuffer = BitConverter.GetBytes(value);
            WriteBuffer(tmpBuffer);
        }

        //文本全部转成UTF8，UTF8兼容性好
        public void WriteString(string value)
        {
            byte[] tmpBuffer = Encoding.UTF8.GetBytes(value);
            WriteBuffer(tmpBuffer);
        }
    }
}
