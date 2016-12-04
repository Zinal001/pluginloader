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
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Patcher.Patches;

namespace Patcher
{
    public class PatchRunner
    {
        public static string[] RequiredFiles => new string[]
        {
            "IR.exe"
        };

        private PatcherData patcherData;
        public PatchRunner(string path)
        {
            Directory.SetCurrentDirectory(path);

            AppDomain.CurrentDomain.AssemblyResolve += (sender, resource) =>
            {
                if (resource.Name.Contains("Mono.Cecil"))
                    return Assembly.Load(Resources.StreamToBytes(Resources.LoadFileFromResources("Mono.Cecil.dll")));
                return null;
            };
        }

        public void LoadData()
        {
            patcherData = new PatcherData();

            var irTargetData = new PatcherTargetData();
            irTargetData.Path = Path.Combine(Directory.GetCurrentDirectory(), "IR.exe");
            irTargetData.Assembly = Assembly.Load(Resources.StreamToBytes(File.Open(irTargetData.Path, FileMode.Open)));

            using (var irAssemblyStream = File.Open(irTargetData.Path, FileMode.Open))
                irTargetData.AssemblyDef = AssemblyDefinition.ReadAssembly(irAssemblyStream);

            irTargetData.Module = irTargetData.AssemblyDef.MainModule;
            patcherData.TargetData.Add(PatcherTarget.InterstellarRift, irTargetData);

            var plTargetData = new PatcherTargetData();
            plTargetData.Path = Path.Combine(Directory.GetCurrentDirectory(), "PluginLoader.dll");
            plTargetData.Assembly = Assembly.Load(Resources.StreamToBytes(Resources.LoadFileFromResources("PluginLoader.dll")));
            plTargetData.AssemblyDef = AssemblyDefinition.ReadAssembly(Resources.LoadFileFromResources("PluginLoader.dll"));
            plTargetData.Module = plTargetData.AssemblyDef.MainModule;
            patcherData.TargetData.Add(PatcherTarget.PluginLoader, plTargetData);
        }

        public void Patch()
        {
            if (patcherData == null)
                LoadData();

            try
            {
                callPatches(patcherData);
            }
            catch (Exception ex)
            {
                Logger.ErrorLine("An error occurred before patching started");
                saveExceptionInfo("patcher", ex);
            }
        }

        public string GetIRVersion()
        {
            var irGlobalsType = patcherData.TargetData[PatcherTarget.InterstellarRift].Assembly.GetType("Game.Configuration.Globals");
            return (string)irGlobalsType.GetField("Version").GetValue(null);
        }

        public string GetPLVersion()
        {
            var plVersionHelpersType = patcherData.TargetData[PatcherTarget.PluginLoader].Assembly.GetType("PluginLoader.Versions");
            return plVersionHelpersType.GetProperty("PluginLoaderVersion").GetValue(null).ToString();
        }

        private void callPatches(PatcherData patcherData)
        {
            var irAssemblyReferences = patcherData.TargetData[PatcherTarget.InterstellarRift].Module.AssemblyReferences;
            if (irAssemblyReferences.Count(assembly =>
                assembly.FullName.StartsWith(patcherData.TargetData[PatcherTarget.InterstellarRift].AssemblyDef.Name.Name)) > 0)
            {
                Logger.InfoLine($"Interstellar Rift seem to be already patched");
                return;
            }

            try
            {
                Logger.Info("Interstellar Rift version ");
                Logger.SuccessLine(GetIRVersion());
            }
            catch (Exception)
            {
                Logger.ErrorLine("could not be detected");
            }

            try
            {
                Logger.Info("PluginLoader version ");
                Logger.SuccessLine(GetPLVersion());
            }
            catch (Exception)
            {
                Logger.ErrorLine("could not be detected");
            }

            var patches = new List<Type>();
            patches.Add(typeof(AccessModifierPatch));
            patches.Add(typeof(ProgramPatch));

            //patches.Add(typeof(AgosGuiPatch));
            patches.Add(typeof(ChatControllerPatch));
            //patches.Add(typeof(ClientDeviceFactoryPatch));
            //patches.Add(typeof(DeviceFactoryPatch));
            //patches.Add(typeof(GameClientPatch));
            //patches.Add(typeof(GameEditorPatch));
            patches.Add(typeof(GameMenuPatch));
            patches.Add(typeof(GameServerPatch));

            try
            {
                patches.ForEach(delegate (Type patch)
                {
                    Logger.Info($"Applying {patch.Name} ...");
                    Activator.CreateInstance(patch, patcherData);
                    Logger.SuccessLine("done");
                });

                var irData = patcherData.TargetData[PatcherTarget.InterstellarRift];
                var plData = patcherData.TargetData[PatcherTarget.PluginLoader];
                saveAssembly(irData.AssemblyDef, irData.Path);
                saveAssembly(plData.AssemblyDef, plData.Path);

                saveResource("IronPython.dll", "IronPython.dll");
                saveResource("IronPython.Modules.dll", "IronPython.Modules.dll");
                saveResource("Microsoft.Dynamic.dll", "Microsoft.Dynamic.dll");
                saveResource("Microsoft.Scripting.dll", "Microsoft.Scripting.dll");

                extractResource("Lib.zip", Directory.GetCurrentDirectory());
            }
            catch (Exception ex)
            {
                Logger.ErrorLine("error");

                Logger.ErrorLine(ex.Message);
                saveExceptionInfo("patcher", ex);
            }
            finally
            {
                Logger.InfoLine("Finished patching");
            }
        }

        private static void saveAssembly(AssemblyDefinition assemblyDef, string path)
        {
            Logger.Info($"Saving {Path.GetFileName(path)} ...");
            assemblyDef.Write(File.OpenWrite(path));
            Logger.SuccessLine("done");
        }

        private static void saveResource(string resourceName, string path)
        {
            Logger.Info($"Creating {Path.GetFileName(path)} ...");
            File.WriteAllBytes(path, Resources.StreamToBytes(Resources.LoadFileFromResources(resourceName)));
            Logger.SuccessLine("done");
        }

        private static void extractResource(string resourceName, string path)
        {
            var newFilePath = Path.Combine(path, resourceName);
            Logger.Info($"Creating {Path.GetFileNameWithoutExtension(newFilePath)} ...");
            File.WriteAllBytes(newFilePath, Resources.StreamToBytes(Resources.LoadFileFromResources(resourceName)));
            Resources.ExtractArchive(newFilePath, true);
            File.Delete(newFilePath);
            Logger.SuccessLine("done");
        }

        private static void saveExceptionInfo(string path, Exception exception)
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var errorLogFile = $"{path.Replace('.', '_')}.patchlog";
            Logger.InfoLine($"Saving error log into file {errorLogFile}");
            File.WriteAllText(Path.Combine(directory, errorLogFile),
                $"{exception.Message} [HRESULT {exception.HResult}]\n{exception.StackTrace}");
        }
    }
}
