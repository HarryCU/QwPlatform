using System;
using System.Net;
using System.Net.Sockets;

namespace QwMicroKernel.Net.Implements
{
    public abstract class AppBaseClient : IAppClient
    {
        private TcpClient _client;
        private string _hostString;
        private int _port;
        //接收数据的缓存
        private DynamicBufferManager _recvBuffer;
        //发送数据的缓存，统一写到内存中，调用一次发送
        private DynamicBufferManager _sendBuffer;

        public int Timeout
        {
            get { return _client.SendTimeout; }
            set
            {
                _client.SendTimeout = value;
                _client.ReceiveTimeout = value;
            }
        }

        protected Socket Client
        {
            get { return _client.Client; }
        }

        /// <summary>
        /// 长度是否使用网络字节顺序
        /// </summary>
        protected virtual bool NetByteOrder
        {
            get { return false; }
        }

        protected AppBaseClient(int timeout = 1000, int receiveBufferSize = 1024*4)
        {
            _client = new TcpClient();
            _client.Client.Blocking = true;
            Timeout = timeout;
            _recvBuffer = new DynamicBufferManager(receiveBufferSize);
            _sendBuffer = new DynamicBufferManager(receiveBufferSize);
        }

        public virtual void Connect(string host, int port)
        {
            _client.Connect(host, port);
            _hostString = host;
            _port = port;
        }

        public virtual void Disconnect()
        {
            _client.Close();
            _client = new TcpClient();
        }

        public void Send(byte[] buffer)
        {
            Send(buffer, 0, buffer.Length);
        }

        public void Send(byte[] buffer, int offset, int count)
        {
            _sendBuffer.Clear();
            OnSend(_sendBuffer, buffer, offset, count);
            Client.Send(_sendBuffer.Buffer, 0, _sendBuffer.Count, SocketFlags.None);
        }

        protected virtual void OnSend(IBufferManager bufferManager, byte[] buffer, int offset, int count)
        {
            //获取总大小
            int totalLength = sizeof(int) + count;
            //写入总大小
            bufferManager.WriteInt(totalLength, false);
            //写入二进制数据
            bufferManager.WriteBuffer(buffer, offset, count);
        }

        protected virtual bool Receive(out byte[] buffer, out int offset, out int count)
        {
            _recvBuffer.Clear();
            Client.Receive(_recvBuffer.Buffer, sizeof(int), SocketFlags.None);
            int packetLength = BitConverter.ToInt32(_recvBuffer.Buffer, 0); //获取包长度
            if (NetByteOrder)
            {
                packetLength = IPAddress.NetworkToHostOrder(packetLength); //把网络字节顺序转为本地字节顺序
            }
            //保证接收有足够的空间
            _recvBuffer.SetBufferSize(sizeof(int) + packetLength);
            Client.Receive(_recvBuffer.Buffer, sizeof(int), packetLength, SocketFlags.None);

            OnReceive(_recvBuffer, packetLength, out buffer, out offset, out count);
            return true;
        }

        protected virtual void OnReceive(IBufferManager bufferManager, int packetLength, out byte[] buffer, out int offset, out int count)
        {
            buffer = _recvBuffer.Buffer;
            offset = sizeof(int);
            count = packetLength - offset;
        }
    }
}
