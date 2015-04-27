
using System.Reflection;

namespace QwMicroKernel.Reflection
{
    public interface IMember
    {
        string Name { get; }
        MemberInfo Member { get; }
    }
}
