using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Patcher
{
    class Program
    {
        static Exception SourceChangedException
        {
            get
            {
                return new Exception("IR code has changed since last PluginLoader release.");
            }
        }

        static void Main(string[] args)
        {
            if (args.Length == 0 && !File.Exists("IR.exe"))
            {
                Console.WriteLine("Usage: pluginloaderpatch.exe <path to IR.exe> [<dest file>]");
                return;
            }
            var sourceFilePath = args.Length > 0 ? args[0] : "IR.exe";
            var destFilePath = args.Length > 1 ? args[1] : $"patched_{sourceFilePath}";

            AppDomain.CurrentDomain.AssemblyResolve += (sender, resource) =>
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var resources = assembly.GetManifestResourceNames().Where(f => f.EndsWith(".dll"));

                if (resources.Count() > 0)
                {
                    var resourceName = resources.First();
                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream == null) return null;
                        var block = new byte[stream.Length];
                        stream.Read(block, 0, block.Length);
                        return System.Reflection.Assembly.Load(block);
                    }
                }
                return null;
            };

            try
            {
                patch(sourceFilePath, destFilePath);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Cannot patch {sourceFilePath}: {ex.Message}");
                if (ex != SourceChangedException)
                {
                    Console.WriteLine($"{SourceChangedException.Message}");
                }
            }
            Console.ReadLine();
        }

        private static void patch(string sourceFilePath, string destFilePath)
        {
            using (MemoryStream assemblyStream = new MemoryStream(File.ReadAllBytes(sourceFilePath)))
            {
                var plAssembly = AssemblyDefinition.ReadAssembly("PluginLoader.dll");
                var plMainModule = plAssembly.MainModule;
                var plType = plMainModule.GetType("PluginLoader.PluginLoader");
                var plMethods = plType.Methods;

                var irAssembly = AssemblyDefinition.ReadAssembly(assemblyStream);
                var irMainModule = irAssembly.MainModule;
                var irAssemblyReferences = irMainModule.AssemblyReferences;
                var irControllerManager = irMainModule.GetType("Game.Server.ControllerManager");
                var irGameServer = irMainModule.GetType("Game.GameStates.GameServer");
                var irGameServerFields = irGameServer.Fields;
                var irGameServerMethods = irGameServer.Methods;

                if (Helpers.GetAssemblyByName(irAssemblyReferences.ToArray(), plAssembly.Name.Name) != null)
                {
                    Console.WriteLine($"{sourceFilePath} is probably already patched.");
                    return;
                }

                irAssemblyReferences.Add(plAssembly.Name);
                irGameServerFields.Add(new FieldDefinition("pluginLoader", FieldAttributes.Private, irMainModule.Import(plType)));

                var irM_controllersField = Helpers.GetFieldByName(irGameServerFields.ToArray(), "m_controllers");
                var irPluginLoaderField = Helpers.GetFieldByName(irGameServerFields.ToArray(), "pluginLoader");
                var plCtor = Helpers.GetMethodByName(plMethods.ToArray(), ".ctor");
                var irGameServerCtor = Helpers.GetMethodByName(irGameServerMethods.ToArray(), ".ctor");

                injectGameServerCtor(irGameServerCtor, irM_controllersField, irPluginLoaderField, irMainModule.Import(plCtor));

                var plTick = Helpers.GetMethodByName(plMethods.ToArray(), "Tick");
                var irGameServerUpdateOffthread = Helpers.GetMethodByName(irGameServerMethods.ToArray(), "UpdateOffthread");
                var irGet_TickrateInSeconds = Helpers.GetMethodByName(irGameServerMethods.ToArray(), "get_TickrateInSeconds");

                injectGameServerUpdateOffthread(irGameServerUpdateOffthread,
                    irPluginLoaderField, irGet_TickrateInSeconds, irMainModule.Import(plTick));

                irAssembly.Write(destFilePath);
                Console.WriteLine("Patched.");
                Console.WriteLine("Thank you Split Polygon!");
            }
        }

        private static void injectGameServerCtor(MethodDefinition method,
            FieldReference m_controllers, FieldReference pluginLoader, MethodReference pluginLoaderCtor)
        {
            var processor = method.Body.GetILProcessor();
            var instructions = new List<Instruction>();
            instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            instructions.Add(Instruction.Create(OpCodes.Ldfld, m_controllers));
            instructions.Add(Instruction.Create(OpCodes.Newobj, pluginLoaderCtor));
            instructions.Add(Instruction.Create(OpCodes.Stfld, pluginLoader));

            if (Helpers.GetInstructionByOffset(processor.Body, 256).OpCode != OpCodes.Callvirt ||
                Helpers.GetInstructionByOffset(processor.Body, 262).OpCode != OpCodes.Ldfld)
            {
                throw SourceChangedException;
            }
            else
            {
                Helpers.InjectInstructionsAfter(processor, instructions.ToArray(), 256);
            }
        }

        private static void injectGameServerUpdateOffthread(MethodDefinition method,
            FieldReference pluginLoader, MethodReference get_TickrateInSeconds, MethodReference Tick)
        {
            var processor = method.Body.GetILProcessor();
            var instructions = new List<Instruction>();
            instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            instructions.Add(Instruction.Create(OpCodes.Ldfld, pluginLoader));
            instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            instructions.Add(Instruction.Create(OpCodes.Call, get_TickrateInSeconds));
            instructions.Add(Instruction.Create(OpCodes.Callvirt, Tick));

            if (Helpers.GetInstructionByOffset(processor.Body, 0).OpCode != OpCodes.Ldarg_0 ||
                Helpers.GetInstructionByOffset(processor.Body, 1).OpCode != OpCodes.Dup ||
                Helpers.GetInstructionByOffset(processor.Body, 2).OpCode != OpCodes.Ldfld)
            {
                throw SourceChangedException;
            }
            else
            {
                Helpers.InjectInstructionsBefore(processor, instructions.ToArray(), 0);
            }
        }
    }
}
