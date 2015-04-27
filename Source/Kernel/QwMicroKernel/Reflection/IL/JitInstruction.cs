using System.Reflection.Emit;

namespace QwMicroKernel.Reflection.IL
{
    /// <summary>
    /// IL 指令
    /// </summary>
    internal sealed class JitInstruction : IJitInstruction
    {
        // Fields
        private OpCode _code;
        private object _operand;
        private byte[] _operandData;
        private int _offset;

        // Properties
        public OpCode Code
        {
            get { return _code; }
            set { _code = value; }
        }

        public object Operand
        {
            get { return _operand; }
            set { _operand = value; }
        }

        public byte[] OperandData
        {
            get { return _operandData; }
            set { _operandData = value; }
        }

        public Label Label
        {
            get;
            private set;
        }

        public int Offset
        {
            get { return _offset; }
            set
            {
                _offset = value;
                Label = new Label(_offset);
            }
        }
    }
}
