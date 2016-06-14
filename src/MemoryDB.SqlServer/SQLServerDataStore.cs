using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using MemoryDB.Core.Interfaces;
using Newtonsoft.Json;

namespace MemoryDB.SqlServer
{
    public class SqlServerDataStore<T> : IDataStore<T> where T : new()
    {
        private readonly string _connectionName;
        private readonly string _connectionString;
        private readonly string _tableName;
        private readonly string _databaseName;


        public SqlServerDataStore(string connectionName)
        {
            _connectionName = connectionName;
            _connectionString = GetConnectionString();
            _databaseName = GetDatabaseName();
            _tableName = GetTableName();


            if (!TableExists(_tableName))
                CreateTable(_tableName);

        }

        private string GetTableName()
        {
            return typeof(T).Name + "_";

        }

        private string GetConnectionString()
        {
            var connectionString = ConfigurationManager.ConnectionStrings[_connectionName].ConnectionString;
            return connectionString;
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
            if (!TableExists(_tableName))
                CreateTable(_tableName);

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

            var sql = @"DELETE FROM @tableName WHERE Id = @id";
            using (var db = new Database(_connectionName))
            {
                db.ExecNonQuery(sql, "@tableName", _tableName, "@id", id);
            }
        }

        private bool DatabaseExists(string databaseName)
        {
            using (var db = new Database(_connectionString))
            {
                var sql = "IF DB_ID('@databaseName') IS NOT NULL SELECT 1 ELSE SELECT 0;";
                var databaseExists = (bool)db.ExecScalar(sql, "@databaseName", databaseName);
                return databaseExists;
            }
        }

        private void CreateTable(string tableName)
        {
            using (var db = new Database(_connectionString))
            {
                var sql = @"CREATE TABLE @tableName (Id INT IDENTITY NOT NULL PRIMARY KEY, Value VARCHAR(MAX));";
                db.ExecNonQuery(sql, "@tableName", tableName);
            }
        }

        /// <summary>
        ///     Maps data record to Json record object
        /// </summary>
        /// <param name="dataRecord">The data record.</param>
        /// <returns></returns>
        private JsonItem FillDataRecord(IDataRecord dataRecord)
        {
            var jsonItem = new JsonItem
            {
                Id = dataRecord.GetInt32(dataRecord.GetOrdinal("Id")),
                Value = dataRecord.GetString(dataRecord.GetOrdinal("Value"))
            };

            return jsonItem;
        }

        /// <summary>
        ///     gets the list of unserialized json records
        /// </summary>
        /// <returns></returns>
        private List<JsonItem> GetJsonList()
        {
            var jsonList = new List<JsonItem>();
            using (var db = new Database(_connectionString))
            {
                using (var rdr = db.ExecDataReader("SELECT Id, Value FROM @Table", "@Table", _tableName))
                {
                    while (rdr.Read())
                    {
                        jsonList.Add(FillDataRecord(rdr));
                    }
                }
            }

            return jsonList;
        }

        private bool TableExists(string tableName)
        {
            using (var db = new Database(_connectionString))
            {
                var sql = "IF OBJECT_ID('@tableName') IS NOT NULL SELECT 1 ELSE SELECT 0;";
                var tableExists = db.ExecScalar(sql, "@tableName", tableName);
                return tableExists.ToString() == "1";
            }
        }

        internal class JsonItem
        {
            internal int Id { get; set; }
            internal string Value { get; set; }
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