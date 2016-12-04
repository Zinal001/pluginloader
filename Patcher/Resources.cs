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
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Patcher
{
    static class Resources
    {
        public static Stream LoadFileFromResources(string filename)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames().First(name => name.Contains(filename));
            return assembly.GetManifestResourceStream(resourceName);
        }

        public static void ExtractArchive(string filename, bool overwrite = false)
        {
            using (var file = File.Open(filename, FileMode.Open))
            using (var zip = new ZipArchive(file))
            {
                var directory = Path.GetDirectoryName(filename);
                if (overwrite)
                    zip.Entries.ToList().ForEach((ZipArchiveEntry entry) =>
                    {
                        var entryPath = Path.Combine(directory, entry.FullName);
                        if (File.Exists(entryPath))
                            File.Delete(entryPath);
                        else if (Directory.Exists(entryPath))
                            Directory.Delete(entryPath, true);
                    });

                zip.ExtractToDirectory(directory);
            }
        }

        public static byte[] StreamToBytes(Stream stream)
        {
            if (stream == null) return null;
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Close();
            return bytes;
        }
    }
}
