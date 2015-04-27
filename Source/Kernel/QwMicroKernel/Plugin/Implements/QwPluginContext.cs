namespace QwMicroKernel.Plugin.Implements
{
    public class QwPluginContext : Disposer, IPluginContext
    {
        private readonly IPluginContainer _container;

        public QwPluginContext(IPluginContainer container)
        {
            _container = container;
        }

        public T GetService<T>() where T : class
        {
            return _container.Service.Get<T>();
        }

        protected override void Release()
        {
        }
    }
}
