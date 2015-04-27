namespace QwFramework.Data
{
    public class LogMessage
    {
        public LogMessage(ErrorLevel level, string message)
        {
            Level = level;
            Message = message;
        }

        public readonly ErrorLevel Level;
        public readonly string Message;

        public override string ToString()
        {
            return Message;
        }
    }
}
