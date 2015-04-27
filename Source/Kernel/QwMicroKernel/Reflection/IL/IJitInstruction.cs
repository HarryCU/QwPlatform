using System.Reflection.Emit;

namespace QwMicroKernel.Reflection.IL
{
    public interface IJitInstruction
    {
        OpCode Code { get; }
        object Operand { get; }
        Label Label { get; }
    }
}
