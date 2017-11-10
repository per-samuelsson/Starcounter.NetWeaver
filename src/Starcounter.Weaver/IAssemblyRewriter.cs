
using Starcounter.Hosting.Schema;

namespace Starcounter.Weaver {

    public interface IAssemblyRewriter {

        void RewriteType(DatabaseType type);
    }
}
