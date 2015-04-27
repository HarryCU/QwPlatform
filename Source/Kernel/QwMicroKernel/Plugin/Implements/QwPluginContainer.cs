using System;
using QwMicroKernel.Collections;
using QwMicroKernel.Core;

namespace QwMicroKernel.Plugin.Implements
{
    public class QwPluginContainer : Disposer, IPluginContainer
    {
        #region PluginNode class

        class PluginNode
        {
            public IPlugin Plugin { get; set; }
            public PluginState State { get; set; }
        }

        #endregion

        private readonly EventManager _eventMgr = EventManager.Create();
        private readonly ICache<long, PluginNode> _plugins;

        private ICache<long, PluginNode> Plugins
        {
            get { return _plugins; }
        }

        #region Event Keys
        private readonly object PLUGIN_EVENT_INSTALLED = new object();
        private readonly object PLUGIN_EVENT_UNINSTALLED = new object();
        private readonly object PLUGIN_EVENT_STARTED = new object();
        private readonly object PLUGIN_EVENT_STOPED = new object();
        #endregion

        public IPluginContext Context
        {
            get;
            private set;
        }

        public IPluginService Service
        {
            get;
            private set;
        }

        public event EventHandler<PluginEventArgs> Installed
        {
            add { _eventMgr.Add(PLUGIN_EVENT_INSTALLED, value); }
            remove { _eventMgr.Remove(PLUGIN_EVENT_INSTALLED, value); }
        }

        public event EventHandler<PluginEventArgs> Uninstalled
        {
            add { _eventMgr.Add(PLUGIN_EVENT_UNINSTALLED, value); }
            remove { _eventMgr.Remove(PLUGIN_EVENT_UNINSTALLED, value); }
        }

        public event EventHandler<PluginEventArgs> Started
        {
            add { _eventMgr.Add(PLUGIN_EVENT_STARTED, value); }
            remove { _eventMgr.Remove(PLUGIN_EVENT_STARTED, value); }
        }

        public event EventHandler<PluginEventArgs> Stoped
        {
            add { _eventMgr.Add(PLUGIN_EVENT_STOPED, value); }
            remove { _eventMgr.Remove(PLUGIN_EVENT_STOPED, value); }
        }

        public QwPluginContainer()
        {
            _plugins = new QwDictionary<long, PluginNode>(MissingValueProvider);
            Initialize();
        }

        protected virtual void Initialize()
        {
            Context = new QwPluginContext(this);
            Service = new QwPluginService(this);
        }

        private static PluginNode MissingValueProvider(long id)
        {
            return null;
        }

        #region Event Triggers
        protected virtual void OnInstalled(PluginEventArgs args)
        {
            var handler = _eventMgr.GetEventHandler<PluginEventArgs>(PLUGIN_EVENT_INSTALLED);
            if (handler != null)
                handler(this, args);
        }
        protected virtual void OnUninstalled(PluginEventArgs args)
        {
            var handler = _eventMgr.GetEventHandler<PluginEventArgs>(PLUGIN_EVENT_UNINSTALLED);
            if (handler != null)
                handler(this, args);
        }
        protected virtual void OnStarted(PluginEventArgs args)
        {
            var handler = _eventMgr.GetEventHandler<PluginEventArgs>(PLUGIN_EVENT_STARTED);
            if (handler != null)
                handler(this, args);
        }
        protected virtual void OnStoped(PluginEventArgs args)
        {
            var handler = _eventMgr.GetEventHandler<PluginEventArgs>(PLUGIN_EVENT_STOPED);
            if (handler != null)
                handler(this, args);
        }
        #endregion

        private static long GenerateId()
        {
            return BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0);
        }

        public long Install(IPlugin plugin)
        {
            if (plugin == null) throw new ArgumentNullException("plugin");

            long id = GenerateId();
            Plugins.Add(id, new PluginNode() { Plugin = plugin, State = PluginState.Install });
            OnInstalled(new PluginEventArgs(plugin));
            return id;
        }

        public void Uninstall(long id)
        {
            Stop(id);
            var node = Plugins.Get(id);
            if (node != null && (node.State & PluginState.Stop) == PluginState.Stop)
            {
                Plugins.Remove(id);
                OnUninstalled(new PluginEventArgs(node.Plugin));
                node.Plugin.Dispose();
            }
        }

        public void Start(long id)
        {
            var node = Plugins.Get(id);
            if (node != null && (node.State & PluginState.Start) != PluginState.Start)
            {
                node.Plugin.Start(Context);
                node.State = PluginState.Start;
                OnStarted(new PluginEventArgs(node.Plugin));
            }
        }

        public void Stop(long id)
        {
            var node = Plugins.Get(id);
            if (node != null && (node.State & PluginState.Stop) != PluginState.Stop)
            {
                node.Plugin.Stop(Context);
                node.State = PluginState.Stop;
                OnStoped(new PluginEventArgs(node.Plugin));
            }
        }

        protected override void Release()
        {
            Plugins.Clear();
            Service.Dispose();
            _eventMgr.Dispose();
        }

        public PluginState GetState(long id)
        {
            var node = Plugins.Get(id);
            if (node == null) return PluginState.None;
            return node.State;
        }
    }
}
