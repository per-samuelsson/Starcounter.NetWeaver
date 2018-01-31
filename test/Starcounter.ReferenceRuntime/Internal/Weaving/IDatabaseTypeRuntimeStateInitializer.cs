
using System;

namespace Starcounter.ReferenceRuntime.Internal.Weaving {

    public interface IDatabaseTypeRuntimeStateInitializer {

        void InitializeType(Type weavedDatabaseType);
    }
}