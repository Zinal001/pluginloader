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
using System;
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
        public String[] DllIncludes;

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
