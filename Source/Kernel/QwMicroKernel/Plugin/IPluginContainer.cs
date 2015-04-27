using System;

namespace QwMicroKernel.Plugin
{
    public interface IPluginContainer : IDisposable
    {
        IPluginContext Context { get; }
        IPluginService Service { get; }

        event EventHandler<PluginEventArgs> Installed;
        event EventHandler<PluginEventArgs> Uninstalled;
        event EventHandler<PluginEventArgs> Started;
        event EventHandler<PluginEventArgs> Stoped;

        long Install(IPlugin plugin);
        void Uninstall(long id);
        void Start(long id);
        void Stop(long id);
        PluginState GetState(long id);
    }
}
