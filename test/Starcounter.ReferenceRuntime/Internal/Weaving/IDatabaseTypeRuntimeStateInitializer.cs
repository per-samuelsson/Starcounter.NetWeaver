
using System;

namespace Starcounter2.Internal.Weaving {

    public interface IDatabaseTypeRuntimeStateInitializer {

        void InitializeType(Type weavedDatabaseType);
    }
}