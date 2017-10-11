using System.Collections.Generic;

namespace Starcounter.Hosting.Schema {
    public class DatabaseType {

        public DatabaseAssembly DefiningAssembly { get; set; }

        public string FullName { get; set; }

        public IEnumerable<DatabaseProperty> Properties { get; }
    }
}
