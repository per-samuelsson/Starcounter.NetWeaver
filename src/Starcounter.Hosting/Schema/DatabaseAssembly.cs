using System.Collections.Generic;

namespace Starcounter.Hosting.Schema {
    public class DatabaseAssembly {
        public string Name { get; set; }

        public IEnumerable<DatabaseType> Types { get; }
    }
}