using MemoryDB.Core;
using MemoryDB.Example.Console.Models;
using MemoryDB.SqlServer;

namespace MemoryDB.Example.Console
{
    public class Database
    {
        public MemoryList<Address> AddressList { get; set; }
        public MemoryList<Person> PersonList { get; set; }
       
        public Database()
        {
            var _connectionName = "DefaultConnection";

            var personStore = new SqlServerDataStore<Address>(_connectionName);
            var addressStore = new SqlServerDataStore<Person>(_connectionName);

        }
    }
}
