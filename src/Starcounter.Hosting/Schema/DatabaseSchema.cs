
using System.Collections.Generic;

namespace Starcounter.Hosting.Schema {
    public sealed class DatabaseSchema {
        public IEnumerable<DatabaseAssembly> Assemblies { get; }
    }
}