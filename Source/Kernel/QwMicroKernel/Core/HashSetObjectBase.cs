namespace QwMicroKernel.Core
{
    public abstract class HashSetObjectBase
    {
        private static int _counter = 0;
        private readonly int _hashCode;

        protected HashSetObjectBase()
        {
            _hashCode = (_counter++).GetHashCode();
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}
