
using System.Collections.Generic;
using System.Reflection;

namespace QwMicroKernel.Reflection
{
    public interface IMethod : IMember
    {
        ICollection<ParameterInfo> Args { get; }
        bool IsStatic { get; }
        object Invoke(object instance, params object[] @params);
    }
}
