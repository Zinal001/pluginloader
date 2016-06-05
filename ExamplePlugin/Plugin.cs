using PluginLoader;
using Game.Server;

namespace Plugin
{
    public class ExamplePlugin : PluginLoader.Plugin
    {
        private PluginLoader.PluginLoader pluginLoader;

        public ExamplePlugin() : base("Example Plugin", VersionInfo.GetVersionString()) {}
        public override void Initialize(PluginLoader.PluginLoader pluginLoader)
        {
            this.pluginLoader = pluginLoader;

            pluginLoader.ControllerManager.Players.OnAddPlayer += Players_OnAddPlayer;
        }

        public override bool HasNewVersion
        {
            get
            {
                return false;
            }
        }

        private void Players_OnAddPlayer(Player obj)
        {
            pluginLoader.ControllerManager.Chat.SendToPlayer(obj, "0000FF", $"This server uses PluginLoader v{VersionInfo.GetVersionString()}");
        }
    }
}
