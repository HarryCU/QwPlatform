using System;
using System.Collections.Generic;

namespace QwMicroKernel.Command
{
    public interface ICommandContext : IDisposable
    {
        ICollection<object> Arguments { get; }

        void AddArgument(string name, object argument);
        object GetArgument(string name);
    }
}
