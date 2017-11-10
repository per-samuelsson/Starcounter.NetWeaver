
using Mono.Cecil;

namespace Starcounter.Weaver {

    /// <summary>
    /// Factory used by weaver engine to instantiate custom weaving
    /// implementations.
    /// </summary>
    public interface IWeaverFactory {
        
        /// <summary>
        /// Provides weaver with the assembly analyzer to use during analysis.
        /// </summary>
        /// <param name="host">Weaver host</param>
        /// <param name="module">The module being analyzed.</param>
        /// <returns>An analyzer that can analyze the given module.</returns>
        IAssemblyAnalyzer ProvideAnalyzer(IWeaverHost host, ModuleDefinition module);

        /// <summary>
        /// Provides weaver with the rewriter to use to transform an assembly. 
        /// </summary>
        /// <param name="analysis">Result of previous analysis.</param>
        /// <returns>A rewriter that will rewrite the source module of the
        /// given analysis result, or null of rewriting is not supported.
        /// </returns>
        IAssemblyRewriter ProviderRewriter(AnalysisResult analysis);
    }
}