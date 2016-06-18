using MemoryDB.Core;
using MemoryDB.Core.Interfaces;
using PetaPoco;
using System.Collections.Generic;
using System.Linq;

namespace MemoryDB.SqlServer
{
    public class SqlServerDataStore<T> : IDataStore<T> where T : new()
    {
        private readonly string _connectionName;
        private readonly string _tableName;

        public SqlServerDataStore(string connectionName)
        {
            _connectionName = connectionName;
            _tableName = GetTableName();
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public T Add(T item)
        {
            var jsonItem = new JsonItem<T>(item);
            using (var database = new Database(_connectionName))
            {
                var sql = $"INSERT INTO [{_tableName}] VALUES ('{jsonItem.Value}' );";
                database.Execute(sql);
            }
            return item;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            using (var database = new Database(_connectionName))
            {
                var sql = $"TRUNCATE TABLE [{_tableName}];";
                database.Execute(sql);
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        /// <returns></returns>
        public List<T> LoadData()
        {
            if (!TableExists())
                CreateTable();

            var jsonList = GetJsonList();

            return jsonList.Select(jsonItem => jsonItem.Deserialize()).ToList();
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Remove(T item)
        {
            var jsonItem = new JsonItem<T>(item);
            using (var database = new Database(_connectionName))
            {
                var sql = $"DELETE FROM [{_tableName}] WHERE Id = {jsonItem.Id};";
                database.Execute(sql);
            }
        }

        private void CreateTable()
        {
            using (var db = new Database(_connectionName))
            {
                var sql = $"CREATE TABLE [{_tableName}] (Id INT NOT NULL PRIMARY KEY, Value VARCHAR(MAX));";
                db.Execute(sql);
            }
        }

        private List<JsonItem<T>> GetJsonList()
        {
            using (var db = new Database(_connectionName))
            {
                var sql = $"SELECT [Id], [Value] FROM [{_tableName}]";
                return db.Fetch<JsonItem<T>>(sql);
            }
        }

        private string GetTableName()
        {
            return typeof(T).Name;
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