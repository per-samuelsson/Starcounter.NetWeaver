
using Mono.Cecil;
using Starcounter.Hosting;
using Starcounter.Weaver;
using System;
using System.Linq;
using System.Reflection;

namespace starweave.Weaver {

    /// <summary>
    /// Implementation of <c>TargetRuntimeFacadeProvider</c> providing runtime facades
    /// by dynamically loading an asssembly and discover a single type in it, used as
    /// the implementation.
    /// </summary>
    public class AssemblyLoadTargetRuntimeProvider : TargetRuntimeFacadeProvider {
        readonly IWeaverHost host;
        readonly string targetAssemblyIdentity;

        public AssemblyLoadTargetRuntimeProvider(IWeaverHost weaverHost, string targetRuntimeAssemblyIdentity) {
            host = weaverHost ?? throw new ArgumentNullException(nameof(weaverHost));
            targetAssemblyIdentity = targetRuntimeAssemblyIdentity ?? throw new ArgumentNullException(nameof(targetRuntimeAssemblyIdentity));
        }

        public override bool IsTargetRuntimeReference(ModuleDefinition module) {
            return module.Name.Equals(targetAssemblyIdentity);
        }

        public override IAssemblyRuntimeFacade ProvideRuntimeFacade(ModuleDefinition targetReference) {

            var a = Assembly.Load(AssemblyName.GetAssemblyName(targetReference.FileName));
            var runtimeType = a.ExportedTypes.SingleOrDefault(t => IsValidRuntimeFacade(t));
            if (runtimeType == null) {
                // Error: the assembly contain no runtime type.
                // Improve error and probably write it to diagnostics instead?
                // TODO:
                throw new Exception($"Assembly {a.FullName} contain no runtime type");
            }

            var defaultCtor = runtimeType.GetConstructor(new Type[] { });
            return (IAssemblyRuntimeFacade)defaultCtor.Invoke(new object[] { });
        }

        public static bool IsValidRuntimeFacade(Type type) {
            if (type.IsAssignableFrom(typeof(IAssemblyRuntimeFacade))) {
                return !type.IsAbstract && type.GetConstructor(new Type[] { }) != null;
            }
            return false;
        }
    }
}
