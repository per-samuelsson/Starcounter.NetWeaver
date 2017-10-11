
using Starcounter.Hosting.Schema;
using Xunit;

namespace Starcounter.Hosting.Tests {
    
    public class SchemaTests {

        [Fact]
        public void Basics() {
            new DatabaseSchema();
        }
    }
}