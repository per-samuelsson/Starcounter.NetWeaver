
namespace Starcounter.Weaver {

    public interface IDiagnosticsFormatter {

        string FormatError(string error, string code);

        string FormatWarning(string warning, string code);
    }
}