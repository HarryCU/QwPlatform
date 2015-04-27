
using System;

namespace QwMicroKernel.Reflection
{
    public interface IProperty : IMember
    {
        bool IsStatic { get; }
        Type Type { get; }
        void SetValue(object instance, object value);
        object GetValue(object instance);
    }
}
