
namespace Starcounter.Weaver {
    /// <summary>
    /// Just some basic diagnstics API to start from.
    /// </summary>
    public class WeaverDiagnostics {

        public static WeaverDiagnostics Quiet {
            get {
                return new WeaverDiagnostics();
            }
        }

        public virtual void WriteError(string msg) {

        }

        public virtual void WriteWarning(string msg) {

        }

        public virtual void Trace(string msg) {

        }
    }
}