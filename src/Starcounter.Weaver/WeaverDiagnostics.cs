namespace Starcounter.Weaver {

    /// <summary>
    /// Provide weaver with methods for writing errors, warnings and
    /// diagnostics.
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