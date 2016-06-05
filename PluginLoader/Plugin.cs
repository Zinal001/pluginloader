namespace PluginLoader
{
    /// <summary>
    /// Stores basic info about plugin name, it's author and URL where you can find more detailed info.
    /// </summary>
    public struct PluginInfo
    {
        public string Name;
        public string URL;
        public string Version;
        public override string ToString()
        {
            return $"{Name} v{Version}";
        }
    }
    /// <summary>
    /// The most important part of the plugin.
    /// PluginLoader looks for classes which inherit from this abstract class and invokes them.
    /// You can define unlimited amount of Plugin objects within one DLL. PluginLoader calls them all.
    /// </summary>
    public abstract class Plugin
    {
        private readonly PluginInfo info;
        public PluginInfo Info
        {
            get
            {
                return info;
            }
        }

        /// <summary>
        /// Helps PluginLoader decide if the plugin has a new version available.
        /// </summary>
        public abstract bool HasNewVersion { get; }

        protected Plugin(string name, string version, string url = "")
        {
            this.info = new PluginInfo();
            this.info.Name = name;
            this.info.Version = version;
            this.info.URL = url;
        }

        /// <summary>
        /// This starts up the plugin itself.
        /// </summary>
        /// <param name="pluginLoader"></param>
        public abstract void Initialize(PluginLoader pluginLoader);
    }
}
