
namespace Starcounter.Weaver.Runtime.Abstractions {

    /// <summary>
    /// Represent state for a given database type as governed by the
    /// weaver. Used by hosts to initialize shared/static data of identified
    /// database types.
    /// </summary>
    public interface IDatabaseTypeState {

        ulong CreateHandle { get; set; }

        ulong DeleteHandle { get; set; }

        ulong GetPropertyHandle(string declaredPropertyName);

        void SetPropertyHandle(string declaredPropertyName, ulong handle);
    }
}
