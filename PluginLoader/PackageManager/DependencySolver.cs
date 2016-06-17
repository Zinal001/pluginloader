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
using System.Linq;

namespace PluginLoader
{
    public class DependencyLoopException : Exception
    {
        public DependencyLoopException(Package package1, Package package2) : base($"Loop dependency between {package1.ToString()} and {package2.ToString()}") { }
    }
    public static class DependencySolver
    {
        public static UnmetDependency[] Solve(Package[] packages)
        {
            var unmetDependencies = new List<UnmetDependency>();
            packages.ToList().ForEach(p =>
            {
                var pluginLoaderDependency = new Dependency
                {
                    UniqueName = nameof(PluginLoader),
                    Version = p.Metadata.PluginLoader
                };

                unmetDependencies.Add(new UnmetDependency
                {
                    Package = p,
                    Dependency = pluginLoaderDependency
                });
                unmetDependencies.AddRange(p.Metadata.Dependencies.ConvertAll(d => new UnmetDependency
                {
                    Package = p,
                    Dependency = d
                }));
            });

            foreach (UnmetDependency unmetDependency in unmetDependencies.ToArray())
            {
                if (unmetDependency.Dependency.UniqueName == nameof(PluginLoader) &&
                    compareVersions(unmetDependency.Dependency.Version, Versions.PluginLoaderVersion))
                    unmetDependencies.Remove(unmetDependency);
                else if (packages.Any(p => meetsDependency(p, unmetDependency.Dependency)))
                    unmetDependencies.Remove(unmetDependency);
            }

            return unmetDependencies.ToArray();
        }

        public static UnmetDependency[] GetRelevantUnmetDependencies(UnmetDependency[] unmetDependencies)
        {
            var newUnmetDependencies = unmetDependencies.ToList();
            newUnmetDependencies.RemoveAll((UnmetDependency unmetDependency) =>
            {
                return unmetDependencies.Any(d =>
                    d.Dependency.UniqueName.ToLower() == unmetDependency.Package.Metadata.UniqueName.ToLower());
            });
            return newUnmetDependencies.ToArray();
        }

        public static UnmetDependency[] GetIrrelevantUnmetDependencies(UnmetDependency[] unmetDependencies)
        {
            var relevant = GetRelevantUnmetDependencies(unmetDependencies);

            return unmetDependencies.Where(d => !relevant.Contains(d)).ToArray();
        }

        public static Package[] Sort(Package[] packages)
        {
            Array.Sort(packages, delegate(Package p1, Package p2){
                var p1p2 = dependsOn(p1, p2);
                var p2p1 = dependsOn(p2, p1);

                if (p1p2 && p2p1)
                    throw new DependencyLoopException(p1, p2);

                return p1p2 ? 1 : p2p1 ? -1 : 0;
            });
            return packages;
        }

        private static bool dependsOn(Package package1, Package package2)
        {
            return package1.Metadata.Dependencies.Any(d =>
                d.UniqueName.ToLower() == package2.Metadata.UniqueName.ToLower());
        }
        private static bool meetsDependency(Package package, Dependency dependency)
        {
            return package.Metadata.UniqueName.ToLower() == dependency.UniqueName.ToLower() &&
                compareVersions(package.Metadata.Version, dependency.Version);
        }

        private static bool compareVersions(Version version1, Version version2)
        {
            var v1 = new Version(version1.Major, version1.Minor);
            var v2 = new Version(version2.Major, version2.Minor);

            return v1 == v2;
        }
    }
}
