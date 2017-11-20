
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Starcounter2.Internal {

    public abstract class CRUDMethodProvider {

        public abstract string CreateMethod { get; }

        public abstract Dictionary<string, string> ReadMethods { get; }

        public abstract Dictionary<string, string> UpdateMethods { get; }

        public abstract string DeleteMethod { get; }

        public IEnumerable<string> SupportedDataTypes {
            get {
                return ReadMethods.Keys.Union(UpdateMethods.Keys);
            }
        }

        public abstract MethodInfo GetMethodByName(string method);

        public virtual MethodInfo GetCreateMethod() {
            return GetMethodByName(CreateMethod);
        }

        public virtual MethodInfo GetReadMethod(string dataType) {
            var methodName = ReadMethods[dataType];
            return GetMethodByName(methodName);
        }

        public virtual MethodInfo GetUpdateMethod(string dataType) {
            var methodName = UpdateMethods[dataType];
            return GetMethodByName(methodName);
        }

        public virtual MethodInfo GetDeleteMethod(string dataType) {
            return GetMethodByName(DeleteMethod);
        }
    }
}
