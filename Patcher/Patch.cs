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
using System.Collections.Generic;
using System.Reflection;

namespace Patcher
{
    enum PatcherTarget
    {
        InterstellarRift,
        PluginLoader
    }
    struct PatcherTargetData
    {
        public Assembly Assembly;
        public AssemblyDefinition AssemblyDef;
        public ModuleDefinition Module;
        public string Path;
    }
    class PatcherData
    {
        public readonly Dictionary<PatcherTarget, PatcherTargetData> TargetData = new Dictionary<PatcherTarget, PatcherTargetData>();
    }
    abstract class Patch
    {
        protected readonly PatcherData patcherData;

        protected Patch(PatcherData patcherData)
        {
            this.patcherData = patcherData;
        }
    }
}
