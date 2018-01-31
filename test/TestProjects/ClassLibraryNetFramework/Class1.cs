using Ninject.Modules;
using Starcounter.ReferenceRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryNetFramework
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

    [Database] public class Class1
    {
    }
}
