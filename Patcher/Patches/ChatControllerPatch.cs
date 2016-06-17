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
using System;
using System.Collections.Generic;

namespace Patcher.Patches
{
    internal class ChatControllerPatch : Patch
    {
        public ChatControllerPatch(PatcherData patcherData) : base(patcherData)
        {
            var irData = patcherData.TargetData[PatcherTarget.InterstellarRift];
            var plData = patcherData.TargetData[PatcherTarget.PluginLoader];
            var irChatControllerType = irData.Module.GetType("Game.Server.ChatController");
            var instructions = new List<Instruction>();

            var stringType = irData.Module.Import(typeof(string));
            var actionType = irData.Module.Import(typeof(Action<,>));
            var dictionaryType = irData.Module.Import(typeof(Dictionary<,>));
            var irPlayerType = irData.Module.GetType("Game.Server.Player");
            var actionInstanceType = new GenericInstanceType(actionType);
            actionInstanceType.GenericArguments.Add(irPlayerType);
            actionInstanceType.GenericArguments.Add(stringType);
            var dictionaryInstanceType = new GenericInstanceType(dictionaryType);
            dictionaryInstanceType.GenericArguments.Add(stringType);
            dictionaryInstanceType.GenericArguments.Add(actionInstanceType);


            var getCommandsMethod = new MethodDefinition("get_Commands", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, dictionaryInstanceType);
            irChatControllerType.Methods.Add(getCommandsMethod);
            irChatControllerType.Properties.Add(new PropertyDefinition("Commands", PropertyAttributes.None, dictionaryInstanceType)
            {
                GetMethod = getCommandsMethod
            });

            instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            instructions.Add(Instruction.Create(OpCodes.Ldfld, Helpers.GetFieldByName(irChatControllerType.Fields.ToArray(), "m_commands")));
            instructions.Add(Instruction.Create(OpCodes.Ret));
            var processor = getCommandsMethod.Body.GetILProcessor();
            instructions.ForEach(instruction => processor.Append(instruction));
        }

    }
}