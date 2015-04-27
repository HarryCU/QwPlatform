using System;
using System.Net.Sockets;

namespace QwMicroKernel.Net.Implements
{
    internal sealed class UserToken
    {
        private SocketAsyncEventArgs _receiveEventArgs;
        private byte[] _asyncReceiveBuffer;
        private SocketAsyncEventArgs _sendEventArgs;
        private DynamicBufferManager _receiveBuffer;
        private SendBufferManager _sendBuffer;
        //协议对象
        private BaseProtocol _protocol;
        private Socket _connectSocket;
        private DateTime _connectDateTime;
        private DateTime _activeDateTime;

        public SocketAsyncEventArgs ReceiveEventArgs
        {
            get { return _receiveEventArgs; }
            set { _receiveEventArgs = value; }
        }
        public SocketAsyncEventArgs SendEventArgs
        {
            get { return _sendEventArgs; }
            set { _sendEventArgs = value; }
        }
        public DynamicBufferManager ReceiveBuffer
        {
            get { return _receiveBuffer; }
            set { _receiveBuffer = value; }
        }
        public SendBufferManager SendBuffer
        {
            get { return _sendBuffer; }
            set { _sendBuffer = value; }
        }
        public Socket ConnectSocket
        {
            get
            {
                return _connectSocket;
            }
            set
            {
                _connectSocket = value;
                if (_connectSocket == null) //清理缓存
                {
                    if (_protocol != null)
                        _protocol.Close();
                    _receiveBuffer.Clear(_receiveBuffer.Count);
                    _sendBuffer.ClearPacket();
                }
                _protocol = null;
                _receiveEventArgs.AcceptSocket = _connectSocket;
                _sendEventArgs.AcceptSocket = _connectSocket;
            }
        }
        public DateTime ConnectDateTime
        {
            get { return _connectDateTime; }
            set { _connectDateTime = value; }
        }
        public DateTime ActiveDateTime
        {
            get { return _activeDateTime; }
            set { _activeDateTime = value; }
        }
        public BaseProtocol Protocol
        {
            get { return _protocol; }
            set { _protocol = value; }
        }

        public UserToken(int asyncReceiveBufferSize, int initBufferSize = 1024*4)
        {
            _connectSocket = null;
            _protocol = null;
            _receiveEventArgs = new SocketAsyncEventArgs();
            _receiveEventArgs.UserToken = this;
            _asyncReceiveBuffer = new byte[asyncReceiveBufferSize];
            _receiveEventArgs.SetBuffer(_asyncReceiveBuffer, 0, _asyncReceiveBuffer.Length);
            _sendEventArgs = new SocketAsyncEventArgs();
            _sendEventArgs.UserToken = this;
            _receiveBuffer = new DynamicBufferManager(initBufferSize);
            _sendBuffer = new SendBufferManager(initBufferSize); ;
        }
    }
}
