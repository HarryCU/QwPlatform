using System;

namespace QwMicroKernel.Host
{
    public class HostLogEventArgs : EventArgs
    {
        private readonly string _message;

        public HostLogEventArgs(string format, params object[] args)
            : this(string.Format(format, args))
        {
        }

        public HostLogEventArgs(string message)
        {
            _message = message;
        }

        public string Message
        {
            get { return _message; }
        }
    }
}
