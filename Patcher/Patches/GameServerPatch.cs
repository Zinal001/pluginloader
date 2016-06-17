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
    internal class GameServerPatch : Patch
    {
        public GameServerPatch(PatcherData patcherData) : base(patcherData)
        {
            var irData = patcherData.TargetData[PatcherTarget.InterstellarRift];
            var plData = patcherData.TargetData[PatcherTarget.PluginLoader];
            var irProgramType = irData.Module.GetType("Game.Program");
            var pluginInjectorType = plData.Module.GetType("PluginLoader.PluginInjector");
            var irPluginInjectorField = Helpers.GetFieldByName(irProgramType.Fields.ToArray(), "PluginInjector");
            var irGameServerType = irData.Module.GetType("Game.GameStates.GameServer");
            var instructions = new List<Instruction>();

            instructions.Add(Instruction.Create(OpCodes.Ldsfld, irPluginInjectorField));
            instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            instructions.Add(Instruction.Create(OpCodes.Callvirt, irData.Module.Import(Helpers.GetMethodByName(pluginInjectorType.Methods.ToArray(), "IRInitializeGameServer"))));
            var processor = Helpers.GetMethodByName(irGameServerType.Methods.ToArray(), ".ctor").Body.GetILProcessor();
            Helpers.InjectInstructionsToEnd(processor, instructions.ToArray());

            instructions.Clear();
            instructions.Add(Instruction.Create(OpCodes.Ldsfld, irPluginInjectorField));
            instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            instructions.Add(Instruction.Create(OpCodes.Callvirt, Helpers.GetMethodByName(irGameServerType.Methods.ToArray(), "get_TickrateInSeconds")));
            instructions.Add(Instruction.Create(OpCodes.Callvirt, irData.Module.Import(Helpers.GetMethodByName(pluginInjectorType.Methods.ToArray(), "IRUpdateGameServer"))));
            processor = Helpers.GetMethodByName(irGameServerType.Methods.ToArray(), "UpdateOffthread").Body.GetILProcessor();
            Helpers.InjectInstructionsToEnd(processor, instructions.ToArray());

            instructions.Clear();
            instructions.Add(Instruction.Create(OpCodes.Ldsfld, irPluginInjectorField));
            instructions.Add(Instruction.Create(OpCodes.Callvirt, irData.Module.Import(Helpers.GetMethodByName(pluginInjectorType.Methods.ToArray(), "IRUnloadGameServer"))));
            processor = Helpers.GetMethodByName(irGameServerType.Methods.ToArray(), "Unload").Body.GetILProcessor();
            Helpers.InjectInstructionsToEnd(processor, instructions.ToArray());
        }
    }
}