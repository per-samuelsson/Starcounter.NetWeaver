﻿
using Mono.Cecil;
using Starcounter.Weaver.Runtime.Abstractions;

namespace starweave.Weaver {

    /// <summary>
    /// Provider of IAssemblyRuntimeFacade implementations.
    /// </summary>
    public abstract class TargetRuntimeFacadeProvider {

        public abstract bool IsTargetRuntimeReference(ModuleDefinition module);

        public abstract IAssemblyRuntimeFacade ProvideRuntimeFacade(ModuleDefinition targetReference);
    }
}