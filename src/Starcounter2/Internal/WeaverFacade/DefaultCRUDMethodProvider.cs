
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Starcounter2.Internal.WeaverFacade {

    public class DefaultCRUDMethodProvider : CRUDMethodProvider {
        readonly static Dictionary<string, string> readMethods = new Dictionary<string, string>() {
            { typeof(int).FullName, nameof(CRUD.GetInt) },
            { typeof(int?).FullName, nameof(CRUD.GetNullableInt) }
        };

        readonly static Dictionary<string, string> writeMethods = new Dictionary<string, string>() {
            { typeof(int).FullName, nameof(CRUD.SetInt) },
            { typeof(int?).FullName, nameof(CRUD.SetNullableInt) }
        };
        
        public override string CreateMethod {
            get {
                return nameof(CRUD.CreateCrud);
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
            return typeof(CRUD).GetTypeInfo().GetMethod(method, BindingFlags.Public | BindingFlags.Static);
        }
    }
}
