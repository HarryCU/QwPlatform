namespace QwMicroKernel.Net
{
    public interface IAppClient
    {
        void Connect(string host, int port);
        void Send(byte[] buffer);
        void Send(byte[] buffer, int offset, int count);
        void Disconnect();
    }
}
