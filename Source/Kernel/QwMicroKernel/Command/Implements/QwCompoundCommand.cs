using System;
using System.Collections.Generic;

namespace QwMicroKernel.Command.Implements
{
    public abstract class QwCompoundCommand : ICommand
    {
        private readonly ICollection<ICommand> _commandList;

        public virtual string Name
        {
            get { return "QwMicroKernel.Command.Implements.QwCompoundCommand"; }
        }

        protected ICollection<ICommand> Commands
        {
            get { return _commandList; }
        }

        protected QwCompoundCommand()
            : this(new List<ICommand>())
        {
        }

        protected QwCompoundCommand(ICollection<ICommand> commands)
        {
            if (commands == null) throw new ArgumentNullException("commands");
            _commandList = commands;
        }

        public abstract bool CanExecute(ICommandContext context);

        public abstract object Execute(ICommandContext context);

        protected virtual QwCompoundCommand Add(ICommand command)
        {
            _commandList.Add(command);
            return this;
        }

        protected virtual QwCompoundCommand Remove(ICommand command)
        {
            _commandList.Remove(command);
            return this;
        }

        public static QwCompoundCommand operator +(QwCompoundCommand cmdCompound, ICommand command)
        {
            return cmdCompound.Add(command);
        }

        public static QwCompoundCommand operator -(QwCompoundCommand cmdCompound, ICommand command)
        {
            return cmdCompound.Remove(command);
        }
    }
}
