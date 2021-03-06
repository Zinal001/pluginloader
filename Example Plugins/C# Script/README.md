##C# PLUGINS

###Required Files

* **plugin.json**

      The _plugin.json_ file is simple text file which is required by every plugin (C# and Python).

      The format is as follows:
   ```json
{
         "UniqueName": "[uniqueName]",
         "PrettyName": "[prettyName]",
         "Version": "[version]",
         "PluginLoader": "[pluginLoaderVersion]",
         "Dependencies": [],
         "PackageType": "CScript"
}
```

      Let break it down:
      
      **[uniqueName]**

      The unique name of the plugin. This has to be unique across all plugins.

      **[prettyName]**

      The pretty name of the plugin. (Can be anything really)

      **[version]**

      The version of the plugin in the following format: x.y.z.
      Example: 0.1.0

      **[pluginLoaderVersion]**

      The PluginLoader version that is required for this plugin to work. Same format as above.

      _Current version is: **0.10.0**_

      **Dependencies**

      A list of other plugin's unique name that is required to be loaded before this plugin can be loaded.

      The dependencies should be in the following format:
      ```json
"Dependencies": [ "PLUGIN1", "PLUGIN2", "PLUGIN3" ]
```

      **PackageType**

      The type of this plugin, should be either **"CScript"** for a C# script or **"Python"** for a Python script.



* **\_\_init\_\_.cs**

   The script file for the plugin.
   
   A basic template for a plugin can be seen below:
   ```C#
   using System;
   using PluginLoader;

namespace Plugins
{
      public class ExamplePlugin : PluginClass
      {
          public override void Init()
          {
              Console.WriteLine("The Example Plugin has been successfully loaded!");
              Global.PluginManager.OnGameServerInitialized += PluginManager_OnGameServerInitialized;
          }
          
          private void PluginManager_OnGameServerInitialized(object arg = null)
          {
              Game.GameStates.GameServer gameServer = (Game.GameStates.GameServer)arg;
              Console.WriteLine("The Game Server has been initialized!");
          }
      }
}
   
   ```
