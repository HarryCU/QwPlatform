using System;
using System.Collections.Generic;

namespace QwMicroKernel.Command.Implements
{
    public class QwCommandContext : Disposer, ICommandContext
    {
        private readonly IDictionary<string, object> _arguments;

        private IDictionary<string, object> ArgumentTable
        {
            get { return _arguments; }
        }

        public ICollection<object> Arguments
        {
            get { return ArgumentTable.Values; }
        }

        public QwCommandContext()
        {
            _arguments = new Dictionary<string, object>();
        }

        public QwCommandContext(IDictionary<string, object> arguments)
        {
            if (arguments == null) throw new ArgumentNullException("arguments");
            _arguments = arguments;
        }

        public virtual void AddArgument(string name, object argument)
        {
            if (argument == null) throw new ArgumentNullException("argument");
            if (!ArgumentTable.ContainsKey(name))
                ArgumentTable.Add(name, argument);
        }

        public virtual object GetArgument(string name)
        {
            if (ArgumentTable.ContainsKey(name))
                return ArgumentTable[name];
            return null;
        }

        protected override void Release()
        {
            if (_arguments != null)
                _arguments.Clear();
        }
    }
}
