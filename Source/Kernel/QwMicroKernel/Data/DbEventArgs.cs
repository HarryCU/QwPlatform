using System;

namespace QwMicroKernel.Data
{
    public class DbAffairEventArgs : EventArgs
    {
        public Exception Error { get; private set; }

        public DbAffairEventArgs(Exception ex)
        {
            Error = ex;
        }
    }

    public class DbServiceRuntimeErrorEventArgs : EventArgs
    {
        public Exception Error { get; private set; }

        public DbServiceRuntimeErrorEventArgs(Exception ex)
        {
            Error = ex;
        }
    }
}
