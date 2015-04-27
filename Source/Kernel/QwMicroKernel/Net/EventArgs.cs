using System;

namespace QwMicroKernel.Net
{
    public class AppServerErrorEventArgs : EventArgs
    {
        public Exception Error
        {
            get;
            private set;
        }

        public AppServerErrorEventArgs(Exception error)
        {
            Error = error;
        }
    }
}
