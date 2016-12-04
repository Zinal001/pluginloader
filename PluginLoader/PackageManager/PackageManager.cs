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
using System;
#pragma warning restore CC0065 // Remove trailing whitespace
#pragma warning restore CC0065 // Remove trailing whitespace
#pragma warning restore CC0065 // Remove trailing whitespace
#pragma warning restore CC0065 // Remove trailing whitespace
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PluginLoader
{
    public class PackageManager
    {
        public string GameDir => AppDomain.CurrentDomain.BaseDirectory;
        public string RootDir => Environment.ExpandEnvironmentVariables("%appdata%\\InterstellarRift\\plugins");

        private List<Package> packages = new List<Package>();
        public List<Package> Packages => packages;

        internal void ScanForPackages()
        {
            var dirs = Directory.GetDirectories(RootDir).ToList();

            foreach (string dir in dirs)
            {
                var definition = Path.Combine(dir, "plugin.json");
                var init = Path.Combine(dir, "__init__.py");
                var csInit = Path.Combine(dir, "__init__.cs");
                if (File.Exists(definition) && (File.Exists(init) || File.Exists(csInit)))
                {
                    try
                    {
                        packages.Add(new Package
                        {
                            Metadata = JsonSerializer.Deserialize<Metadata>(File.ReadAllText(definition)),
                            Path = dir
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[{nameof(PackageManager)}] Error loading module {Path.GetDirectoryName(init)}");
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        internal void Sort()
        {
            packages = DependencySolver.Sort(packages.ToArray()).ToList();
        }
    }
}
