
using Starcounter.Weaver.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Starcounter.Weaver.Runtime.Tests {

    public class TypeSystemTests {
        string[] databaseTypes = new string[] {
            "test",
            "test2.test3.foo",
            "a",
            "number.of.types",
            "this.is.it"
        };

        string[] dataTypes = new string[] {
            "test7",
            "test8.bar.foo",
            "allisgood",
            "newnameontheblock",
            "per.samuelsson"
        };

        [Fact]
        public void DefiningTypesProduceUniqueHandles() {
            var handles = new HashSet<int>();

            var ts = new TypeSystem();

            ForEachDatabaseType(t => Assert.True(handles.Add(ts.DefineDatabaseType(t))));
            ForEachDataType(t => Assert.True(handles.Add(ts.DefineDataType(t))));
            
            Assert.Equal(databaseTypes.Length + dataTypes.Length, handles.Count);
        }

        [Fact]
        public void DataTypesCanBeDefinedWhenAlreadyExist() {
            var ts = new TypeSystem();

            var h = ts.DefineDataType("test");
            var h2 = ts.DefineDataType("test");
            Assert.Equal(h2, h);
        }

        [Fact]
        public void DataTypesCantHaveSameNameAsDatabaseType() {
            var ts = new TypeSystem();

            var h = ts.DefineDatabaseType("test");
            var e = Assert.Throws<InvalidOperationException>(() => ts.DefineDataType("test"));
            Assert.Contains("test", e.Message);
        }

        [Fact]
        public void DatabaseTypesCanNotBeDefinedWhenAlreadyExist() {
            var ts = new TypeSystem();

            var h = ts.DefineDatabaseType("test");
            var e = Assert.Throws<InvalidOperationException>(() => ts.DefineDatabaseType("test"));
            Assert.Contains("test", e.Message);
        }

        [Fact]
        public void DatabaseTypesCanNotBeDefinedWhenDataTypeAlreadyExist() {
            var ts = new TypeSystem();

            ts.DefineDataType("test");
            var e = Assert.Throws<InvalidOperationException>(() => ts.DefineDatabaseType("test"));
            Assert.Contains("test", e.Message);
        }

        [Fact]
        public void ContainsTypeMethodCanBeUsedToCheckForTypeWithName() {
            var ts = new TypeSystem();

            ForEachDatabaseType(t => ts.DefineDatabaseType(t));
            ForEachDataType(t => ts.DefineDataType(t));

            ForEachNamedType(t => Assert.True(ts.ContainsType(t)));
        }

        [Fact]
        public void AllDatabaseTypesCanBeEnumerated() {
            var ts = new TypeSystem();

            ForEachDatabaseType(t => ts.DefineDatabaseType(t));
            Assert.Equal(databaseTypes.Length, ts.DatabaseTypes.Count());
            
            ForEachDatabaseType(t => Assert.NotNull(ts.DatabaseTypes.SingleOrDefault(s => s.Equals(t))));
        }

        [Fact]
        public void AllDataTypesCanBeEnumerated() {
            var ts = new TypeSystem();

            ForEachDataType(t => ts.DefineDataType(t));
            Assert.Equal(dataTypes.Length, ts.DataTypes.Count());

            ForEachDataType(t => Assert.NotNull(ts.DataTypes.SingleOrDefault(s => s.Equals(t))));
        }

        [Fact]
        public void TypeHandlesCanBeRetreivedByName() {
            var ts = new TypeSystem();

            var handles = new Dictionary<int, string>(databaseTypes.Length + dataTypes.Length);
            ForEachDatabaseType(t => handles.Add(ts.DefineDatabaseType(t), t));
            ForEachDatabaseType(t => {
                var h = ts.GetTypeHandleByName(t, out bool isDataType);
                Assert.False(isDataType);
                Assert.Equal(handles[h], t);
            });
            
            ForEachDataType(t => handles.Add(ts.DefineDataType(t), t));
            ForEachDataType(t => {
                var h = ts.GetTypeHandleByName(t, out bool isDataType);
                Assert.True(isDataType);
                Assert.Equal(handles[h], t);
            });
        }

        void TypeNamesCanBeRetreivedByHandle() {
            var ts = new TypeSystem();

            var handles = new Dictionary<int, string>(databaseTypes.Length + dataTypes.Length);

            ForEachDatabaseType(t => ts.DefineDatabaseType(t));
            foreach(var h in handles) {
                var name = ts.GetTypeNameByHandle(h.Key);
                Assert.Equal(name, h.Value);
                
                name = ts.GetTypeNameByHandle(h.Key, out bool isDataType);
                Assert.Equal(name, h.Value);
                Assert.False(isDataType);
            }

            ForEachDataType(t => ts.DefineDataType(t));
            foreach (var h in handles) {
                var name = ts.GetTypeNameByHandle(h.Key);
                Assert.Equal(name, h.Value);
                
                name = ts.GetTypeNameByHandle(h.Key, out bool isDataType);
                Assert.Equal(name, h.Value);
                Assert.True(isDataType);
            }
        }

        void ForEachDatabaseType(Action<string> action) {
            foreach (var dbtype in databaseTypes) {
                action(dbtype);
            }
        }

        void ForEachDataType(Action<string> action) {
            foreach (var type in dataTypes) {
                action(type);
            }
        }

        void ForEachNamedType(Action<string> action) {
            ForEachDatabaseType(action);
            ForEachDataType(action);
        }
    }
}