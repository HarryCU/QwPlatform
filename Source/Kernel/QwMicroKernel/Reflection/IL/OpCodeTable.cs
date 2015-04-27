using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace QwMicroKernel.Reflection.IL
{
    internal static class OpCodeTable
    {
        private static Dictionary<int, object> Cache = new Dictionary<int, object>();
        private static OpCode[] _multiByteOpCodes;
        private static OpCode[] _singleByteOpCodes;

        public static OpCode[] MultiByteOpCodes
        {
            get { return _multiByteOpCodes; }
        }
        public static OpCode[] SingleByteOpCodes
        {
            get { return _singleByteOpCodes; }
        }

        static OpCodeTable()
        {
            _singleByteOpCodes = new OpCode[0x100];
            _multiByteOpCodes = new OpCode[0x100];
            var fields = typeof(OpCodes).GetFields();
            for (int index = 0; index < fields.Length; index++)
            {
                FieldInfo field = fields[index];
                if (field.FieldType != typeof(OpCode))
                    continue;
                OpCode code = (OpCode)field.GetValue(null);
                ushort codeValue = (ushort)code.Value;
                if (codeValue < 0x100)
                {
                    _singleByteOpCodes[(int)codeValue] = code;
                }
                else
                {
                    if ((codeValue & 0xff00) != 0xfe00)
                    {
                        throw new Exception("Invalid OpCode.");
                    }
                    _multiByteOpCodes[codeValue & 0xff] = code;
                }
            }
        }
    }
}
