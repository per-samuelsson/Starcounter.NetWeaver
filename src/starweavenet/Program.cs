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

            var dir = Path.GetDirectoryName(args[0]);
            dir = Path.Combine(dir, ".starcounter");
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }

            var w = new AssemblyWeaver(args[0]);
            w.Weave(dir);

            Console.WriteLine($"Weaved {w.AssemblyPath} -> {dir}");

            return 0;
        }
    }
}