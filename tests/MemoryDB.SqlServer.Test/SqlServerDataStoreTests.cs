using MemoryDB.SqlServer.Test.Models;
using NUnit.Framework;

namespace MemoryDB.SqlServer.Test
{
    public class SqlServerDataStoreTests
    {
        private static readonly Database Database = new Database();

        [Test]
        public void LoadDataReturnsList()
        {
            Database.AddressList.Add(new Address
            {
                AddressLine1 = "6 ",
                AddressLine2 = "Some Avenue",
                PostCode = "FU0 7SW"
            });

            Assert.IsNotEmpty(Database.AddressList);
        }

        public void Prep()
        {
            Database.AddressList.Clear();
            Database.PersonList.Clear();
        }
    }
}