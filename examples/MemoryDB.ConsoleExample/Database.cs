using System;
using MemoryDB.ConsoleExample.Models;
using MemoryDB.Core;
using MemoryDB.SqlServer;

namespace MemoryDB.ConsoleExample
{
    public class Database 
    {
        public MemoryList<Address> AddressList { get; set; }
        public MemoryList<Person> PersonList { get; set; }
       
        public Database()
        {
            var _connectionName = "DefaultConnection";

            var addressStore = new SqlServerDataStore<Address>(_connectionName);
            var personStore = new SqlServerDataStore<Person>(_connectionName);

            AddressList = new MemoryList<Address>(addressStore);
            PersonList = new MemoryList<Person>(personStore);

        }


    }
}
