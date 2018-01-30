
using System;
using System.Collections.Generic;
using Starcounter.Weaver.Runtime.Abstractions;
using System.Reflection;
using System.Linq;

namespace Starcounter.Weaver.Runtime {

    public class DefaultDatabaseTypeStateWriter : IDatabaseTypeState {
        readonly Type type;
        readonly DatabaseTypeStateNames names;
        readonly IEnumerable<FieldInfo> fields;
        readonly FieldInfo createHandle;
        readonly FieldInfo deleteHandle;

        public DefaultDatabaseTypeStateWriter(Type databaseType) : this(databaseType, new DatabaseTypeStateNames()) {

        }

        public DefaultDatabaseTypeStateWriter(Type databaseType, DatabaseTypeStateNames typeStateNames) {
            type = databaseType ?? throw new ArgumentNullException(nameof(databaseType));
            names = typeStateNames ?? throw new ArgumentNullException(nameof(typeStateNames));
            
            fields = type.GetTypeInfo().DeclaredFields;
            
            createHandle = GetHandleField(names.CreateHandle);
            deleteHandle = GetHandleField(names.DeleteHandle);
        }

        public ulong CreateHandle {
            get => GetULongValue(createHandle);
            set => SetULongValue(createHandle, value);
        }

        public ulong DeleteHandle {
            get => GetULongValue(deleteHandle);
            set => SetULongValue(deleteHandle, value);
        }

        public ulong GetPropertyHandle(string declaredPropertyName) {
            return GetULongValue(GetPropertyHandleField(declaredPropertyName));
        }

        public void SetPropertyHandle(string declaredPropertyName, ulong handle) {
            SetULongValue(GetPropertyHandleField(declaredPropertyName), handle);
        }
        
        FieldInfo GetPropertyHandleField(string declaredPropertyName) {
            return GetHandleField(names.GetPropertyHandleName(declaredPropertyName));
        }

        FieldInfo GetHandleField(string name) {
            var field = fields.FirstOrDefault(f => f.IsStatic && f.Name == name);
            if (field == null) {
                throw new ArgumentException($"{type.FullName} missing field {name}");
            }
            return field;
        }

        ulong GetULongValue(FieldInfo field) {
            return (ulong) field.GetValue(null);
        }

        void SetULongValue(FieldInfo field, ulong value) {
            field.SetValue(null, value);
        }
    }
}
