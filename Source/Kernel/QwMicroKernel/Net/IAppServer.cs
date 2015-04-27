using System;
using System.Net;

namespace QwMicroKernel.Net
{
    public interface IAppServer
    {
        int Timeout { get; set; }
        event EventHandler<AppServerErrorEventArgs> Error;
        void Start(string ipString, int port);
        void Start(IPEndPoint localEndPoint);
    }
}
