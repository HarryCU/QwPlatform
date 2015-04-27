using System;
using QwMicroKernel.Collections;

namespace QwMicroKernel.Plugin.Implements
{
    public class QwPluginService : Disposer, IPluginService
    {
        private readonly IPluginContainer _container;
        private readonly ICache<Type, object> _services;

        private ICache<Type, object> Services
        {
            get { return _services; }
        }

        public QwPluginService(IPluginContainer container)
        {
            _container = container;
            _services = new QwDictionary<Type, object>(MissingValueProvider);
        }

        private static object MissingValueProvider(Type type)
        {
            return null;
        }

        public void Add<T>(T service) where T : class
        {
            Services.Add(typeof(T), service);
        }

        public T Get<T>() where T : class
        {
            return Services.Get(typeof(T)) as T;
        }

        public T Remove<T>() where T : class
        {
            var service = Get<T>();
            if (service != null)
            {
                Services.Remove(typeof(T));
            }
            return service;
        }

        protected override void Release()
        {
            Services.Clear();
        }
    }
}
