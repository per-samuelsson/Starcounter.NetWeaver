
namespace Starcounter.Weaver.Tests.ExternalCode {

    public interface IDbProxyInExternalAssembly {

        ulong GetDbId();

        void SetDbId(ulong id);

        ulong GetDbRef();

        void SetDbRef(ulong @ref);
    }
}