using MemoryDB.Core;
using MemoryDB.SqlServer.Test.Models;

namespace MemoryDB.SqlServer.Test
{
    public class Database 
    {
        

        public MemoryList<Address> AddressList { get; set; }
        public MemoryList<Person> PersonList { get; set; }
       
        public Database(bool identityInsert)
        {
            var _connectionName = "DefaultConnection";

            var addressStore = new SqlServerDataStore<Address>(_connectionName, identityInsert);
            var personStore = new SqlServerDataStore<Person>(_connectionName, identityInsert);

            AddressList = new MemoryList<Address>(addressStore);
            PersonList = new MemoryList<Person>(personStore);

        }


    }
}
