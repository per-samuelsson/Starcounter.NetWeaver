
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Starcounter.ReferenceRuntime.Internal.Weaving {

    public class DefaultDbCrudMethodProvider : DbCrudMethodProvider {
        readonly static Dictionary<string, string> readMethods = new Dictionary<string, string>() {
            { typeof(int).FullName, nameof(DbCrud.GetInt) },
            { typeof(int?).FullName, nameof(DbCrud.GetNullableInt) }
        };

        readonly static Dictionary<string, string> writeMethods = new Dictionary<string, string>() {
            { typeof(int).FullName, nameof(DbCrud.SetInt) },
            { typeof(int?).FullName, nameof(DbCrud.SetNullableInt) }
        };
        
        public override string CreateMethod {
            get {
                return nameof(DbCrud.Create);
            }
        }

        public override Dictionary<string, string> ReadMethods {
            get {
                return readMethods;
            }
        }

        public override Dictionary<string, string> UpdateMethods {
            get {
                return writeMethods;
            }
        }

        public override string DeleteMethod {
            get {
                throw new NotImplementedException();
            }
        }

        public override MethodInfo GetMethodByName(string method) {
            return typeof(DbCrud).GetTypeInfo().GetMethod(method, BindingFlags.Public | BindingFlags.Static);
        }
    }
}
