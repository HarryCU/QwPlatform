using System;
using System.Reflection;

namespace QwMicroKernel.Data.Implements
{
    public class EmptyMappingResolver : IMappingResolver
    {
        public virtual string ResolveFieldName<TModel>(MemberInfo member)
        {
            return member.Name;
        }

        public virtual string ResolveAliasName<TModel>(MemberInfo member)
        {
            return member.Name;
        }

        public virtual string ResolveAliasName<TModel>(Type type)
        {
            return type.Name;
        }
    }
}
