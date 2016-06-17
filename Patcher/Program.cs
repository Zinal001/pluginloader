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
using System.IO;
using System.Linq;

namespace Patcher
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                Console.WriteLine("Usage: Patcher.exe [<path>]\n");

            var path = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();
            var notFound = false;
            PatchRunner.RequiredFiles.Where(file => !File.Exists(Path.Combine(path, file))).ToList().ForEach(file =>
            {
                Console.WriteLine($"File {Path.Combine(path, file)} could not be found.");
                notFound = true;
            });
            if (notFound)
            {
                Console.ReadLine();
                return;
            }

            Logger.OnInfo += (string text) => Console.Write(text);

            Logger.OnError += (string text) =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(text);
                Console.ResetColor();
            };

            Logger.OnSuccess += (string text) =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(text);
                Console.ResetColor();
            };

            var patcher = new PatchRunner(path);
            patcher.LoadData();
            patcher.Patch();
        }
    }
}
