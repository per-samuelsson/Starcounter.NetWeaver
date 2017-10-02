using Starcounter.Weaver;
using System;

namespace starweave
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0) {
                Console.WriteLine("Usage: starweave <assembly>");
                return 1;
            }

            var w = new AssemblyWeaver(args[0]);
            w.Weave();

            return 0;
        }
    }
}
