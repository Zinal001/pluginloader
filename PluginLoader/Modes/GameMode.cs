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
using Game.Networking;
using System;
using System.IO;

namespace PluginLoader
{
    abstract class GameMode
    {
        protected readonly PluginManager pluginManager;
        protected GameMode(PluginManager pluginManager)
        {
            this.pluginManager = pluginManager;
        }

        private void Initialize()
        {
            Console.WriteLine();
            Console.WriteLine($"[{nameof(PluginLoader)}] Starting {this.GetType().Name} mode");
            Console.WriteLine();
        }

        public virtual void StartGameMode()
        {
            Initialize();
        }

        public static string AddFileToFS(NetFilesystem filesystem, string path, string filename, byte[] data)
        {
            var key = $"{path}{filename}";

            filesystem.FileSystem.WriteFile(path, filename, data);
            var netSource = new NetFilestreamSource
            {
                Path = path,
                Filename = filename,
            };
            filesystem.IdentifierSources.Add(key, netSource);
            return key;
        }

        public static Stream LoadFileFromFS(NetFilesystem filesystem, string path, string filename)
        {
            return filesystem.FileSystem.ReadFile(path, filename);
        }
    }
}