
namespace Starcounter.Weaver {

    public class DefaultWeaverHost : IWeaverHost {
        readonly WeaverDiagnostics diagnostics;

        public WeaverDiagnostics Diagnostics => diagnostics;

        public DefaultWeaverHost(WeaverDiagnostics weaverDiagnostics) {
            Guard.NotNull(weaverDiagnostics, nameof(weaverDiagnostics));
            diagnostics = weaverDiagnostics;
        }
    }
}