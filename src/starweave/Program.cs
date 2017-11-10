
using Starcounter.Weaver;
using System;
using System.IO;
using Mono.Cecil;
using Starcounter.Hosting.Schema;
using Starcounter.Weaver.Rewriting;
using System.Collections.Generic;
using System.Linq;
using starweave.Weaver;

namespace starweave {
    
    class Program {

        static int Main(string[] args) {
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
            
            var weaver = WeaverBuilder.BuildDefaultFromAssemblyFile(assemblyFile, dir, new StarcounterWeaverFactory());
            // weaver.Weave();

            Console.WriteLine($"Weaved {assemblyFile} -> {dir}");

            return 0;
        }
    }
}