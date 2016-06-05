## Synopsis

PluginLoader adds support for server-side plugins to [**Interstellar Rift**](http://interstellarrift.com/) from **Split Polygon**. Exposes `Game.Server.ControllerManager` and server tick event.

*Please note that PL uses MSIL injection to edit original Interstellar Rift binary code. It can break your game executables.*

## Motivation

Interstellar Rift is a great game! It feels like Space Engineers, Pulsar and Star Citizen had a child(at least to me). When I noticed that devs are really open-minded about modifying game, I've decided to write PluginLoader to support community.

Thank you **Split Polygon** for your attitude! :) (and for not obfuscating)

## Game Version

Tested for Interstellar Rift version **0.1.23c1**  
When a new game version is published, try to repatch binaries. There is some chance it will work just normally. Otherwise you will have to wait for PluginLoader update.

## Installation

Release tree:
```
   plugins/
        ExamplePlugin.dll
   PluginLoader.dll
   pluginloaderpatch.exe
   README.txt
```

1. Locate *%AppData%/InterstellarRift/* dir and copy whole *plugins* folder here.
2. Locate Interstellar Rift installation dir and copy *PluginLoader.dll* and *pluginloaderpatch.exe* there. Stay in dir for next step.
3. CMD > `pluginloaderpatch IR.exe newIR.exe`
4. Start server with the new executable > `newIR -server`
5. You should see something like this:  
![Dedicated Server Console](http://i.imgur.com/bEg5aEc.png)
6. If you now join to the server, you will see:  
![Game Chat](http://i.imgur.com/OlwH2ux.png)

## API Reference

Just check the code, it isn't anything special. For actual plugin writing you should inspect game assembly(JustDecompile).

## TODO

- Better version controlling
- Decide whether TickEvent is usefull or not

## Donate

Donations are greatly appreciated! I am just a poor medical student and probably will have to sell a kidney or two.  
[Donate via PayPal](http://bit.ly/1t9MBsj)

## License

The MIT License (MIT)

Copyright (c) 2016 Tomas Bosek

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
