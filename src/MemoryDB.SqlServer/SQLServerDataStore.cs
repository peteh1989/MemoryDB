using MemoryDB.Core;
using MemoryDB.Core.Interfaces;
using Newtonsoft.Json;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace MemoryDB.SqlServer
{
    public class SqlServerDataStore<T> : IDataStore<T> where T : new()
    {
        private readonly string _connectionName;
        private readonly string _databaseName;
        private readonly string _tableName;

        public SqlServerDataStore(string connectionName)
        {
            _connectionName = connectionName;
            _databaseName = GetDatabaseName();
            _tableName = GetTableName();
        }

        public T Add(T item)
        {
            var jsonItem = new JsonItem<T>(item);
            using (var database = new Database(_connectionName))
            {
                var sql = $"INSERT INTO {_tableName} VALUES ( {jsonItem.Id}, {jsonItem.Value} );";
                database.Execute(sql);
            }
            return item;
        }

        public void Clear()
        {
            using (var database = new Database(_connectionName))
            {
                var sql = $"TRUNCATE TABLE {_tableName};";
                database.Execute(sql);
            }
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
            var jsonItem = new JsonItem<T>(item);
            using (var database = new Database(_connectionName))
            {
                var sql = $"DELETE FROM {_tableName} WHERE Id = {jsonItem.Id};";
                database.Execute(sql);
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

        private string GetDatabaseName()
        {
            var connectionString = ConfigurationManager.ConnectionStrings[_connectionName].ConnectionString;
            var builder = new SqlConnectionStringBuilder(connectionString);
            return builder.InitialCatalog;
        }

        /// <summary>
        ///     gets the list of unserialized json records
        /// </summary>
        /// <returns></returns>
        private List<JsonItem<T>> GetJsonList()
        {
            var jsonList = new List<JsonItem>();
            using (var db = new Database(_connectionName))
            {
                var sql = $"SELECT Id, Value FROM  {_tableName}";
                db.Fetch<JsonItem>(sql);
            }

            return jsonList;
        }

        private string GetTableName()
        {
            return typeof(T).Name + "_";
        }

        private bool TableExists()
        {
            using (var db = new Database(_connectionName))
            {
                var sql = $"IF OBJECT_ID('{_tableName}') IS NOT NULL SELECT 1 ELSE SELECT 0;";
                return db.ExecuteScalar<bool>(sql);
            }
        }
    }
}