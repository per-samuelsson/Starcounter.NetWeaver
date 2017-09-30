using Starcounter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppNet {

    [Database] public class Foo {
        public string Bar { get; set; }

        public string Bar2 { get { return "Hej"; } }
    }

    class Program {
        static void Main(string[] args) {
            var f = new Foo();
            f.Bar = "hello";
            Console.WriteLine(f.Bar);
        }
    }
}
