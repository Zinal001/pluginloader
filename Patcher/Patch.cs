﻿#region Licence
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
