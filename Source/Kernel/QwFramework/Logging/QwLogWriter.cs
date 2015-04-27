using System;
using log4net;
using QwMicroKernel.Logging;

namespace QwFramework.Logging
{
    public sealed class QwLogWriter : EmptyLogWriter
    {
        private readonly ILog _log;

        private ILog LogPrivoder
        {
            get { return _log; }
        }

        public QwLogWriter(ILog log)
            : base(LevelParse(log))
        {
            _log = log;
        }

        private static LoggingLevel LevelParse(ILog log)
        {
            if (log.IsDebugEnabled)
                return LoggingLevel.Debug;
            if (log.IsErrorEnabled)
                return LoggingLevel.Error;
            if (log.IsFatalEnabled)
                return LoggingLevel.Fatal;
            if (log.IsInfoEnabled)
                return LoggingLevel.Info;
            if (log.IsWarnEnabled)
                return LoggingLevel.Warn;
            return LoggingLevel.None;
        }

        protected override void LogInternal(LoggingLevel level, object obj, Exception exception)
        {
            string message = obj == null ? string.Empty : obj.ToString();

            if (level == LoggingLevel.Debug)
            {
                LogPrivoder.Debug(message);
            }
            else if (level == LoggingLevel.Error)
            {
                if (exception == null)
                    LogPrivoder.Error(message);
                else
                    LogPrivoder.Error(message, exception);
            }
            else if (level == LoggingLevel.Fatal)
            {
                LogPrivoder.Fatal(message);
            }
            else if (level == LoggingLevel.Warn)
            {
                LogPrivoder.Warn(message);
            }
            else
            {
                LogPrivoder.Info(message);
            }
        }
    }
}
