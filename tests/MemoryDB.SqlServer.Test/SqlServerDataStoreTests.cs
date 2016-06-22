using MemoryDB.SqlServer.Test.Models;
using NUnit.Framework;

namespace MemoryDB.SqlServer.Test
{
    public class SqlServerDataStoreTests
    {
        public SqlServerDataStoreTests()
        {
           
        }

        [Test]
        public void ClearListRemovesData()
        {
            var database = new Database(true);
            database.AddressList.Clear();
            Assert.IsEmpty(database.AddressList);
        }

        [Test]
        public void AddItem()
        {
            var database = new Database(true);

            database.AddressList.Add(new Address
            {
                AddressLine1 = "6 ",
                AddressLine2 = "Some Avenue",
                PostCode = "FU0 7SW"
            });

            Assert.IsNotEmpty(database.AddressList);
        }

        [Test]
        public void LoadDataResturnsList()
        {
            var database = new Database(true);
            
        }



    }
}