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
using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace Patcher.Patches
{
    internal class AgosGuiPatch : Patch
    {
        public AgosGuiPatch(PatcherData patcherData) : base(patcherData)
        {
            var irData = patcherData.TargetData[PatcherTarget.InterstellarRift];
            var plData = patcherData.TargetData[PatcherTarget.PluginLoader];
            var irProgramType = irData.Module.GetType("Game.Program");
            var pluginInjectorType = plData.Module.GetType("PluginLoader.PluginInjector");
            var irPluginInjectorField = Helpers.GetFieldByName(irProgramType.Fields.ToArray(), "PluginInjector");
            var irAgosGuiType = irData.Module.GetType("Game.Client.AgosGui");

            var instructions = new List<Instruction>
            {
                Instruction.Create(OpCodes.Ldsfld, irPluginInjectorField),
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Callvirt, irData.Module.Import(Helpers.GetMethodByName(pluginInjectorType.Methods.ToArray(), "IRInitializeAgosGui")))
            };
            var processor = Helpers.GetMethodByName(irAgosGuiType.Methods.ToArray(), ".ctor").Body.GetILProcessor();
            Helpers.InjectInstructionsToEnd(processor, instructions.ToArray());
        }
    }
}