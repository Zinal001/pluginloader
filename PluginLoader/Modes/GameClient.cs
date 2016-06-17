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
    class GameClient : GameMode
    {
        private readonly Game.GameStates.GameClient gameClient;

        public GameClient(PluginManager pluginManager, Game.GameStates.GameClient gameClient) : base(pluginManager)
        {
            this.gameClient = gameClient;
        }
        public override void StartGameMode()
        {
            base.StartGameMode();

            pluginManager.Start();

            //gameClient.Controllers.Network.Net.RpcDispatcher.Functions[typeof(ServerPluginLoaderWelcome)] =
            //    new RPCDelegate(delegate (RPCData data)
            //    {
            //        var deserializedObject = (ServerPluginLoaderWelcome)data.DeserializedObject;
            //        this.pluginManager.Log.Info($"Server is using PluginLoader v{deserializedObject.Version.ToString()}", "PluginLoader");
            //        deserializedObject.PluginList.ForEach(delegate(string dll)
            //        {
            //            this.pluginManager.Log.Info($"Requesting plugin {dll}", "PluginLoader");
            //            var loadingScreen = Globals.loadingScreen;
            //            Globals.loadingScreen.RenderLoadingScreenOnce(0, "Loading plugins");
            //            this.gameClient.Controllers.Network.NetFS.Download(
            //                this.gameClient.Controllers.Network.Net.ServerConnection(), $"pluginloader_{dll.ToLower()}", "pluginloader", dll, true, delegate (NetFilesystem.TransferStatus status, NetFilesystem.TransferState state)
            //                {
            //                });
            //        });
            //    });
        }
    }
}