using System;

namespace QwMicroKernel.Plugin
{
    public interface IPlugin : IDisposable
    {
        void Start(IPluginContext context);
        void Stop(IPluginContext context);
    }
}
