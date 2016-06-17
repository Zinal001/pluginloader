#region Licence
// Copyright (c) 2016 Tomas Bosek
// 
// This file is part of PluginLoader.
// 
// PluginLoader is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
#endregion
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace Patcher
{
    static class Helpers
    {
        public static FieldDefinition GetFieldByName(FieldDefinition[] fields, string name)
        {
            return fields.Single(field => field.Name == name);
        }

        public static Instruction GetInstructionByOffset(MethodBody method, int offset)
        {
            return method.Instructions.Single(instruction => instruction.Offset == offset);
        }

        public static MethodDefinition GetMethodByName(MethodDefinition[] methods, string name)
        {
            return methods.Single(method => method.Name == name);
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

        public static void InjectInstructionsToEnd(ILProcessor processor, Instruction[] instructions)
        {
            var lastInstruction = processor.Body.Instructions.Last();
            processor.Replace(lastInstruction, instructions.First());

            if (instructions.Length > 0)
                instructions.ToList().GetRange(1, instructions.Length - 1).ForEach(instruction => processor.Append(instruction));
            processor.Append(Instruction.Create(OpCodes.Ret));
        }

        public static string ExecuteCmd(string command)
        {
            var cmd = new System.Diagnostics.Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine(command);
            cmd.StandardInput.WriteLine("exit");
            cmd.StandardInput.Flush();
            cmd.WaitForExit();
            var output = cmd.StandardOutput.ReadToEnd();
            cmd.Dispose();
            return output;
        }
    }
}
