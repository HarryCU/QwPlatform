namespace QwMicroKernel.Command
{
    public interface ICommand
    {
        string Name { get; }
        bool CanExecute(ICommandContext context);
        object Execute(ICommandContext context);
    }
}
