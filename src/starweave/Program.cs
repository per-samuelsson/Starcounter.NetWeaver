using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter.Weaver;

namespace starweave {
    class Program {
        static void Main(string[] args) {
            var weaver = new AssemblyWeaver(System.IO.Path.GetFullPath(@"..\..\..\AppNet\bin\debug\AppNet.exe"));
            weaver.Weave();
        }
    }
}
