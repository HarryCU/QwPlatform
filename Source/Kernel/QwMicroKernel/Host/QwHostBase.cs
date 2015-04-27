using System;

namespace QwMicroKernel.Host
{
    public abstract class QwHostBase : Disposer, IHost
    {
        public event EventHandler<HostLogEventArgs> Logged;

        public void Startup()
        {
            DoStartup();
            OnLogged("Host Startup at {0}.", DateTime.Now);
        }

        protected abstract void DoStartup();

        public void Shotdown()
        {
            DoShotdown();
            OnLogged("Host Shotdown at {0}.", DateTime.Now);
        }

        protected abstract void DoShotdown();

        protected virtual void OnLogged(string format, params object[] args)
        {
            var handler = Logged;
            if (handler != null)
                handler(this, new HostLogEventArgs(format, args));
        }

        protected override void Release()
        {
            Shotdown();
        }
    }
}
