using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using MemoryDB.Core;
using MemoryDB.Core.Interfaces;
using Newtonsoft.Json;
using PetaPoco;

namespace MemoryDB.SqlServer
{
    public class SqlServerDataStore<T> : IDataStore<T> where T : new()
    {
        private readonly string _connectionName;
        private readonly string _tableName;
        private readonly string _databaseName;


        public SqlServerDataStore(string connectionName)
        {
            _connectionName = connectionName;
            _databaseName = GetDatabaseName();
            _tableName = GetTableName();
        }

        private string GetTableName()
        {
            return typeof(T).Name + "_";
        }

        private string GetDatabaseName()
        {
            var connectionString = ConfigurationManager.ConnectionStrings[_connectionName].ConnectionString;
            var builder = new SqlConnectionStringBuilder(connectionString);
            return builder.InitialCatalog;
        }


        public T Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public List<T> LoadData()
        {
            if (!TableExists())
                CreateTable();

            var js = new JsonSerializer();

            var jsonList = GetJsonList();

            var deserializeList = new List<T>();
            foreach (var jsonItem in jsonList)
            {
                using (var stringReader = new StringReader(jsonItem.Value))
                {
                    using (var jsonReader = new JsonTextReader(stringReader))
                    {
                        var item = js.Deserialize<T>(jsonReader);
                        deserializeList.Add(item);
                    }
                }
            }

            return deserializeList;
        }

        public void Remove(T item)
        {
            var pkProperty = GetKeyProperty();
            var id = pkProperty.GetValue(item, null);

            using (var db = new Database(_connectionName))
            {
                var sql = $"DELETE FROM {_tableName} WHERE Id = @0";
                db.Execute(sql, id);
            }
        }

        private void CreateTable()
        {
            using (var db = new Database(_connectionName))
            {
                var sql = $"CREATE TABLE {_tableName} (Id INT IDENTITY NOT NULL PRIMARY KEY, Value VARCHAR(MAX));";
                db.Execute(sql);
            }
        }

        /// <summary>
        ///     gets the list of unserialized json records
        /// </summary>
        /// <returns></returns>
        private List<JsonItem> GetJsonList()
        {
            var jsonList = new List<JsonItem>();
            using (var db = new Database(_connectionName))
            {
                var sql = $"SELECT Id, Value FROM  {_tableName}";
                db.Fetch<JsonItem>(sql);
            }

            return jsonList;
        }

        private bool TableExists()
        {
            using (var db = new Database(_connectionName))
            {
                var sql = $"IF OBJECT_ID('{_tableName}') IS NOT NULL SELECT 1 ELSE SELECT 0;";
                return  db.ExecuteScalar<bool>(sql);
            }
        }


        protected virtual PropertyInfo GetKeyProperty()
        {
            var myObject = new T();
            var myType = myObject.GetType();
            var myProperties = myType.GetProperties();
            var objectTypeName = myType.Name;

            // Is there a property named id (case irrelevant)?
            var pkProperty = myProperties.FirstOrDefault(n => n.Name.Equals("id", StringComparison.InvariantCultureIgnoreCase));
            if (pkProperty == null)
            {
                // Is there a property named TypeNameId (case irrelevant)?
                string findName = $"{objectTypeName}{"id"}";
                pkProperty = myProperties.FirstOrDefault(n => n.Name.Equals(findName, StringComparison.InvariantCultureIgnoreCase));
            }
            if (pkProperty != null) return pkProperty;
            var keyNotDefinedMessageFormat = ""
                                                + "No key property is defined on {0}. Please define a property which forms a unique key for objects of this type.";
            throw new Exception(string.Format(keyNotDefinedMessageFormat, objectTypeName));
        }
    }
}