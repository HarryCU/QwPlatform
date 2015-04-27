using System;

namespace QwMicroKernel.Data
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public sealed class DbServiceStartupAttribute : Attribute
    {
        public Type StartupType { get; private set; }

        public DbServiceStartupAttribute(Type startupType)
        {
            StartupType = startupType;
        }
    }
}
