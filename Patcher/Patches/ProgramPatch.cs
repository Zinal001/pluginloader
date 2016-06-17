#region Licence
// Copyright (c) 2016 Tomas Bosek
#pragma warning disable CC0065 // Remove trailing whitespace
// 
// This file is part of PluginLoader.
#pragma warning disable CC0065 // Remove trailing whitespace
// 
// PluginLoader is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
#pragma warning disable CC0065 // Remove trailing whitespace
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
#pragma warning disable CC0065 // Remove trailing whitespace
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
#endregion
using Mono.Cecil;
#pragma warning restore CC0065 // Remove trailing whitespace
#pragma warning restore CC0065 // Remove trailing whitespace
#pragma warning restore CC0065 // Remove trailing whitespace
#pragma warning restore CC0065 // Remove trailing whitespace
using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace Patcher.Patches
{
    internal class ProgramPatch : Patch
    {
        public ProgramPatch(PatcherData patcherData) : base(patcherData)
        {
            var irData = patcherData.TargetData[PatcherTarget.InterstellarRift];
            var plData = patcherData.TargetData[PatcherTarget.PluginLoader];
            var irProgramType = irData.Module.GetType("Game.Program");
            var irProgramCtor = Helpers.GetMethodByName(irProgramType.Methods.ToArray(), ".cctor");
            var pluginInjectorType = plData.Module.GetType("PluginLoader.PluginInjector");
            var pluginInjectorCtor = Helpers.GetMethodByName(pluginInjectorType.Methods.ToArray(), ".ctor");
            var irPluginInjectorField = new FieldDefinition("PluginInjector", FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.InitOnly, irData.Module.Import(pluginInjectorType));
            irProgramType.Fields.Add(irPluginInjectorField);

            var instructions = new List<Instruction>
            {
                Instruction.Create(OpCodes.Ldsfld, Helpers.GetFieldByName(irProgramType.Fields.ToArray(), "log")),
                Instruction.Create(OpCodes.Newobj, irData.Module.Import(pluginInjectorCtor)),
                Instruction.Create(OpCodes.Stsfld, irPluginInjectorField)
            };
            var processor = irProgramCtor.Body.GetILProcessor();
            Helpers.InjectInstructionsToEnd(processor, instructions.ToArray());
        }
    }
}