using System.Collections.Generic;

namespace MemoryDB.SqlServer.Test.Models
{
    public class Person
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public ICollection<Address> Addresses { get; set; } 
    }
}
