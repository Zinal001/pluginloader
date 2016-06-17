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
using Game.Framework;
using IronPython.Hosting;
using System;
using System.Linq;
using System.Reflection;

namespace PluginLoader
{
    public delegate void PLEvent(object arg = null);
    public class PluginManager
    {
        private readonly PluginInjector pluginInjector;

        public event PLEvent OnClientHotReload;
        public event PLEvent OnEditorHotReload;
        public event PLEvent OnMenuHotReload;
        public event PLEvent OnServerHotReload;

        public event PLEvent OnAgosGuiInitialized;

        public event PLEvent OnGameClientActivated;
        public event PLEvent OnGameClientDeactivated;
        public event PLEvent OnGameClientInitialized;
        public event PLEvent OnGameClientUnload;
        public event PLEvent OnGameClientUpdate;

        public event PLEvent OnGameEditorInitialized;
        public event PLEvent OnGameEditorUnload;
        public event PLEvent OnGameEditorUpdate;

        public event PLEvent OnGameMenuInitialized;

        public event PLEvent OnGameServerInitialized;
        public event PLEvent OnGameServerUnload;
        public event PLEvent OnGameServerUpdate;

        private readonly PackageManager PackageManager = new PackageManager();

        private readonly Logger log;
        public Logger Log => log;

        private Game.GameStates.GameClient gameClient;
        private Game.GameStates.GameShipEditor gameEditor;
        private Game.GameStates.GameMainMenu gameMenu;
        private Game.GameStates.GameServer gameServer;
        private bool initialized;

        internal PluginManager(PluginInjector pluginInjector)
        {
            this.pluginInjector = pluginInjector;
            log = pluginInjector.Log;

            pluginInjector.OnAgosGuiInitialized += delegate (object arg) { OnAgosGuiInitialized?.Invoke(arg); };

            pluginInjector.OnGameClientActivated += delegate (object arg) { OnGameClientActivated?.Invoke(arg); };
            pluginInjector.OnGameClientDeactivated += delegate (object arg) { OnGameClientDeactivated?.Invoke(arg); };
            pluginInjector.OnGameClientInitialized += delegate (object arg)
            {
                init();

                unsetGameStates();
                gameClient = (Game.GameStates.GameClient)arg;
                new GameClient(this, gameClient).StartGameMode();

                OnGameClientInitialized?.Invoke(gameClient);
            };
            pluginInjector.OnGameClientUnload += delegate (object arg) { OnGameClientUnload?.Invoke(arg); };
            pluginInjector.OnGameClientUpdate += delegate (object arg) { OnGameClientUpdate?.Invoke(arg); };

            pluginInjector.OnGameEditorInitialized += delegate (object arg)
            {
                init();

                unsetGameStates();
                gameEditor = (Game.GameStates.GameShipEditor)arg;
                new GameEditor(this, gameEditor).StartGameMode();

                OnGameEditorInitialized?.Invoke(arg);
            };
            pluginInjector.OnGameEditorUnload += delegate (object arg) { OnGameEditorUnload?.Invoke(arg); };
            pluginInjector.OnGameEditorUpdate += delegate (object arg) { OnGameEditorUpdate?.Invoke(arg); };

            pluginInjector.OnGameMenuInitialized += delegate (object arg)
            {
                init();

                unsetGameStates();
                gameMenu = (Game.GameStates.GameMainMenu)arg;
                new GameMenu(this, gameMenu).StartGameMode();

                OnGameMenuInitialized?.Invoke(arg);
            };

            pluginInjector.OnGameServerInitialized += delegate (object arg)
            {
                init();

                unsetGameStates();
                gameServer = (Game.GameStates.GameServer)arg;
                new GameServer(this, gameServer).StartGameMode();

                OnGameServerInitialized?.Invoke(arg);
            };
            pluginInjector.OnGameServerUnload += delegate (object arg) { OnGameServerUnload?.Invoke(arg); };
            pluginInjector.OnGameServerUpdate += delegate (object arg) { OnGameServerUpdate?.Invoke(arg); };
        }

        private void init()
        {
            if (initialized)
                return;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Starting {nameof(PluginLoader)} v{Versions.PluginLoaderVersion}");
            Console.ResetColor();
            Serialization.GlobalSerializer.AddTypes(NetSerializable.GetTypesWithAttribute(Assembly.GetExecutingAssembly()));

            initialized = true;
        }

        private void loadPackages()
        {
            unsubscribeEvents(OnClientHotReload);
            unsubscribeEvents(OnEditorHotReload);
            unsubscribeEvents(OnMenuHotReload);
            unsubscribeEvents(OnServerHotReload);

            unsubscribeEvents(OnAgosGuiInitialized);

            unsubscribeEvents(OnGameClientActivated);
            unsubscribeEvents(OnGameClientDeactivated);
            unsubscribeEvents(OnGameClientInitialized);
            unsubscribeEvents(OnGameClientUnload);
            unsubscribeEvents(OnGameClientUpdate);

            unsubscribeEvents(OnGameEditorInitialized);
            unsubscribeEvents(OnGameEditorUnload);
            unsubscribeEvents(OnGameEditorUpdate);

            unsubscribeEvents(OnGameMenuInitialized);

            unsubscribeEvents(OnGameServerInitialized);
            unsubscribeEvents(OnGameServerUnload);
            unsubscribeEvents(OnGameServerUpdate);

            Console.WriteLine($"[{nameof(PluginManager)}] Looking for packages");
            PackageManager.ScanForPackages();
            PackageManager.Packages.ForEach(package => Console.WriteLine($"[{nameof(PluginManager)}] Loaded {package.ToString()}"));

            Console.WriteLine($"[{nameof(PluginManager)}] Solving dependencies");

            var unmetDependencies = DependencySolver.Solve(PackageManager.Packages.ToArray());
            if(unmetDependencies.Length > 0)
            {
                unmetDependencies = DependencySolver.GetRelevantUnmetDependencies(unmetDependencies);

                Console.ForegroundColor = ConsoleColor.Red;
                unmetDependencies.ToList().ForEach(unmetDependency =>
                    Console.WriteLine($"[{nameof(PluginManager)}] {unmetDependency.ToString()}"));
                Console.WriteLine($"[{nameof(PluginManager)}] Please resolve issues and restart the game");
                Console.ResetColor();
                while (true)
                {
                    Console.ReadLine();
                }
            }

            try
            {
                PackageManager.Sort();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{nameof(PluginManager)}] Could not sort dependencies");
                Console.WriteLine(ex.Message);
                Console.WriteLine($"[{nameof(PluginManager)}] Please resolve issues and restart the game");
                Console.ResetColor();
                while (true)
                {
                    Console.ReadLine();
                }
            }
        }

        public void Start()
        {
            try
            {
                var reload = PackageManager.Packages != null && PackageManager.Packages.Count > 0;
                if (!reload)
                    Console.WriteLine($"[{nameof(PluginManager)}] START");
                else
                    Console.WriteLine($"[{nameof(PluginManager)}] RESTART");

                loadPackages();
                var packages = PackageManager.Packages.ToArray();
                var paths = PackageManager.Packages.ConvertAll(package =>
                    package.Path.Split('\\').Last()).ToArray();

                var engine = Python.CreateEngine();
                var searchPaths = engine.GetSearchPaths();
                searchPaths.Add(PackageManager.RootDir);
                engine.SetSearchPaths(searchPaths);

                Console.WriteLine($"[{nameof(PluginManager)}] Executing packages");
                var scope = engine.CreateScope();

                #pragma warning disable CC0021 // Use nameof
                engine.Runtime.Globals.SetVariable("GameDir", PackageManager.GameDir);
                engine.Runtime.Globals.SetVariable("Globals", engine.Runtime.Globals.GetVariableNames());
                engine.Runtime.Globals.SetVariable("Packages", packages);
                engine.Runtime.Globals.SetVariable("PluginManager", this);
                engine.Runtime.Globals.SetVariable("RootDir", PackageManager.RootDir);
                engine.Runtime.Globals.SetVariable("Versions", typeof(Versions));
                #pragma warning restore CC0021 // Use nameof

                paths.ToList().ForEach((string module) => {
                    try
                    {
                        engine.Execute($"from {module} import *", scope);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[{nameof(PluginManager)}] Error while executing {module}");
                        Console.WriteLine(ex);
                    }
                });
                Console.WriteLine($"[{nameof(PluginManager)}] All packages invoked");

                if (reload && gameClient != null)
                    OnClientHotReload?.Invoke(gameClient);
                if (reload && gameEditor != null)
                    OnEditorHotReload?.Invoke(gameEditor);
                if (reload && gameMenu != null)
                    OnMenuHotReload?.Invoke(gameMenu);
                if (reload && gameServer != null)
                    OnServerHotReload?.Invoke(gameServer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{nameof(PluginManager)}] Error while starting plugins");
                Console.WriteLine(ex);
            }
        }

        private static void unsubscribeEvents(PLEvent plevent)
        {
            if (plevent != null)
                plevent.GetInvocationList().ToList().ForEach(d => plevent -= (PLEvent)d);
        }

        private void unsetGameStates()
        {
            gameClient = null;
            gameEditor = null;
            gameMenu = null;
            gameServer = null;
        }
    }
}
