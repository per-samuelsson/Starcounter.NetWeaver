
using System;
using System.Reflection;

namespace WeaverOutputToMsBuildErrorsAndWarnings {

    // After this project build, as part of AfterBuild, we'll run the exe and capture
    // its output, producing MsBuild errors and warnings from it.

    // Using this format:
    // https://github.com/Microsoft/msbuild/blob/master/src/Shared/CanonicalError.cs
    //
    // Some background:
    // http://blogs.msdn.com/b/msbuild/archive/2006/11/03/msbuild-visual-studio-aware-error-messages-and-message-formats.aspx

    class Program {
        static void Main(string[] args) {
            Console.Error.WriteLine($"{Assembly.GetExecutingAssembly().Location}: warning CS9999: This is a warning");
            // Console.Error.WriteLine($"{Assembly.GetExecutingAssembly().Location}: error CS9999: This is an error");
            // Environment.ExitCode = 1;
        }
    }
}