using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using QwMicroKernel.Core;

namespace QwMicroKernel.Net.Implements
{
    public abstract class AppBaseServer : IAppServer
    {
        private readonly EventManager _eventMgr = EventManager.Create();
        private Socket _listenSocket;
        //最大支持连接个数
        private readonly int _connectionNumber;
        //Socket最大超时时间，单位为MS
        private int _socketTimeout;
        private readonly UserTokenPool _userTokenPool;
        private readonly UserTokenList _userTokenList;
        //每个连接接收缓存大小
        private readonly int _receiveBufferSize;
        //限制访问接收连接的线程数，用来控制最大并发数
        private readonly Semaphore _maxNumberAcceptedClients;

        #region Event Keys
        private readonly object EVENT_ERROR = new object();
        #endregion

        public event EventHandler<AppServerErrorEventArgs> Error
        {
            add { _eventMgr.Add(EVENT_ERROR, value); }
            remove { _eventMgr.Remove(EVENT_ERROR, value); }
        }

        public int Timeout
        {
            get { return _socketTimeout; }
            set { _socketTimeout = value; }
        }

        public AppBaseServer(int connectionNumber, int receiveBufferSize = 1024*4)
        {
            _connectionNumber = connectionNumber;
            _receiveBufferSize = receiveBufferSize;

            _userTokenPool = new UserTokenPool(connectionNumber);
            _userTokenList = new UserTokenList();
            _maxNumberAcceptedClients = new Semaphore(connectionNumber, connectionNumber);

            Initialize();
        }

        private void Initialize()
        {
            //按照连接数建立读写对象
            for (int i = 0; i < _connectionNumber; i++)
            {
                var userToken = new UserToken(_receiveBufferSize);
                userToken.ReceiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                userToken.SendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                _userTokenPool.Push(userToken);
            }
        }

        public void Start(string ipString, int port)
        {
            Start(new IPEndPoint(IPAddress.Parse(ipString), port));
        }

        public void Start(IPEndPoint localEndPoint)
        {
            _listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listenSocket.Bind(localEndPoint);
            _listenSocket.Listen(_connectionNumber);
            StartAccept(null);
        }

        private void StartAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            if (acceptEventArgs == null)
            {
                acceptEventArgs = new SocketAsyncEventArgs();
                acceptEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
            }
            else
            {
                acceptEventArgs.AcceptSocket = null; //释放上次绑定的Socket，等待下一个Socket连接
            }

            _maxNumberAcceptedClients.WaitOne(); //获取信号量
            bool willRaiseEvent = _listenSocket.AcceptAsync(acceptEventArgs);
            if (!willRaiseEvent)
            {
                ProcessAccept(acceptEventArgs);
            }
        }

        private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs acceptEventArgs)
        {
            try
            {
                ProcessAccept(acceptEventArgs);
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        private void ProcessAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            UserToken userToken = _userTokenPool.Pop();
            _userTokenList.Add(userToken); //添加到正在连接列表
            userToken.ConnectSocket = acceptEventArgs.AcceptSocket;
            userToken.ConnectDateTime = DateTime.Now;

            try
            {
                bool willRaiseEvent = userToken.ConnectSocket.ReceiveAsync(userToken.ReceiveEventArgs); //投递接收请求
                if (!willRaiseEvent)
                {
                    lock (userToken)
                    {
                        ProcessReceive(userToken.ReceiveEventArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
            StartAccept(acceptEventArgs); //把当前异步事件释放，等待下次连接
        }

        private void IO_Completed(object sender, SocketAsyncEventArgs asyncEventArgs)
        {
            UserToken userToken = asyncEventArgs.UserToken as UserToken;
            userToken.ActiveDateTime = DateTime.Now;
            try
            {
                lock (userToken)
                {
                    if (asyncEventArgs.LastOperation == SocketAsyncOperation.Receive)
                        ProcessReceive(asyncEventArgs);
                    else if (asyncEventArgs.LastOperation == SocketAsyncOperation.Send)
                        ProcessSend(asyncEventArgs);
                    else
                        throw new ArgumentException("The last operation completed on the socket was not a receive or send");
                }
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs receiveEventArgs)
        {
            UserToken userToken = receiveEventArgs.UserToken as UserToken;
            if (userToken.ConnectSocket == null)
                return;
            userToken.ActiveDateTime = DateTime.Now;
            if (userToken.ReceiveEventArgs.BytesTransferred > 0 && userToken.ReceiveEventArgs.SocketError == SocketError.Success)
            {
                int offset = userToken.ReceiveEventArgs.Offset;
                int count = userToken.ReceiveEventArgs.BytesTransferred;
                if ((userToken.Protocol == null) & (userToken.ConnectSocket != null)) //存在Socket对象，并且没有绑定协议对象，则进行协议对象绑定
                {
                    int size = BuildingSocketProtocol(userToken);
                    userToken.Protocol.Initialize(this, userToken);

                    offset = offset + size;
                    count = count - size;
                }
                if (userToken.Protocol == null) //如果没有解析对象，提示非法连接并关闭连接
                {
                    CloseClientSocket(userToken);
                }
                else
                {
                    if (count > 0) //处理接收数据
                    {
                        if (!userToken.Protocol.ProcessReceive(userToken.ReceiveEventArgs.Buffer, offset, count))
                        { //如果处理数据返回失败，则断开连接
                            CloseClientSocket(userToken);
                        }
                        else //否则投递下次介绍数据请求
                        {
                            bool willRaiseEvent = userToken.ConnectSocket.ReceiveAsync(userToken.ReceiveEventArgs); //投递接收请求
                            if (!willRaiseEvent)
                                ProcessReceive(userToken.ReceiveEventArgs);
                        }
                    }
                    else
                    {
                        bool willRaiseEvent = userToken.ConnectSocket.ReceiveAsync(userToken.ReceiveEventArgs); //投递接收请求
                        if (!willRaiseEvent)
                            ProcessReceive(userToken.ReceiveEventArgs);
                    }
                }
            }
            else
            {
                CloseClientSocket(userToken);
            }
        }

        private int BuildingSocketProtocol(UserToken userToken)
        {
            // 处理协议
            uint size;
            var protocol = CreateProtocol(userToken.ReceiveEventArgs.Buffer, userToken.ReceiveEventArgs.Offset, out size);
            userToken.Protocol = protocol;

            if (userToken.Protocol != null)
            {
                throw new Exception(string.Format("Building socket invoke element {0}.Local Address: {1}, Remote Address: {2}",
                    userToken.Protocol, userToken.ConnectSocket.LocalEndPoint, userToken.ConnectSocket.RemoteEndPoint));
            }
            return (int)size;
        }

        private bool ProcessSend(SocketAsyncEventArgs sendEventArgs)
        {
            var userToken = sendEventArgs.UserToken as UserToken;
            if (userToken.Protocol == null)
                return false;
            userToken.ActiveDateTime = DateTime.Now;
            if (sendEventArgs.SocketError == SocketError.Success)
                return userToken.Protocol.OnSendCompleted();
            else
            {
                CloseClientSocket(userToken);
                return false;
            }
        }

        internal bool SendAsyncEventArgs(Socket connectSocket, SocketAsyncEventArgs sendEventArgs, byte[] buffer, int offset, int count)
        {
            if (connectSocket == null)
                return false;
            sendEventArgs.SetBuffer(buffer, offset, count);
            bool willRaiseEvent = connectSocket.SendAsync(sendEventArgs);
            if (!willRaiseEvent)
            {
                return ProcessSend(sendEventArgs);
            }
            else
                return true;
        }

        private void CloseClientSocket(UserToken userToken)
        {
            if (userToken.ConnectSocket == null)
                return;
            string socketInfo = string.Format("Local Address: {0} Remote Address: {1}", userToken.ConnectSocket.LocalEndPoint,
                userToken.ConnectSocket.RemoteEndPoint);
            try
            {
                userToken.ConnectSocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
            userToken.ConnectSocket.Close();
            //释放引用，并清理缓存，包括释放协议对象等资源
            userToken.ConnectSocket = null;

            _maxNumberAcceptedClients.Release();
            _userTokenPool.Push(userToken);
            _userTokenList.Remove(userToken);
        }

        protected abstract BaseProtocol CreateProtocol(byte[] buffer, int offset, out uint size);

        #region Event Triggers

        protected void OnError(Exception error)
        {
            OnError(new AppServerErrorEventArgs(error));
        }

        protected virtual void OnError(AppServerErrorEventArgs args)
        {
            var handler = _eventMgr.GetEventHandler<AppServerErrorEventArgs>(EVENT_ERROR);
            if (handler != null)
                handler(this, args);
        }
        #endregion
    }
}
