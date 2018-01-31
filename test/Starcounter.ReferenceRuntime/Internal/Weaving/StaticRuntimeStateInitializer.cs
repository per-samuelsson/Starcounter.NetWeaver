
using System;

namespace Starcounter.ReferenceRuntime.Internal.Weaving {
    
    public static class StaticRuntimeStateInitializer {

        internal static IDatabaseTypeRuntimeStateInitializer Current { get; set; }

        public static void InitializeDatabaseType(Type weavedDatabaseType) {
            // If no intializer is installed at the point of this call,
            // thats an internal, and recognized, error.

            Current.InitializeType(weavedDatabaseType);
        }
    }
}