using System.IO;

namespace Starcounter.Weaver {

    public class TextWriterWeaverDiagnostics : WeaverDiagnostics {
        protected readonly TextWriter writer;
        protected readonly IDiagnosticsFormatter formatter;

        public TextWriterWeaverDiagnostics(TextWriter textWriter, IDiagnosticsFormatter errorAndWarningFormatter) {
            Guard.NotNull(writer, nameof(writer));
            Guard.NotNull(errorAndWarningFormatter, nameof(errorAndWarningFormatter));
            writer = textWriter;
            formatter = errorAndWarningFormatter;
        }

        public override void WriteError(string msg, string code = null) {
            writer.WriteLine(formatter.FormatError(msg, code));
        }

        public override void WriteWarning(string msg, string code = null) {
            writer.WriteLine(formatter.FormatWarning(msg, code));
        }

        public override void Trace(string msg) {
            writer.WriteLine(msg);
        }
    }
}