using Ninject.Modules;
using System;

namespace ConsoleAppNetCore
{
    /// <summary>
    /// This code will force weaver to resolve dependency to Ninject,
    /// to be able to accurately assess if TestModule is a database class.
    /// </summary>
    public class TestModule : NinjectModule {
        public override void Load() {
            throw new NotImplementedException();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
