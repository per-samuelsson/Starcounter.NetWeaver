
namespace Starcounter.Weaver {

    public class MsBuildAdheringFormatter : IDiagnosticsFormatter {
        readonly string origin;
        readonly string defaultWarning;
        readonly string defaultError;

        public MsBuildAdheringFormatter(string messageOrigin, string defaultWarningCode = null, string defaultErrorCode = null) {
            Guard.NotNull(messageOrigin, nameof(messageOrigin));
            origin = messageOrigin;
            defaultWarning = defaultWarningCode ?? "SCW00001";
            defaultError = defaultErrorCode ?? "SCE00001";
        }

        // Using this format:
        // https://github.com/Microsoft/msbuild/blob/master/src/Shared/CanonicalError.cs
        //
        // Some background:
        // http://blogs.msdn.com/b/msbuild/archive/2006/11/03/msbuild-visual-studio-aware-error-messages-and-message-formats.aspx

        string IDiagnosticsFormatter.FormatError(string error, string code) {
            code = string.IsNullOrWhiteSpace(code) ? defaultError : code;
            return $"{origin}: error {code}: {error}";
        }

        string IDiagnosticsFormatter.FormatWarning(string warning, string code) {
            code = string.IsNullOrWhiteSpace(code) ? defaultWarning : code;
            return $"{origin}: warning {code}: {warning}";
        }
    }
}