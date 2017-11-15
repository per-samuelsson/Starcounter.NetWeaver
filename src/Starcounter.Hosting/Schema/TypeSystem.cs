
using System;
using System.Collections.Generic;
using System.Linq;

namespace Starcounter.Hosting.Schema {

    public sealed class TypeSystem {
        public const int InvalidTypeHandle = -1;
        readonly List<string> indexOfAllNamedTypes = new List<string>();
        readonly HashSet<int> dataTypes = new HashSet<int>();

        /// <summary>
        /// Return the name of each type that has been registered as a
        /// known data type.
        /// </summary>
        public IEnumerable<string> DataTypes {
            get {
                return dataTypes.Select(i => indexOfAllNamedTypes[i]);
            }
        }

        /// <summary>
        /// Return the name of each type that has been registered as a
        /// database type.
        /// </summary>
        public IEnumerable<string> DatabaseTypes {
            get {
                return indexOfAllNamedTypes.Except(DataTypes);
            }
        }

        /// <summary>
        /// Define <paramref name="typeName"/> as a data type, i.e. a type
        /// that is legal to reference from a database property.
        /// </summary>
        /// <remarks>
        /// Defining a type already defined as a data type will return the
        /// index of the already defined type, and render no error.
        /// </remarks>
        /// <param name="typeName">Name of the type</param>
        /// <returns>The type handle of the defined data type.</returns>
        public int DefineDataType(string typeName) {
            if (string.IsNullOrWhiteSpace(typeName)) {
                throw new ArgumentNullException(nameof(typeName));
            }

            bool isDataType;
            int index = GetTypeHandleByName(typeName, out isDataType);
            if (index != InvalidTypeHandle) {
                if (!isDataType) {
                    throw new InvalidOperationException($"Can't define data type {typeName}: a database type with that name already exist");
                }
                return index;
            }

            indexOfAllNamedTypes.Add(typeName);
            index = indexOfAllNamedTypes.IndexOf(typeName);
            dataTypes.Add(index);

            return index;
        }

        /// <summary>
        /// Define <paramref name="typeName"/> as a database type, i.e. a type
        /// that is legal to reference from a database property as well as use
        /// as a base class for any other database type.
        /// </summary>
        /// <remarks>
        /// Defining a type already defined will render an error.
        /// </remarks>
        /// <param name="typeName">Name of the type</param>
        /// <returns>The type handle of the defined database type.</returns>
        public int DefineDatabaseType(string typeName) {
            if (string.IsNullOrWhiteSpace(typeName)) {
                throw new ArgumentNullException(nameof(typeName));
            }

            bool isDataType;
            int index = GetTypeHandleByName(typeName, out isDataType);
            if (index != InvalidTypeHandle) {
                var error = $"Can't define database type {typeName}: ";
                if (isDataType) {
                    error += "a data type with that name already exist";
                }
                else {
                    error += "a database type with that name already exist";
                }

                throw new InvalidOperationException(error);
            }

            indexOfAllNamedTypes.Add(typeName);
            return indexOfAllNamedTypes.IndexOf(typeName);
        }

        public bool ContainsType(string typeName) {
            return indexOfAllNamedTypes.Contains(typeName);
        }

        public string GetTypeNameByHandle(int handle) {
            // Intentionally no validation; we use this to retreive type as fast
            // as possible when the given handle is expected to be proven valid.
            // If it's still not, let it blow.
            return indexOfAllNamedTypes[handle];
        }

        public string GetTypeNameByHandle(int handle, out bool isDataType) {
            // Intentionally no validation; we use this to retreive type as fast
            // as possible when the given handle is expected to be proven valid.
            // If it's still not, let it blow.
            var name = indexOfAllNamedTypes[handle];
            isDataType = dataTypes.Contains(handle);
            return name;
        }
        
        public int GetTypeHandleByName(string typeName, out bool isDataType) {
            var index = indexOfAllNamedTypes.IndexOf(typeName);
            if (index == -1) {
                throw new ArgumentOutOfRangeException(nameof(typeName));
            }
            isDataType = dataTypes.Contains(index);
            return index;
        }
    }
}