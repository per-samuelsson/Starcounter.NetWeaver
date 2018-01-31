
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Starcounter.ReferenceRuntime.Internal.Weaving {

    public abstract class DbCrudMethodProvider {

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
