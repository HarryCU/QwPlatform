using System;

namespace QwMicroKernel.Plugin
{
    public class PluginEventArgs : EventArgs
    {
        public IPlugin Plugin { get; private set; }

        public PluginEventArgs(IPlugin plugin)
        {
            Plugin = plugin;
        }
    }
}
