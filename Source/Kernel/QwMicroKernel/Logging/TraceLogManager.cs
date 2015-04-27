using System;

namespace QwMicroKernel.Logging
{
    public static class TraceLogManager
    {
        private static readonly TraceLogWriterFactory TraceLogWriterFactory = new TraceLogWriterFactory();

        public static ILogWriter GetLogger(Type type)
        {
            return GetLogger(type, t => TraceLogWriterFactory.Get(t.FullName));
        }

        public static ILogWriter GetLogger(Type type, Func<Type, ILogWriter> provider)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (provider == null)
                throw new ArgumentNullException("provider");

            return provider(type);
        }
    }
}
