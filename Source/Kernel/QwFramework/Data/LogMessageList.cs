using System.Collections.Generic;

namespace QwFramework.Data
{
    public class LogMessageList : List<LogMessage>
    {
        public void AddError(string message, params object[] args)
        {
            Add(ErrorLevel.Error, message, args);
        }

        public void Add(ErrorLevel level, string message, params object[] args)
        {
            Add(new LogMessage(level, string.Format(message, args)));
        }
    }
}
