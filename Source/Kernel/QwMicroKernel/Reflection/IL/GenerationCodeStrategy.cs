using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using QwMicroKernel.Text;

namespace QwMicroKernel.Reflection.IL
{
    public class GenerationCodeStrategy : IGenerationStrategy
    {
        public static readonly GenerationCodeStrategy Strategy = new GenerationCodeStrategy();

        public StringBuilder GenerationProc(MethodInfo method, ICollection<IJitInstruction> instructionSet)
        {
            var builder = new StringBuilder(1024);
            foreach (var instruction in instructionSet)
            {
                builder.Append(instruction.Label);
                builder.Append(": ");
                builder.Append(instruction.Code);
                if (instruction.Operand == null) continue;
                var operand = instruction.Operand;
                switch (instruction.Code.OperandType)
                {
                    case OperandType.InlineField:
                        var field = operand as FieldInfo;
                        builder.AppendFormat(" {0} {1}::{2}", ProcessSpecialTypes(field.FieldType), ProcessSpecialTypes(field.ReflectedType), field.Name);
                        break;
                    case OperandType.InlineMethod:
                        var m = operand as MethodInfo;
                        if (m != null)
                        {
                            builder.Append(" ");
                            if (!m.IsStatic) builder.Append("instance ");
                            builder.AppendFormat("{0} {1}::{2}()", ProcessSpecialTypes(m.ReturnType), ProcessSpecialTypes(m.ReflectedType), m.Name);
                        }
                        else
                        {
                            var ctor = operand as ConstructorInfo;
                            if (ctor != null)
                            {
                                builder.Append(" ");
                                if (!ctor.IsStatic) builder.Append("instance ");
                                builder.AppendFormat("void {0}::{1}()", ProcessSpecialTypes(ctor.ReflectedType), ctor.Name);
                            }
                        }
                        break;
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.InlineBrTarget:
                        var label = operand as Label;
                        builder.Append(" ").Append(label);
                        break;
                    case OperandType.InlineType:
                        builder.Append(ProcessSpecialTypes(operand as Type));
                        break;
                    case OperandType.InlineString:
                        var str = operand as string;
                        builder.Append(str.Replace("\\", "\\\\"));
                        break;
                    case OperandType.InlineVar:
                    case OperandType.ShortInlineVar:
                    case OperandType.InlineI:
                    case OperandType.InlineI8:
                    case OperandType.InlineR:
                    case OperandType.ShortInlineI:
                    case OperandType.ShortInlineR:
                        builder.Append(operand as string);
                        break;
                    case OperandType.InlineTok:
                        var type = operand as Type;
                        if (type != null)
                            builder.Append(type.FullName);
                        else
                            throw new NotSupportedException();
                        break;
                    case OperandType.InlineSwitch:
                        var cases = operand as Label[];

                        break;
                    case OperandType.InlineSig:

                        break;
                    case OperandType.InlineNone:
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            return builder;
        }

        private static string ProcessSpecialTypes(Type type)
        {
            string typeName = type.FullName;
            string result = typeName;
            switch (typeName)
            {
                case "System.string":
                case "System.String":
                case "String":
                    result = "string"; break;
                case "System.Int32":
                case "Int":
                case "Int32":
                    result = "int"; break;
            }
            return result;
        }
    }
}