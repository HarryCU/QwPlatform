using System.Collections.Generic;
using System.Reflection;
using QwMicroKernel.Text;

namespace QwMicroKernel.Reflection.IL
{
    public interface IGenerationStrategy
    {
        StringBuilder GenerationProc(MethodInfo method, ICollection<IJitInstruction> instructionSet);
    }
}
