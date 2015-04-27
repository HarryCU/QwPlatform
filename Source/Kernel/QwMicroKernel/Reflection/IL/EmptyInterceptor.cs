namespace QwMicroKernel.Reflection.IL
{
    public class EmptyInterceptor : IInterceptor
    {
        public virtual void OnBuildInstruction(IJitInstruction instruction)
        {
        }
    }
}
