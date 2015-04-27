using System;
using System.Reflection;

namespace QwMicroKernel.Data
{
    public interface IMappingResolver
    {
        string ResolveFieldName<TModel>(MemberInfo member);
        string ResolveAliasName<TModel>(MemberInfo member);
        string ResolveAliasName<TModel>(Type type);
    }
}
