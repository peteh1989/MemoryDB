using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using MemoryDB.Core.Interfaces;
using Newtonsoft.Json;

namespace MemoryDB.SqlServer
{
    public class SqlServerDataStore<T> : IDataStore<T> where T : new()
    {
        private readonly string _connectionName;
        private readonly string _tableName;

        public SqlServerDataStore(string connectionName)
        {
            _connectionName = connectionName;
        }

        public SqlServerDataStore(string connectionName, string tableName)
        {
            _connectionName = connectionName;
            _tableName = tableName;
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
            throw new NotImplementedException();
        }

        private void CreateTable(string tableName)
        {
            using (var db = new Database(_connectionName))
            {
                var sql = @"CREATE TABLE @tableName (Id INT IDENTITY NOT NULL, Value VARCHAR(MAX));";
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
            using (var db = new Database(_connectionName))
            {
                using (var rdr = db.ExecDataReader("", "", ""))
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
            using (var db = new Database(_connectionName))
            {
                var sql = "IF OBJECT_ID('@tableName') IS NOT NULL SELECT 1 ELSE SELECT 0;";
                var tableExists = (bool) db.ExecScalar(sql, "@tableName", tableName);
                return tableExists;
            }
        }

        internal class JsonItem
        {
            internal int Id { get; set; }
            internal string Value { get; set; }
        }
    }
}