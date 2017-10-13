
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

        public virtual void WriteError(string msg, string code = null) {

        }

        public virtual void WriteWarning(string msg, string code = null) {

        }

        public virtual void Trace(string msg) {

        }
    }
}