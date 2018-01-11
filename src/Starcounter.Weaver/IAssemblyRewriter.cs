
using Starcounter.Weaver.Runtime;

namespace Starcounter.Weaver {

    public interface IAssemblyRewriter {

        void RewriteType(DatabaseType type);
    }
}
