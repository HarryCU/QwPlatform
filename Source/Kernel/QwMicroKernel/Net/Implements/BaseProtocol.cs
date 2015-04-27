using System;
using System.Net;

namespace QwMicroKernel.Net.Implements
{
    //异步Socket调用对象，所有的协议处理都从本类继承
    public abstract class BaseProtocol
    {
        private AppBaseServer _socketServer;
        private UserToken _token;
        private DateTime _connectTime;
        private DateTime _activeTime;
        //标识是否有发送异步事件
        private bool _sendAsync;

        public DateTime ConnectTime
        {
            get { return _connectTime; }
        }

        public DateTime ActiveTime
        {
            get { return _activeTime; }
        }

        /// <summary>
        /// 长度是否使用网络字节顺序
        /// </summary>
        public virtual bool NetByteOrder
        {
            get { return false; }
        }

        public BaseProtocol()
        {
            _sendAsync = false;
            _connectTime = DateTime.UtcNow;
            _activeTime = DateTime.UtcNow;
        }

        internal void Initialize(AppBaseServer socketServer, UserToken token)
        {
            _socketServer = socketServer;
            _token = token;
        }

        //接收异步事件返回的数据，用于对数据进行缓存和分包
        internal bool ProcessReceive(byte[] buffer, int offset, int count)
        {
            _activeTime = DateTime.UtcNow;
            DynamicBufferManager receiveBuffer = _token.ReceiveBuffer;

            receiveBuffer.WriteBuffer(buffer, offset, count);
            bool result = true;
            while (receiveBuffer.Count > sizeof(int))
            {
                //按照长度分包
                int packetLength = BitConverter.ToInt32(receiveBuffer.Buffer, 0); //获取包长度
                if (NetByteOrder)
                    packetLength = IPAddress.NetworkToHostOrder(packetLength); //把网络字节顺序转为本地字节顺序


                if ((packetLength > 10 * 1024 * 1024) | (receiveBuffer.Count > 10 * 1024 * 1024)) //最大Buffer异常保护
                    return false;

                if ((receiveBuffer.Count - sizeof(int)) >= packetLength) //收到的数据达到包长度
                {
                    result = ProcessPacket(receiveBuffer.Buffer, sizeof(int), packetLength);
                    if (result)
                        receiveBuffer.Clear(packetLength + sizeof(int)); //从缓存中清理
                    else
                        return result;
                }
                else
                {
                    return true;
                }
            }
            return true;
        }

        //处理分完包后的数据，把命令和数据分开，并对命令进行解析
        internal bool ProcessPacket(byte[] buffer, int offset, int count)
        {
            if (count < sizeof(int))
                return false;

            return OnProcessPacket(buffer, offset, count);
        }

        protected abstract bool OnProcessPacket(byte[] buffer, int offset, int count);

        protected virtual void OnSend(IBufferManager bufferManager, byte[] buffer, int offset, int count)
        {
            //获取总大小
            int totalLength = sizeof(int) + count;
            bufferManager.WriteInt(totalLength, false);
            bufferManager.WriteBuffer(buffer, offset, count);
        }

        public bool Send(byte[] buffer, int offset, int count)
        {
            SendBufferManager asyncSendBufferManager = _token.SendBuffer;
            asyncSendBufferManager.StartPacket();
            OnSend(asyncSendBufferManager.BufferManager, buffer, offset, count);
            asyncSendBufferManager.EndPacket();

            bool result = true;
            if (!_sendAsync)
            {
                int packetOffset = 0;
                int packetCount = 0;
                if (asyncSendBufferManager.GetFirstPacket(ref packetOffset, ref packetCount))
                {
                    _sendAsync = true;
                    result = _socketServer.SendAsyncEventArgs(_token.ConnectSocket, _token.SendEventArgs,
                        asyncSendBufferManager.BufferManager.Buffer, packetOffset, packetCount);
                }
            }
            return result;
        }

        public virtual void Close()
        {
            _token.ConnectSocket.Close();
            _token.ReceiveEventArgs.AcceptSocket.Close();
            _token.SendEventArgs.AcceptSocket.Close();
        }

        internal bool OnSendCompleted()
        {
            _activeTime = DateTime.UtcNow;
            _sendAsync = false;
            SendBufferManager asyncSendBufferManager = _token.SendBuffer;
            //清除已发送的包
            asyncSendBufferManager.ClearFirstPacket();
            int offset = 0;
            int count = 0;
            if (asyncSendBufferManager.GetFirstPacket(ref offset, ref count))
            {
                _sendAsync = true;
                return _socketServer.SendAsyncEventArgs(_token.ConnectSocket, _token.SendEventArgs,
                    asyncSendBufferManager.BufferManager.Buffer, offset, count);
            }
            else
                return OnSendCallback();
        }

        protected virtual bool OnSendCallback()
        {
            return true;
        }
    }
}