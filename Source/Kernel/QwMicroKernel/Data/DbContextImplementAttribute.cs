using System;

namespace QwMicroKernel.Data
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class DbContextImplementAttribute : Attribute
    {
        public Type ContextType
        {
            get;
            private set;
        }

        public DbContextImplementAttribute(Type contextType)
        {
            ContextType = contextType;
        }
    }
}
