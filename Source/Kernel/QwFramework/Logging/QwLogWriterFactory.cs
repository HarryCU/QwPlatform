using log4net;
using log4net.Config;
using QwFramework.Helper;
using QwMicroKernel.Logging;

namespace QwFramework.Logging
{
    public sealed class QwLogWriterFactory : ILogWriterFactory
    {
        private static readonly QwLogWriterFactory _current = new QwLogWriterFactory();

        static QwLogWriterFactory()
        {
            var fileName = FileHelper.GetCurrent("log.modules.cfg");

            XmlConfigurator.Configure(Log4NetHelper.Configure(fileName));

            CurrentLogWriter = Current.Get("GwALL");
        }

        public static ILogWriterFactory Current
        {
            get { return _current; }
        }

        public static ILogWriter CurrentLogWriter
        {
            get;
            private set;
        }

        public ILogWriter Get(string name)
        {
            return new QwLogWriter(LogManager.GetLogger(name));
        }

        public void Shutdown()
        {
            LogManager.Shutdown();
        }
    }
}
