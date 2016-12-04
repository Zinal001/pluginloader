using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginLoader
{
    public sealed class PluginGlobal
    {
        public String GameDir { get; private set; }
        public Package[] Packages { get; private set; }
        public PluginManager PluginManager { get; private set; }
        public String RootDir { get; private set; }

        private Dictionary<String, Object> _Values = new Dictionary<String, Object>();

        public Object this[String key]
        {
            get
            {
                if (_Values.ContainsKey(key))
                    return _Values[key];

                return null;
            }
            set
            {
                if (_Values.ContainsKey(key))
                    _Values[key] = value;
                else
                    _Values.Add(key, value);
            }
        }

        public PluginGlobal(String GameDir, Package[] Packages, PluginManager PluginManager, String RootDir)
        {
            this.GameDir = GameDir;
            this.Packages = Packages;
            this.PluginManager = PluginManager;
            this.RootDir = RootDir;
        }

        public String[] Keys()
        {
            String[] Keys = new String[_Values.Count + 4];
            Keys[0] = "GameDir";
            Keys[1] = "Packages";
            Keys[2] = "PluginManager";
            Keys[3] = "RootDir";
            _Values.Keys.CopyTo(Keys, 4);

            return Keys;
        }

    }
}
