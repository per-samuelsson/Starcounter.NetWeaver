using System;

namespace Starcounter2.Internal.WeaverFacade {

    public static class CRUD {
        public static void Insert(ulong createHandle, out ulong dbId) {
            dbId = 42;
        }

        public static string ReadString(ulong dbId, ulong readHandle) {
            return "42";
        }

        public static void WriteString(ulong dbId, ulong writeHandle, string value) {
        }
    }
}