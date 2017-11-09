
namespace Starcounter.Weaver {

    /// <summary>
    /// Represent the running weaver. Provided to weaver implementations
    /// during weaving.
    /// </summary>
    public interface IWeaverHost {

        WeaverDiagnostics Diagnostics { get; }
    }
}