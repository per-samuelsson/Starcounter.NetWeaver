
using System;
using System.Collections.Generic;

namespace Starcounter.Hosting.Schema {

    public class DatabaseType {
        readonly Dictionary<string, DatabaseProperty> properties = new Dictionary<string, DatabaseProperty>();

        public DatabaseAssembly DefiningAssembly { get; internal set; }

        public string FullName { get; internal set; }

        public string BaseTypeName { get; internal set; }

        public IEnumerable<DatabaseProperty> Properties {
            get {
                return properties.Values;
            }
        }

        public void AddProperty(DatabaseProperty property) {
            if (property == null) {
                throw new ArgumentNullException(nameof(property));
            }
            properties.Add(property.Name, property);
            property.DeclaringType = this;
        }
    }
}