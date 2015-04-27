using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace QwMicroKernel.Reflection.IL
{
    public sealed class MethodBodyReader
    {
        private byte[] _buffer = null;
        private MethodInfo _method = null;
        private IInterceptor _interceptor;

        public MethodBodyReader(MethodInfo method)
            : this(method, new EmptyInterceptor())
        {
        }

        public MethodBodyReader(MethodInfo method, IInterceptor interceptor)
        {
            _interceptor = interceptor;
            _method = method;
            var body = method.GetMethodBody();
            if (body != null)
            {
                _buffer = body.GetILAsByteArray();
            }
        }

        public ICollection<IJitInstruction> CreateInstructionSet()
        {
            var module = _method.Module;
            byte[] buffer = this._buffer;
            int position = 0;
            IList<IJitInstruction> instructions = new List<IJitInstruction>();
            while (position < buffer.Length)
            {
                JitInstruction instruction = new JitInstruction();
                // get the operation code of the current instruction
                OpCode code;//= OpCodes.Nop;
                ushort value = buffer[position++];
                if (value != 0xfe)
                {
                    code = OpCodeTable.SingleByteOpCodes[(int)value];
                }
                else
                {
                    value = buffer[position++];
                    code = OpCodeTable.MultiByteOpCodes[(int)value];
                    value = (ushort)(value | 0xfe00);
                }
                instruction.Code = code;
                instruction.Offset = position - 1;
                int metadataToken = 0;
                // get the operand of the current operation
                switch (code.OperandType)
                {
                    case OperandType.InlineField:
                        metadataToken = ReadInt32(buffer, ref position);
                        instruction.Operand = module.ResolveField(metadataToken);
                        break;
                    case OperandType.InlineMethod:
                        metadataToken = ReadInt32(buffer, ref position);
                        try
                        {
                            instruction.Operand = module.ResolveMethod(metadataToken);
                        }
                        catch
                        {
                            instruction.Operand = module.ResolveMember(metadataToken);
                        }
                        break;
                    case OperandType.InlineSig:
                        metadataToken = ReadInt32(buffer, ref position);
                        instruction.Operand = module.ResolveSignature(metadataToken);
                        break;
                    case OperandType.InlineTok:
                        metadataToken = ReadInt32(buffer, ref position);
                        try
                        {
                            instruction.Operand = module.ResolveType(metadataToken);
                        }
                        catch
                        {

                        }
                        // SSS : see what to do here
                        break;
                    case OperandType.InlineType:
                        metadataToken = ReadInt32(buffer, ref position);
                        // now we call the ResolveType always using the generic attributes type in order
                        // to support decompilation of generic methods and classes

                        // thanks to the guys from code project who commented on this missing feature

                        instruction.Operand = module.ResolveType(metadataToken, this._method.DeclaringType.GetGenericArguments(), this._method.GetGenericArguments());
                        break;
                    case OperandType.InlineI:
                        {
                            instruction.Operand = ReadInt32(buffer, ref position);
                            break;
                        }
                    case OperandType.InlineI8:
                        {
                            instruction.Operand = ReadInt64(buffer, ref position);
                            break;
                        }
                    case OperandType.InlineNone:
                        {
                            instruction.Operand = null;
                            break;
                        }
                    case OperandType.InlineR:
                        {
                            instruction.Operand = ReadDouble(buffer, ref position);
                            break;
                        }
                    case OperandType.InlineString:
                        {
                            metadataToken = ReadInt32(buffer, ref position);
                            instruction.Operand = module.ResolveString(metadataToken);
                            break;
                        }
                    case OperandType.InlineSwitch:
                        {
                            int count = ReadInt32(buffer, ref position);
                            int[] casesAddresses = new int[count];
                            for (int i = 0; i < count; i++)
                            {
                                casesAddresses[i] = ReadInt32(buffer, ref position);
                            }
                            Label[] cases = new Label[count];
                            for (int i = 0; i < count; i++)
                            {
                                cases[i] = new Label(position + casesAddresses[i]);
                            }
                            instruction.Operand = cases;
                            break;
                        }
                    case OperandType.InlineVar:
                        {
                            instruction.Operand = ReadUInt16(buffer, ref position);
                            break;
                        }
                    case OperandType.ShortInlineBrTarget:
                        {
                            instruction.Operand = new Label(ReadSByte(buffer, ref position) + position);
                            break;
                        }
                    case OperandType.InlineBrTarget:
                        metadataToken = ReadInt32(buffer, ref position);
                        metadataToken += position;
                        instruction.Operand = new Label(metadataToken);
                        break;
                    case OperandType.ShortInlineI:
                        {
                            instruction.Operand = ReadSByte(buffer, ref position);
                            break;
                        }
                    case OperandType.ShortInlineR:
                        {
                            instruction.Operand = ReadSingle(buffer, ref position);
                            break;
                        }
                    case OperandType.ShortInlineVar:
                        {
                            instruction.Operand = ReadByte(buffer, ref position);
                            break;
                        }
                    default:
                        {
                            throw new Exception("Unknown operand type.");
                        }
                }

                _interceptor.OnBuildInstruction(instruction);

                instructions.Add(instruction);
            }
            return instructions;
        }

        public string Generation(IGenerationStrategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException("strategy");
            var instructions = CreateInstructionSet();
            using (var builder = strategy.GenerationProc(_method, instructions))
            {
                return builder.ToString();
            }
        }

        public string GenerationILCode()
        {
            return Generation(GenerationCodeStrategy.Strategy);
        }

        #region IL Read Methods

        private static int ReadInt16(byte[] ilByte, ref int position)
        {
            return ((ilByte[position++] | (ilByte[position++] << 8)));
        }
        private static ushort ReadUInt16(byte[] ilByte, ref int position)
        {
            return (ushort)((ilByte[position++] | (ilByte[position++] << 8)));
        }
        private static int ReadInt32(byte[] ilByte, ref int position)
        {
            return (((ilByte[position++] | (ilByte[position++] << 8)) | (ilByte[position++] << 0x10)) | (ilByte[position++] << 0x18));
        }
        private static ulong ReadInt64(byte[] ilByte, ref int position)
        {
            return (ulong)(((ilByte[position++] | (ilByte[position++] << 8)) | (ilByte[position++] << 0x10)) | (ilByte[position++] << 0x18) | (ilByte[position++] << 0x20) | (ilByte[position++] << 0x28) | (ilByte[position++] << 0x30) | (ilByte[position++] << 0x38));
        }
        private static double ReadDouble(byte[] ilByte, ref int position)
        {
            return (((ilByte[position++] | (ilByte[position++] << 8)) | (ilByte[position++] << 0x10)) | (ilByte[position++] << 0x18) | (ilByte[position++] << 0x20) | (ilByte[position++] << 0x28) | (ilByte[position++] << 0x30) | (ilByte[position++] << 0x38));
        }
        private static sbyte ReadSByte(byte[] ilByte, ref int position)
        {
            return (sbyte)ilByte[position++];
        }
        private static byte ReadByte(byte[] ilByte, ref int position)
        {
            return (byte)ilByte[position++];
        }
        private static Single ReadSingle(byte[] ilByte, ref int position)
        {
            return (Single)(((ilByte[position++] | (ilByte[position++] << 8)) | (ilByte[position++] << 0x10)) | (ilByte[position++] << 0x18));
        }

        #endregion
    }
}
