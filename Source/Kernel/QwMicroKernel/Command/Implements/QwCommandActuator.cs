namespace QwMicroKernel.Command.Implements
{
    public class QwCommandActuator : Disposer
    {
        private readonly ICommandContext _context;
        private readonly ICommand _command;

        public ICommandContext Context
        {
            get { return _context; }
        }

        public QwCommandActuator(ICommand command)
            : this(new QwCommandContext(), command)
        {
        }

        public QwCommandActuator(ICommandContext context, ICommand command)
        {
            _command = command;
            _context = context;
        }

        public object Execute()
        {
            if (_command.CanExecute(Context))
                return _command.Execute(Context);
            return null;
        }

        protected override void Release()
        {
            _context.Dispose();
        }
    }
}
