
using System;

namespace Starcounter.Weaver.Runtime.Abstractions {

    public class RoutedInterfaceSpecification {

        public Type InterfaceType { get; set; }

        public Type RoutingTarget { get; set; }

        public Type PassThroughType { get; set; }
    }
}