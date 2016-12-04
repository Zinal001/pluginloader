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
#pragma warning disable CC0074 // Make field readonly
using System;
#pragma warning restore CC0065 // Remove trailing whitespace
#pragma warning restore CC0065 // Remove trailing whitespace
#pragma warning restore CC0065 // Remove trailing whitespace
#pragma warning restore CC0065 // Remove trailing whitespace
using System.Collections.Generic;

namespace PluginLoader
{
    [Serializable]
    public struct UnmetDependency
    {
        public Package Package;
        public Dependency Dependency;

        public override string ToString()
        {
            return $"{Package.ToString()} is missing dependency {Dependency.ToString()}";
        }
    }
    [Serializable]
    public struct Dependency
    {
        public string UniqueName;
        public Version Version;

        public override string ToString()
        {
            return $"{UniqueName} v{Version}";
        }
    }
    [Serializable]
    public struct Metadata
    {
        public string UniqueName;
        public string PrettyName;
        public Version Version;
        public Version PluginLoader;
        public List<Dependency> Dependencies;
        public PackageType PackageType;

        public override string ToString()
        {
            return $"{UniqueName} v{Version} ({PackageType.ToString()})";
        }
    }
    [Serializable]
    public struct Package
    {
        public Metadata Metadata;
        public string Path;

        public override string ToString()
        {
            return Metadata.ToString();
        }
    }

    [Serializable]
    public enum PackageType : int
    {
        Python = 0,
        CScript = 1
    }
}
