
using System;

namespace Starcounter2.Internal {

    public interface IObject {

    }

    public static class DbCrud {

        public static void Create(out ulong dbId, out ulong dbRef, ulong crudHandle) {
            dbId = 42;
            dbRef = 42;
        }

        public static int GetInt(ulong dbId, ulong dbRef, ulong crudHandle) {
            var v = 42;
            Console.WriteLine($"{nameof(GetInt)}({dbId}/{dbRef}): {v}");
            return 42;
        }

        public static void SetInt(ulong dbId, ulong dbRef, ulong crudHandle, int value) {
            Console.WriteLine($"{nameof(SetInt)}({dbId}/{dbRef}): {value}");
        }

        public static int? GetNullableInt(ulong dbId, ulong dbRef, ulong crudHandle) {
            return null;
        }

        public static void SetNullableInt(ulong dbId, ulong dbRef, ulong crudHandle, int? value) {

        }

        public static IObject GetObject(ulong dbId, ulong dbRef, ulong crudHandle) {
            return null;
        }

        public static void SetObject(ulong dbId, ulong dbRef, ulong crudHandle, IObject value) {

        }
    }
}