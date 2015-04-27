namespace QwMicroKernel.Reflection.IL
{
    public sealed class Label
    {
        public int Index { get; private set; }

        public Label(int labelIndex)
        {
            Index = labelIndex;
        }

        public override string ToString()
        {
            return string.Concat("IL_", Index.ToString().PadLeft(4, '0'));
        }
    }
}
