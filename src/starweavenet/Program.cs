using Starcounter.Weaver;
using System;
using System.IO;

namespace starweavenet {
    class Program {
        static int Main(string[] args) {
            if (args.Length == 0) {
                Console.WriteLine("Usage: starweavenet <assembly>");
                return 1;
            }

            var assemblyFile = args[0];
            var dir = Path.GetDirectoryName(assemblyFile);
            dir = Path.Combine(dir, ".starcounter");
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }


            var weaver = WeaverBuilder.BuildDefaultFromAssemblyFile(assemblyFile, dir);
            weaver.Weave();

            Console.WriteLine($"Weaved {assemblyFile} -> {dir}");

            return 0;
        }
    }
}