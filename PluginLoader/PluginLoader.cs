using System;
using System.Reflection;
using System.Collections.Generic;
using Game.Server;

namespace PluginLoader
{
    public delegate void PluginEvent();
    public delegate void ServerTickEvent(float dt);
    public class PluginLoader
    {
        /// <summary>
        /// Invoked when PluginLoader initializes all plugins.
        /// </summary>
        public event PluginEvent OnPluginLoaderStart;
        /// <summary>
        /// Invoked when GameServer calls tick update(before actual update).
        /// </summary>
        public event ServerTickEvent OnServerTick;

        private readonly List<Plugin> plugins = new List<Plugin>();
        public List<Plugin> Plugins
        {
            get
            {
                return plugins;
            }
        }
        private readonly ControllerManager controllerManager;
        public ControllerManager ControllerManager
        {
            get
            {
                return controllerManager;
            }
        }

        public PluginLoader(ControllerManager controllerManager)
        {
            this.controllerManager = controllerManager;

            Console.WriteLine($"Initializing PluginLoader v{VersionInfo.GetVersionString()}");
            if (VersionInfo.GetLatestCommitHash() != VersionInfo.GetCommitHash())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"PluginLoader has new version available. Check {VersionInfo.GitLink}");
                Console.ResetColor();
            }
            #pragma warning disable CC0021
            var dlls = GetDLLFiles("plugins");
            #pragma warning restore CC0021
            Console.WriteLine($"Found {dlls.Length} plugins.");
            Console.WriteLine();

            foreach (string dll in dlls)
            {
                var pluginDLLName = System.IO.Path.GetFileName(dll);
                Console.WriteLine($"Loading {pluginDLLName}");

                var assembly = Assembly.LoadFile(System.IO.Path.GetFullPath(dll));
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.BaseType == typeof(Plugin))
                    {
                        try {
                            this.plugins.Add((Plugin)Activator.CreateInstance(type));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
                Console.WriteLine();

                foreach (Plugin plugin in this.plugins)
                {
                    Console.WriteLine($"Initializing {plugin.Info.ToString()}");
                    try {
                        if (plugin.HasNewVersion)
                        {
                            var newVersionLink = "";
                            if (plugin.Info.URL != String.Empty)
                            {
                                newVersionLink = $"Check {plugin.Info.URL}";
                            }

                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"{plugin.Info.Name} has new version available. {newVersionLink}");
                            Console.ResetColor();
                        }
                        plugin.Initialize(this);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        Console.WriteLine($"Removing plugin {plugin.Info.Name} from plugin list.");
                        this.plugins.Remove(plugin);
                    }
                }
                Console.WriteLine();
            }
            this.OnPluginLoaderStart?.Invoke();
        }

        private static string[] GetDLLFiles(string directory)
        {
            return System.IO.Directory.GetFiles(directory, "*.dll");
        }

        public void Tick(float dt)
        {
            OnServerTick?.Invoke(dt);
        }
    }
}
