﻿
using Starcounter.Weaver;
using System;
using System.IO;
using starweave.Weaver;
using System.Diagnostics;
using Starcounter2.Internal;
using System.Reflection;
using System.Linq;

namespace starweave {

    class Program {

        public static void CheckForDebugSwitch(ref string[] args) {
            if (args.Length > 0) {
                var first = args[0].TrimStart('-');
                if (first.Equals("sc-debug", StringComparison.InvariantCultureIgnoreCase)) {
                    Debugger.Launch();
                    var stripped = new string[args.Length - 1];
                    Array.Copy(args, 1, stripped, 0, args.Length - 1);
                    args = stripped;
                }
            }
        }

        static int Main(string[] args) {
            CheckForDebugSwitch(ref args);

            // The default weaver use a strategy that identify database classes using
            // a custom attribute. Each auto-implemented property in that will be considered
            // a database property. What we need from the analyzer from the target runtime
            // is then: 1, the name of the runtime (so we can locate stuff) and 2, the type
            // of the database attribute. With that, we can carry out analysis. We expect
            // the shared database schema model (i.e. Starcounter Hosting).
            
            if (args.Length == 0) {
                Console.Error.WriteLine("Usage: starweave <assembly>");
                return 1;
            }

            var assemblyFile = args[0];
            if (!File.Exists(assemblyFile)) {
                Console.Error.WriteLine($"File not found: {assemblyFile}");
                Console.Error.WriteLine("Usage: starweave <assembly>");
                return 1;
            }

            var dir = Path.GetDirectoryName(assemblyFile);
            dir = Path.Combine(dir, ".starcounter");
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            
            var weaverFactory = new StarcounterWeaverFactory("Starcounter2.dll", new DatabaseTypeStateNames());
            var weaver = WeaverBuilder.BuildDefaultFromAssemblyFile(assemblyFile, dir, weaverFactory);
            weaver.Weave();

            Console.WriteLine($"Weaved {assemblyFile} -> {dir}");

            return 0;
        }
    }
}