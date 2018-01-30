
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Starcounter.Weaver.Runtime.Abstractions {
    
    /// <summary>
    /// Define the facade of a runtime weaver can weave against.
    /// </summary>
    public interface IAssemblyRuntimeFacade {

        // Instead of storing sígnatures in weaved assembly, should
        // we keep a simple version? And store just that. Require that
        // its actively maintained though, and updated every time API
        // actually change.
        //
        // Maybe combine that. Store just an opaque number returned
        // from here. And then runtime can construct that from API if
        // desired.
        //
        // TODO:

        Type DatabaseAttributeType { get; }

        Type ProxyConstructorSignatureType { get; }

        Type InsertConstructorSignatureType { get; }

        MethodInfo CreateMethod { get; }

        IEnumerable<string> SupportedDataTypes { get; }

        IEnumerable<RoutedInterfaceSpecification> RoutedInterfaces { get; }

        Type DbProxyStateInterfaceType { get; }

        MethodInfo GetReadMethod(string type);

        MethodInfo GetWriteMethod(string type);
    }
}