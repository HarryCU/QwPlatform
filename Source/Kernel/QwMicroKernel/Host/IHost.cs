using System;

namespace QwMicroKernel.Host
{
    public interface IHost : IDisposable
    {
        event EventHandler<HostLogEventArgs> Logged;
        void Startup();
        void Shotdown();
    }
}
