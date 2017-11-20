
using Starcounter2;

namespace ConsoleAppNetFrameworkSingleDbProperty {

    [Database]
    public class Foo {
        public int Bar { get; set; }
    }

    class Program {
        static void Main(string[] args) {
            var f = new Foo();
            f.Bar = 42;
            var v = f.Bar;
        }
    }
}