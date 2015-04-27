using System;

namespace QwVirtualFileSystem
{
    public class VFSRuntimeException : Exception
    {
        public VFSRuntimeException(string message, params  object[] args)
            : this(null, message, args)
        {
        }

        public VFSRuntimeException(Exception innerException, string message, params  object[] args)
            : base(string.Format(message, args), innerException)
        {
        }
    }
}
