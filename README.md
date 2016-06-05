## Synopsis

PluginLoader adds support for server-side plugins to [**Interstellar Rift**](http://interstellarrift.com/) from **Split Polygon**. Please note that it uses MSIL injection to edit original Interstellar Rift binary code so it can break your game executables.

## Motivation

Interstellar Rift is a great game. It feels like Space Engineers, Pulsar and Star Citizen had a child(at least to me). When I noticed that devs are really open-minded about modifying game, I've decided to write PluginLoader to support community.

Thank you **Split Polygon** for your attitude!

## Installation

Release directory tree:
* plugins/ExamplePlugin.dll
* PluginLoader.dll
* pluginloaderpatch.exe

ad 1)\
Move **whole plugins folder** into %AppData%/InterstellarRift/\
All plugin dlls are going in there!

ad 2)\
**PluginLoader.dll** and **pluginloaderpatch.exe** goes to your game directory(next to IR.exe)\
Now you have to start **pluginloaderpatch.exe**. It will output your new game executable. You can call it just like the original one(patched_ir -server).

## API Reference

Just check the code, it isn't anything special. For actual plugin writing you should inspect game assembly(JustDecompile). 

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
