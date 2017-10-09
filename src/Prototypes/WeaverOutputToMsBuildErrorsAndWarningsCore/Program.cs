using System;
using System.Reflection;

namespace WeaverOutputToMsBuildErrorsAndWarningsCore {
    class Program {
        static void Main(string[] args) {
            Console.Error.WriteLine($"{Assembly.GetExecutingAssembly().Location}: warning CS9999: This is a warning");
            Console.Error.WriteLine($"{Assembly.GetExecutingAssembly().Location}: error CS9999: This is an error");
            Environment.ExitCode = 1;
        }
    }
}
