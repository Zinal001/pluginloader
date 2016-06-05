using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Patcher
{
    static class Helpers
    {
        public static AssemblyNameReference GetAssemblyByName(AssemblyNameReference[] assemblies, string name)
        {
            foreach (AssemblyNameReference assembly in assemblies)
            {
                if (assembly.Name == name)
                {
                    return assembly;
                }
            }
            return null;
        }
        public static FieldDefinition GetFieldByName(FieldDefinition[] fields, string name)
        {
            foreach (FieldDefinition field in fields)
            {
                if (field.Name == name)
                {
                    return field;
                }
            }
            return null;
        }

        public static Instruction GetInstructionByOffset(MethodBody method, int offset)
        {
            foreach (Instruction instruction in method.Instructions)
            {
                if (instruction.Offset == offset)
                {
                    return instruction;
                }
            }
            return null;
        }

        public static MethodDefinition GetMethodByName(MethodDefinition[] methods, string name)
        {
            foreach (MethodDefinition method in methods)
            {
                if (method.Name == name)
                {
                    return method;
                }
            }
            return null;
        }

        public static void InjectInstructionsAfter(ILProcessor processor, Instruction[] instructions, int offset)
        {
            var lastInstruction = GetInstructionByOffset(processor.Body, offset);
            foreach (Instruction instruction in instructions)
            {
                processor.InsertAfter(lastInstruction, instruction);
                lastInstruction = instruction;
            }
        }

        public static void InjectInstructionsBefore(ILProcessor processor, Instruction[] instructions, int offset)
        {
            Instruction lastInstruction = null;
            foreach (Instruction instruction in instructions)
            {
                if(lastInstruction == null)
                {
                    processor.InsertBefore(GetInstructionByOffset(processor.Body, offset), instruction);
                }
                else
                {
                    processor.InsertAfter(lastInstruction, instruction);
                }
                lastInstruction = instruction;
            }
        }
    }
}
