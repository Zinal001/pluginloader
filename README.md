## Synopsis

PluginLoader adds support for IronPython plugins and plain C# scripts to [**Interstellar Rift**](http://interstellarrift.com/) from **Split Polygon**.

*Please note that PL uses MSIL injection to edit original Interstellar Rift binary code. It can break your game executables.*

## Motivation

Interstellar Rift is a great game! It feels like Space Engineers, Pulsar and Star Citizen had a child(at least to me). When I noticed that devs are really open-minded about modifying game, I've decided to write PluginLoader to support community.

Thank you **Split Polygon** for your attitude! :) (and for not obfuscating)

## Game Version

When a new game version is published, try to repatch binaries. There is a chance it will work as it should. If not, I'll do my best to release new version ASAP.

## Modding

Only server-side hooks work for now. Client-side is under development and will be in one of next releases. We want to be sure it is working properly without too much fiddling with files(on client).

If you are interested in modding and would like to help us test/develop some mods for client-side, contact me via [bosektom@gmail.com](mailto:bosektom@gmail.com)

## Installation

Release usually contains:
```
	Patcher.exe
	PatcherGUI.exe
```
For console interface run **Patcher.exe**  
For graphical interface run **PatcherGUI.exe**  

1. When asked, provide path to game executables(usually `Steam/steamapps/common/Interstellar Rift/Build/`)
2. Start server with the new executable > `IR -server`
3. You should see something like this:  
![Dedicated Server Console](http://i.imgur.com/pbJ2npr.png)

## Adding plugins

1. Locate Interstellar Rift user-related data directory. It should be something like `%appdata%/InterstellarRift/`  
2. Check if **plugins** directory exists. If not, create it.
3. Copy folder with plugin into **plugins** directory.
4. You should see something like this(sorry for weird language):  
![Plugins directory](http://i.imgur.com/0YDImc4.png)
5. Content:  
![ConsoleExtension directory](http://i.imgur.com/500oLcE.png)

Name of plugin folder(*ConsoleExtension* in this case) is not important. You can name it as you wish.

Please note that if plugin folder does not contain valid `__init__.py` and `plugin.json`, it will not be loaded!


## API Reference

We are currently working on some basic API docs for Python. For more game API info use ILspy or JustDecompile. We are still trying to find out, what can we achieve.

## Donate

Donations are greatly appreciated!  
[Donate via PayPal](http://bit.ly/1rtm7Ac)

## License

AGPLv3

Copyright (c) 2016 Tomas Bosek

PluginLoader is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as
published by the Free Software Foundation, either version 3 of the
License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.

