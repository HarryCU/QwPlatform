using System;

namespace QwMicroKernel.Plugin
{
    public interface IPluginService : IDisposable
    {
        void Add<T>(T service) where T : class;
        T Get<T>() where T : class;
        T Remove<T>() where T : class;
    }
}
