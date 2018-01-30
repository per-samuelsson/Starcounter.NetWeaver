
namespace Starcounter2.Internal {
    /// <summary>
    /// Provide what all routed interfaces will need in terms of state from
    /// the pass-through type (typed as this). Weaver will explicitly implement
    /// this and have knowledge on how to do that.
    /// </summary>
    public interface IDbProxyState {

        ulong GetDbId();

        void SetDbId(ulong id);

        ulong GetDbRef();

        void SetDbRef(ulong id);
    }
}