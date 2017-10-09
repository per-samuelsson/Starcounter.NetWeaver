using System;

namespace Starcounter2.Internal.WeaverFacade {

    public static class CRUD {

        public static void CreateCrud(out ulong dbId, out ulong dbRef, ulong crudHandle) {
            dbId = 42;
            dbRef = 42;
        }

        public static int GetInt(ulong dbId, ulong dbRef, ulong crudHandle) {
            return 42;
        }

        public static void SetInt(ulong dbId, ulong dbRef, ulong crudHandle, int value) {

        }

        public static int? GetNullableInt(ulong dbId, ulong dbRef, ulong crudHandle) {
            return null;
        }

        public static void SetNullableInt(ulong dbId, ulong dbRef, ulong crudHandle, int? value) {

        }
    }
}