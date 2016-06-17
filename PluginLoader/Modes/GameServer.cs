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
namespace PluginLoader
#pragma warning restore CC0065 // Remove trailing whitespace
#pragma warning restore CC0065 // Remove trailing whitespace
#pragma warning restore CC0065 // Remove trailing whitespace
#pragma warning restore CC0065 // Remove trailing whitespace
{
    class GameServer : GameMode
    {
        private readonly Game.GameStates.GameServer gameServer;
        public GameServer(PluginManager pluginManager, Game.GameStates.GameServer gameServer) : base(pluginManager)
        {
            this.gameServer = gameServer;
        }
        public override void StartGameMode()
        {
            base.StartGameMode();

            pluginManager.Start();

            gameServer.Controllers.Players.OnAddPlayer += Players_OnAddPlayer;

            //foreach (Package package in pluginManager.PackageManager.Packages)
            //{
            //    Console.WriteLine($"[{nameof(PluginManager)}] Pushing {filename} into filesystem");
            //    AddFileToFS(gameServer.Controllers.Network.NetFS, nameof(PluginLoader), filename, File.ReadAllBytes(package.ArchivePath));
            //}

            //var welcomePacket = new ServerPLWelcome
            //{
            //};
            //this.gameServer.Controllers.Players.OnAddPlayer += delegate (Game.Server.Player player)
            //{
            //    player.SendRPC(welcomePacket);
            //};
        }

        private void Players_OnAddPlayer(Game.Server.Player obj)
        {
            gameServer.Controllers.Chat.SendToPlayer(obj, Game.Configuration.Config.Singleton.NotificationChatColor, $"This server is using PluginLoader v{Versions.PluginLoaderVersion}", nameof(PluginLoader), "Server");
        }
    }
}
