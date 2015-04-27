namespace QwMicroKernel.Plugin
{
    public interface IPluginContext
    {
        T GetService<T>() where T : class;
    }
}
