using System;
using System.IO;
using QwMicroKernel.Core;
using QwMicroKernel.Reflection;

namespace QwMicroKernel.Data.Implements
{
    public sealed class DbServiceRuntime : Disposer
    {
        private readonly EventManager _eventMgr = EventManager.Create();

        private readonly string _pluginFolder;
        private readonly IDbExtendedService _extendedService;

        #region Event Keys
        private readonly object EVENT_ERROR = new object();
        #endregion

        public event EventHandler<DbServiceRuntimeErrorEventArgs> Error
        {
            add { _eventMgr.Add(EVENT_ERROR, value); }
            remove { _eventMgr.Remove(EVENT_ERROR, value); }
        }

        public DbServiceRuntime(string folder)
        {
            if (string.IsNullOrWhiteSpace(folder)) throw new ArgumentNullException("folder");

            _pluginFolder = folder;
            _extendedService = new DbExtendedService();
        }

        #region Event Triggers
        private void OnError(DbServiceRuntimeErrorEventArgs args)
        {
            var handler = _eventMgr.GetEventHandler<DbServiceRuntimeErrorEventArgs>(EVENT_ERROR);
            if (handler != null)
                handler(this, args);
        }
        #endregion

        public void LoadService()
        {
            var dllFiles = Directory.GetFiles(_pluginFolder, "*.dll");
            foreach (var dllFile in dllFiles)
            {
                try
                {
                    var assembly = AssemblyResolver.Load(dllFile, _pluginFolder);
                    var startupAttr = ReflectionHelper.GetAttributeOne<DbServiceStartupAttribute>(assembly);
                    if (startupAttr == null) continue;
                    var startup = startupAttr.StartupType.NewDef() as IDbExtendedServiceStartup;
                    if (startup == null) continue;

                    startup.Run(_extendedService);
                }
                catch (Exception ex)
                {
                    OnError(new DbServiceRuntimeErrorEventArgs(ex));
                }
            }
        }

        public T GetProvider<T>() where T : class
        {
            return _extendedService.Get<T>();
        }

        protected override void Release()
        {
            _extendedService.Dispose();
            _eventMgr.Dispose();
        }
    }
}
