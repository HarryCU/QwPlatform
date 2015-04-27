using System.Collections.Generic;

namespace QwMicroKernel.Net.Implements
{
    //由于是异步发送，有可能接收到两个命令，写入了两次返回，发送需要等待上一次回调才发下一次的响应
    internal sealed class SendBufferManager
    {
        struct SendBufferPacket
        {
            public int Offset;
            public int Count;
        }

        private readonly DynamicBufferManager _bufferManager;
        private readonly IList<SendBufferPacket> _sendBufferList;
        private SendBufferPacket _sendBufferPacket;

        public DynamicBufferManager BufferManager
        {
            get { return _bufferManager; }
        }

        public SendBufferManager(int bufferSize)
        {
            _bufferManager = new DynamicBufferManager(bufferSize);
            _sendBufferList = new List<SendBufferPacket>();
            _sendBufferPacket.Offset = 0;
            _sendBufferPacket.Count = 0;
        }

        public void StartPacket()
        {
            _sendBufferPacket.Offset = _bufferManager.Count;
            _sendBufferPacket.Count = 0;
        }

        public void EndPacket()
        {
            _sendBufferPacket.Count = _bufferManager.Count - _sendBufferPacket.Offset;
            _sendBufferList.Add(_sendBufferPacket);
        }

        public bool GetFirstPacket(ref int offset, ref int count)
        {
            if (_sendBufferList.Count <= 0)
                return false;
            //_sendBufferList[0].Offset;清除了第一个包后，后续的包往前移，因此Offset都为0
            offset = 0;
            count = _sendBufferList[0].Count;
            return true;
        }

        public bool ClearFirstPacket()
        {
            if (_sendBufferList.Count <= 0)
                return false;
            int count = _sendBufferList[0].Count;
            _bufferManager.Clear(count);
            _sendBufferList.RemoveAt(0);
            return true;
        }

        public void ClearPacket()
        {
            _sendBufferList.Clear();
            _bufferManager.Clear(_bufferManager.Count);
        }
    }
}
