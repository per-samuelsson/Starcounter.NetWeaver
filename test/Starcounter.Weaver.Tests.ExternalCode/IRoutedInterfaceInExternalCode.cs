
namespace Starcounter.Weaver.Tests.ExternalCode {

    public class ExternalParameterType { }

    public interface IRoutedInterfaceInExternalCode {

        ulong Test();

        bool TestWithExternalParameter(ExternalParameterType t);
    }
}