
using Mono.Cecil;
using System;
using System.IO;

namespace SymbolFileWritten {

    // Read current assembly with symbols
    // Write current assembly out to other dir, with symbols.
    // Check there is a pdb there as expected.

    class Program {

        static int Main(string[] args) {
            var rewriters = new Action<ModuleDefinition>[] {
                (m) => {},
                AddSingleType,
            };

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var assemblyPath = assembly.Location;
            var outDirectory = Path.Combine(Path.GetDirectoryName(assemblyPath), ".starcounter");
            var outFile = Path.Combine(outDirectory, Path.GetFileName(assemblyPath));

            Console.WriteLine($"SymbolFileWritten test: {assemblyPath}");

            var result = 0;
            foreach (var action in rewriters) {
                result = TestCase(assemblyPath, outDirectory, outFile, action);
                if (result != 0) {
                    break;
                }
            }
            
            if (result != 0) {
                Console.Error.WriteLine("SymbolFileWritten test failed.");
                return 1;
            }

            Console.WriteLine("SymbolFileWritten test succeeded.");
            return 0;
        }

        static int TestCase(string assemblyPath, string outputDirectory, string outputAssembly, Action<ModuleDefinition> rewriter) {
            Console.WriteLine($"Testing {rewriter.Method.Name}");

            try {
                Directory.Delete(outputDirectory, true);
            }
            catch (DirectoryNotFoundException) { }
            Directory.CreateDirectory(outputDirectory);

            var readerParameters = new ReaderParameters { ReadSymbols = true };
            var definition = ModuleDefinition.ReadModule(assemblyPath, readerParameters);

            rewriter(definition);

            var writerParameters = new WriterParameters { WriteSymbols = true };
            definition.Write(outputAssembly, writerParameters);

            if (!File.Exists(outputAssembly)) {
                Console.Error.WriteLine($"Output assembly {outputAssembly} missing.");
                return 1;
            }

            var symbolFile = Path.ChangeExtension(outputAssembly, ".pdb");
            if (!File.Exists(symbolFile)) {
                Console.Error.WriteLine($"Output symbol file {symbolFile} missing.");
                return 1;
            }
            
            return 0;
        }

        static void AddSingleType(ModuleDefinition module) {
            var type = new TypeDefinition("test", "test", TypeAttributes.Class | TypeAttributes.Public);
            module.Types.Add(type);
        }
    }
}