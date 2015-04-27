namespace QwMicroKernel.Reflection.IL
{
    public interface IInterceptor
    {
        void OnBuildInstruction(IJitInstruction instruction);
    }
}
